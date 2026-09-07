import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import {
    createUser,
    fetchUsersWithCourse,
    updateUser,
} from "../services/userService";
import UsersTable from "../components/UsersTable";
import UsersToolbar from "../components/UsersToolbar";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import Pagination from "../components/Pagination";
import ModalWrapper from "../components/ModalWrapper";
import UserForm from "../components/UserForm";
import type { UserCreateRequest } from "../interfaces/user/UserCreateRequest";

import type { UserUpdateRequest } from "../interfaces/user/UserUpdateRequest";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 20;
const DEFAULT_SORT = "name-asc";

export default function Users() {
    const [users, setUsers] = useState<UserWithCourseResponse[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(DEFAULT_PAGE);
    const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState(DEFAULT_SORT);
    const [showUserForm, setShowUserForm] = useState(false);
    const [refreshKey, setRefreshKey] = useState(0);
    const [userFormError, setUserFormError] = useState<string>();
    const [editingUser, setEditingUser] =
        useState<UserWithCourseResponse | null>(null);

    const handleSearchChange = (value: string) => {
        setSearch(value);
        setPage(DEFAULT_PAGE);
    };

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(DEFAULT_PAGE);
    };

    const handleCreateUser = async (values: UserCreateRequest) => {
        try {
            setUserFormError(undefined);

            await createUser(values);

            setShowUserForm(false);
            setRefreshKey((current) => current + 1);
        } catch (error) {
            setUserFormError(
                error instanceof Error
                    ? error.message
                    : "Could not create user.",
            );
        }
    };

    const handleUpdateUser = async (values: UserUpdateRequest) => {
        if (!editingUser) {
            return;
        }

        await updateUser(editingUser.id, values);

        setEditingUser(null);
        setRefreshKey((current) => current + 1);
    };

    useEffect(() => {
        const loadUsers = async () => {
            const [sortField, sortDirection = "asc"] = sortBy.split("-");

            const query: QueryParameters = {
                search,
                sortBy: sortField,
                direction: sortDirection,
                page,
                pageSize,
            };

            const data = await fetchUsersWithCourse(query);

            setUsers(data.items);
            setTotalCount(data.totalCount);
        };

        void loadUsers();
    }, [search, sortBy, page, pageSize, refreshKey]);

    return (
        <div className="p-4">
            <UsersToolbar
                search={search}
                sortBy={sortBy}
                onSearchChange={handleSearchChange}
                onSortChange={handleSortChange}
                onAddUser={() => setShowUserForm(true)}
            />

            <UsersTable
                users={users}
                onEdit={(id) => {
                    const user = users.find((user) => user.id === id);

                    if (user) {
                        setEditingUser(user);
                    }
                }}
                onDelete={(id) => console.log("Delete", id)}
                onAssignCourse={(id) => console.log("Assign course", id)}
            />

            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={setPage}
            />

            {showUserForm && (
                <ModalWrapper
                    open={showUserForm}
                    onClose={() => setShowUserForm(false)}
                    title="Create user"
                >
                    <UserForm
                        mode="create"
                        onSubmit={handleCreateUser}
                        onCancel={() => setShowUserForm(false)}
                        submitError={userFormError}
                    />
                </ModalWrapper>
            )}

            {editingUser && (
                <ModalWrapper
                    open={true}
                    onClose={() => setEditingUser(null)}
                    title="Edit user"
                >
                    <UserForm
                        mode="edit"
                        initialValues={{
                            name: editingUser.name,
                            email: editingUser.email ?? "",
                            role: editingUser.role,
                            status: editingUser.status,
                        }}
                        onSubmit={handleUpdateUser}
                        onCancel={() => setEditingUser(null)}
                    />
                </ModalWrapper>
            )}
        </div>
    );
}

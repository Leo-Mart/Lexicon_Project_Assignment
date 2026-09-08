import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import {
    createUser,
    fetchUsersWithCourse,
    updateUser,
    deleteUser,
} from "../services/userService";
import UsersTable from "../components/UsersTable";
import UsersToolbar from "../components/UsersToolbar";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import Pagination from "../components/Pagination";
import ModalWrapper from "../components/ModalWrapper";
import UserForm from "../components/UserForm";
import type { UserCreateRequest } from "../interfaces/user/UserCreateRequest";
import type { UserUpdateRequest } from "../interfaces/user/UserUpdateRequest";
import ConfirmDialog from "../components/ConfirmDialog";
import { fetchCourses } from "../services/courseService";
import {
    assignOrChangeCourse,
    removeCourse,
} from "../services/enrollmentService";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import AssignCourseForm from "../components/AssignCourseForm";
import type { UserFormValues } from "../components/UserForm";

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
    const [userToDelete, setUserToDelete] =
        useState<UserWithCourseResponse | null>(null);

    const handleSearchChange = (value: string) => {
        setSearch(value);
        setPage(DEFAULT_PAGE);
    };

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(DEFAULT_PAGE);
    };

    const [courses, setCourses] = useState<CourseResponse[]>([]);

    const [assigningUser, setAssigningUser] =
        useState<UserWithCourseResponse | null>(null);

    const [assignCourseError, setAssignCourseError] = useState<string>();

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

    const handleUpdateUser = async (values: UserFormValues) => {
        if (!editingUser) {
            return;
        }

        try {
            setUserFormError(undefined);

            const userUpdate: UserUpdateRequest = {
                name: values.name,
                email: values.email,
                role: values.role,
                status: values.status,
            };

            await updateUser(editingUser.id, userUpdate);

            if (
                values.role === "Student" &&
                values.courseId !== editingUser.courseId
            ) {
                if (values.courseId) {
                    await assignOrChangeCourse(editingUser.id, values.courseId);
                } else {
                    await removeCourse(editingUser.id);
                }
            }

            if (
                editingUser.role === "Student" &&
                values.role === "Teacher" &&
                editingUser.courseId
            ) {
                await removeCourse(editingUser.id);
            }

            setEditingUser(null);
            setRefreshKey((current) => current + 1);
        } catch (error) {
            setUserFormError(
                error instanceof Error
                    ? error.message
                    : "Could not update user.",
            );
        }
    };

    const handleDeleteUser = async (id: string) => {
        await deleteUser(id);

        setRefreshKey((current) => current + 1);
    };

    const handleAssignCourse = async (courseId: string) => {
        if (!assigningUser) {
            return;
        }

        try {
            setAssignCourseError(undefined);

            await assignOrChangeCourse(assigningUser.id, courseId);

            setAssigningUser(null);
            setRefreshKey((current) => current + 1);
        } catch (error) {
            setAssignCourseError(
                error instanceof Error
                    ? error.message
                    : "Could not assign course.",
            );
        }
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

    useEffect(() => {
        const loadCourses = async () => {
            const data = await fetchCourses({
                search: "",
                sortBy: "name",
                direction: "asc",
                page: 1,
                pageSize: 200,
            });

            setCourses(data.items);
        };

        void loadCourses();
    }, []);

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
                sortBy={sortBy}
                onSortChange={handleSortChange}
                onEdit={(id) => {
                    const user = users.find((user) => user.id === id);

                    if (user) {
                        setEditingUser(user);
                    }
                }}
                onDelete={(id) => {
                    const user = users.find((user) => user.id === id);

                    if (user) {
                        setUserToDelete(user);
                    }
                }}
                onAssignCourse={(id) => {
                    const user = users.find((user) => user.id === id);

                    if (user) {
                        setAssignCourseError(undefined);
                        setAssigningUser(user);
                    }
                }}
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
                            courseId: editingUser.courseId ?? "",
                        }}
                        courses={courses}
                        onSubmit={handleUpdateUser}
                        onCancel={() => setEditingUser(null)}
                        submitError={userFormError}
                    />
                </ModalWrapper>
            )}

            {userToDelete && (
                <ConfirmDialog
                    open={true}
                    title="Delete user"
                    message={`Are you sure you want to delete ${userToDelete.name}?`}
                    onCancel={() => setUserToDelete(null)}
                    onConfirm={async () => {
                        await handleDeleteUser(userToDelete.id);
                        setUserToDelete(null);
                    }}
                />
            )}

            {assigningUser && (
                <ModalWrapper
                    open={true}
                    onClose={() => setAssigningUser(null)}
                    title={`Assign course to ${assigningUser.name}`}
                >
                    <AssignCourseForm
                        courses={courses}
                        onSubmit={(courseId) => {
                            void handleAssignCourse(courseId);
                        }}
                        onCancel={() => setAssigningUser(null)}
                        submitError={assignCourseError}
                    />
                </ModalWrapper>
            )}
        </div>
    );
}

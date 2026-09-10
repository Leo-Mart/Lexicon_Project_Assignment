import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import {
    createUser,
    fetchUsersWithCourse,
    updateUser,
    deleteUser,
} from "../services/userService";
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
import type { SortOption } from "../types/SortOption";
import TableToolbar from "../components/TableToolbar";
import DataTable from "../components/DataTable";
import type { Column } from "../types/Column";
import UserBadge from "../components/UserBadge";
import Button from "../components/Button";
import {
    UserStatus,
    type UserStatus as UserStatusType,
} from "../constants/UserConstant";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 20;
const DEFAULT_SORT = "name-asc";

const USER_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "course-asc", label: "Course A-Z" },
    { value: "course-desc", label: "Course Z-A" },
    { value: "role-asc", label: "Role A-Z" },
    { value: "role-desc", label: "Role Z-A" },
    { value: "status", label: "Status" },
];

export default function Users() {
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
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

    const userColumns: Column<UserWithCourseResponse>[] = [
        {
            key: "name",
            field: "name",
            render: (user) => user.name,
            header: "Name",
        },
        {
            key: "email",
            field: "email",
            render: (user) => user.email,
            header: "Email",
        },
        {
            key: "status",
            field: "status",
            render: (user) => getUserStatusName(user.status),
            header: "Status",
        },
        {
            key: "role",
            field: "role",
            render: (user) => <UserBadge role={user.role} />,
            header: "Role",
        },
        {
            key: "course",
            field: "course",
            render: (user) => user.courseName ?? "Not assigned",
            header: "Course",
        },
        {
            key: "actions",
            render: (user) => (
                <div className="flex gap-2">
                    <Button onClick={() => handleEdit(user)}>Edit</Button>
                    <Button variant="cancel" onClick={() => handleDelete(user)}>
                        Delete
                    </Button>
                    {user.role === "Student" && !user.courseId && (
                        <Button onClick={() => handleAssignCourseClick(user)}>
                            Assign course
                        </Button>
                    )}
                </div>
            ),
            header: "Actions",
        },
    ];

    const getUserStatusName = (status: UserStatusType): string => {
        return (
            Object.entries(UserStatus).find(
                ([, value]) => value === status,
            )?.[0] ?? "Unknown"
        );
    };

    const handleEdit = (user: UserWithCourseResponse) => {
        setEditingUser(user);
    };

    const handleDelete = (user: UserWithCourseResponse) => {
        setUserToDelete(user);
    };

    const handleAssignCourseClick = (user: UserWithCourseResponse) => {
        setAssignCourseError(undefined);
        setAssigningUser(user);
    };

    useEffect(() => {
        const loadUsers = async () => {
            setLoading(true);
            try {
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
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch course",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
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

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;

    return (
        <div className="p-4">
            <TableToolbar
                tableTitle="Users"
                search={search}
                sortBy={sortBy}
                sortOptions={USER_SORT_OPTIONS}
                onSearchChange={handleSearchChange}
                onSortChange={handleSortChange}
                addAction={{
                    label: "Add user",
                    onAdd: () => setShowUserForm(true),
                }}
            />

            <DataTable
                items={users}
                columns={userColumns}
                getKey={(user) => user.id}
                sortBy={sortBy}
                isLoading={loading}
                onSortChange={handleSortChange}
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

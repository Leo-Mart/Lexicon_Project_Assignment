import SortableTh from "./SortableTableHead";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import UserTableRow from "./UserTableRow";

interface UsersTableProps {
    users: UserWithCourseResponse[];
    sortBy: string;
    isLoading: boolean;
    onSortChange: (value: string) => void;
    onEdit: (id: string) => void;
    onDelete: (id: string) => void;
    onAssignCourse: (id: string) => void;
}

export default function UsersTable({
    users,
    sortBy,
    isLoading,
    onSortChange,
    onEdit,
    onDelete,
    onAssignCourse,
}: UsersTableProps) {
    return (
        <div className="overflow-x-auto rounded-lg border border-gray-600">
            <table className="w-full text-left text-gray-100">
                <thead className="bg-gray-700">
                    <tr>
                        <SortableTh
                            field="name"
                            label="Name"
                            sortBy={sortBy}
                            isLoading={isLoading}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="email"
                            label="Email"
                            sortBy={sortBy}
                            isLoading={isLoading}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="status"
                            label="Status"
                            sortBy={sortBy}
                            isLoading={isLoading}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="role"
                            label="Role"
                            sortBy={sortBy}
                            isLoading={isLoading}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="course"
                            label="Course"
                            sortBy={sortBy}
                            isLoading={isLoading}
                            onSortChange={onSortChange}
                        />
                        <th className="px-4 py-3">Actions</th>
                    </tr>
                </thead>

                <tbody>
                    {users.map((user) => (
                        <UserTableRow
                            key={user.id}
                            user={user}
                            onEdit={onEdit}
                            onDelete={onDelete}
                            onAssignCourse={onAssignCourse}
                        />
                    ))}
                </tbody>
            </table>
        </div>
    );
}

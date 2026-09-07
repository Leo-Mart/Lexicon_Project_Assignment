import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import UserTableRow from "./UserTableRow";

interface UsersTableProps {
    users: UserWithCourseResponse[];
    onEdit: (id: string) => void;
    onDelete: (id: string) => void;
    onAssignCourse: (id: string) => void;
}

export default function UsersTable({
    users,
    onEdit,
    onDelete,
    onAssignCourse,
}: UsersTableProps) {
    return (
        <div className="overflow-x-auto rounded-lg border border-gray-600">
            <table className="w-full text-left text-gray-100">
                <thead className="bg-gray-700">
                    <tr>
                        <th className="px-4 py-3">Name</th>
                        <th className="px-4 py-3">Email</th>
                        <th className="px-4 py-3">Status</th>
                        <th className="px-4 py-3">Role</th>
                        <th className="px-4 py-3">Course</th>
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

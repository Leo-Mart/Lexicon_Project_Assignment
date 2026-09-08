import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import UserTableRow from "./UserTableRow";

interface UsersTableProps {
    users: UserWithCourseResponse[];
    sortBy: string;
    onSortChange: (value: string) => void;
    onEdit: (id: string) => void;
    onDelete: (id: string) => void;
    onAssignCourse: (id: string) => void;
}

type SortableColumn = "name" | "email" | "status" | "role" | "course";

function SortableTh({
    field,
    label,
    sortBy,
    onSortChange,
}: {
    field: SortableColumn;
    label: string;
    sortBy: string;
    onSortChange: (value: string) => void;
}) {
    const [currentField, currentDirection = "asc"] = sortBy.split("-");
    const isActive = currentField === field;
    const isAsc = currentDirection === "asc";

    const handleClick = () => {
        // Same column: toggle direction. New column: start ascending.
        onSortChange(isActive && isAsc ? `${field}-desc` : `${field}-asc`);
    };

    return (
        <th className="px-4 py-3">
            <button
                type="button"
                onClick={handleClick}
                className="inline-flex items-center gap-1 font-normal cursor-pointer hover:text-slate-300"
                aria-label={`Sort by ${label}`}
            >
                {label}
                {isActive && <span aria-hidden>{isAsc ? "▲" : "▼"}</span>}
            </button>
        </th>
    );
}

export default function UsersTable({
    users,
    sortBy,
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
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="email"
                            label="Email"
                            sortBy={sortBy}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="status"
                            label="Status"
                            sortBy={sortBy}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="role"
                            label="Role"
                            sortBy={sortBy}
                            onSortChange={onSortChange}
                        />
                        <SortableTh
                            field="course"
                            label="Course"
                            sortBy={sortBy}
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

import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import UserBadge from "./UserBadge";
import {
    UserStatus,
    type UserStatus as UserStatusType,
} from "../constants/UserConstant";

interface UserTableRowProps {
    user: UserWithCourseResponse;
    onEdit: (id: string) => void;
    onDelete: (id: string) => void;
    onAssignCourse: (id: string) => void;
}

export default function UserTableRow({
    user,
    onEdit,
    onDelete,
    onAssignCourse,
}: UserTableRowProps) {
    return (
        <tr className="border-t border-gray-700 hover:bg-gray-700/40">
            <td className="px-4 py-3">{user.name}</td>
            <td>{user.email}</td>
            <td>{getUserStatusName(user.status)}</td>
            <td><UserBadge role={user.role} /></td>
            <td>{user.courseName ?? "Not assigned"}</td>

            <td>
                <div className="flex gap-2">
                    <button
                        type="button"
                        onClick={() => onEdit(user.id)}
                    >
                        Edit
                    </button>

                    <button
                        type="button"
                        onClick={() => onDelete(user.id)}
                    >
                        Delete
                    </button>

                    <button
                        type="button"
                        onClick={() => onAssignCourse(user.id)}
                    >
                        Assign course
                    </button>
                </div>
            </td>
        </tr>
    );
}

const getUserStatusName = (status: UserStatusType): string => {
    return Object.entries(UserStatus).find(
        ([, value]) => value === status,
    )?.[0] ?? "Unknown";
};
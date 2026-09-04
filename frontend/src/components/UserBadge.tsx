import type { UserRole } from "../constants/UserConstant";

interface UserBadgeProps {
    role: UserRole;
}

export default function UserBadge({ role }: UserBadgeProps) {
    const roleClass =
        role === "Teacher"
            ? "bg-blue-100 text-blue-700"
            : "bg-green-100 text-green-700";

    return (
        <span className={`px-2 py-1 text-xs rounded ${roleClass}`}>{role}</span>
    );
}

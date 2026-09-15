import type { UserRole } from "../constants/UserConstant";

interface UserBadgeProps {
    role: UserRole;
}

export default function UserBadge({ role }: UserBadgeProps) {
    const roleClass =
        role === "Teacher"
            ? "bg-accent-teacher text-white"
            : "bg-accent-student text-white";

    return (
        <span className={`px-2 py-1 text-xs rounded ${roleClass}`}>{role}</span>
    );
}

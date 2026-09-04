import UserBadge from "./UserBadge";
import { useAuth } from "../hooks/useAuth";
import { LogOut } from "lucide-react";

export default function UserInfo() {
    const { isAuthenticated, name, role, logoutUser } = useAuth();

    if (!isAuthenticated || !role || !name) {
        return null;
    }

    return (
        <div className="flex items-center gap-2 border rounded-md px-2 py-2">
            <UserBadge role={role} />
            <span>{name}</span>
            <button
                onClick={logoutUser}
                className="cursor-pointer"
                aria-label="Logout"
                title="Logout"
            >
                <LogOut size={18} />
            </button>
        </div>
    );
}

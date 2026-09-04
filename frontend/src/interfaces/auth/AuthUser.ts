import type { UserRole } from "../../constants/UserConstant";

export interface AuthUser {
    userId: string;
    name: string;
    email: string;
    roles: UserRole[];
}

import type { UserRole } from "../../constants/UserConstant";
import type { UserStatus } from "../../constants/UserConstant";

export interface UserUpdateDto {
    name?: string | null;
    email?: string | null;
    status?: UserStatus | null;
    role?: UserRole | null;
}
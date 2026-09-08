import type { UserRole } from "../../constants/UserConstant";

export interface UserCreateRequest {
    name: string;
    email: string;
    password: string;
    role: UserRole;
}

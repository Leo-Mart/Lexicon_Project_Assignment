import type { UserRole } from "../../constants/UserConstant";

export interface UserCreateDto {
    name: string;
    email: string;
    password: string;
    role: UserRole;
}
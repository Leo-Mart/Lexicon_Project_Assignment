import type { UserStatus } from "../../constants/UserConstant";

export interface UserDto {
    id: string;
    name: string;
    email: string;
    status: UserStatus;
}
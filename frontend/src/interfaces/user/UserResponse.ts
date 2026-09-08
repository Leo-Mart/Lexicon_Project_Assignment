import type { UserStatus } from "../../constants/UserConstant";

export interface UserResponse {
    id: string;
    name: string;
    email: string;
    status: UserStatus;
}

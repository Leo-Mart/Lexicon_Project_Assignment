import type { UserStatus } from "../../constants/UserConstant";

export interface UserStatusUpdateDto {
    status: UserStatus;
}
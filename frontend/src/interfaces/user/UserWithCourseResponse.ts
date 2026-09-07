import type { UserStatus } from "../../constants/UserConstant";
import type { UserRole } from "../../constants/UserConstant";

export interface UserWithCourseResponse {
    id: string;
    name: string;
    email: string | null;
    status: UserStatus;
    role: UserRole;
    courseId: string | null;
    courseName: string | null;
}
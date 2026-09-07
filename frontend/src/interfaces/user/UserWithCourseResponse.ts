import type { UserStatus } from "../../constants/UserConstant";

export interface UserWithCourseResponse {
    id: string;
    name: string;
    email: string | null;
    status: UserStatus;
    courseId: string | null;
    courseName: string | null;
}
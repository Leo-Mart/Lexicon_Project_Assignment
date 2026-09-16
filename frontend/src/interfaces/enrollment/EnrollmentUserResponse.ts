import type { UserResponse } from "../user/UserResponse";

export interface EnrollmentUserResponse {
    studentId: string;
    courseId: string;
    student: UserResponse;
}

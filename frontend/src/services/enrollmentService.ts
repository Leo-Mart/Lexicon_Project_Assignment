import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod } from "../constants/Constants";
import type { CourseResponse } from "../interfaces/course/CourseResponse";

const API_URL = API_BASE_URL + "/enrollments";

export const fetchStudentCourse = async (): Promise<CourseResponse> => {
    const response = await authFetch(`${API_URL}/course`);

    if (!response.ok) {
        throw new Error(`Failed to fetch student course: ${response.status}`);
    }

    return (await response.json()) as CourseResponse;
};

export const assignOrChangeCourse = async (
    studentId: string,
    courseId: string,
): Promise<void> => {
    const response = await authFetch(
        `${API_URL}/students/${studentId}/course/${courseId}`,
        {
            method: HttpMethod.PUT,
        },
    );

    if (!response.ok) {
        throw new Error(
            `Could not assign or change course: ${response.status}`,
        );
    }
};

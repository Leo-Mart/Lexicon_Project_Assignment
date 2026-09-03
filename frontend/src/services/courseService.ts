import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { CourseRequest } from "../interfaces/course/CourseRequest";

const API_URL = API_BASE_URL + "/courses";

export const fetchCourses = async (): Promise<CourseResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch course: ${response.status}`);
    }

    return (await response.json()) as CourseResponse[];
};

export const fetchCourse = async (id: string): Promise<CourseResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch course: ${response.status}`);
    }

    return (await response.json()) as CourseResponse;
};

export const deleteCourse = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the course: ${response.status}`);
    }
};

export const createCourse = async (
    newCourse: CourseRequest,
): Promise<CourseResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newCourse),
    });

    if (!response.ok) {
        throw new Error(`Could not create the course: ${response.status}`);
    }

    return (await response.json()) as CourseResponse;
};

export const updateCourse = async (
    id: string,
    updateCourse: CourseRequest,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updateCourse),
    });

    if (!response.ok) {
        throw new Error(`Could not update the course: ${response.status}`);
    }
};

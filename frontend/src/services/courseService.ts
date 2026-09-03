import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { CourseDto } from "../interfaces/course/CourseDto";
import type { CreateNewCourseDto } from "../interfaces/course/CreateNewCourseDto";
import type { UpdateCourseDto } from "../interfaces/course/UpdateCourseDto";

const API_URL = API_BASE_URL + "/courses";

export const fetchCourses = async (): Promise<CourseDto[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch course: ${response.status}`);
    }

    return (await response.json()) as CourseDto[];
};

export const fetchCourse = async (id: string): Promise<CourseDto> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch course: ${response.status}`);
    }

    return (await response.json()) as CourseDto;
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
    newCourse: CreateNewCourseDto,
): Promise<CourseDto> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newCourse),
    });

    if (!response.ok) {
        throw new Error(`Could not create the course: ${response.status}`);
    }

    return (await response.json()) as CourseDto;
};

export const updateCourse = async (
    id: string,
    updateCourse: UpdateCourseDto,
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

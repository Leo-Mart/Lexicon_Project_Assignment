import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { CourseRequest } from "../interfaces/course/CourseRequest";
import type { PagedResponse } from "../interfaces/common/PagedResponse";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";

const API_URL = API_BASE_URL + "/courses";

export const fetchCourses = async (
    query: QueryParameters,
): Promise<PagedResponse<CourseResponse>> => {
    const params = new URLSearchParams({
        search: query.search,
        sortBy: query.sortBy,
        direction: query.direction,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
    });

    const response = await authFetch(`${API_URL}?${params.toString()}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch courses: ${response.status}`);
    }

    return (await response.json()) as PagedResponse<CourseResponse>;
};

export const fetchCourse = async (id: string): Promise<CourseResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch course: ${response.status}`);
    }

    return (await response.json()) as CourseResponse;
};

export const fetchModulesForCourse = async (
    courseId: string,
): Promise<ModuleResponse[]> => {
    const response = await authFetch(`${API_URL}/${courseId}/get-modules`);

    if (!response.ok) {
        throw new Error(
            `Failed to fetch modules for course: ${response.status}`,
        );
    }

    return (await response.json()) as ModuleResponse[];
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

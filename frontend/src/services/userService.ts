import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { UserResponse } from "../interfaces/user/UserResponse";
import type { UserCreateRequest as UserCreateRequest } from "../interfaces/user/UserCreateRequest";
import type { UserUpdateRequest } from "../interfaces/user/UserUpdateRequest";
import type { UserStatusUpdateDto } from "../interfaces/user/UserStatusUpdateDto";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import type { PagedResponse } from "../interfaces/common/PagedResponse";
import type { QueryParameters } from "../interfaces/common/QueryParameters";

const API_URL = API_BASE_URL + "/users";

export const fetchUsers = async (): Promise<UserResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch user: ${response.status}`);
    }

    return (await response.json()) as UserResponse[];
};

export const fetchUser = async (id: string): Promise<UserResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch user: ${response.status}`);
    }

    return (await response.json()) as UserResponse;
};

export const deleteUser = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the user: ${response.status}`);
    }
};

export const createUser = async (
    newUser: UserCreateRequest,
): Promise<UserResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newUser),
    });

    if (!response.ok) {
        throw new Error(`Could not create the user: ${response.status}`);
    }

    return (await response.json()) as UserResponse;
};

export const updateUser = async (
    id: string,
    updatedUser: UserUpdateRequest,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updatedUser),
    });

    if (!response.ok) {
        throw new Error(`Could not update the course: ${response.status}`);
    }
};

export const updateUserStatus = async (
    id: string,
    statusUpdate: UserStatusUpdateDto,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}/status`, {
        method: HttpMethod.PATCH,
        headers: JSON_HEADERS,
        body: JSON.stringify(statusUpdate),
    });

    if (!response.ok) {
        throw new Error(`Could not update user status: ${response.status}`);
    }
};

export const fetchUsersWithCourse = async (
    query: QueryParameters,
): Promise<PagedResponse<UserWithCourseResponse>> => {
    const params = new URLSearchParams({
        search: query.search,
        sortBy: query.sortBy,
        direction: query.direction,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
    });

    const response = await authFetch(
        `${API_URL}/with-course?${params.toString()}`,
    );

    if (!response.ok) {
        throw new Error(
            `Failed to fetch users with course: ${response.status}`,
        );
    }

    return (await response.json()) as PagedResponse<UserWithCourseResponse>;
};

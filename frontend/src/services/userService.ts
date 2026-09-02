import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { UserDto } from "../interfaces/user/UserDto";
import type { UserCreateDto } from "../interfaces/user/UserCreateDto";
import type { UserUpdateDto } from "../interfaces/user/UserUpdateDto";
import type { UserStatusUpdateDto } from "../interfaces/user/UserStatusUpdateDto";

const API_URL = API_BASE_URL + "/users";

export const fetchUsers = async (): Promise<UserDto[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch user: ${response.status}`);
    }

    return (await response.json()) as UserDto[];
};

export const fetchUser = async (id: string): Promise<UserDto> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch user: ${response.status}`);
    }

    return (await response.json()) as UserDto;
};

export const deleteUser = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the user: ${response.status}`);
    }
};

export const createCourse = async (
    newUser: UserCreateDto,
): Promise<UserDto> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newUser),
    });

    if (!response.ok) {
        throw new Error(`Could not create the user: ${response.status}`);
    }

    return (await response.json()) as UserDto;
};

export const updateUser = async (
    id: string,
    updatedUser: UserUpdateDto,
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

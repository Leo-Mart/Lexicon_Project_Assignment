import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ActivityDto } from "../interfaces/activity/ActivityDto";
import type { ActivityCreateDto } from "../interfaces/activity/ActivityCreateDto";
import type { ActivityUpdateDto } from "../interfaces/activity/ActivityUpdateDto";

const API_URL = API_BASE_URL + "/course";

export const fetchActivitys = async (): Promise<ActivityDto[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch activity: ${response.status}`);
    }

    return (await response.json()) as ActivityDto[];
};

export const fetchActivity = async (id: string): Promise<ActivityDto> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch activity: ${response.status}`);
    }

    return (await response.json()) as ActivityDto;
};

export const deleteActivity = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the activity: ${response.status}`);
    }
};

export const createActivity = async (
    newActivity: ActivityCreateDto,
): Promise<ActivityDto> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newActivity),
    });

    if (!response.ok) {
        throw new Error(`Could not create the course: ${response.status}`);
    }

    return (await response.json()) as ActivityDto;
};

export const updateCourse = async (
    id: string,
    updatedActivity: ActivityUpdateDto,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updatedActivity),
    });

    if (!response.ok) {
        throw new Error(`Could not update the activity: ${response.status}`);
    }
};

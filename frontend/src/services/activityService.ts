import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";

const API_URL = API_BASE_URL + "/activity";

export const fetchActivitys = async (): Promise<ActivityResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch activity: ${response.status}`);
    }

    return (await response.json()) as ActivityResponse[];
};

export const fetchActivity = async (id: string): Promise<ActivityResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch activity: ${response.status}`);
    }

    return (await response.json()) as ActivityResponse;
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
    newActivity: ActivityRequest,
): Promise<ActivityResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newActivity),
    });

    if (!response.ok) {
        throw new Error(`Could not create the course: ${response.status}`);
    }

    return (await response.json()) as ActivityResponse;
};

export const updateCourse = async (
    id: string,
    updatedActivity: ActivityRequest,
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

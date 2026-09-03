import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";

const API_URL = API_BASE_URL + "/resources";

export const fetchResources = async (): Promise<ResourceResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceResponse[];
};

export const fetchResource = async (id: string): Promise<ResourceResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceResponse;
};

export const deleteResource = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the Resource: ${response.status}`);
    }
};

export const createResource = async (
    newResource: ResourceRequest,
): Promise<ResourceResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newResource),
    });

    if (!response.ok) {
        throw new Error(`Could not create the Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceResponse;
};

export const updateResource = async (
    id: string,
    updatedResource: ResourceRequest,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updatedResource),
    });

    if (!response.ok) {
        throw new Error(`Could not update the Resource: ${response.status}`);
    }
};

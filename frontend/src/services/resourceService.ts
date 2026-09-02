import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ResourceDto } from "../interfaces/resource/ResourceDto";
import type { ResourceCreateDto } from "../interfaces/resource/ResourceCreateDto";
import type { ResourceUpdateDto } from "../interfaces/resource/ResourceUpdateDto";
 
const API_URL = API_BASE_URL + "/resources";

export const fetchResources = async (): Promise<ResourceDto[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceDto[];
};

export const fetchResource = async (id: string): Promise<ResourceDto> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceDto;
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
    newResource: ResourceCreateDto,
): Promise<ResourceDto> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newResource),
    });

    if (!response.ok) {
        throw new Error(`Could not create the Resource: ${response.status}`);
    }

    return (await response.json()) as ResourceDto;
};

export const updateResource = async (
    id: string,
    updatedResource: ResourceUpdateDto,
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
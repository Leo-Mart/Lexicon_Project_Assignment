import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { ModuleRequest } from "../interfaces/module/ModuleRequest";

const API_URL = API_BASE_URL + "/modules";

export const fetchModuleById = async (
    moduleId: string,
): Promise<ModuleResponse> => {
    const response = await authFetch(`${API_URL}/${moduleId}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch module: ${response.status}`);
    }

    return (await response.json()) as ModuleResponse;
};

export const fetchModules = async (): Promise<ModuleResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch module: ${response.status}`);
    }

    return (await response.json()) as ModuleResponse[];
};

export const deleteModule = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (!response.ok) {
        throw new Error(`Could not delete the module: ${response.status}`);
    }
};

export const createModule = async (
    newModule: ModuleRequest,
): Promise<ModuleResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newModule),
    });

    if (!response.ok) {
        throw new Error(`Could not create the module: ${response.status}`);
    }

    return (await response.json()) as ModuleResponse;
};

export const updateModule = async (
    id: string,
    updatedModule: ModuleRequest,
): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updatedModule),
    });

    if (!response.ok) {
        throw new Error(`Could not update the module: ${response.status}`);
    }
};

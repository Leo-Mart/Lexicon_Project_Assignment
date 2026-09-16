import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { ModuleRequest } from "../interfaces/module/ModuleRequest";
import type { ErrorResponse } from "../interfaces/error/ErrorResponse";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import type { PagedResponse } from "../interfaces/common/PagedResponse";

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

export const fetchModules = async (
    query: QueryParameters,
): Promise<PagedResponse<ModuleResponse>> => {
    const params = new URLSearchParams({
        search: query.search,
        sortBy: query.sortBy,
        direction: query.direction,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
    });

    const response = await authFetch(`${API_URL}?${params.toString()}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch module: ${response.status}`);
    }

    return (await response.json()) as PagedResponse<ModuleResponse>;
};

export const deleteModule = async (id: string): Promise<void> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.DELETE,
    });

    if (response.status === 500) {
        const json = (await response.json()) as ErrorResponse;
        console.error(json);
        throw new Error(
            "Error Deleting Module: Cannot delete module with submissions.",
        );
    }
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

    if (response.status === 400) {
        const err = (await response.json()) as ErrorResponse;
        throw new Error(err.detail);
    }

    if (!response.ok) {
        throw new Error(`Could not create the module: ${response.status}`);
    }

    return (await response.json()) as ModuleResponse;
};

export const updateModule = async (
    id: string,
    updatedModule: ModuleRequest,
): Promise<ModuleResponse> => {
    const response = await authFetch(`${API_URL}/${id}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(updatedModule),
    });

    if (response.status === 400) {
        const err = (await response.json()) as ErrorResponse;
        throw new Error(err.detail);
    }

    if (!response.ok) {
        throw new Error(`Could not update the module: ${response.status}`);
    }

    return (await response.json()) as ModuleResponse;
};

import type { Module } from "../interfaces/Module";

const API_URL = "https://localhost:7250/api/modules";

export const fetchModuleById = async (moduleId: string): Promise<Module> => {
    const response = await fetch(`${API_URL}/${moduleId}`);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
};

export const fetchModules = async (): Promise<Module[]> => {
    const response = await fetch(`${API_URL}`);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
};

import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { LoginDto } from "../dtos/auth/LoginDto";
import type { AccessTokenResponse } from "../dtos/auth/AccessTokenDto";

const API_URL = API_BASE_URL + "/auth";

let accessToken: string | null = null;
let refreshPromise: Promise<boolean> | null = null;

export const login = async (loginDto: LoginDto): Promise<void> => {
    const response = await fetch(`${API_URL}/login`, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        credentials: "include",
        body: JSON.stringify(loginDto),
    });

    if (!response.ok) {
        throw new Error(`Login failed: ${response.status}`);
    }

    const data = (await response.json()) as AccessTokenResponse;

    accessToken = data.accessToken;
};

export const refreshSession = async (): Promise<boolean> => {
    try {
        const response = await fetch(`${API_URL}/refresh`, {
            method: HttpMethod.POST,
            credentials: "include",
        });

        if (!response.ok) {
            accessToken = null;
            return false;
        }

        const data = (await response.json()) as AccessTokenResponse;

        accessToken = data.accessToken;

        return true;
    } catch {
        accessToken = null;
        return false;
    }
};

export const authFetch = async (
    input: RequestInfo | URL,
    init: RequestInit = {},
): Promise<Response> => {
    let response = await fetchWithAccessToken(input, init);

    if (response.status !== 401) {
        return response;
    }

    const refreshed = await getRefreshPromise();

    if (!refreshed) {
        return response;
    }

    response = await fetchWithAccessToken(input, init);

    return response;
};

export const checkAuth = async (): Promise<boolean> => {
    const response = await authFetch(`${API_URL}/me`);

    return response.ok;
};

export const logout = async (): Promise<void> => {
    try {
        const response = await fetch(`${API_URL}/logout`, {
            method: HttpMethod.POST,
            credentials: "include",
        });

        if (!response.ok) {
            throw new Error(`Logout failed: ${response.status}`);
        }
    } finally {
        accessToken = null;
    }
};

const fetchWithAccessToken = async (
    input: RequestInfo | URL,
    init: RequestInit = {},
): Promise<Response> => {
    const headers = new Headers(init.headers);

    if (accessToken) {
        headers.set("Authorization", `Bearer ${accessToken}`);
    }

    return fetch(input, {
        ...init,
        headers,
    });
};

const getRefreshPromise = (): Promise<boolean> => {
    if (!refreshPromise) {
        refreshPromise = refreshSession().finally(() => {
            refreshPromise = null;
        });
    }

    return refreshPromise;
};

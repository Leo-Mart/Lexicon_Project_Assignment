import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import type { AccessTokenResponse } from "../interfaces/auth/AccessTokenDto";
import type { AuthUser } from "../interfaces/auth/AuthUser";

const API_URL = API_BASE_URL + "/auth";

let accessToken: string | null = null;
let refreshPromise: Promise<boolean> | null = null;

type Listener = (token: string | null) => void;
const listeners = new Set<Listener>();

const setAccessToken = (token: string | null) => {
    accessToken = token;
    listeners.forEach((l) => l(token));
};

export const getAccessToken = () => accessToken;

export const subscribeToken = (listener: Listener): (() => void) => {
    listeners.add(listener);
    return () => listeners.delete(listener);
};

export const login = async (loginDto: LoginDto): Promise<void> => {
    const response = await fetch(`${API_URL}/login`, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        credentials: "include",
        body: JSON.stringify(loginDto),
    });

    if (response.status === 401) {
        throw new Error("Invalid email and/or password");
    }

    if (!response.ok) {
        throw new Error(`Login failed: ${response.status}`);
    }

    const data = (await response.json()) as AccessTokenResponse;

    setAccessToken(data.accessToken);
};

export const refreshSession = async (): Promise<boolean> => {
    try {
        const response = await fetch(`${API_URL}/refresh`, {
            method: HttpMethod.POST,
            credentials: "include",
        });

        if (!response.ok) {
            setAccessToken(null);
            return false;
        }

        const data = (await response.json()) as AccessTokenResponse;

        setAccessToken(data.accessToken);

        return true;
    } catch {
        setAccessToken(null);
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
        setAccessToken(null);
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

export const getCurrentUser = async (): Promise<AuthUser> => {
    const response = await authFetch(`${API_URL}/me`);

    if (!response.ok) {
        throw new Error(`Could not fetch current user: ${response.status}`);
    }

    return (await response.json()) as AuthUser;
};

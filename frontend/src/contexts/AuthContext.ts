import { createContext } from "react";
import type { LoginDto } from "../interfaces/auth/LoginDto";

type AuthContext = {
    isAuthenticated: boolean;
    isLoading: boolean;
    loginUser: (loginPayload: LoginDto) => void;
    loginError: string | undefined;
    logoutUser: () => void;
};

export const AuthContext = createContext<AuthContext | undefined>(undefined);

import { createContext } from "react";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import type { UserRole } from "../constants/UserConstant";

type AuthContext = {
    isAuthenticated: boolean;
    isLoading: boolean;
    loginUser: (loginPayload: LoginDto) => void;
    loginError: string | undefined;
    logoutUser: () => void;
    name: string | null;
    role: UserRole | null;
};

export const AuthContext = createContext<AuthContext | undefined>(undefined);

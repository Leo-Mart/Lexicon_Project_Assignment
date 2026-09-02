/* eslint-disable react-refresh/only-export-components */
// seems to be an issue with eslint or something see: https://github.com/ArnaudBarre/eslint-plugin-react-refresh/issues/25#issuecomment-1729071347
//https://www.gatsbyjs.com/docs/reference/local-development/fast-refresh/#how-it-works
// second solution is probably better though.
import { createContext, useContext, useState } from "react";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import { loginService, logoutService } from "../services/authService";

interface ProviderProps {
    token: string;
    login: (loginPayload: LoginDto) => void;
    loginError: string | undefined;
    logout: () => void;
}

const AuthContext = createContext<ProviderProps>({
    token: "",
    login: () => {},
    loginError: "",
    logout: () => {},
});

const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const [token, setToken] = useState<string>("");
    const [loginError, setLoginError] = useState<string>();

    const login = async (loginPayload: LoginDto) => {
        setLoginError("");
        try {
            await loginService(loginPayload);
            setToken("bladiba");
        } catch (error) {
            if (error instanceof Error) {
                setLoginError(error.message);
            }
        }
    };

    const logout = () => {
        logoutService();
        // call the authService, and clear state
    };

    return (
        <AuthContext.Provider value={{ token, login, loginError, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth has to be used within a Provider");
    }
    return ctx;
};

export { AuthProvider, useAuth };

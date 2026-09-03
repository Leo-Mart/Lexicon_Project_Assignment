import { useEffect, useState, useSyncExternalStore } from "react";
import {
    getAccessToken,
    login,
    logout,
    refreshSession,
    subscribeToken,
} from "../services/authService";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import { AuthContext } from "./AuthContext";
import Spinner from "../components/Spinner";
import { useNavigate } from "react-router-dom";

const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const token = useSyncExternalStore(
        subscribeToken,
        getAccessToken,
        getAccessToken,
    );
    const nav = useNavigate();
    const [isLoading, setIsLoading] = useState(true);
    const [loginError, setLoginError] = useState<string>();

    useEffect(() => {
        refreshSession().finally(() => setIsLoading(false));
    }, []);

    const loginUser = async (loginPayload: LoginDto) => {
        setLoginError("");
        try {
            await login(loginPayload);
            nav("/index");
        } catch (error) {
            if (error instanceof Error) {
                setLoginError(error.message);
            }
        }
    };

    const logoutUser = () => {
        logout();
    };

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated: !!token,
                isLoading,
                loginUser,
                loginError,
                logoutUser,
            }}
        >
            {isLoading ? <Spinner /> : children}
        </AuthContext.Provider>
    );
};

export default AuthProvider;

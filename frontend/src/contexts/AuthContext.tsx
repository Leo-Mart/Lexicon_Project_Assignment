/* eslint-disable react-refresh/only-export-components */
// seems to be an issue with eslint or something see: https://github.com/ArnaudBarre/eslint-plugin-react-refresh/issues/25#issuecomment-1729071347
//https://www.gatsbyjs.com/docs/reference/local-development/fast-refresh/#how-it-works
// second solution is probably better though.
import {
    createContext,
    useContext,
    useEffect,
    useState,
    useSyncExternalStore,
} from "react";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import {
    getAccessToken,
    login,
    logout,
    refreshSession,
    subscribeToken,
} from "../services/authService";
import Spinner from "../components/Spinner";

type ProviderProps = {
    isAuthenticated: boolean;
    isLoading: boolean;
    loginUser: (loginPayload: LoginDto) => void;
    loginError: string | undefined;
    logoutUser: () => void;
};

const AuthContext = createContext<ProviderProps | undefined>(undefined);

const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const token = useSyncExternalStore(
        subscribeToken,
        getAccessToken,
        getAccessToken,
    );
    const [isLoading, setIsLoading] = useState(true);
    const [loginError, setLoginError] = useState<string>();

    useEffect(() => {
        refreshSession().finally(() => setIsLoading(false));
    }, []);

    const loginUser = async (loginPayload: LoginDto) => {
        setLoginError("");
        try {
            await login(loginPayload);
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

const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth has to be used within a Provider");
    }
    return ctx;
};

export { AuthProvider, useAuth };

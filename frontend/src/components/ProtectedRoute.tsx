import { useEffect, useState } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { checkAuth } from "../services/authService";

const ProtectedRoute = () => {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(
        null,
    );

    useEffect(() => {
        let isMounted = true;

        const verifyAuthentication = async (): Promise<void> => {
            try {
                const authenticated = await checkAuth();

                if (isMounted) {
                    setIsAuthenticated(authenticated);
                }
            } catch (error) {
                console.error("Authentication check failed:", error);

                if (isMounted) {
                    setIsAuthenticated(false);
                }
            }
        };

        void verifyAuthentication();

        return () => {
            isMounted = false;
        };
    }, []);

    if (isAuthenticated === null) {
        return <p>Loading...</p>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return <Outlet />;
};

export default ProtectedRoute;

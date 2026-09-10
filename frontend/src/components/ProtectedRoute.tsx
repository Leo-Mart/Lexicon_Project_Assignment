import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import type { UserRole } from "../constants/UserConstant";

type ProtectedRouteProps = {
    allowedRoles?: UserRole[];
};

const ProtectedRoute = ({ allowedRoles }: ProtectedRouteProps) => {
    const { isAuthenticated, isLoading, role, courseId } = useAuth();

    if (isLoading) {
        return <p>Loading...</p>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && (!role || !allowedRoles.includes(role))) {
        if (role === "Student" && courseId) {
            return <Navigate to={`/courses/${courseId}`} replace />;
        }

        return <Navigate to="/index" replace />;
    }

    return <Outlet />;
};

export default ProtectedRoute;

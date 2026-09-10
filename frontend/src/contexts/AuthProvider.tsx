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
import { getCurrentUser } from "../services/authService";
import { fetchStudentCourse } from "../services/enrollmentService";

const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const token = useSyncExternalStore(
        subscribeToken,
        getAccessToken,
        getAccessToken,
    );
    const nav = useNavigate();
    const [isLoading, setIsLoading] = useState(true);
    const [loginError, setLoginError] = useState<string>();
    const [name, setName] = useState<string | null>(null);
    const [role, setRole] = useState<"Teacher" | "Student" | null>(null);
    const [courseId, setCourseId] = useState<string | null>(null);

    useEffect(() => {
        const loadUser = async () => {
            const refreshed = await refreshSession();

            if (refreshed) {
                const user = await getCurrentUser();
                const userRole = user.roles[0] ?? null;
                setName(user.name);
                setRole(user.roles[0] ?? null);

                if (userRole === "Student") {
                    const course = await fetchStudentCourse();
                    setCourseId(course.courseId);
                } else {
                    setCourseId(null);
                }
            }

            setIsLoading(false);
        };

        void loadUser();
    }, []);

    const loginUser = async (loginPayload: LoginDto) => {
        setLoginError("");
        try {
            await login(loginPayload);

            const user = await getCurrentUser();
            const userRole = user.roles[0] ?? null;
            setName(user.name);
            setRole(userRole);

            if (userRole === "Teacher") {
                nav("/index");
            } else if (userRole === "Student") {
                const course = await fetchStudentCourse();
                setCourseId(course.courseId);
                nav(`/courses/${course.courseId}`);
            }
        } catch (error) {
            if (error instanceof Error) {
                setLoginError(error.message);
            }
        }
    };

    const logoutUser = () => {
        logout();
        setName(null);
        setRole(null);
        setCourseId(null);
    };

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated: !!token,
                isLoading,
                loginUser,
                loginError,
                logoutUser,
                name,
                role,
                courseId,
            }}
        >
            {isLoading ? <Spinner /> : children}
        </AuthContext.Provider>
    );
};

export default AuthProvider;

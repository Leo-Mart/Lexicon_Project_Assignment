import { Link } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

const NotFound = () => {
    const { role, currentModuleId } = useAuth();

    if (role === "Student") {
        return (
            <div className="text-text-dark  text-3xl h-full min-h-screen dark:text-text-light flex flex-col gap-2 justify-center items-center">
                <p>404 - Page Not Found</p>
                <Link
                    className="hover:underline"
                    to={`/module/${currentModuleId}`}
                >
                    Return home
                </Link>
            </div>
        );
    } else if (role === "Teacher") {
        return (
            <div className="text-text-dark  text-3xl h-full min-h-screen dark:text-text-light flex flex-col gap-2 justify-center items-center">
                <p>404 - Page Not Found</p>
                <Link className="hover:underline" to="/index">
                    Return home
                </Link>
            </div>
        );
    } else {
        return (
            <div className="text-text-dark  text-3xl h-full min-h-screen dark:text-text-light flex flex-col gap-2 justify-center items-center">
                <p>404 - Page Not Found</p>
                <Link className="hover:underline" to="/login">
                    Return to login
                </Link>
            </div>
        );
    }
};

export default NotFound;

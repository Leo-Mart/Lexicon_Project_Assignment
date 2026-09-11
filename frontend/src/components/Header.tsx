import { NavLink } from "react-router-dom";
import { routes } from "../routes/config";
import UserInfo from "./UserInfo";
import { useAuth } from "../hooks/useAuth";

export default function MainHeader() {
    // Filter routes that should appear in the header

    const { isAuthenticated, role, courseId, currentModuleId } = useAuth();
    const homePath = currentModuleId
        ? `/module/${currentModuleId}`
        : `/courses/${courseId}`;

    const headerRoutes = routes.filter((route) => {
        if (!route.createHeader) {
            return false;
        }

        if (isAuthenticated && route.path === "/login") {
            return false;
        }

        if (!route.isProtected) {
            return true;
        }

        if (!isAuthenticated) {
            return false;
        }

        if (!route.allowedRoles) {
            return true;
        }

        return role !== null && route.allowedRoles.includes(role);
    });

    return (
        <nav
            className="relative bg-bg-header dark:bg-bg-header-dark text-text-light min-h-15 flex flex-wrap items-center justify-center"
            role="navigation"
        >
            <ul className="flex gap-5 text-3xl">
                {isAuthenticated && role === "Student" && courseId && (
                    <li>
                        <NavLink to={homePath}>Home</NavLink>
                    </li>
                )}
                {headerRoutes.map((route) => {
                    return (
                        <li key={route.path}>
                            <NavLink
                                to={route.path || "/"} // Handle empty path
                                end={
                                    route.path === "" || route.path === "/index"
                                }
                            >
                                {route.displayName || "Dashboard"}
                            </NavLink>
                        </li>
                    );
                })}
            </ul>
            <div className="w-full flex justify-end px-2  md:w-auto md:absolute md:right-4 md:top-1/2 md:-translate-y-1/2 md:px-0">
                <UserInfo />
            </div>
        </nav>
    );
}

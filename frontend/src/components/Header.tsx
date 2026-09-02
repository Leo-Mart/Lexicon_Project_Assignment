import { NavLink } from "react-router-dom";
import { routes } from "../routes/config"; // Adjust the import path

export default function MainHeader() {
    // Filter routes that should appear in the header
    const headerRoutes = routes.filter((route) => route.createHeader);

    return (
        <nav
            className="bg-bg-header dark:bg-bg-header-dark text-text-light h-15"
            role="navigation"
        >
            <ul className="flex justify-center gap-5 text-3xl">
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
        </nav>
    );
}

import { NavLink } from "react-router-dom";
import { routes } from "../routes/config"; // Adjust the import path
import UserInfo from "./UserInfo";

export default function MainHeader() {
    // Filter routes that should appear in the header
    const headerRoutes = routes.filter((route) => route.createHeader);

    return (
        <nav
            className="relative bg-bg-header dark:bg-bg-header-dark text-text-light min-h-15 flex flex-wrap items-center justify-center"
            role="navigation"
        >
            <ul className="flex gap-5 text-3xl">
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

import { NavLink } from "react-router-dom";

export default function MainHeader() {
    return (
        <>
            <nav
                className="bg-bg-header text-text-light h-15"
                role="navigation"
            >
                <ul className="flex justify-center gap-5 text-3xl">
                    <li>
                        <NavLink to="/" end>
                            <u>Dashboard</u>
                        </NavLink>
                    </li>
                    <li>
                        <NavLink to="/Modules">Modules</NavLink>
                    </li>
                    <li>
                        <NavLink to="/Materials">Materials</NavLink>
                    </li>
                </ul>
            </nav>
        </>
    );
}

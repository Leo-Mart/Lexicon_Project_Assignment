import { useNavigate, useLocation } from "react-router-dom";
import Button from "./Button";
import { useAuth } from "../hooks/useAuth";

const tabs = [
    { label: "Course Management", param: "course-management" },
    { label: "User Management", param: "user-management" },
    { label: "Module Management", param: "module-management" },
    { label: "Resource Management", param: "resource-management" },
    { label: "Submissions", param: "submissions" },
];

// interface DashboardTabsProps {
//     isAbsolute?: boolean;
// }

export default function DashboardTabs() {
    const { isAuthenticated, role } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    const handleTabClick = (param: string) => {
        if (location.pathname === "/index") {
            navigate(`?tab=${param}`);
        } else {
            navigate(`/index?tab=${param}`);
        }
    };

    const searchParams = new URLSearchParams(location.search);
    const activeTabParam = searchParams.get("tab");

    if (isAuthenticated && role !== "Teacher") {
        return <></>;
    }

    return (
        <nav className={`flex flex-col`}>
            <ul className="flex flex-row gap-x-3 rounded-lg p-3">
                {tabs.map((tab) => {
                    const isActive = activeTabParam === tab.param;
                    return (
                        <div className="" key={tab.param}>
                            <Button
                                onClick={() => handleTabClick(tab.param)}
                                className={`size-full hover:cursor-pointer ${
                                    isActive
                                        ? "bg-accent-teacher text-white"
                                        : ""
                                }`}
                            >
                                {tab.label}
                            </Button>
                        </div>
                    );
                })}
            </ul>
        </nav>
    );
}

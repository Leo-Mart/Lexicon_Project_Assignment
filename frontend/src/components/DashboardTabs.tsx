import { useNavigate, useLocation } from "react-router-dom";
import Button from "./Button";

const tabs = [
    { label: "Course Management", param: "course-management" },
    { label: "User Management", param: "user-management" },
    { label: "Resource Management", param: "resource-management" },
    { label: "Submissions", param: "submissions" },
];

export default function DashboardTabs() {
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

    return (
        <nav className="flex flex-col pr-3 w-70">
            <ul className="flex flex-col gap-5 border-4 border-accent-teacher rounded-lg p-3 h-fit">
                {tabs.map((tab) => {
                    const isActive = activeTabParam === tab.param;
                    return (
                        <li key={tab.param}>
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
                        </li>
                    );
                })}
            </ul>
        </nav>
    );
}

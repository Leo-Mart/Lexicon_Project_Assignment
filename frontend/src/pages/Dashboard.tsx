import { useState } from "react";
import Button from "../components/Button";
import CourseListPage from "./CourseListPage";
import ResourceManagement from "./ResourceManagement";
import Users from "./Users";
import Submissions from "./Submissions";
import ModuleListPage from "./ModuleListPage";

const tabs = [
    { label: "Overview" },
    { label: "Course Management" },
    { label: "Module Management" },
    { label: "User Management" },
    { label: "Resource Management" },
    { label: "Submissions" },
];

export default function Dashboard() {
    const [activeTab, setActiveTab] = useState(1);
    return (
        <div className="bg-bg flex dark:bg-bg-dark min-h-screen p-10">
            <nav className="flex flex-col w-2/13 pr-3">
                <ul className="flex flex-col gap-5 border-4 border-accent-teacher rounded-lg p-3 h-fit">
                    {tabs.map((tab, index) => {
                        return (
                            <li key={index}>
                                <Button
                                    onClick={() => setActiveTab(index + 1)}
                                    className="size-full hover:cursor-pointer"
                                >
                                    {tab.label}
                                </Button>
                            </li>
                        );
                    })}
                </ul>
            </nav>
            <div className="w-full">
                {activeTab === 1 && (
                    <div className=" text-text-dark dark:text-text-light">
                        Maybe some overview fields here?
                    </div>
                )}
                {activeTab === 2 && (
                    <div>
                        <CourseListPage />
                    </div>
                )}
                {activeTab === 3 && (
                    <div>
                        <ModuleListPage />
                    </div>
                )}
                {activeTab === 4 && (
                    <div className="text-text-light">
                        <div>
                            {" "}
                            <Users />
                        </div>
                    </div>
                )}
                {activeTab === 5 && (
                    <div className="text-text-light">
                        <ResourceManagement />
                    </div>
                )}
                {activeTab === 6 && <Submissions />}
            </div>
        </div>
    );
}

import { useState } from "react";
import Button from "../components/Button";
import Schedule from "../components/Schedule";
import CourseListPage from "./CourseListPage";
import ResourceManagement from "./ResourceManagement";
import Users from "./Users";

const tabs = [
    { label: "Overview" },
    { label: "Course Management" },
    { label: "User Management" },
    { label: "Resource Management" },
    { label: "Schedule" },
];

export default function Dashboard() {
    const [activeTab, setActiveTab] = useState(1);
    return (
        <div className="bg-bg flex dark:bg-bg-dark min-h-screen p-10">
            <nav className="flex flex-col w-1/7 mr-3">
                <ul className="flex flex-col gap-5">
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
                    <div className="text-text-light">
                        Maybe some overview fields here?
                    </div>
                )}
                {activeTab === 2 && (
                    <div>
                        <CourseListPage />
                    </div>
                )}
                {activeTab === 3 && (
                    <div className="text-text-light">
                        <div>
                            {" "}
                            <Users />
                        </div>
                    </div>
                )}
                {activeTab === 4 && (
                    <div className="text-text-light">
                        <ResourceManagement />
                    </div>
                )}
                {activeTab === 5 && (
                    <div className="text-text-light">
                        <Schedule />
                    </div>
                )}
            </div>
        </div>
    );
}

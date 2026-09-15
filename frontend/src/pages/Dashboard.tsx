import { useSearchParams } from "react-router-dom";
import DashboardTabs from "../components/DashboardTabs";
import CourseListPage from "./CourseListPage";
import ResourceManagement from "./ResourceManagement";
import Users from "./Users";
import Submissions from "./Submissions";

export default function Dashboard() {
    const [searchParams] = useSearchParams();
    const activeTabParam = searchParams.get("tab");

    const getActiveTabIndex = () => {
        switch (activeTabParam) {
            case "user-management":
                return 2;
            case "resource-management":
                return 3;
            case "submissions":
                return 4;
            case "course-management":
            default:
                return 1;
        }
    };

    const activeTab = getActiveTabIndex();

    return (
        <div className="bg-bg flex dark:bg-bg-dark min-h-screen p-10">
            <DashboardTabs />
            <div className="w-full">
                {activeTab === 1 && <CourseListPage />}
                {activeTab === 2 && <Users />}
                {activeTab === 3 && <ResourceManagement />}
                {activeTab === 4 && <Submissions />}
            </div>
        </div>
    );
}

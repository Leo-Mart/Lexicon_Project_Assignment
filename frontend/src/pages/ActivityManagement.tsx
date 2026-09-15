// import { useEffect, useState } from "react";
// import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
// import type { SortOption } from "../types/SortOption";
// import { fetchActivities } from "../services/activityService";
// const ACTIVITY_SORT_OPTIONS: SortOption[] = [
//     { value: "name-asc", label: "Name A-Z" },
//     { value: "name-desc", label: "Name Z-A" },
//     { value: "description-asc", label: "Description A-Z" },
//     { value: "description-desc", label: "Description Z-A" },
// ];
// const DEFAULT_PAGE = 1;
// const DEFAULT_PAGE_SIZE = 10;

const ActivityManagement = () => {
    // const [loading, setLoading] = useState<boolean>(false);
    // const [error, setError] = useState<string | null>(null);
    // const [activities, setActivities] = useState<
    //     ActivityResponse[] | undefined
    // >(undefined);
    // const [search, setSearch] = useState("");
    // const [sortBy, setSortBy] = useState("name-asc");
    // const [page, setPage] = useState(DEFAULT_PAGE);
    // const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    // const [totalCount, setTotalCount] = useState(0);
    // useEffect(() => {
    //     const fetchAllActivities = async () => {
    //         setLoading(true);
    //         setError(null);
    //         try {
    //             const [sortField, sortDirection = "asc"] = sortBy.split("-");
    //
    //             const courseData = await fetchActivities({
    //                 search,
    //                 sortBy: sortField,
    //                 direction: sortDirection,
    //                 page,
    //                 pageSize,
    //             });
    //             setActivities(courseData.items);
    //             setTotalCount(courseData.totalCount);
    //         } catch (err) {
    //             setError(
    //                 err instanceof Error
    //                     ? err.message
    //                     : "Failed to fetch resources",
    //             );
    //             console.error("Fetch error:", err);
    //         } finally {
    //             setLoading(false);
    //         }
    //     };
    //
    //     fetchAllActivities();
    // }, [sortBy, search, page, pageSize]);
    //
    // return <div>Activity Management</div>;
};

export default ActivityManagement;

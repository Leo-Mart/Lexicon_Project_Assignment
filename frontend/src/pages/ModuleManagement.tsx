import { useEffect, useState } from "react";
import { fetchModules } from "../services/moduleService";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";

const MODULE_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
];
const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 10;

const ModuleManagement = () => {
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [modules, setModules] = useState<ModuleResponse[] | undefined>(
        undefined,
    );
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("name-asc");
    const [page, setPage] = useState(DEFAULT_PAGE);
    const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    const [totalCount, setTotalCount] = useState(0);

    useEffect(() => {
        const fetchAllModules = async () => {
            setLoading(true);
            setError(null);
            try {
                const [sortField, sortDirection = "asc"] = sortBy.split("-");

                const courseData = await fetchModules({
                    search,
                    sortBy: sortField,
                    direction: sortDirection,
                    page,
                    pageSize,
                });
                setModules(courseData.items);
                setTotalCount(courseData.totalCount);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch resources",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAllModules();
    }, [sortBy, search, page, pageSize]);

    return <div>Module Management</div>;
};

export default ModuleManagement;

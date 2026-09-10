import { useEffect, useState } from "react";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import { fetchResources } from "../services/resourceService";
import Spinner from "./Spinner";
import type { Column } from "../types/Column";
import DataTable from "./DataTable";

const ResourceList = () => {
    const [resources, setResources] = useState<ResourceResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchAllResources = async () => {
            setLoading(true);
            setError(null);
            try {
                const courseData = await fetchResources({
                    search: "",
                    sortBy: "name",
                    direction: "asc",
                    page: 1,
                    pageSize: 200,
                });
                setResources(courseData.items);
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

        fetchAllResources();
    }, []);

    const resourceColumns: Column<ResourceResponse>[] = [
        { key: "name", header: "Name", field: "name", render: (r) => r.name },
        {
            key: "description",
            header: "Description",
            field: "description",
            render: (r) => r.description,
        },
        {
            key: "content",
            header: "Content",
            field: "content",
            render: (r) => r.content,
        },
        { key: "url", header: "URL", field: "url", render: (r) => r.uri },
        { key: "createdAt", header: "Created At", render: (r) => r.createdAt },
    ];

    return (
        <div>
            {loading && <Spinner />}
            {error && <div className="text-red-700">{error}</div>}
            <DataTable
                items={resources}
                columns={resourceColumns}
                getKey={(resource) => resource.resourceId}
                sortBy=""
                isLoading={loading}
                onSortChange={() => {}}
            />
        </div>
    );
};

export default ResourceList;

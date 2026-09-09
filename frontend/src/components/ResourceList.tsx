import { useEffect, useState } from "react";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import ResourceListItem from "./ResourceListItem";
import { fetchResources } from "../services/resourceService";
import Spinner from "./Spinner";

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
    return (
        <div>
            {loading && <Spinner />}
            {error && <div className="text-red-700">{error}</div>}
            <table className="w-full table-auto text-left text-text-dark dark:text-text-light">
                <thead className="bg-bg-window dark:bg-bg-window-dark h-10 border-b border-accent-blue text-text-dark dark:text-text-light">
                    <tr>
                        <th className="px-2">Name</th>
                        <th className="px-2">Description</th>
                        <th className="px-2">Content</th>
                        <th className="px-2">URL</th>
                        <th className="px-2">Created At</th>
                    </tr>
                </thead>
                <tbody>
                    {resources.map((resource, index) => (
                        <ResourceListItem
                            item={resource}
                            index={index}
                            key={resource.name}
                        />
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default ResourceList;

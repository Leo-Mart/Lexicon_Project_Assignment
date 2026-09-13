import { useEffect, useState } from "react";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import {
    deleteResource,
    fetchResources,
    updateResource,
} from "../services/resourceService";
import Spinner from "./Spinner";
import type { Column } from "../types/Column";
import DataTable from "./DataTable";
import TableToolbar from "./TableToolbar";
import type { SortOption } from "../types/SortOption";
import ModalCreateResource from "./ModalCreateResource";
import { createPortal } from "react-dom";
import Button from "./Button";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import ConfirmDialog from "./ConfirmDialog";
import FormModal from "./FormModal";
import { createResourceFormConfig } from "../types/formSchemas";
import { ActivityDate } from "../utils/ActivityTimeConverter";

const RESOURCE_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
];

const ResourceList = () => {
    const [resources, setResources] = useState<ResourceResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("name-asc");
    const [showCreateResourceModal, setShowCreateResourceModal] =
        useState(false);
    const [editingResource, setEditingResource] = useState<
        ResourceResponse | undefined
    >(undefined);
    const [deletingResource, setDeletingResource] = useState<
        ResourceResponse | undefined
    >(undefined);

    const handleSortChange = (value: string) => {
        setSortBy(value);
    };

    const handleSearchChange = (value: string) => {
        setSearch(value);
    };

    const handleResourceEdit = async (
        resourceId: string,
        payload: ResourceRequest,
    ) => {
        await updateResource(resourceId, payload);
        const updatedResources: ResourceResponse[] = resources!.map(
            (resource) => {
                if (resource.resourceId === resourceId) {
                    resource.name = payload.name;
                    resource.description = payload.description;
                    resource.content = payload.content;
                    resource.uri = payload.uri ?? undefined;

                    return resource;
                } else {
                    return resource;
                }
            },
        );
        setResources(updatedResources);
    };
    const handleDeleteResource = async (resourceId: string) => {
        await deleteResource(resourceId);
        setResources(
            resources!.filter((resource) => resource.resourceId !== resourceId),
        );
        setDeletingResource(undefined);
    };

    useEffect(() => {
        const fetchAllResources = async () => {
            setLoading(true);
            setError(null);
            try {
                const [sortField, sortDirection = "asc"] = sortBy.split("-");

                const courseData = await fetchResources({
                    search: "",
                    sortBy: sortField,
                    direction: sortDirection,
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
    }, [sortBy]);

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
        {
            key: "createdAt",
            header: "Created At",
            render: (r) => ActivityDate(r.createdAt),
        },
        {
            key: "actions",
            header: "Actions",
            className: "whitespace-nowrap",
            render: (resource) => (
                <div className="flex items-center gap-2">
                    <Button onClick={() => setEditingResource(resource)}>
                        Edit
                    </Button>
                    <Button
                        variant="cancel"
                        onClick={() => setDeletingResource(resource)}
                    >
                        Delete
                    </Button>
                </div>
            ),
        },
    ];

    return (
        <div>
            {loading && <Spinner />}
            {error && <div className="text-red-700">{error}</div>}
            <TableToolbar
                tableTitle="Resources"
                search={search}
                sortBy={sortBy}
                sortOptions={RESOURCE_SORT_OPTIONS}
                onSearchChange={handleSearchChange}
                onSortChange={handleSortChange}
                addAction={{
                    label: "Add Resource",
                    onAdd: () => setShowCreateResourceModal(true),
                }}
            />
            <DataTable
                items={resources}
                columns={resourceColumns}
                getKey={(resource) => resource.resourceId}
                sortBy={sortBy}
                isLoading={loading}
                onSortChange={handleSortChange}
            />
            {showCreateResourceModal &&
                createPortal(
                    <ModalCreateResource
                        open={showCreateResourceModal}
                        onClose={() => setShowCreateResourceModal(false)}
                        entityId="123"
                        createFor="course"
                    />,

                    document.body,
                )}
            {deletingResource && (
                <ConfirmDialog
                    open={true}
                    title="Delete Resource"
                    message={`Are you sure you want to delete the resource: ${deletingResource.name}`}
                    onCancel={() => setDeletingResource(undefined)}
                    onConfirm={() =>
                        handleDeleteResource(deletingResource.resourceId)
                    }
                />
            )}
            {editingResource && (
                <FormModal
                    config={createResourceFormConfig}
                    initialValue={{
                        name: editingResource.name,
                        description: editingResource.description,
                        content: editingResource.content,
                        uri: editingResource.uri,
                    }}
                    onSave={async (data) =>
                        handleResourceEdit(editingResource?.resourceId, data)
                    }
                    onClose={() => setEditingResource(undefined)}
                />
            )}
        </div>
    );
};

export default ResourceList;

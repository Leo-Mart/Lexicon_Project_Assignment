import { useEffect, useState } from "react";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import {
    addResourceToActivity,
    addResourceToCourse,
    addResourceToModule,
    deleteResource,
    fetchResources,
    updateResource,
} from "../services/resourceService";
import type { Column } from "../types/Column";
import type { SortOption } from "../types/SortOption";
import { createPortal } from "react-dom";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import { editResourceFormConfig } from "../types/formSchemas";
import { ActivityDate } from "../utils/ActivityTimeConverter";
import Button from "../components/Button";
import Spinner from "../components/Spinner";
import TableToolbar from "../components/TableToolbar";
import DataTable from "../components/DataTable";
import Pagination from "../components/Pagination";
import ModalCreateResource from "../components/ModalCreateResource";
import ModalAddResourceToEntity from "../components/ModalAddResourceToEntity";
import ConfirmDialog from "../components/ConfirmDialog";
import FormModal from "../components/FormModal";

const RESOURCE_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
];
const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 10;

const ResourceManagement = () => {
    const [resources, setResources] = useState<ResourceResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("name-asc");
    const [page, setPage] = useState(DEFAULT_PAGE);
    const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    const [totalCount, setTotalCount] = useState(0);
    const [showCreateResourceModal, setShowCreateResourceModal] =
        useState(false);
    const [addingResourceToEntity, setAddingResourceToEntity] = useState<
        ResourceResponse | undefined
    >(undefined);
    const [editingResource, setEditingResource] = useState<
        ResourceResponse | undefined
    >(undefined);
    const [deletingResource, setDeletingResource] = useState<
        ResourceResponse | undefined
    >(undefined);

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(DEFAULT_PAGE);
    };

    const handleSearchChange = (value: string) => {
        setSearch(value);
        setPage(DEFAULT_PAGE);
    };

    const handleAssignResource = async (
        resourceId: string,
        entityId: string,
        forEntity: string,
    ) => {
        switch (forEntity) {
            case "course":
                try {
                    await addResourceToCourse(resourceId, entityId);
                    setAddingResourceToEntity(undefined);
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }

                break;

            case "module":
                try {
                    await addResourceToModule(resourceId, entityId);
                    setAddingResourceToEntity(undefined);
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }
                break;

            case "activity":
                try {
                    await addResourceToActivity(resourceId, entityId);
                    setAddingResourceToEntity(undefined);
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }
                break;

            default:
                throw new Error("THat entity does not exist, try again.");
        }
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
                    search,
                    sortBy: sortField,
                    direction: sortDirection,
                    page,
                    pageSize,
                });
                setResources(courseData.items);
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

        fetchAllResources();
    }, [sortBy, search, page, pageSize]);

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
            field: "createdat",
            render: (r) => ActivityDate(r.createdAt),
        },
        {
            key: "actions",
            header: "Actions",
            className: "whitespace-nowrap pr-4",
            render: (resource) => (
                <div className="flex items-center gap-2">
                    <Button
                        className="hover:cursor-pointer"
                        onClick={() => setEditingResource(resource)}
                    >
                        Edit
                    </Button>
                    <Button
                        className="hover:cursor-pointer"
                        onClick={() => setAddingResourceToEntity(resource)}
                    >
                        Add to Entity
                    </Button>
                    <Button
                        variant="cancel"
                        className="hover:cursor-pointer"
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
            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={setPage}
            />
            {showCreateResourceModal &&
                createPortal(
                    <ModalCreateResource
                        open={showCreateResourceModal}
                        onClose={() => setShowCreateResourceModal(false)}
                    />,
                    document.body,
                )}

            {addingResourceToEntity &&
                createPortal(
                    <ModalAddResourceToEntity
                        open={true}
                        resourceId={addingResourceToEntity.resourceId}
                        onClose={() => setAddingResourceToEntity(undefined)}
                        handleAddToEntity={handleAssignResource}
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
                    config={editResourceFormConfig}
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

export default ResourceManagement;

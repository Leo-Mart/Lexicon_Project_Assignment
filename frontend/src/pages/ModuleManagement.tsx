import { useEffect, useState } from "react";
import { deleteModule, fetchModules } from "../services/moduleService";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { Column } from "../types/Column";
import type { SortOption } from "../types/SortOption";
import Button from "../components/Button";
import { Link } from "react-router-dom";
import ErrorDisplay from "../components/ErrorDisplay";
import TableToolbar from "../components/TableToolbar";
import DataTable from "../components/DataTable";
import Pagination from "../components/Pagination";
import ConfirmDialog from "../components/ConfirmDialog";
import { createPortal } from "react-dom";
import ModalCreateModule from "../components/ModalCreateModule";

const MODULE_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
    { value: "start-asc", label: "Start Date Old-New" },
    { value: "start-desc", label: "Start Date New-Old" },
    { value: "end-asc", label: "End Date Old-New" },
    { value: "end-desc", label: "End Date New-Old" },
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

    const [showCreateModuleModal, setShowCreateModuleModal] = useState(false);

    const [editingModule, setEditingModule] = useState<
        ModuleResponse | undefined
    >(undefined);
    const [deletingModule, setDeletingModule] = useState<
        ModuleResponse | undefined
    >(undefined);

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(DEFAULT_PAGE);
    };

    const handleSearchChange = (value: string) => {
        setSearch(value);
        setPage(DEFAULT_PAGE);
    };

    const handleModuleState = (newModule: ModuleResponse) => {
        setModules([...modules!, newModule]);
    };

    const handleUpdateModule = (updatedModule: ModuleResponse) => {
        const updatedModules: ModuleResponse[] = modules!.map((module) => {
            if (module.moduleId === updatedModule.moduleId) {
                module.name = updatedModule.name;
                module.description = updatedModule.description;
                module.startDate = updatedModule.startDate;
                module.endDate = updatedModule.endDate;
                module.courseId = updatedModule.courseId;
                
                return module;
            } else {
                return module;
            }
        });
        setModules(updatedModules);
    };

    const handleDeleteModule = async (moduleId: string) => {
        await deleteModule(moduleId);
        setModules(modules?.filter((module) => module.moduleId !== moduleId));
        setDeletingModule(undefined);
    };

    const moduleColumns: Column<ModuleResponse>[] = [
        {
            key: "name",
            field: "name",
            header: "Name",
            render: (module) => (
                <Link
                    className="font-bold underline text-buttons dark:text-buttons-dark text-lg"
                    to={`/module/${module.moduleId}`}
                >
                    {module.name}
                </Link>
            ),
        },
        {
            key: "description",
            field: "description",
            header: "Description",
            render: (course) => course.description,
        },
        {
            key: "startDate",
            field: "startDate",
            header: "Start date",
            render: (course) => course.startDate,
        },
        {
            key: "endDate",
            field: "endDate",
            header: "End date",
            render: (course) => course.endDate,
        },
        {
            key: "actions",
            header: "Actions",
            className: "whitespace-nowrap pr-4",
            render: (module) => (
                <div className="flex items-center gap-2">
                    <Button
                        className="hover:cursor-pointer"
                        onClick={() => setEditingModule(module)}
                    >
                        Edit
                    </Button>
                    <Button
                        variant="cancel"
                        className="hover:cursor-pointer"
                        onClick={() => setDeletingModule(module)}
                    >
                        Delete
                    </Button>
                </div>
            ),
        },
    ];

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

    if (error) return <ErrorDisplay errorResp={error} />;

    if (!modules)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Modules not found
                </h1>
            </div>
        );

    return (
        <>
            <div className="m-3 flex justify-between">
                <TableToolbar
                    tableTitle="Modules"
                    search={search}
                    sortBy={sortBy}
                    sortOptions={MODULE_SORT_OPTIONS}
                    onSearchChange={handleSearchChange}
                    onSortChange={handleSortChange}
                    addAction={{
                        label: "Add Module",
                        onAdd: () => setShowCreateModuleModal(true),
                    }}
                />
            </div>
            <DataTable
                items={modules}
                columns={moduleColumns}
                getKey={(module) => module.moduleId}
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
            {editingModule &&
                createPortal(
                    <ModalCreateModule
                        open={true}
                        onClose={() => setEditingModule(undefined)}
                        handleUpdateState={handleUpdateModule}
                        isEditing={true}
                        moduleToEdit={editingModule}
                    />,
                    document.getElementById("root")!,
                )}
            {showCreateModuleModal &&
                createPortal(
                    <ModalCreateModule
                        open={showCreateModuleModal}
                        onClose={() => setShowCreateModuleModal(false)}
                        handleUpdateState={handleModuleState}
                    />,
                    document.getElementById("root")!,
                )}
            {deletingModule && (
                <ConfirmDialog
                    open={true}
                    title="Delete Module"
                    message={`Are you sure you want to delete the module: ${deletingModule.name}`}
                    onCancel={() => setDeletingModule(undefined)}
                    onConfirm={() =>
                        handleDeleteModule(deletingModule.moduleId)
                    }
                />
            )}
        </>
    );
};

export default ModuleManagement;

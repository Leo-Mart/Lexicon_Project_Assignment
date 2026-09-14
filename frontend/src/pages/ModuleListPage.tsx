import { useState, useEffect } from "react";
import "../index.css";
/* import ModuleModal from "../components/ModuleModal"; */
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import { fetchModules } from "../services/moduleService";
import { deleteModule } from "../services/moduleService";
import ModalCreateResource from "../components/ModalCreateResource";
import { createPortal } from "react-dom";
import type { SortOption } from "../types/SortOption";
import TableToolbar from "../components/TableToolbar";
import DataTable from "../components/DataTable";
import type { Column } from "../types/Column";
import { Link } from "react-router-dom";
import Button from "../components/Button";

const MODULES_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
    { value: "start-asc", label: "Start Date Old-New" },
    { value: "start-desc", label: "Start Date New-Old" },
    { value: "end-asc", label: "End Date Old-New" },
    { value: "end-desc", label: "End Date New-Old" },
];

export default function ModuleListPage() {
    // STATE
    const newModule = {
        moduleId: "",
        courseId: "",
        name: "",
        description: "",
        startDate: "",
        endDate: "",
        activities: [],
        course: {} as ModuleResponse["course"],
    };

    const [modules, setModules] = useState<ModuleResponse[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const [search, setSearch] = useState("");
    const [isModuleModalVisible, setIsModuleModalVisible] = useState(false);
    const [resourceTarget, setResourceTarget] = useState<ModuleResponse | null>(
        null,
    );
    const [selectedRow, setSelectedRow] = useState<ModuleResponse>(newModule);

    const [sortBy, setSortBy] = useState("name-asc"); // or const DEFAULT_SORT = "name-asc"

    const handleSortChange = (value: string) => {
        setSortBy(value);
    };

    const handleSearchChange = (value: string) => {
        setSearch(value);
    };

    const handleSubmitModuleModal = (returnData: ModuleResponse) => {
        setIsModuleModalVisible(false);

        if (selectedRow.moduleId != "")
        // Update the module in the list
        {
            setModules(
                modules.map((c) =>
                    c.moduleId === returnData.moduleId ? returnData : c,
                ),
            );
        } else
        //show added module in the list
        {
            setModules([...modules, returnData]);
        }
    };

    const handleShowModuleModal = (module: ModuleResponse) => {
        setSelectedRow(module);
        setIsModuleModalVisible(true);
    };

    const isFirstLoad = loading && modules.length === 0;

    // READ ALL
    useEffect(() => {
        const fetchAllModules = async () => {
            setLoading(true);
            setError(null);
            try {
                const moduleData = await fetchModules();
                setModules(moduleData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch module",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAllModules();
    }, [search, sortBy]);

    // DELETE
    async function handleDelete(module: ModuleResponse) {
        if (
            !window.confirm(
                'Are you sure you want to delete the module "' +
                    module.name +
                    '"?',
            )
        ) {
            return;
        }

        try {
            await deleteModule(module.moduleId);
            // Filter the deleted module from state
            setModules(modules!.filter((c) => c.moduleId !== module.moduleId));
        } catch (error) {
            console.error("Error on render:", error);
        }
    }

    const moduleColumns: Column<ModuleResponse>[] = [
        {
            key: "name",
            field: "name",
            header: "Name",
            render: (module) => (
                <Link
                    className="font-bold underline text-buttons dark:text-buttons-dark text-lg"
                    to={`/modules/${module.moduleId}`}
                >
                    {module.name}
                </Link>
            ),
        },
        {
            key: "description",
            field: "description",
            header: "Description",
            render: (module) => module.description,
        },
        {
            key: "startDate",
            field: "startDate",
            header: "Start date",
            render: (module) => module.startDate,
        },
        {
            key: "endDate",
            field: "endDate",
            header: "End date",
            render: (module) => module.endDate,
        },
        {
            key: "actions",
            header: "Interact",
            className: "whitespace-nowrap",
            render: (module) => (
                <div className="flex items-center gap-2">
                    <Button onClick={() => handleShowModuleModal(module)}>
                        Update
                    </Button>
                    <Button onClick={() => setResourceTarget(module)}>
                        Create Resource
                    </Button>
                    <Button
                        variant="cancel"
                        onClick={() => handleDelete(module)}
                    >
                        Delete
                    </Button>
                </div>
            ),
        },
    ];

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (isFirstLoad) return <p>Loading...</p>;
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
                    sortOptions={MODULES_SORT_OPTIONS}
                    onSearchChange={handleSearchChange}
                    onSortChange={handleSortChange}
                    addAction={{
                        label: "Add module",
                        onAdd: () => handleShowModuleModal(newModule),
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
        </>
    );
}

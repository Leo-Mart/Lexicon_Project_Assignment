import Button from "./Button";
import TableSearchBar from "./TableSearchBar";
import type { SortOption } from "../types/SortOption";

interface TableToolbarProps {
    tableTitle: string;
    search: string;
    sortBy: string;
    sortOptions: SortOption[];
    onSearchChange: (value: string) => void;
    onSortChange: (value: string) => void;
    addAction?: {
        label: string;
        onAdd: () => void;
    };
}

export default function TableToolbar({
    tableTitle,
    search,
    sortBy,
    sortOptions,
    onSearchChange,
    onSortChange,
    addAction,
}: TableToolbarProps) {
    return (
        <div className="flex items-center gap-5 mb-4 px-2 w-full">
            <div className="flex items-center gap-5 flex-1">
                <TableSearchBar
                    search={search}
                    onSearchChange={onSearchChange}
                />

                <select
                    value={sortBy}
                    onChange={(event) => onSortChange(event.target.value)}
                    className="bg-bg-header-dark text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                >
                    {sortOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                            {option.label}
                        </option>
                    ))}
                </select>
            </div>

            <h1 className="text-2xl text-text-light bg-bg-header p-3 rounded-2xl font-bold">
                {tableTitle}
            </h1>

            <div className="flex-1 flex justify-end">
                {addAction && (
                    <Button
                        onClick={addAction.onAdd}
                        className="ml-auto cursor-pointer"
                    >
                        {addAction.label}
                    </Button>
                )}
            </div>
        </div>
    );
}

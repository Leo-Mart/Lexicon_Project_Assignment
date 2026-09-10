import Button from "./Button";
import TableSearchBar from "./TableSearchBar";
import type { SortOption } from "../types/SortOption";

interface TableToolbarProps {
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
    search,
    sortBy,
    sortOptions,
    onSearchChange,
    onSortChange,
    addAction,
}: TableToolbarProps) {
    return (
        <div className="flex items-center gap-3 mb-4 px-2">
            <TableSearchBar search={search} onSearchChange={onSearchChange} />

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

            {addAction && (
                <Button
                    onClick={addAction.onAdd}
                    className="ml-auto cursor-pointer"
                >
                    {addAction.label}
                </Button>
            )}
        </div>
    );
}

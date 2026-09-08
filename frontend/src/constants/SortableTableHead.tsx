type SortableColumn = "name" | "email" | "status" | "role" | "course";

export default function SortableTh({
    field,
    label,
    sortBy,
    onSortChange,
}: {
    field: SortableColumn;
    label: string;
    sortBy: string;
    onSortChange: (value: string) => void;
}) {
    const [currentField, currentDirection = "asc"] = sortBy.split("-");
    const isActive = currentField === field;
    const isAsc = currentDirection === "asc";

    const handleClick = () => {
        // Same column: toggle direction. New column: start ascending.
        onSortChange(isActive && isAsc ? `${field}-desc` : `${field}-asc`);
    };

    return (
        <th className="px-4 py-3">
            <button
                type="button"
                onClick={handleClick}
                className="inline-flex items-center gap-1 font-normal cursor-pointer hover:text-slate-300"
                aria-label={`Sort by ${label}`}
            >
                {label}
                {isActive && <span aria-hidden>{isAsc ? "▲" : "▼"}</span>}
            </button>
        </th>
    );
}

import Spinner from "./Spinner";

type SortableColumn =
    | "name"
    | "email"
    | "status"
    | "role"
    | "course"
    | "description"
    | "startDate"
    | "endDate"
    | "student"
    | "activity"
    | "deadline"
    | "submitted"
    | "late"
    | "review"
    | "days";

export default function SortableTh({
    field,
    label,
    sortBy,
    onSortChange,
    isLoading = false,
}: {
    field: SortableColumn;
    label: string;
    sortBy: string;
    onSortChange: (value: string) => void;
    isLoading?: boolean;
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
                className="inline-flex items-center gap-1 font-normal cursor-pointer"
                aria-label={`Sort by ${label}`}
            >
                {label}
                {isLoading && isActive ? (
                    <Spinner className="w-4 h-4" />
                ) : (
                    isActive && <span aria-hidden>{isAsc ? "▲" : "▼"}</span>
                )}
            </button>
        </th>
    );
}

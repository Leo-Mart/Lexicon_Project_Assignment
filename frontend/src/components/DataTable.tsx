import SortableTh from "./SortableTableHead";
import type { Column } from "../types/Column";

interface DataTableProps<T> {
    items: T[];
    columns: Column<T>[];
    getKey: (item: T) => string;
    sortBy: string;
    isLoading: boolean;
    onSortChange: (value: string) => void;
    /** Extra classes for the <tbody>, e.g. text colors */
    bodyClassName?: string;
}

export default function DataTable<T>({
    items,
    columns,
    getKey,
    sortBy,
    isLoading,
    onSortChange,
    bodyClassName,
}: DataTableProps<T>) {
    return (
        <div className="overflow-x-auto rounded-lg border border-bg-header">
            <table className="w-full text-left text-text-light">
                <thead className="bg-bg-header">
                    <tr>
                        {columns.map((column) =>
                            column.field ? (
                                <SortableTh
                                    key={column.key}
                                    field={column.field}
                                    label={column.header}
                                    sortBy={sortBy}
                                    isLoading={isLoading}
                                    onSortChange={onSortChange}
                                />
                            ) : (
                                <th
                                    key={column.key}
                                    className={
                                        column.headerClassName ?? "px-4 py-3"
                                    }
                                >
                                    {column.header}
                                </th>
                            ),
                        )}
                    </tr>
                </thead>

                <tbody className={bodyClassName}>
                    {items.map((item, index) => (
                        <tr
                            key={getKey(item)}
                            className={
                                index % 2 === 0
                                    ? "bg-white dark:bg-bg-window-dark text-text-dark"
                                    : "bg-bg dark:bg-bg-dark text-text-dark"
                            }
                        >
                            {columns.map((column) => (
                                <td
                                    key={column.key}
                                    className={column.className ?? "px-4 py-3"}
                                >
                                    {column.render(item, index)}
                                </td>
                            ))}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

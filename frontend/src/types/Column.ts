import type { ReactNode } from "react";
import type { SortableColumn } from "./TableColumns";

export interface Column<T> {
    key: string;
    header: string;
    field?: SortableColumn;
    render: (item: T, index: number) => ReactNode;
    className?: string;
    headerClassName?: string;
}

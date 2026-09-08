import Button from "./Button";

interface PaginationProps {
    page: number;
    pageSize: number;
    totalCount: number;
    onPageChange: (page: number) => void;
}

export default function Pagination({
    page,
    pageSize,
    totalCount,
    onPageChange,
}: PaginationProps) {
    const totalPages = Math.ceil(totalCount / pageSize);

    return (
        <div className="flex items-center justify-center gap-4 mt-4">
            <Button onClick={() => onPageChange(page - 1)} disabled={page <= 1}>
                Previous
            </Button>

            <span>
                Page {page} of {totalPages}
            </span>

            <Button
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
            >
                Next
            </Button>
        </div>
    );
}

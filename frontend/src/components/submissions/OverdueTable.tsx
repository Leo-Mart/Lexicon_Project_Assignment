import { Link } from "react-router-dom";
import Pagination from "../Pagination";
import DataTable from "../DataTable";
import { ActivityDate, ActivityTime } from "../../constants/ActivityTimeConverter";
import { daysOverdue } from "../../constants/SubmissionDates";
import type { Column } from "../../types/Column";
import type { OverdueSubmission } from "../../interfaces/submission/OverdueSubmission";
import type { SubmissionLookups } from "../../interfaces/submission/SubmissionLookups";

interface OverdueRow extends OverdueSubmission {
    moduleId: string | undefined;
    activityName: string;
    courseName: string;
    courseId: string | undefined;
    deadline: string | null;
}

interface OverdueTableProps {
    overdueByActivity: Map<string, OverdueSubmission[]>;
    lookups: SubmissionLookups;
    search: string;
    sortBy: string;
    onSortChange: (value: string) => void;
    page: number;
    pageSize: number;
    onPageChange: (page: number) => void;
}

// Enrolled students with no submission, past the deadline - one row per
// student per overdue activity, built client-side from the per-activity
// fetches Submissions already holds.
export default function OverdueTable({
    overdueByActivity,
    lookups,
    search,
    sortBy,
    onSortChange,
    page,
    pageSize,
    onPageChange,
}: OverdueTableProps) {
    const { activityNameById, activityModuleById, activityDeadlineById, courseNameForActivity, courseIdForActivity } =
        lookups;

    const rows: OverdueRow[] = [...overdueByActivity.entries()].flatMap(
        ([activityId, students]) =>
            students.map((u) => ({
                ...u,
                activityId,
                moduleId: activityModuleById.get(activityId),
                activityName: activityNameById.get(activityId) ?? activityId,
                courseName: courseNameForActivity(activityId) || "Unknown course",
                courseId: courseIdForActivity(activityId),
                deadline: activityDeadlineById.get(activityId) ?? null,
            })),
    );

    const filteredRows = search.trim()
        ? rows.filter((row) =>
              row.studentName.toLowerCase().includes(search.trim().toLowerCase()),
          )
        : rows;

    const [sortField, sortDirection = "asc"] = sortBy.split("-");
    const sortedRows = [...filteredRows].sort((a, b) => {
        let cmp = 0;
        if (sortField === "student") {
            cmp = a.studentName.localeCompare(b.studentName);
        } else if (sortField === "course") {
            cmp = a.courseName.localeCompare(b.courseName);
        } else if (sortField === "activity") {
            cmp = a.activityName.localeCompare(b.activityName);
        } else if (sortField === "deadline") {
            // Days overdue is derived from the deadline, so one sort covers both columns.
            const timeA = a.deadline ? new Date(a.deadline).getTime() : 0;
            const timeB = b.deadline ? new Date(b.deadline).getTime() : 0;
            cmp = timeA - timeB;
        }
        return sortDirection === "desc" ? -cmp : cmp;
    });

    if (filteredRows.length === 0) {
        return (
            <p className="text-text-dark dark:text-text-light">
                Nobody's overdue.
            </p>
        );
    }

    const pagedRows = sortedRows.slice((page - 1) * pageSize, page * pageSize);

    const columns: Column<OverdueRow>[] = [
        {
            key: "student",
            field: "student",
            header: "Student",
            render: (row) => row.studentName,
        },
        {
            key: "course",
            field: "course",
            header: "Course",
            render: (row) =>
                row.courseId ? (
                    <Link className="underline text-buttons" to={`/courses/${row.courseId}`}>
                        {row.courseName}
                    </Link>
                ) : (
                    row.courseName
                ),
        },
        {
            key: "activity",
            field: "activity",
            header: "Activity",
            render: (row) =>
                row.moduleId ? (
                    <Link className="underline text-buttons" to={`/module/${row.moduleId}`}>
                        {row.activityName}
                    </Link>
                ) : (
                    row.activityName
                ),
        },
        {
            key: "deadline",
            field: "deadline",
            header: "Deadline",
            render: (row) =>
                row.deadline ? (
                    <>
                        {ActivityDate(row.deadline)} {ActivityTime(row.deadline)}
                        <div className="text-xs opacity-70">
                            {daysOverdue(row.deadline)} days overdue
                        </div>
                    </>
                ) : (
                    "-"
                ),
        },
    ];

    return (
        <>
            <DataTable
                items={pagedRows}
                columns={columns}
                getKey={(row) => `${row.studentId}-${row.activityId}`}
                sortBy={sortBy}
                isLoading={false}
                onSortChange={onSortChange}
            />
            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={filteredRows.length}
                onPageChange={onPageChange}
            />
        </>
    );
}

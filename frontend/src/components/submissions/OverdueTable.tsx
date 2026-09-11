import { Link } from "react-router-dom";
import Pagination from "../Pagination";
import SortableTh from "../SortableTableHead";
import { ActivityDate, ActivityTime } from "../../constants/ActivityTimeConverter";
import { daysOverdue } from "../../constants/SubmissionDates";
import type { OverdueSubmission } from "../../interfaces/submission/OverdueSubmission";
import type { SubmissionLookups } from "../../interfaces/submission/SubmissionLookups";

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

    const rows = [...overdueByActivity.entries()].flatMap(
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

    const pagedRows = sortedRows.slice((page - 1) * pageSize, page * pageSize);

    if (filteredRows.length === 0) {
        return (
            <p className="text-text-dark dark:text-text-light">
                Nobody's overdue.
            </p>
        );
    }

    return (
        <>
            <div className="overflow-x-auto rounded-lg border border-accent-blue">
                <table className="w-full text-left text-text-dark dark:text-text-light">
                    <thead className="bg-bg-window dark:bg-bg-window-dark">
                        <tr>
                            <SortableTh
                                field="student"
                                label="Student"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                            />
                            <SortableTh
                                field="course"
                                label="Course"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                            />
                            <SortableTh
                                field="activity"
                                label="Activity"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                            />
                            <SortableTh
                                field="deadline"
                                label="Deadline"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                            />
                        </tr>
                    </thead>
                    <tbody>
                        {pagedRows.map((row, i) => (
                            <tr
                                key={`${row.studentId}-${i}`}
                                className="border-t border-accent-blue hover:bg-bg-window/40 dark:hover:bg-bg-window-dark/40"
                            >
                                <td className="px-4 py-3">{row.studentName}</td>
                                <td className="px-4 py-3">
                                    {row.courseId ? (
                                        <Link
                                            className="underline text-buttons"
                                            to={`/courses/${row.courseId}`}
                                        >
                                            {row.courseName}
                                        </Link>
                                    ) : (
                                        row.courseName
                                    )}
                                </td>
                                <td className="px-4 py-3">
                                    {row.moduleId ? (
                                        <Link
                                            className="underline text-buttons"
                                            to={`/module/${row.moduleId}`}
                                        >
                                            {row.activityName}
                                        </Link>
                                    ) : (
                                        row.activityName
                                    )}
                                </td>
                                <td className="px-4 py-3">
                                    {row.deadline ? (
                                        <>
                                            {ActivityDate(row.deadline)}{" "}
                                            {ActivityTime(row.deadline)}
                                            <div className="text-xs opacity-70">
                                                {daysOverdue(row.deadline)} days
                                                overdue
                                            </div>
                                        </>
                                    ) : (
                                        "-"
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={filteredRows.length}
                onPageChange={onPageChange}
            />
        </>
    );
}

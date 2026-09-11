import { Link } from "react-router-dom";
import Button from "../Button";
import Pagination from "../Pagination";
import SortableTh from "../SortableTableHead";
import { ActivityDate, ActivityTime } from "../../constants/ActivityTimeConverter";
import { daysOverdue, daysLate } from "../../constants/SubmissionDates";
import type { SubmissionResponse } from "../../interfaces/submission/SubmissionResponse";
import type { SubmissionLookups } from "../../interfaces/submission/SubmissionLookups";
import type { SubmissionTab } from "./SubmissionTabs";

interface SubmissionsTableProps {
    tab: Exclude<SubmissionTab, "overdue">;
    submissions: SubmissionResponse[];
    lookups: SubmissionLookups;
    sortBy: string;
    onSortChange: (value: string) => void;
    tableLoading: boolean;
    hasAnySubmissions: boolean;
    page: number;
    pageSize: number;
    totalCount: number;
    onPageChange: (page: number) => void;
    onReview: (submission: SubmissionResponse) => void;
}

// The Not reviewed / Needs completion / Done table: one server-paged,
// server-sorted page of submissions plus the review action.
export default function SubmissionsTable({
    tab,
    submissions,
    lookups,
    sortBy,
    onSortChange,
    tableLoading,
    hasAnySubmissions,
    page,
    pageSize,
    totalCount,
    onPageChange,
    onReview,
}: SubmissionsTableProps) {
    const {
        activityNameById,
        activityModuleById,
        activityDeadlineById,
        courseNameForActivity,
        courseIdForActivity,
        studentName,
    } = lookups;
    const columnCount = tab === "not-reviewed" ? 5 : 6;

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
                                isLoading={tableLoading}
                            />
                            <SortableTh
                                field="course"
                                label="Course"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                                isLoading={tableLoading}
                            />
                            <SortableTh
                                field="activity"
                                label="Activity"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                                isLoading={tableLoading}
                            />
                            <SortableTh
                                field="deadline"
                                label="Deadline"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                                isLoading={tableLoading}
                            />
                            {tab !== "not-reviewed" && (
                                <SortableTh
                                    field="reviewed"
                                    label="Reviewed"
                                    sortBy={sortBy}
                                    onSortChange={onSortChange}
                                    isLoading={tableLoading}
                                />
                            )}
                            <th className="px-4 py-3"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {submissions.map((s) => {
                            const deadline = activityDeadlineById.get(s.activityId);
                            // Done shows how late the submission itself was; the other
                            // tabs show how overdue it still is, growing until resolved.
                            const lateDays = deadline
                                ? tab === "done"
                                    ? daysLate(deadline, s.submittedAt)
                                    : daysOverdue(deadline)
                                : null;
                            const moduleId = activityModuleById.get(s.activityId);
                            const courseId = courseIdForActivity(s.activityId);
                            const courseName = courseNameForActivity(s.activityId);

                            return (
                                <tr
                                    key={s.submissionId}
                                    className="border-t border-accent-blue hover:bg-bg-window/40 dark:hover:bg-bg-window-dark/40"
                                >
                                    <td className="px-4 py-3">
                                        {studentName(s.studentId)}
                                        {s.resubmittedAt != null && (
                                            <span className="ml-2 rounded-full bg-accent-blue/30 px-2 py-0.5 text-xs">
                                                Resubmitted
                                            </span>
                                        )}
                                    </td>
                                    <td className="px-4 py-3">
                                        {courseId ? (
                                            <Link
                                                className="underline text-buttons"
                                                to={`/courses/${courseId}`}
                                            >
                                                {courseName || "-"}
                                            </Link>
                                        ) : (
                                            (courseName ?? "-")
                                        )}
                                    </td>
                                    <td className="px-4 py-3">
                                        {moduleId ? (
                                            <Link
                                                className="underline text-buttons"
                                                to={`/module/${moduleId}`}
                                            >
                                                {activityNameById.get(s.activityId) ??
                                                    s.activityId}
                                            </Link>
                                        ) : (
                                            (activityNameById.get(s.activityId) ??
                                                s.activityId)
                                        )}
                                    </td>
                                    <td className="px-4 py-3">
                                        {deadline ? (
                                            <>
                                                {ActivityDate(deadline)}{" "}
                                                {ActivityTime(deadline)}
                                                {lateDays != null && lateDays > 0 && (
                                                    <div className="text-xs opacity-70">
                                                        {lateDays} days{" "}
                                                        {tab === "done"
                                                            ? "late"
                                                            : "overdue"}
                                                    </div>
                                                )}
                                            </>
                                        ) : (
                                            "-"
                                        )}
                                    </td>
                                    {tab !== "not-reviewed" && (
                                        <td className="px-4 py-3">
                                            {s.feedbackAt
                                                ? `${ActivityDate(s.feedbackAt)} ${ActivityTime(s.feedbackAt)}`
                                                : "-"}
                                        </td>
                                    )}
                                    <td className="px-4 py-3">
                                        <Button onClick={() => onReview(s)}>
                                            {s.reviewStatus != null
                                                ? "Edit review"
                                                : "Review"}
                                        </Button>
                                    </td>
                                </tr>
                            );
                        })}
                        {tableLoading && (
                            <tr>
                                <td
                                    colSpan={columnCount}
                                    className="px-4 py-3 text-center"
                                >
                                    Loading...
                                </td>
                            </tr>
                        )}
                        {!tableLoading && submissions.length === 0 && (
                            <tr>
                                <td
                                    colSpan={columnCount}
                                    className="px-4 py-3 text-center"
                                >
                                    {hasAnySubmissions
                                        ? "No submissions match your search."
                                        : "No submissions yet."}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={onPageChange}
            />
        </>
    );
}

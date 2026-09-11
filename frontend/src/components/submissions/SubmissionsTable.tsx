import { Link } from "react-router-dom";
import Button from "../Button";
import Pagination from "../Pagination";
import DataTable from "../DataTable";
import { ActivityDate, ActivityTime } from "../../constants/ActivityTimeConverter";
import { daysOverdue, daysLate } from "../../constants/SubmissionDates";
import type { Column } from "../../types/Column";
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

    if (!tableLoading && submissions.length === 0) {
        return (
            <p className="text-text-dark dark:text-text-light">
                {hasAnySubmissions
                    ? "No submissions match your search."
                    : "No submissions yet."}
            </p>
        );
    }

    const columns: Column<SubmissionResponse>[] = [
        {
            key: "student",
            field: "student",
            header: "Student",
            render: (s) => (
                <>
                    {studentName(s.studentId)}
                    {s.resubmittedAt != null && (
                        <span className="ml-2 rounded-full bg-accent-blue/30 px-2 py-0.5 text-xs">
                            Resubmitted
                        </span>
                    )}
                </>
            ),
        },
        {
            key: "course",
            field: "course",
            header: "Course",
            render: (s) => {
                const courseId = courseIdForActivity(s.activityId);
                const courseName = courseNameForActivity(s.activityId);
                return courseId ? (
                    <Link className="underline text-buttons" to={`/courses/${courseId}`}>
                        {courseName || "-"}
                    </Link>
                ) : (
                    (courseName ?? "-")
                );
            },
        },
        {
            key: "activity",
            field: "activity",
            header: "Activity",
            render: (s) => {
                const moduleId = activityModuleById.get(s.activityId);
                const activityName = activityNameById.get(s.activityId) ?? s.activityId;
                return moduleId ? (
                    <Link className="underline text-buttons" to={`/module/${moduleId}`}>
                        {activityName}
                    </Link>
                ) : (
                    activityName
                );
            },
        },
        {
            key: "deadline",
            field: "deadline",
            header: "Deadline",
            render: (s) => {
                const deadline = activityDeadlineById.get(s.activityId);
                if (!deadline) return "-";

                // Done shows how late the submission itself was; the other
                // tabs show how overdue it still is, growing until resolved.
                const lateDays =
                    tab === "done"
                        ? daysLate(deadline, s.submittedAt)
                        : daysOverdue(deadline);
                return (
                    <>
                        {ActivityDate(deadline)} {ActivityTime(deadline)}
                        {lateDays > 0 && (
                            <div className="text-xs opacity-70">
                                {lateDays} days {tab === "done" ? "late" : "overdue"}
                            </div>
                        )}
                    </>
                );
            },
        },
        ...(tab !== "not-reviewed"
            ? [
                  {
                      key: "reviewed",
                      field: "reviewed",
                      header: "Reviewed",
                      render: (s) =>
                          s.feedbackAt
                              ? `${ActivityDate(s.feedbackAt)} ${ActivityTime(s.feedbackAt)}`
                              : "-",
                  } as Column<SubmissionResponse>,
              ]
            : []),
        {
            key: "actions",
            header: "",
            render: (s) => (
                <Button onClick={() => onReview(s)}>
                    {s.reviewStatus != null ? "Edit review" : "Review"}
                </Button>
            ),
        },
    ];

    return (
        <>
            <DataTable
                items={submissions}
                columns={columns}
                getKey={(s) => s.submissionId}
                sortBy={sortBy}
                isLoading={tableLoading}
                onSortChange={onSortChange}
            />

            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={onPageChange}
            />
        </>
    );
}

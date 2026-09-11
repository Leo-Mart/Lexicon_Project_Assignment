import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import Button from "../components/Button";
import Pagination from "../components/Pagination";
import SortableTh from "../components/SortableTableHead";
import ReviewSubmissionModal from "../components/ReviewSubmissionModal";
import {
    fetchSubmissionsPage,
    fetchOverdueByActivityId,
    setFeedback,
    fetchAllSubmissions,
} from "../services/submissionService";
import { ActivityDate, ActivityTime } from "../utils/ActivityTimeConverter.ts";
import { useAuth } from "../hooks/useAuth";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { OverdueSubmission } from "../interfaces/submission/OverdueSubmission";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";
import SubmissionCategoryButtons from "../components/SubmissionCategoryButtons";
import type { SubmissionTabs } from "../types/SubmissionTabs";
import { daysLate, daysOverdue } from "../utils/deadlines.ts";
import {
    createActivityLookups,
    type ActivityLookups,
} from "../utils/IdsFromActivity.ts";
import { fetchActivities } from "../services/activityService.ts";
import { fetchCourses } from "../services/courseService.ts";
import { fetchUsers } from "../services/userService.ts";
import TableSearchBar from "../components/TableSearchBar.tsx";
import DataTable from "../components/DataTable.tsx";
import type { Column } from "../types/Column.ts";
/* import DataTable from "../components/DataTable";
import type { Column } from "../types/Column.ts"; */

const PAGE_SIZE = 10;

const DEFAULT_SORT = "review-asc";

// Mock teacher review page: everything fetched and joined client-side.
export default function Submissions() {
    const { role } = useAuth();
    const [submissions, setSubmissions] = useState<SubmissionResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [reviewing, setReviewing] = useState<SubmissionResponse | null>(null);
    const [sortBy, setSortBy] = useState(DEFAULT_SORT);
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [tab, setTab] = useState<SubmissionTabs>("not-reviewed");
    const [lookups, setLookups] = useState<ActivityLookups | null>(null);

    // The Submissions tab's table: one page at a time, searched/sorted on
    // the server. Separate from `submissions` above, which stays a full
    // fetch for the overview stats and the Overdue tab.
    const [pagedSubmissions, setPagedSubmissions] = useState<
        SubmissionResponse[]
    >([]);
    const [totalCount, setTotalCount] = useState(0);
    const [tableLoading, setTableLoading] = useState(true);

    // Per-activity overdue view: a different concept from a submission, so
    // it's its own type/fetch, not a filter over SubmissionResponse[].
    // Fetched eagerly for every activity so the summary bar is ready before
    // the Overdue tab is even opened.
    // Ascending deadline puts the most overdue (earliest deadline) first.
    const [overdueSortBy, setOverdueSortBy] = useState("deadline-asc");
    const [overduePage, setOverduePage] = useState(1);
    const [overdueByActivity, setOverdueByActivity] = useState<
        Map<string, OverdueSubmission[]>
    >(new Map());
    const [overdueChecked, setOverdueChecked] = useState(false);

    useEffect(() => {
        if (role !== "Teacher") return;

        void (async () => {
            setLoading(true);
            try {
                const [submissionData, activities, users, courses] =
                    await Promise.all([
                        fetchAllSubmissions(),
                        fetchActivities(),
                        fetchUsers(),
                        fetchCourses({
                            search: "",
                            sortBy: "name",
                            direction: "asc",
                            page: 1,
                            pageSize: 200,
                        }),
                    ]);

                setSubmissions(submissionData);
                setLookups(
                    createActivityLookups(activities, courses.items, users),
                );
            } finally {
                setLoading(false);
            }
        })();
    }, [role]);

    // Bumped after a review is saved to force the page below to re-fetch, so
    // a submission that no longer matches the current tab's filter (e.g.
    // reviewed while on Not reviewed) drops out instead of lingering until
    // the next navigation.
    const [reloadToken, setReloadToken] = useState(0);

    useEffect(() => {
        if (role !== "Teacher" || tab === "overdue") return;

        const loadPage = async () => {
            setTableLoading(true);

            const [sortField, sortDirection = "asc"] = sortBy.split("-");
            const reviewStatus =
                tab === "needs-completion"
                    ? SubmissionReviewStatus.NeedsCompletion
                    : tab === "done"
                      ? SubmissionReviewStatus.Approved
                      : undefined;
            const data = await fetchSubmissionsPage(
                {
                    search,
                    sortBy: sortField,
                    direction: sortDirection,
                    page,
                    pageSize: PAGE_SIZE,
                },
                reviewStatus,
            );

            setPagedSubmissions(data.items);
            setTotalCount(data.totalCount);
            setTableLoading(false);
        };

        void loadPage();
    }, [role, tab, sortBy, search, page, reloadToken]);

    // Overdue students for every activity, so the picker can flag which ones
    // need attention and the summary bar/"All activities" need no extra fetch.
    useEffect(() => {
        if (overdueChecked || !lookups || submissions.length === 0) {
            return;
        }

        const check = async () => {
            const activityIds = [
                ...new Set(submissions.map(({ activityId }) => activityId)),
            ];
            const entries = await Promise.all(
                activityIds.map(async (id) => {
                    const overdue = await fetchOverdueByActivityId(id);
                    return [id, overdue] as const;
                }),
            );

            setOverdueByActivity(new Map(entries));
            setOverdueChecked(true);
        };

        void check();
    }, [overdueChecked, lookups, submissions]);

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(1);
    };

    const handleTabChange = (value: typeof tab) => {
        setTab(value);
        setPage(1);
    };

    const handleOverdueSortChange = (value: string) => {
        setOverdueSortBy(value);
        setOverduePage(1);
    };

    const handleReview = async (data: FeedbackRequest) => {
        if (!reviewing) return;

        const updated = await setFeedback(reviewing.submissionId, data);

        setSubmissions((prev) =>
            prev.map((s) =>
                s.submissionId === updated.submissionId ? updated : s,
            ),
        );
        setReloadToken((t) => t + 1);
        setReviewing(null);
    };

    const onSearchChange = (value: string) => {
        setSearch(value);
        setPage(1);
        setOverduePage(1);
    };

    const submissionColumns: Column<SubmissionResponse>[] = useMemo(() => {
        const baseColumns: Column<SubmissionResponse>[] = [
            {
                key: "student",
                field: "student",
                header: "Student",
                className: "px-4 py-3",
                render: (submission) => (
                    <>
                        {lookups?.studentName(submission.studentId)}
                        {submission.resubmittedAt != null && (
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
                className: "px-4 py-3",
                render: (submission) => {
                    const courseId = lookups?.courseIdForActivity(
                        submission.activityId,
                    );
                    const courseName =
                        lookups?.courseNameForActivity(submission.activityId) ||
                        "-";
                    return courseId ? (
                        <Link
                            className="underline text-buttons dark:text-buttons-dark"
                            to={`/courses/${courseId}`}
                        >
                            {courseName}
                        </Link>
                    ) : (
                        courseName
                    );
                },
            },
            {
                key: "activity",
                field: "activity",
                header: "Activity",
                className: "px-4 py-3",
                render: (submission) => {
                    const moduleId = lookups?.moduleIdForActivity(
                        submission.activityId,
                    );
                    const activityName =
                        lookups?.activityName(submission.activityId) ??
                        submission.activityId;
                    return moduleId ? (
                        <Link
                            className="underline text-buttons dark:text-buttons-dark"
                            to={`/module/${moduleId}`}
                        >
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
                className: "px-4 py-3",
                render: (submission) => {
                    const deadline = lookups?.deadlineForActivity(
                        submission.activityId,
                    );
                    const lateDays = deadline
                        ? tab === "done"
                            ? daysLate(deadline, submission.submittedAt)
                            : daysOverdue(deadline)
                        : null;
                    return deadline ? (
                        <>
                            {ActivityDate(deadline)} {ActivityTime(deadline)}
                            {lateDays != null && lateDays > 0 && (
                                <div className="text-xs opacity-70">
                                    {lateDays} days{" "}
                                    {tab === "done" ? "late" : "overdue"}
                                </div>
                            )}
                        </>
                    ) : (
                        "-"
                    );
                },
            },
        ];

        // Conditionally add the "reviewed" column
        if (tab !== "not-reviewed") {
            baseColumns.push({
                key: "reviewed",
                field: "reviewed",
                header: "Reviewed",
                className: "px-4 py-3",
                render: (submission) =>
                    submission.feedbackAt
                        ? `${ActivityDate(submission.feedbackAt)} ${ActivityTime(submission.feedbackAt)}`
                        : "-",
            });
        }

        // Always add the "actions" column
        baseColumns.push({
            key: "actions",
            header: "Interact",
            className: "whitespace-nowrap px-4 py-3",
            render: (submission) => (
                <Button onClick={() => setReviewing(submission)}>
                    {submission.reviewStatus != null ? "Edit review" : "Review"}
                </Button>
            ),
        });

        return baseColumns;
    }, [tab, lookups]); // Recompute when `tab` or `lookups` changes

    if (role !== "Teacher") {
        return (
            <div className="p-4 text-text-dark dark:text-text-light">
                Teachers only.
            </div>
        );
    }

    if (loading) return <div className="p-4">Loading...</div>;

    const totalOverdue = [...overdueByActivity.values()].reduce(
        (sum, list) => sum + list.length,
        0,
    );
    const notReviewedCount = submissions.filter(
        (s) => s.reviewStatus == null,
    ).length;
    const needsCompletionCount = submissions.filter(
        (s) => s.reviewStatus === SubmissionReviewStatus.NeedsCompletion,
    ).length;
    const doneCount = submissions.filter(
        (s) => s.reviewStatus === SubmissionReviewStatus.Approved,
    ).length;

    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold text-text-dark dark:text-text-light mb-2">
                Submissions
            </h1>
            <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
                <TableSearchBar
                    search={search}
                    onSearchChange={onSearchChange}
                />

                <SubmissionCategoryButtons
                    notReviewedCount={notReviewedCount}
                    overdueChecked={overdueChecked}
                    totalOverdue={totalOverdue}
                    needsCompletionCount={needsCompletionCount}
                    doneCount={doneCount}
                    tab={tab}
                    handleTabChange={handleTabChange}
                />
            </div>

            {tab !== "overdue" && (
                <div>
                    <DataTable
                        items={pagedSubmissions}
                        columns={submissionColumns}
                        getKey={(submission) => submission.submissionId}
                        sortBy={sortBy}
                        isLoading={tableLoading}
                        onSortChange={handleSortChange}
                        bodyClassName="text-text-dark dark:text-text-light"
                    />

                    <Pagination
                        page={page}
                        pageSize={PAGE_SIZE}
                        totalCount={totalCount}
                        onPageChange={setPage}
                    />
                </div>
            )}

            {tab === "overdue" &&
                (() => {
                    const rows = [...overdueByActivity.entries()].flatMap(
                        ([activityId, students]) =>
                            students.map((u) => ({
                                ...u,
                                activityId,
                                moduleId:
                                    lookups?.moduleIdForActivity(activityId),
                                activityName:
                                    lookups?.activityName(activityId) ??
                                    activityId,
                                courseName:
                                    lookups?.courseNameForActivity(
                                        activityId,
                                    ) || "Unknown course",
                                courseId:
                                    lookups?.courseIdForActivity(activityId),
                                deadline:
                                    lookups?.deadlineForActivity(activityId) ??
                                    null,
                            })),
                    );

                    const filteredRows = search.trim()
                        ? rows.filter((row) =>
                              row.studentName
                                  .toLowerCase()
                                  .includes(search.trim().toLowerCase()),
                          )
                        : rows;

                    const [sortField, sortDirection = "asc"] =
                        overdueSortBy.split("-");
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
                            const timeA = a.deadline
                                ? new Date(a.deadline).getTime()
                                : 0;
                            const timeB = b.deadline
                                ? new Date(b.deadline).getTime()
                                : 0;
                            cmp = timeA - timeB;
                        }
                        return sortDirection === "desc" ? -cmp : cmp;
                    });

                    const pagedRows = sortedRows.slice(
                        (overduePage - 1) * PAGE_SIZE,
                        overduePage * PAGE_SIZE,
                    );

                    return (
                        <>
                            {filteredRows.length === 0 ? (
                                <p className="text-text-dark dark:text-text-light">
                                    Nobody's overdue.
                                </p>
                            ) : (
                                <div className="overflow-x-auto rounded-lg border border-accent-blue">
                                    <table className="w-full text-left text-text-dark dark:text-text-light">
                                        <thead className="bg-bg-window dark:bg-bg-window-dark">
                                            <tr>
                                                <SortableTh
                                                    field="student"
                                                    label="Student"
                                                    sortBy={overdueSortBy}
                                                    onSortChange={
                                                        handleOverdueSortChange
                                                    }
                                                />
                                                <SortableTh
                                                    field="course"
                                                    label="Course"
                                                    sortBy={overdueSortBy}
                                                    onSortChange={
                                                        handleOverdueSortChange
                                                    }
                                                />
                                                <SortableTh
                                                    field="activity"
                                                    label="Activity"
                                                    sortBy={overdueSortBy}
                                                    onSortChange={
                                                        handleOverdueSortChange
                                                    }
                                                />
                                                <SortableTh
                                                    field="deadline"
                                                    label="Deadline"
                                                    sortBy={overdueSortBy}
                                                    onSortChange={
                                                        handleOverdueSortChange
                                                    }
                                                />
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {pagedRows.map((row, i) => (
                                                <tr
                                                    key={`${row.studentId}-${i}`}
                                                    className="border-t border-accent-blue hover:bg-bg-window/40 dark:hover:bg-bg-window-dark/40"
                                                >
                                                    <td className="px-4 py-3">
                                                        {row.studentName}
                                                    </td>
                                                    <td className="px-4 py-3">
                                                        {row.courseId ? (
                                                            <Link
                                                                className="underline text-buttons dark:text-buttons-dark"
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
                                                                className="underline text-buttons dark:text-buttons-dark"
                                                                to={`/module/${row.moduleId}`}
                                                            >
                                                                {
                                                                    row.activityName
                                                                }
                                                            </Link>
                                                        ) : (
                                                            row.activityName
                                                        )}
                                                    </td>
                                                    <td className="px-4 py-3">
                                                        {row.deadline ? (
                                                            <>
                                                                {ActivityDate(
                                                                    row.deadline,
                                                                )}{" "}
                                                                {ActivityTime(
                                                                    row.deadline,
                                                                )}
                                                                <div className="text-xs opacity-70">
                                                                    {daysOverdue(
                                                                        row.deadline,
                                                                    )}{" "}
                                                                    days overdue
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
                            )}
                            {filteredRows.length > 0 && (
                                <Pagination
                                    page={overduePage}
                                    pageSize={PAGE_SIZE}
                                    totalCount={filteredRows.length}
                                    onPageChange={setOverduePage}
                                />
                            )}
                        </>
                    );
                })()}

            {reviewing &&
                (() => {
                    const deadline = lookups?.deadlineForActivity(
                        reviewing.activityId,
                    );
                    const deadlineText = deadline
                        ? `${ActivityDate(deadline)} ${ActivityTime(deadline)}${
                              new Date() > new Date(deadline)
                                  ? ` (${daysOverdue(deadline)} days overdue)`
                                  : ""
                          }`
                        : "None";

                    return (
                        <ReviewSubmissionModal
                            submission={reviewing}
                            studentName={
                                lookups?.studentName(reviewing.studentId) ??
                                reviewing.studentId
                            }
                            courseName={
                                lookups?.courseNameForActivity(
                                    reviewing.activityId,
                                ) || "-"
                            }
                            activityName={
                                lookups?.activityName(reviewing.activityId) ??
                                reviewing.activityId
                            }
                            deadlineText={deadlineText}
                            onSave={handleReview}
                            onClose={() => setReviewing(null)}
                        />
                    );
                })()}
        </div>
    );
}

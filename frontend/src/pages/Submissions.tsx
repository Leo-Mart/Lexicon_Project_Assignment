import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import Button from "../components/Button";
import Pagination from "../components/Pagination";
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
import { daysOverdue } from "../utils/deadlines.ts";
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
import { fetchModules } from "../services/moduleService.ts";

const PAGE_SIZE = 10;

const DEFAULT_SORT = "review-asc";

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
                const [submissionData, activities, users, courses, modules] =
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
                        fetchModules(),
                    ]);

                setSubmissions(submissionData);
                setLookups(
                    createActivityLookups(
                        activities,
                        courses.items,
                        users,
                        modules,
                    ),
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
                key: "module",
                field: "module",
                header: "Module",
                className: "px-4 py-3",
                render: (submission) => {
                    const moduleId = lookups?.moduleIdForActivity(
                        submission.activityId,
                    );
                    const moduleName = lookups?.moduleNameForActivity(
                        submission.activityId,
                    );
                    return moduleId ? (
                        <Link
                            className="underline text-buttons dark:text-buttons-dark"
                            to={`/module/${moduleId}`}
                        >
                            {moduleName}
                        </Link>
                    ) : (
                        moduleName
                    );
                },
            },
            {
                key: "activity",
                field: "activity",
                header: "Activity",
                className: "px-4 py-3",
                render: (submission) => {
                    const activityName =
                        lookups?.activityName(submission.activityId) ??
                        submission.activityId;
                    return activityName;
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
                    if (!deadline) return "-";

                    const lateDays = daysOverdue(deadline);
                    /* const isOverdueTab = tab === "overdue"; */
                    const isDoneTab = tab === "done";

                    return (
                        <>
                            {ActivityDate(deadline)} {ActivityTime(deadline)}
                            {lateDays > 0 && (
                                <div className="text-xs opacity-70">
                                    {lateDays} days{" "}
                                    {isDoneTab ? "late" : "overdue"}
                                </div>
                            )}
                        </>
                    );
                },
            },
        ];

        // Conditionally add the "reviewed" column
        if (tab !== "not-reviewed" && tab !== "overdue") {
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

        // Only add the "actions" column for non-overdue tabs
        if (tab !== "overdue") {
            baseColumns.push({
                key: "actions",
                header: "Interact",
                className: "whitespace-nowrap px-4 py-3",
                render: (submission) => (
                    <Button onClick={() => setReviewing(submission)}>
                        {submission.reviewStatus != null
                            ? "Edit review"
                            : "Review"}
                    </Button>
                ),
            });
        }

        return baseColumns;
    }, [tab, lookups]);

    const overdueColumns: Column<OverdueSubmission & { activityId: string }>[] =
        useMemo(
            () => [
                {
                    key: "student",
                    field: "student",
                    header: "Student",
                    className: "px-4 py-3",
                    render: (item) => item.studentName,
                },
                {
                    key: "course",
                    field: "course",
                    header: "Course",
                    className: "px-4 py-3",
                    render: (item) => {
                        const courseId = lookups?.courseIdForActivity(
                            item.activityId,
                        );
                        const courseName =
                            lookups?.courseNameForActivity(item.activityId) ||
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
                    render: (item) => {
                        const activityName =
                            lookups?.activityName(item.activityId) ??
                            item.activityId;
                        return activityName;
                    },
                },
                {
                    key: "deadline",
                    field: "deadline",
                    header: "Deadline",
                    className: "px-4 py-3",
                    render: (item) => {
                        const deadline = lookups?.deadlineForActivity(
                            item.activityId,
                        );
                        if (!deadline) return "-";

                        return (
                            <>
                                {ActivityDate(deadline)}{" "}
                                {ActivityTime(deadline)}
                                <div className="text-xs opacity-70">
                                    {daysOverdue(deadline)} days overdue
                                </div>
                            </>
                        );
                    },
                },
            ],
            [lookups],
        );

    // Then transform the overdue data without nulling values
    const overdueItems = useMemo(() => {
        if (!overdueChecked || !lookups) return [];

        return [...overdueByActivity.entries()].flatMap(
            ([activityId, students]) =>
                students.map((u) => ({
                    ...u,
                    activityId,
                })),
        );
    }, [overdueByActivity, overdueChecked, lookups]);

    // Filtered overdue items
    const filteredOverdueItems = useMemo(() => {
        return search.trim()
            ? overdueItems.filter((item) =>
                  item.studentName
                      .toLowerCase()
                      .includes(search.trim().toLowerCase()),
              )
            : overdueItems;
    }, [overdueItems, search]);

    // Paged overdue items
    const pagedOverdueItems = useMemo(() => {
        return filteredOverdueItems.slice(
            (overduePage - 1) * PAGE_SIZE,
            overduePage * PAGE_SIZE,
        );
    }, [filteredOverdueItems, overduePage]);

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

            {tab === "overdue" && (
                <div>
                    {filteredOverdueItems.length === 0 ? (
                        <p className="text-text-dark dark:text-text-light">
                            Nobody's overdue.
                        </p>
                    ) : (
                        <>
                            <DataTable
                                items={pagedOverdueItems}
                                columns={overdueColumns}
                                getKey={(item) =>
                                    `${item.studentId}-${item.activityId}`
                                }
                                sortBy={overdueSortBy}
                                isLoading={!overdueChecked}
                                onSortChange={handleOverdueSortChange}
                                bodyClassName="text-text-dark dark:text-text-light"
                            />
                            <Pagination
                                page={overduePage}
                                pageSize={PAGE_SIZE}
                                totalCount={filteredOverdueItems.length}
                                onPageChange={setOverduePage}
                            />
                        </>
                    )}
                </div>
            )}

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

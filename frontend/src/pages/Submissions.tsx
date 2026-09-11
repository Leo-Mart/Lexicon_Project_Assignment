import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Button from "../components/Button";
import Pagination from "../components/Pagination";
import SortableTh from "../components/SortableTableHead";
import ReviewSubmissionModal from "../components/ReviewSubmissionModal";
import {
    fetchAllSubmissions,
    fetchSubmissionsPage,
    fetchOverdueByActivityId,
    setFeedback,
} from "../services/submissionService";
import { fetchActivitys } from "../services/activityService";
import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import { fetchUsers } from "../services/userService";
import { fetchCourses } from "../services/courseService";
import { useAuth } from "../hooks/useAuth";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { OverdueSubmission } from "../interfaces/submission/OverdueSubmission";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";

const PAGE_SIZE = 10;
const MS_PER_DAY = 24 * 60 * 60 * 1000;
const DEFAULT_SORT = "review-asc";

// How many whole days ago a deadline passed. Only meaningful once it's past.
const daysOverdue = (deadline: string): number =>
    Math.floor((Date.now() - new Date(deadline).getTime()) / MS_PER_DAY);

// How many whole days after the deadline a submission came in.
const daysLate = (deadline: string, submittedAt: string): number =>
    Math.floor(
        (new Date(submittedAt).getTime() - new Date(deadline).getTime()) /
            MS_PER_DAY,
    );

// Mock teacher review page: everything fetched and joined client-side.
export default function Submissions() {
    const { role } = useAuth();
    const [submissions, setSubmissions] = useState<SubmissionResponse[]>([]);
    const [activityNameById, setActivityNameById] = useState<
        Map<string, string>
    >(new Map());
    const [courseNameByModuleId, setCourseNameByModuleId] = useState<
        Map<string, string>
    >(new Map());
    const [courseIdByModuleId, setCourseIdByModuleId] = useState<
        Map<string, string>
    >(new Map());
    const [studentNameById, setStudentNameById] = useState<Map<string, string>>(
        new Map(),
    );
    const [activityModuleById, setActivityModuleById] = useState<
        Map<string, string>
    >(new Map());
    const [activityDeadlineById, setActivityDeadlineById] = useState<
        Map<string, string | null>
    >(new Map());
    const [loading, setLoading] = useState(true);
    const [reviewing, setReviewing] = useState<SubmissionResponse | null>(null);
    const [sortBy, setSortBy] = useState(DEFAULT_SORT);
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [tab, setTab] = useState<
        "not-reviewed" | "overdue" | "needs-completion" | "done"
    >("not-reviewed");

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

        const load = async () => {
            setLoading(true);

            const [submissionData, activities, users, courses] =
                await Promise.all([
                    fetchAllSubmissions(),
                    fetchActivitys(),
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
            setActivityNameById(
                new Map(activities.map((a) => [a.activityId, a.name])),
            );
            setActivityModuleById(
                new Map(activities.map((a) => [a.activityId, a.moduleId])),
            );
            setActivityDeadlineById(
                new Map(activities.map((a) => [a.activityId, a.deadline])),
            );
            setStudentNameById(new Map(users.map((u) => [u.id, u.name])));
            setCourseNameByModuleId(
                new Map(
                    courses.items.flatMap((c) =>
                        c.modules.map((m) => [m.moduleId, c.name] as const),
                    ),
                ),
            );
            setCourseIdByModuleId(
                new Map(
                    courses.items.flatMap((c) =>
                        c.modules.map((m) => [m.moduleId, c.courseId] as const),
                    ),
                ),
            );

            setLoading(false);
        };

        void load();
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
        if (overdueChecked || activityNameById.size === 0) {
            return;
        }

        const check = async () => {
            const entries = await Promise.all(
                [...activityNameById.keys()].map(async (id) => {
                    const overdue = await fetchOverdueByActivityId(id);
                    return [id, overdue] as const;
                }),
            );

            setOverdueByActivity(new Map(entries));
            setOverdueChecked(true);
        };

        void check();
    }, [overdueChecked, activityNameById]);

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

    if (role !== "Teacher") {
        return (
            <div className="p-4 text-text-dark dark:text-text-light">
                Teachers only.
            </div>
        );
    }

    if (loading) return <div className="p-4">Loading...</div>;

    const courseNameForActivity = (activityId: string) => {
        const moduleId = activityModuleById.get(activityId);
        return (moduleId && courseNameByModuleId.get(moduleId)) ?? "";
    };
    const courseIdForActivity = (activityId: string) => {
        const moduleId = activityModuleById.get(activityId);
        return moduleId ? courseIdByModuleId.get(moduleId) : undefined;
    };
    const courseName = (s: SubmissionResponse) =>
        courseNameForActivity(s.activityId);
    const studentName = (s: SubmissionResponse) =>
        studentNameById.get(s.studentId) ?? s.studentId;

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
                <div className="flex items-center gap-3">
                    <input
                        id="student-search"
                        type="search"
                        aria-label="Search for student"
                        placeholder="Search for student..."
                        className="bg-slate-700 text-white placeholder:text-slate-400 border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                        value={search}
                        onChange={(e) => {
                            setSearch(e.target.value);
                            setPage(1);
                            setOverduePage(1);
                        }}
                    />
                </div>

                <div className="flex flex-wrap items-center gap-2">
                    <Button
                        variant="primary"
                        className={
                            tab === "not-reviewed" ? "bg-accent-blue" : ""
                        }
                        onClick={() => handleTabChange("not-reviewed")}
                    >
                        Not reviewed
                        <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                            {notReviewedCount}
                        </span>
                    </Button>
                    <Button
                        variant="primary"
                        className={tab === "overdue" ? "bg-accent-blue" : ""}
                        onClick={() => handleTabChange("overdue")}
                    >
                        Overdue
                        <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                            {overdueChecked ? totalOverdue : "…"}
                        </span>
                    </Button>
                    <Button
                        variant="primary"
                        className={
                            tab === "needs-completion" ? "bg-accent-blue" : ""
                        }
                        onClick={() => handleTabChange("needs-completion")}
                    >
                        Needs completion
                        <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                            {needsCompletionCount}
                        </span>
                    </Button>
                    <Button
                        variant="primary"
                        className={tab === "done" ? "bg-accent-blue" : ""}
                        onClick={() => handleTabChange("done")}
                    >
                        Done
                        <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                            {doneCount}
                        </span>
                    </Button>
                </div>
            </div>

            {tab !== "overdue" && (
                <>
                    <div className="overflow-x-auto rounded-lg border border-accent-blue">
                        <table className="w-full text-left text-text-dark dark:text-text-light">
                            <thead className="bg-bg-window dark:bg-bg-window-dark">
                                <tr>
                                    <SortableTh
                                        field="student"
                                        label="Student"
                                        sortBy={sortBy}
                                        onSortChange={handleSortChange}
                                        isLoading={tableLoading}
                                    />
                                    <SortableTh
                                        field="course"
                                        label="Course"
                                        sortBy={sortBy}
                                        onSortChange={handleSortChange}
                                        isLoading={tableLoading}
                                    />
                                    <SortableTh
                                        field="activity"
                                        label="Activity"
                                        sortBy={sortBy}
                                        onSortChange={handleSortChange}
                                        isLoading={tableLoading}
                                    />
                                    <SortableTh
                                        field="deadline"
                                        label="Deadline"
                                        sortBy={sortBy}
                                        onSortChange={handleSortChange}
                                        isLoading={tableLoading}
                                    />
                                    {tab !== "not-reviewed" && (
                                        <SortableTh
                                            field="reviewed"
                                            label="Reviewed"
                                            sortBy={sortBy}
                                            onSortChange={handleSortChange}
                                            isLoading={tableLoading}
                                        />
                                    )}
                                    <th className="px-4 py-3"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {pagedSubmissions.map((s) => {
                                    const deadline = activityDeadlineById.get(
                                        s.activityId,
                                    );
                                    return (
                                        <tr
                                            key={s.submissionId}
                                            className="border-t border-accent-blue hover:bg-bg-window/40 dark:hover:bg-bg-window-dark/40"
                                        >
                                            <td className="px-4 py-3">
                                                {studentName(s)}
                                                {s.resubmittedAt != null && (
                                                    <span className="ml-2 rounded-full bg-accent-blue/30 px-2 py-0.5 text-xs">
                                                        Resubmitted
                                                    </span>
                                                )}
                                            </td>
                                            <td className="px-4 py-3">
                                                {courseIdForActivity(
                                                    s.activityId,
                                                ) ? (
                                                    <Link
                                                        className="hover:underline"
                                                        to={`/courses/${courseIdForActivity(s.activityId)}`}
                                                    >
                                                        {courseName(s) || "-"}
                                                    </Link>
                                                ) : (
                                                    (courseName(s) ?? "-")
                                                )}
                                            </td>
                                            <td className="px-4 py-3">
                                                {activityModuleById.get(
                                                    s.activityId,
                                                ) ? (
                                                    <Link
                                                        className="hover:underline"
                                                        to={`/module/${activityModuleById.get(s.activityId)}`}
                                                    >
                                                        {activityNameById.get(
                                                            s.activityId,
                                                        ) ?? s.activityId}
                                                    </Link>
                                                ) : (
                                                    (activityNameById.get(
                                                        s.activityId,
                                                    ) ?? s.activityId)
                                                )}
                                            </td>
                                            <td className="px-4 py-3">
                                                {deadline ? (
                                                    <>
                                                        {ActivityDate(deadline)}{" "}
                                                        {ActivityTime(deadline)}
                                                        {tab === "done"
                                                            ? daysLate(
                                                                  deadline,
                                                                  s.submittedAt,
                                                              ) > 0 && (
                                                                  <div className="text-xs opacity-70">
                                                                      {daysLate(
                                                                          deadline,
                                                                          s.submittedAt,
                                                                      )}{" "}
                                                                      days late
                                                                  </div>
                                                              )
                                                            : daysOverdue(
                                                                  deadline,
                                                              ) > 0 && (
                                                                  <div className="text-xs opacity-70">
                                                                      {daysOverdue(
                                                                          deadline,
                                                                      )}{" "}
                                                                      days
                                                                      overdue
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
                                                <Button
                                                    onClick={() =>
                                                        setReviewing(s)
                                                    }
                                                >
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
                                            colSpan={
                                                tab === "not-reviewed" ? 5 : 6
                                            }
                                            className="px-4 py-3 text-center"
                                        >
                                            Loading...
                                        </td>
                                    </tr>
                                )}
                                {!tableLoading &&
                                    pagedSubmissions.length === 0 && (
                                        <tr>
                                            <td
                                                colSpan={
                                                    tab === "not-reviewed"
                                                        ? 5
                                                        : 6
                                                }
                                                className="px-4 py-3 text-center"
                                            >
                                                {submissions.length === 0
                                                    ? "No submissions yet."
                                                    : "No submissions match your search."}
                                            </td>
                                        </tr>
                                    )}
                            </tbody>
                        </table>
                    </div>

                    <Pagination
                        page={page}
                        pageSize={PAGE_SIZE}
                        totalCount={totalCount}
                        onPageChange={setPage}
                    />
                </>
            )}

            {tab === "overdue" &&
                (() => {
                    const rows = [...overdueByActivity.entries()].flatMap(
                        ([activityId, students]) =>
                            students.map((u) => ({
                                ...u,
                                activityId,
                                moduleId: activityModuleById.get(activityId),
                                activityName:
                                    activityNameById.get(activityId) ??
                                    activityId,
                                courseName:
                                    courseNameForActivity(activityId) ||
                                    "Unknown course",
                                courseId: courseIdForActivity(activityId),
                                deadline:
                                    activityDeadlineById.get(activityId) ??
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
                                                                className="hover:underline"
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
                                                                className="hover:underline"
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
                    const deadline = activityDeadlineById.get(
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
                            studentName={studentName(reviewing)}
                            courseName={courseName(reviewing) || "-"}
                            activityName={
                                activityNameById.get(reviewing.activityId) ??
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

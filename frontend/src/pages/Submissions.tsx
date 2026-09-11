import { useEffect, useState } from "react";
import ReviewSubmissionModal from "../components/ReviewSubmissionModal";
import SubmissionTabs, {
    type SubmissionTab,
} from "../components/submissions/SubmissionTabs";
import SubmissionsTable from "../components/submissions/SubmissionsTable";
import OverdueTable from "../components/submissions/OverdueTable";
import {
    fetchAllSubmissions,
    fetchSubmissionsPage,
    fetchOverdueByActivityId,
    setFeedback,
} from "../services/submissionService";
import { fetchActivitys } from "../services/activityService";
import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import { daysOverdue } from "../constants/SubmissionDates";
import { fetchUsers } from "../services/userService";
import { fetchCourses } from "../services/courseService";
import { useAuth } from "../hooks/useAuth";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { OverdueSubmission } from "../interfaces/submission/OverdueSubmission";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import type { SubmissionLookups } from "../interfaces/submission/SubmissionLookups";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";

const PAGE_SIZE = 10;
const DEFAULT_SORT = "review-asc";

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
    const [tab, setTab] = useState<SubmissionTab>("not-reviewed");

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

    const handleTabChange = (value: SubmissionTab) => {
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
    const studentName = (studentId: string) =>
        studentNameById.get(studentId) ?? studentId;

    const lookups: SubmissionLookups = {
        activityNameById,
        activityModuleById,
        activityDeadlineById,
        courseNameForActivity,
        courseIdForActivity,
        studentName,
    };

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

                <SubmissionTabs
                    tab={tab}
                    onChange={handleTabChange}
                    notReviewedCount={notReviewedCount}
                    overdueCount={overdueChecked ? totalOverdue : null}
                    needsCompletionCount={needsCompletionCount}
                    doneCount={doneCount}
                />
            </div>

            {tab === "overdue" ? (
                <OverdueTable
                    overdueByActivity={overdueByActivity}
                    lookups={lookups}
                    search={search}
                    sortBy={overdueSortBy}
                    onSortChange={handleOverdueSortChange}
                    page={overduePage}
                    pageSize={PAGE_SIZE}
                    onPageChange={setOverduePage}
                />
            ) : (
                <SubmissionsTable
                    tab={tab}
                    submissions={pagedSubmissions}
                    lookups={lookups}
                    sortBy={sortBy}
                    onSortChange={handleSortChange}
                    tableLoading={tableLoading}
                    hasAnySubmissions={submissions.length > 0}
                    page={page}
                    pageSize={PAGE_SIZE}
                    totalCount={totalCount}
                    onPageChange={setPage}
                    onReview={setReviewing}
                />
            )}

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
                            studentName={studentName(reviewing.studentId)}
                            courseName={
                                courseNameForActivity(reviewing.activityId) || "-"
                            }
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

import { useEffect, useState } from "react";
import Button from "../components/Button";
import Pagination from "../components/Pagination";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
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
import {
    SubmissionReviewStatus,
    SubmissionReviewStatusNames,
} from "../constants/SubmissionReviewStatus";

// Extends FeedbackRequest with a read-only field so the modal can show what
// the student wrote; submissionText is stripped back out before saving.
interface ReviewFormValues extends FeedbackRequest {
    submissionText: string;
}

const reviewFormConfig: EntityFormConfig<ReviewFormValues> = {
    title: "Review submission",
    fields: [
        {
            name: "submissionText",
            label: "Submission",
            type: "textarea",
            readOnly: true,
        },
        {
            name: "feedback",
            label: "Feedback",
            type: "textarea",
            required: true,
            maxLength: 2000,
        },
        {
            name: "reviewStatus",
            label: "Outcome",
            type: "select",
            required: true,
            options: [
                { value: "", label: "Select an outcome..." },
                {
                    value: String(SubmissionReviewStatus.Approved),
                    label: "Approved",
                },
                {
                    value: String(SubmissionReviewStatus.NeedsCompletion),
                    label: "Needs completion",
                },
            ],
        },
    ],
};

const PAGE_SIZE = 10;
const MS_PER_DAY = 24 * 60 * 60 * 1000;

type SortOption = "not-reviewed-first" | "student" | "course" | "late-first";

const sortOptions: { value: SortOption; label: string }[] = [
    { value: "not-reviewed-first", label: "Not reviewed first" },
    { value: "late-first", label: "Submitted late first" },
    { value: "student", label: "Student name" },
    { value: "course", label: "Course name" },
];

// How many whole days ago a deadline passed. Only meaningful once it's past.
const daysOverdue = (deadline: string): number =>
    Math.floor((Date.now() - new Date(deadline).getTime()) / MS_PER_DAY);

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
    const [sortBy, setSortBy] = useState<SortOption>("not-reviewed-first");
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [tab, setTab] = useState<"submitted" | "overdue">("submitted");

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
    // Fetched eagerly for every activity so the summary bar and the picker's
    // warning markers are ready before the Overdue tab is even opened.
    const [overdueViewActivityId, setOverdueViewActivityId] = useState("");
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

            setLoading(false);
        };

        void load();
    }, [role]);

    useEffect(() => {
        if (role !== "Teacher") return;

        const loadPage = async () => {
            setTableLoading(true);

            const data = await fetchSubmissionsPage({
                search,
                sortBy,
                direction: "asc",
                page,
                pageSize: PAGE_SIZE,
            });

            setPagedSubmissions(data.items);
            setTotalCount(data.totalCount);
            setTableLoading(false);
        };

        void loadPage();
    }, [role, sortBy, search, page]);

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

    const handleReview = async (data: ReviewFormValues) => {
        if (!reviewing) return;

        const updated = await setFeedback(reviewing.submissionId, {
            feedback: data.feedback,
            reviewStatus: Number(data.reviewStatus) as SubmissionReviewStatus,
        });

        setSubmissions((prev) =>
            prev.map((s) =>
                s.submissionId === updated.submissionId ? updated : s,
            ),
        );
        setPagedSubmissions((prev) =>
            prev.map((s) =>
                s.submissionId === updated.submissionId ? updated : s,
            ),
        );
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
    const courseName = (s: SubmissionResponse) =>
        courseNameForActivity(s.activityId);
    const studentName = (s: SubmissionResponse) =>
        studentNameById.get(s.studentId) ?? s.studentId;
    const submittedCountFor = (activityId: string) =>
        submissions.filter((s) => s.activityId === activityId).length;

    const totalOverdue = [...overdueByActivity.values()].reduce(
        (sum, list) => sum + list.length,
        0,
    );
    const activitiesWithOverdue = [...overdueByActivity.values()].filter(
        (list) => list.length > 0,
    ).length;
    const notReviewedCount = submissions.filter(
        (s) => s.reviewStatus == null,
    ).length;

    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold text-text-dark dark:text-text-light mb-2">
                Submissions
            </h1>
            <p className="text-text-dark dark:text-text-light mb-4">
                {submissions.length} submission
                {submissions.length === 1 ? "" : "s"} total, {notReviewedCount}{" "}
                not reviewed
                {overdueChecked && (
                    <>
                        ,{" "}
                        <span
                            className={
                                totalOverdue > 0 ? "text-red-500 font-bold" : ""
                            }
                        >
                            {totalOverdue} overdue
                        </span>{" "}
                        across {activitiesWithOverdue} activit
                        {activitiesWithOverdue === 1 ? "y" : "ies"}
                    </>
                )}
            </p>

            <div className="flex gap-2 mb-4">
                <Button
                    variant="primary"
                    className={tab === "submitted" ? "bg-accent-blue" : ""}
                    onClick={() => setTab("submitted")}
                >
                    Submissions
                </Button>
                <Button
                    variant="primary"
                    className={tab === "overdue" ? "bg-accent-blue" : ""}
                    onClick={() => setTab("overdue")}
                >
                    Overdue
                </Button>
            </div>

            {tab === "submitted" && (
                <>
                    <div className="flex flex-wrap items-center gap-3 mb-4">
                        <label
                            htmlFor="sort-by"
                            className="text-text-dark dark:text-text-light"
                        >
                            Sort by
                        </label>
                        <select
                            id="sort-by"
                            className="bg-slate-700 text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                            value={sortBy}
                            onChange={(e) => {
                                setSortBy(e.target.value as SortOption);
                                setPage(1);
                            }}
                        >
                            {sortOptions.map((opt) => (
                                <option key={opt.value} value={opt.value}>
                                    {opt.label}
                                </option>
                            ))}
                        </select>

                        <label
                            htmlFor="student-search"
                            className="text-text-dark dark:text-text-light"
                        >
                            Student
                        </label>
                        <input
                            id="student-search"
                            type="search"
                            placeholder="Search by name..."
                            className="bg-slate-700 text-white placeholder:text-slate-400 border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                            value={search}
                            onChange={(e) => {
                                setSearch(e.target.value);
                                setPage(1);
                            }}
                        />
                    </div>

                    <div className="overflow-x-auto rounded-lg border border-accent-blue">
                        <table className="w-full text-left text-text-dark dark:text-text-light">
                            <thead className="bg-bg-window dark:bg-bg-window-dark">
                                <tr>
                                    <th className="px-4 py-3">Student</th>
                                    <th className="px-4 py-3">Course</th>
                                    <th className="px-4 py-3">Activity</th>
                                    <th className="px-4 py-3">Deadline</th>
                                    <th className="px-4 py-3">
                                        Submitted Late
                                    </th>
                                    <th className="px-4 py-3">Review</th>
                                    <th className="px-4 py-3">Feedback</th>
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
                                            </td>
                                            <td>{courseName(s) || "-"}</td>
                                            <td>
                                                {activityNameById.get(
                                                    s.activityId,
                                                ) ?? s.activityId}
                                            </td>
                                            <td>
                                                {deadline
                                                    ? `${ActivityDate(deadline)} ${ActivityTime(deadline)}`
                                                    : "-"}
                                            </td>
                                            <td>
                                                {s.submittedLate ? "Yes" : "No"}
                                            </td>
                                            <td>
                                                {s.reviewStatus != null
                                                    ? SubmissionReviewStatusNames[
                                                          s.reviewStatus
                                                      ]
                                                    : "Not reviewed"}
                                            </td>
                                            <td
                                                className="max-w-xs truncate"
                                                title={s.feedback ?? undefined}
                                            >
                                                {s.feedback ?? "-"}
                                            </td>
                                            <td>
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
                                            colSpan={8}
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
                                                colSpan={8}
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

            {tab === "overdue" && (
                <>
                    <div className="flex items-center gap-3 mb-4">
                        <label
                            htmlFor="overdue-activity"
                            className="text-text-dark dark:text-text-light"
                        >
                            Activity
                        </label>
                        <select
                            id="overdue-activity"
                            className="bg-slate-700 text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                            value={overdueViewActivityId}
                            onChange={(e) =>
                                setOverdueViewActivityId(e.target.value)
                            }
                        >
                            <option value="">Pick an activity...</option>
                            <option value="all">All activities</option>
                            {[...activityNameById.entries()]
                                .sort((a, b) => a[1].localeCompare(b[1]))
                                .map(([id, name]) => {
                                    const flagged =
                                        (overdueByActivity.get(id)?.length ??
                                            0) > 0;
                                    const label = `${name} (${submittedCountFor(id)} submitted)`;
                                    return (
                                        <option key={id} value={id}>
                                            {flagged ? `⚠ ${label}` : label}
                                        </option>
                                    );
                                })}
                        </select>
                    </div>

                    {overdueViewActivityId &&
                        (() => {
                            const isAll = overdueViewActivityId === "all";
                            const rows: {
                                studentId: string;
                                studentName: string;
                                activityId: string;
                                activityName: string;
                            }[] = isAll
                                ? [...overdueByActivity.entries()].flatMap(
                                      ([id, students]) =>
                                          students.map((u) => ({
                                              ...u,
                                              activityId: id,
                                              activityName:
                                                  activityNameById.get(id) ??
                                                  id,
                                          })),
                                  )
                                : (
                                      overdueByActivity.get(
                                          overdueViewActivityId,
                                      ) ?? []
                                  ).map((u) => ({
                                      ...u,
                                      activityId: overdueViewActivityId,
                                      activityName:
                                          activityNameById.get(
                                              overdueViewActivityId,
                                          ) ?? overdueViewActivityId,
                                  }));

                            // Group by course so "All activities" doesn't read
                            // as one undifferentiated list once there's more
                            // than a handful of courses.
                            const rowsByCourse = new Map<string, typeof rows>();
                            for (const row of rows) {
                                const course =
                                    courseNameForActivity(row.activityId) ||
                                    "Unknown course";
                                rowsByCourse.set(course, [
                                    ...(rowsByCourse.get(course) ?? []),
                                    row,
                                ]);
                            }

                            return (
                                <>
                                    {!isAll &&
                                        (() => {
                                            const deadline =
                                                activityDeadlineById.get(
                                                    overdueViewActivityId,
                                                );
                                            const isPast =
                                                deadline != null &&
                                                new Date() > new Date(deadline);
                                            return (
                                                <p className="mb-2">
                                                    <span className="text-text-dark dark:text-text-light">
                                                        Deadline:{" "}
                                                    </span>
                                                    {deadline ? (
                                                        <span
                                                            className={
                                                                isPast
                                                                    ? "text-red-500 font-bold"
                                                                    : "text-text-dark dark:text-text-light"
                                                            }
                                                        >
                                                            {ActivityDate(
                                                                deadline,
                                                            )}{" "}
                                                            {ActivityTime(
                                                                deadline,
                                                            )}
                                                            {isPast
                                                                ? ` (${daysOverdue(deadline)} days overdue)`
                                                                : " (not passed yet)"}
                                                        </span>
                                                    ) : (
                                                        <span className="text-text-dark dark:text-text-light">
                                                            None - can never be
                                                            overdue
                                                        </span>
                                                    )}
                                                </p>
                                            );
                                        })()}
                                    {rows.length === 0 ? (
                                        <p className="text-text-dark dark:text-text-light">
                                            Nobody's overdue.
                                        </p>
                                    ) : (
                                        [...rowsByCourse.entries()].map(
                                            ([course, courseRows]) => (
                                                <div
                                                    key={course}
                                                    className="mb-4"
                                                >
                                                    {isAll && (
                                                        <p className="text-text-dark dark:text-text-light font-bold mb-2">
                                                            {course}
                                                        </p>
                                                    )}
                                                    <div className="overflow-x-auto rounded-lg border border-accent-blue">
                                                        <table className="w-full text-left text-text-dark dark:text-text-light">
                                                            <thead className="bg-bg-window dark:bg-bg-window-dark">
                                                                <tr>
                                                                    <th className="px-4 py-3">
                                                                        Student
                                                                    </th>
                                                                    {isAll && (
                                                                        <>
                                                                            <th className="px-4 py-3">
                                                                                Activity
                                                                            </th>
                                                                            <th className="px-4 py-3">
                                                                                Days
                                                                                overdue
                                                                            </th>
                                                                        </>
                                                                    )}
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                {courseRows.map(
                                                                    (u, i) => {
                                                                        const deadline =
                                                                            activityDeadlineById.get(
                                                                                u.activityId,
                                                                            );
                                                                        return (
                                                                            <tr
                                                                                key={`${u.studentId}-${i}`}
                                                                                className="border-t border-accent-blue hover:bg-bg-window/40 dark:hover:bg-bg-window-dark/40"
                                                                            >
                                                                                <td className="px-4 py-3">
                                                                                    {
                                                                                        u.studentName
                                                                                    }
                                                                                </td>
                                                                                {isAll && (
                                                                                    <>
                                                                                        <td>
                                                                                            {
                                                                                                u.activityName
                                                                                            }
                                                                                        </td>
                                                                                        <td>
                                                                                            {deadline
                                                                                                ? daysOverdue(
                                                                                                      deadline,
                                                                                                  )
                                                                                                : "-"}
                                                                                        </td>
                                                                                    </>
                                                                                )}
                                                                            </tr>
                                                                        );
                                                                    },
                                                                )}
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            ),
                                        )
                                    )}
                                </>
                            );
                        })()}
                </>
            )}

            {reviewing && (
                <FormModal
                    config={reviewFormConfig}
                    initialValue={{
                        submissionText: reviewing.text,
                        feedback: reviewing.feedback ?? "",
                        // "" so the teacher has to pick an outcome, not silently keep a default.
                        reviewStatus:
                            reviewing.reviewStatus ??
                            ("" as unknown as SubmissionReviewStatus),
                    }}
                    onSave={handleReview}
                    onClose={() => setReviewing(null)}
                />
            )}
        </div>
    );
}

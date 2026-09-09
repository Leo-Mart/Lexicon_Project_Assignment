import { useState } from "react";
import { ActivityTime, ActivityDate } from "../constants/ActivityTimeConverter";
import { ActivityTypeNames } from "../constants/ActivityType";
import Button from "../components/Button";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
import { createSubmission } from "../services/submissionService";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import { SubmissionReviewStatusNames } from "../constants/SubmissionReviewStatus";
import { useAuth } from "../hooks/useAuth";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";

// Submitted/late state: drives the header dot and the "Status: ..." line.
const statusDotColor: Record<string, string> = {
    "Not submitted": "bg-gray-400",
    Submitted: "bg-blue-400",
    "Submitted (Late)": "bg-blue-400",
};
const statusTextColor: Record<string, string> = {
    Overdue: "text-red-600",
    "Not submitted": "text-gray-500",
    Submitted: "text-blue-600",
    "Submitted (Late)": "text-blue-600",
};

// Whether it's been reviewed, and the outcome: separate from submitted/late above.
const reviewTextColor: Record<string, string> = {
    "Not reviewed": "text-gray-500",
    Approved: "text-green-600",
    "Needs completion": "text-yellow-600",
};

const submissionFormConfig: EntityFormConfig<SubmissionRequest> = {
    title: "Add submission",
    fields: [
        {
            name: "text",
            label: "Submission",
            type: "textarea",
            required: true,
            maxLength: 2000,
        },
    ],
};

export default function ActivityCard({
    activity,
    submission,
    onSubmitted,
}: {
    activity: ActivityResponse;
    submission?: SubmissionResponse;
    onSubmitted?: (submission: SubmissionResponse) => void;
}) {
    const [isExpanded, setIsExpanded] = useState(false);
    const [addingSubmission, setAddingSubmission] = useState(false);

    // Submissions are a student-only concern: teachers get none of this.
    const { role } = useAuth();
    const isStudent = role === "Student";

    // Nothing to submit before the activity has even started.
    const hasStarted = new Date() >= new Date(activity.startAt);

    // No submission row yet: derive lateness from the deadline instead.
    const isPastDeadline =
        activity.deadline != null && new Date() > new Date(activity.deadline);
    const missingAndLate = !submission && isPastDeadline;
    const submissionStatusText = submission
        ? submission.submittedLate
            ? "Submitted (Late)"
            : "Submitted"
        : isPastDeadline
          ? "Overdue"
          : "Not submitted";

    // Only meaningful once submitted.
    const reviewStatusText = submission
        ? submission.reviewStatus != null
            ? SubmissionReviewStatusNames[submission.reviewStatus]
            : "Not reviewed"
        : null;

    // Corner badge, shown even collapsed: graded > overdue > submitted >
    // due soon > not submitted.
    const daysUntilDeadline =
        activity.deadline != null
            ? Math.ceil(
                  (new Date(activity.deadline).getTime() - new Date().getTime()) /
                      (1000 * 60 * 60 * 24),
              )
            : null;
    let cornerBadge: { text: string; color: string; textColor: string } | null = null;
    if (!isStudent) {
        // Teachers don't see per-student submission status here.
    } else if (reviewStatusText === "Approved") {
        cornerBadge = { text: "Graded", color: "bg-green-500", textColor: "text-white" };
    } else if (reviewStatusText === "Needs completion") {
        cornerBadge = {
            text: "Needs completion",
            color: "bg-bg-warning",
            textColor: "text-text-dark",
        };
    } else if (missingAndLate) {
        cornerBadge = { text: "Overdue", color: "bg-red-500", textColor: "text-white" };
    } else if (submission) {
        cornerBadge = {
            text: submissionStatusText,
            color: "bg-blue-400",
            textColor: "text-white",
        };
    } else if (
        hasStarted &&
        daysUntilDeadline != null &&
        daysUntilDeadline >= 0 &&
        daysUntilDeadline <= 5
    ) {
        cornerBadge = {
            text:
                daysUntilDeadline === 0
                    ? "Due today"
                    : `Due in ${daysUntilDeadline}d`,
            color: "bg-bg-warning",
            textColor: "text-text-dark",
        };
    } else if (hasStarted) {
        cornerBadge = { text: "Not submitted", color: "bg-gray-400", textColor: "text-white" };
    }

    // The header dot mirrors the corner badge so both use the same color.
    const headerDotColor = cornerBadge
        ? cornerBadge.color
        : statusDotColor[submissionStatusText];
    const headerDotLabel = cornerBadge ? cornerBadge.text : submissionStatusText;

    return (
        <div key={activity.activityId} className="relative w-80% m-3">
            <div className="rounded overflow-hidden shadow-lg bg-white">
            <div
                className="bg-bg-header w-full p-4 grid grid-cols-3 items-center cursor-pointer"
                role="button"
                tabIndex={0}
                aria-expanded={isExpanded}
                onClick={() => setIsExpanded(!isExpanded)}
                onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") {
                        e.preventDefault();
                        setIsExpanded(!isExpanded);
                    }
                }}
            >
                <div className="flex flex-row items-center gap-2">
                    <span
                        className={`text-xl transition-transform ${isExpanded ? "rotate-180" : ""}`}
                        aria-hidden="true"
                    >
                        ▾
                    </span>
                    <h2 className="font-bold text-xl">{activity.name}</h2>
                </div>
                <h3 className="font-bold text-l bg-bg-window text-text-dark p-1.5 rounded justify-self-center">
                    {ActivityTypeNames[activity.type]}
                </h3>
                <div className="flex flex-row justify-end items-center gap-2 justify-self-end">
                    {cornerBadge && (
                        <span
                            className={`text-xs font-bold px-2 py-1 rounded ${cornerBadge.color} ${cornerBadge.textColor}`}
                        >
                            {cornerBadge.text}
                        </span>
                    )}
                    {isStudent && hasStarted && missingAndLate && (
                        <div
                            className="rotate-45 w-5 h-5 bg-red-400"
                            title="Overdue"
                            aria-label="Overdue"
                            role="img"
                        ></div>
                    )}
                    {isStudent && hasStarted && !missingAndLate && (
                        <div
                            className={`rounded-full w-5 h-5 ${headerDotColor}`}
                            title={headerDotLabel}
                            aria-label={headerDotLabel}
                            role="img"
                        ></div>
                    )}
                </div>
            </div>
            {isExpanded && (
                <div className="bg-bg-window w-full">
                    <p className="text-m text-text-dark  p-3">
                        {activity.description}
                    </p>
                    <div className="flex flex-row justify-between items-center">
                        <p className="text-sm text-text-dark  p-3 pt-0">
                            {ActivityDate(activity.startAt)}
                            {" | "}
                            {ActivityTime(activity.startAt)}-
                            {ActivityTime(activity.endAt)}
                        </p>
                        {isStudent && hasStarted && (
                            <div className="flex flex-col items-start gap-2 px-3">
                                <span
                                    className={`text-base font-bold ${statusTextColor[submissionStatusText]}`}
                                >
                                    Status: {submissionStatusText}
                                </span>
                                {reviewStatusText && (
                                    <span
                                        className={`text-sm font-bold ${reviewTextColor[reviewStatusText]}`}
                                    >
                                        Review: {reviewStatusText}
                                    </span>
                                )}
                                {activity.deadline != null && !submission && (
                                    <p
                                        className={`text-sm ${missingAndLate ? "text-red-600" : "text-text-dark"}`}
                                    >
                                        {" Deadline "}
                                        {ActivityDate(activity.deadline)} {"  "}
                                        {ActivityTime(activity.deadline)}
                                    </p>
                                )}
                            </div>
                        )}
                    </div>
                    {isStudent && hasStarted && !submission && (
                        <Button
                            variant="primary"
                            onClick={() => setAddingSubmission(true)}
                        >
                            Add submission
                        </Button>
                    )}
                </div>
            )}
            </div>
            {addingSubmission && (
                <FormModal
                    config={submissionFormConfig}
                    initialValue={{ activityId: activity.activityId, text: "" }}
                    onSave={async (data) => {
                        const created = await createSubmission(data);
                        onSubmitted?.(created);
                    }}
                    onClose={() => setAddingSubmission(false)}
                />
            )}
        </div>
    );
}

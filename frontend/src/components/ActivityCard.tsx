import { useState } from "react";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import { ActivityTime, ActivityDate } from "../constants/ActivityTimeConverter";
import { ActivityTypeNames } from "../constants/ActivityType";
import Button from "../components/Button";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
import { createSubmission } from "../services/submissionService";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import { SubmissionReviewStatusNames } from "../constants/SubmissionReviewStatus";

// Colors per status text, for the header dot and the "Status: ..." line.
const statusDotColor: Record<string, string> = {
    Submitted: "bg-green-400",
    "Submitted (Late)": "bg-orange-400",
    Approved: "bg-green-400",
    "Needs completion": "bg-yellow-400",
};
const statusTextColor: Record<string, string> = {
    Late: "text-red-600",
    "Submitted (Late)": "text-orange-500",
    Submitted: "text-green-600",
    Approved: "text-green-600",
    "Needs completion": "text-yellow-600",
    "Not submitted": "text-gray-500",
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
    activity: ActivityRequest;
    submission?: SubmissionResponse;
    onSubmitted?: (submission: SubmissionResponse) => void;
}) {
    const [isExpanded, setIsExpanded] = useState(false);
    const [addingSubmission, setAddingSubmission] = useState(false);

    // Nothing to submit before the activity has even started.
    const hasStarted = new Date() >= new Date(activity.startAt);

    // No submission row yet: derive lateness from the deadline instead.
    const isPastDeadline =
        activity.deadline != null && new Date() > new Date(activity.deadline);
    const missingAndLate = !submission && isPastDeadline;
    const submissionStatusText = submission
        ? submission.reviewStatus != null
            ? SubmissionReviewStatusNames[submission.reviewStatus]
            : submission.isLate
              ? "Submitted (Late)"
              : "Submitted"
        : isPastDeadline
          ? "Late"
          : "Not submitted";

    return (
        <div
            key={activity.activityId}
            className="w-80% rounded overflow-hidden shadow-lg bg-white m-3"
        >
            <div className="bg-bg-header w-full p-4 grid grid-cols-3 items-center">
                <h2 className="font-bold text-xl">{activity.name}</h2>
                <h3 className="font-bold text-l bg-bg-window text-text-dark p-1.5 rounded justify-self-center">
                    {ActivityTypeNames[activity.type]}
                </h3>
                <div className="flex flex-row justify-end items-center gap-2 justify-self-end">
                    {hasStarted && missingAndLate && (
                        <div className="rotate-45 w-5 h-5 bg-red-400"></div>
                    )}
                    {hasStarted && submission && (
                        <div
                            className={`rounded-full w-5 h-5 ${statusDotColor[submissionStatusText]}`}
                        ></div>
                    )}
                    {hasStarted && !submission && !missingAndLate && (
                        <div className="rounded-full w-5 h-5 bg-gray-400"></div>
                    )}
                    <button
                        className="border-2 border-bg-header-dark p-1"
                        onClick={() => setIsExpanded(!isExpanded)}
                    >
                        {isExpanded ? "Show Less" : "Show More"}
                    </button>
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
                        {hasStarted && (
                            <div className="flex flex-col items-start gap-1 px-3">
                                <span
                                    className={`text-base font-bold ${statusTextColor[submissionStatusText]}`}
                                >
                                    Status: {submissionStatusText}
                                </span>
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
                    {hasStarted && !submission && (
                        <Button
                            variant="primary"
                            onClick={() => setAddingSubmission(true)}
                        >
                            Add submission
                        </Button>
                    )}
                </div>
            )}
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

import { useState } from "react";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import { ActivityTime, ActivityDate } from "../constants/ActivityTimeConverter";
import { ActivityTypeNames } from "../constants/ActivityType";
import Button from "../components/Button";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
import { createSubmission } from "../services/submissionService";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import { SubmissionStatusNames } from "../constants/SubmissionStatus";

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
}: {
    activity: ActivityRequest;
    submission?: SubmissionResponse;
}) {
    const [isExpanded, setIsExpanded] = useState(false);
    const [addingSubmission, setAddingSubmission] = useState(false);

    // No submission row yet: derive a status from the deadline instead.
    const isPastDeadline =
        activity.deadline != null && new Date() > new Date(activity.deadline);
    const submissionStatusText = submission
        ? SubmissionStatusNames[submission.status]
        : isPastDeadline
          ? "Late"
          : "Not submitted";

    return (
        <div
            key={activity.activityId}
            className="w-80% rounded overflow-hidden shadow-lg bg-white m-3"
        >
            <div className="bg-bg-header w-full p-4 flex flex-row justify-between items-center">
                <h2 className="font-bold text-xl">{activity.name}</h2>
                <h3 className="font-bold text-l bg-bg-window text-text-dark p-1.5 rounded">
                    {ActivityTypeNames[activity.type]}
                </h3>
                <div className="flex flex-row justify-between items-center w-35">
                    {activity.deadline != null && (
                        <div className="rotate-45 w-5 h-5 bg-red-400 flex items-center"></div>
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
                        {activity.deadline != null && (
                            <p className="text-sm text-red-600  p-3 pt-0">
                                {" Deadline "}
                                {ActivityDate(activity.deadline)} {"  "}
                                {ActivityTime(activity.deadline)}
                            </p>
                        )}
                    </div>
                    <p className="text-sm text-text-dark p-3 pt-0">
                        Status: {submissionStatusText}
                    </p>
                    {!submission && (
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
                        await createSubmission(data);
                    }}
                    onClose={() => setAddingSubmission(false)}
                />
            )}
        </div>
    );
}

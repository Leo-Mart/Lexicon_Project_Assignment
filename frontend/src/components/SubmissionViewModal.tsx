import Button from "./Button";
import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import { SubmissionReviewStatusNames } from "../constants/SubmissionReviewStatus";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import { ActivityType, ActivityTypeNames } from "../constants/ActivityType";
import { SUBMISSION_MAX_LENGTH } from "./ActivityCard";

interface SubmissionViewModalProps {
    submission: SubmissionResponse;
    activityName: string;
    activityType: ActivityType;
    onClose: () => void;
}

// Same color scheme as ActivityCard's corner badge, so a status reads the
// same way everywhere in the app.
const reviewBadge: Record<string, string> = {
    Approved: "bg-green-500 text-white",
    "Needs completion": "bg-bg-warning text-text-dark",
    "Not reviewed": "bg-gray-400 text-white",
};

// Its own modal chrome instead of ModalWrapper - this one wants to be much
// bigger than every other modal, and ModalWrapper is shared by several that
// don't.
export default function SubmissionViewModal({
    submission,
    activityName,
    activityType,
    onClose,
}: SubmissionViewModalProps) {
    const reviewStatusText =
        submission.reviewStatus != null
            ? SubmissionReviewStatusNames[submission.reviewStatus]
            : "Not reviewed";

    return (
        <>
            <div className="fixed inset-0 z-40 backdrop-blur-xs transition-opacity"></div>
            <dialog className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 z-50 flex w-full sm:w-1/2 h-[75vh] flex-col bg-bg rounded-lg">
                <nav className="bg-bg-header dark:bg-bg-header-dark text-text-light rounded-t-md flex items-center justify-between px-4 py-2">
                    <h2 className="flex items-center gap-2 text-lg text-text-light">
                        Your submission for
                        <span className="font-bold text-l bg-bg-window text-text-dark p-1.5 rounded">
                            {ActivityTypeNames[activityType]}
                        </span>
                        {activityName}
                    </h2>
                    <div className="flex items-center gap-2">
                        <span
                            className={`text-sm font-bold px-3 py-1.5 rounded ${reviewBadge[reviewStatusText]}`}
                        >
                            {reviewStatusText}
                        </span>
                        <button
                            className="bg-btn-cancel py-1 px-2 hover:brightness-110 hover:cursor-pointer rounded-full text-sm"
                            onClick={onClose}
                        >
                            &#10005;
                        </button>
                    </div>
                </nav>
                <div className="bg-bg py-3 px-3 flex-1 flex flex-col min-h-0">
                    <p className="text-sm text-text-dark mb-1">
                        {submission.text.length} / {SUBMISSION_MAX_LENGTH}{" "}
                        chars, submitted{" "}
                        {ActivityDate(submission.submittedAt)}{" "}
                        {ActivityTime(submission.submittedAt)}
                    </p>

                    {/* Grows to fill the modal's height; only this scrolls if the text is long. */}
                    <p className="whitespace-pre-wrap bg-bg-window dark:bg-bg-window-dark text-text-dark dark:text-text-light rounded p-2 mb-1 flex-1 min-h-0 overflow-y-auto">
                        {submission.text}
                    </p>

                    {submission.feedback && (
                        <>
                            <span
                                className={`self-start text-sm font-bold px-3 py-1.5 rounded mb-1 ${reviewBadge[reviewStatusText]}`}
                            >
                                {reviewStatusText}
                            </span>
                            <p className="text-sm font-semibold text-text-dark mb-1">
                                Feedback from teacher
                            </p>
                            <p className="whitespace-pre-wrap bg-bg-window dark:bg-bg-window-dark text-text-dark dark:text-text-light rounded p-2 mb-4 max-h-32 overflow-y-auto">
                                {submission.feedback}
                            </p>
                        </>
                    )}

                    <div className="flex items-center justify-end">
                        <Button variant="primary" onClick={onClose}>
                            Close
                        </Button>
                    </div>
                </div>
            </dialog>
        </>
    );
}

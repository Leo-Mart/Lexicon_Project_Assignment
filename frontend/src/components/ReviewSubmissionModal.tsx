import { useState } from "react";
import BigModal from "./BigModal";
import Button from "./Button";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";

const FEEDBACK_MAX_LENGTH = 2000;

interface ReviewSubmissionModalProps {
    submission: SubmissionResponse;
    studentName: string;
    courseName: string;
    activityName: string;
    deadlineText: string;
    onSave: (data: FeedbackRequest) => Promise<void>;
    onClose: () => void;
}

export default function ReviewSubmissionModal({
    submission,
    studentName,
    courseName,
    activityName,
    deadlineText,
    onSave,
    onClose,
}: ReviewSubmissionModalProps) {
    const [feedback, setFeedback] = useState(submission.feedback ?? "");
    // "" so the teacher has to pick an outcome, not silently keep a default.
    const [reviewStatus, setReviewStatus] = useState(
        submission.reviewStatus != null ? String(submission.reviewStatus) : "",
    );
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        try {
            await onSave({
                feedback,
                reviewStatus: Number(reviewStatus) as SubmissionReviewStatus,
            });
            onClose();
        } catch (err) {
            setError(err instanceof Error ? err.message : "Could not save.");
            setSaving(false);
        }
    };

    return (
        <BigModal
            title="Review submission"
            onClose={onClose}
            heightClass="h-[85vh]"
            onSubmit={handleSubmit}
        >
            <p className="text-sm text-text-dark mb-2">
                {studentName} &middot; {courseName} &middot; {activityName}{" "}
                &middot; Deadline: {deadlineText}
            </p>

            <div className="flex justify-end mb-1">
                <span className="text-sm text-text-dark">
                    {submission.text.length} chars
                </span>
            </div>
            {/* Grows to fill the space; scrolls if the text is long. */}
            <p className="whitespace-pre-wrap bg-bg-window dark:bg-bg-window-dark text-text-dark dark:text-text-light rounded p-2 mb-4 flex-1 min-h-0 overflow-y-auto">
                {submission.text}
            </p>

            <label
                htmlFor="feedback"
                className="text-sm font-semibold text-text-dark mb-1"
            >
                Feedback
            </label>
            <textarea
                id="feedback"
                className="shadow appearance-none border rounded w-full p-2 bg-white h-40 resize-none"
                maxLength={FEEDBACK_MAX_LENGTH}
                value={feedback}
                onChange={(e) => setFeedback(e.target.value)}
                required
            />
            <div className="flex justify-end mb-4">
                <span className="text-sm text-text-dark">
                    {feedback.length}/{FEEDBACK_MAX_LENGTH} chars
                </span>
            </div>

            {error && <p className="text-red-700 mb-2">{error}</p>}

            <div className="flex items-center justify-between gap-3">
                <div className="flex items-center gap-2">
                    <label
                        htmlFor="reviewStatus"
                        className="text-sm font-semibold text-text-dark"
                    >
                        Outcome
                    </label>
                    <select
                        id="reviewStatus"
                        className="shadow appearance-none border rounded p-2 bg-white"
                        value={reviewStatus}
                        onChange={(e) => setReviewStatus(e.target.value)}
                        required
                    >
                        <option value="">Select an outcome...</option>
                        <option value={SubmissionReviewStatus.Approved}>
                            Approved
                        </option>
                        <option value={SubmissionReviewStatus.NeedsCompletion}>
                            Needs completion
                        </option>
                    </select>
                </div>
                <div className="flex gap-3">
                    <Button type="submit" variant="confirm" disabled={saving}>
                        Save
                    </Button>
                    <Button type="button" variant="cancel" onClick={onClose}>
                        Cancel
                    </Button>
                </div>
            </div>
        </BigModal>
    );
}

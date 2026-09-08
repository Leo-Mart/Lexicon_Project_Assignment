import type { SubmissionReviewStatus } from "../../constants/SubmissionReviewStatus";

export interface SubmissionResponse {
    submissionId: string;
    activityId: string;
    studentId: string;
    text: string;
    submittedAt: string;
    submittedLate: boolean;
    reviewStatus: SubmissionReviewStatus | null;
    feedback: string | null;
    feedbackByTeacherId: string | null;
    feedbackAt: string | null;
    createdAt: string;
    updatedAt: string;
}

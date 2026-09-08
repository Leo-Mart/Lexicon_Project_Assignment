import type { SubmissionReviewStatus } from "../../constants/SubmissionReviewStatus";

export interface FeedbackRequest {
    feedback: string;
    reviewStatus: SubmissionReviewStatus;
}

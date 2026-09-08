import type { SubmissionStatus } from "../../constants/SubmissionStatus";

export interface SubmissionResponse {
    submissionId: string;
    activityId: string;
    studentId: string;
    text: string;
    submittedAt: string;
    status: SubmissionStatus;
    feedback: string | null;
    feedbackByTeacherId: string | null;
    feedbackAt: string | null;
    createdAt: string;
    updatedAt: string;
}

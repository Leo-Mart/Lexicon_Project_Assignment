// Set by a teacher after reviewing a submission. Null on the response until then.
export const SubmissionReviewStatus = {
    Approved: 1,
    NeedsCompletion: 2,
} as const;

export type SubmissionReviewStatus =
    (typeof SubmissionReviewStatus)[keyof typeof SubmissionReviewStatus];

export const SubmissionReviewStatusNames: Record<SubmissionReviewStatus, string> = {
    [SubmissionReviewStatus.Approved]: "Approved",
    [SubmissionReviewStatus.NeedsCompletion]: "Needs completion",
};

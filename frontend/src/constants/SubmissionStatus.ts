export const SubmissionStatus = {
    Submitted: 1,
    Late: 2,
} as const;

export type SubmissionStatus =
    (typeof SubmissionStatus)[keyof typeof SubmissionStatus];

export const SubmissionStatusNames: Record<SubmissionStatus, string> = {
    [SubmissionStatus.Submitted]: "Submitted",
    [SubmissionStatus.Late]: "Late",
};

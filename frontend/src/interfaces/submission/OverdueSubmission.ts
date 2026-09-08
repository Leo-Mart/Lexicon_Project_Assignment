// An enrolled student with no submission, past the activity's deadline -
// not a real submission, so it isn't shaped like SubmissionResponse.
export interface OverdueSubmission {
    activityId: string;
    studentId: string;
    studentName: string;
}

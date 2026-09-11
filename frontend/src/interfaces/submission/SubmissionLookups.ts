// Shared join data the submission tables need: activity/course/student
// names, keyed the same way across the main table and the overdue table.
export interface SubmissionLookups {
    activityNameById: Map<string, string>;
    activityModuleById: Map<string, string>;
    activityDeadlineById: Map<string, string | null>;
    courseNameForActivity: (activityId: string) => string;
    courseIdForActivity: (activityId: string) => string | undefined;
    studentName: (studentId: string) => string;
}

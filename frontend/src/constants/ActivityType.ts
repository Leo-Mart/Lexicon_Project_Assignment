export const ActivityType = {
    ELearning: 1,
    Lecture: 2,
    Practice: 3,
    Task: 4,
    Other: 5,
} as const;

export type ActivityType = (typeof ActivityType)[keyof typeof ActivityType];

export const ActivityTypeNames: Record<ActivityType, string> = {
    [ActivityType.ELearning]: "E-Learning",
    [ActivityType.Lecture]: "Lecture",
    [ActivityType.Practice]: "Practice",
    [ActivityType.Task]: "Task",
    [ActivityType.Other]: "Other",
};

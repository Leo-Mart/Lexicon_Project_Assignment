export const ActivityType = {
    ELearning: 1,
    Lecture: 2,
    Practice: 3,
    Task: 4,
    Other: 5,
} as const;

export type ActivityType = (typeof ActivityType)[keyof typeof ActivityType];

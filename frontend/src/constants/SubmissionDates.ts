const MS_PER_DAY = 24 * 60 * 60 * 1000;

// Whole days between a deadline and a later point in time.
const daysSince = (deadline: string, at: number): number =>
    Math.floor((at - new Date(deadline).getTime()) / MS_PER_DAY);

// How many whole days ago a deadline passed. Only meaningful once it's past.
export const daysOverdue = (deadline: string): number =>
    daysSince(deadline, Date.now());

// How many whole days after the deadline a submission came in.
export const daysLate = (deadline: string, submittedAt: string): number =>
    daysSince(deadline, new Date(submittedAt).getTime());

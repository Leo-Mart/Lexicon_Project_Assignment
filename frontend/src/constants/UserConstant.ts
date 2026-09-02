export const UserStatus = {
    Active: 1,
    Inactive: 2,
    Suspended: 3,
} as const;

export type UserStatus = (typeof UserStatus)[keyof typeof UserStatus];

export type UserRole = "Student" | "Teacher";

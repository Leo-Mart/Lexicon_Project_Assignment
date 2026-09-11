/* activityLookups.ts */
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { UserResponse } from "../interfaces/user/UserResponse";

export interface ActivityLookups {
    courseNameForActivity: (activityId: string) => string;
    courseIdForActivity: (activityId: string) => string | undefined;
    activityName: (activityId: string) => string;
    moduleIdForActivity: (activityId: string) => string | undefined;
    deadlineForActivity: (activityId: string) => string | null;
    studentName: (studentId: string) => string;
}

export const createActivityLookups = (
    activities: ActivityResponse[],
    courses: CourseResponse[],
    users: UserResponse[],
): ActivityLookups => {
    const activityNameById = new Map(
        activities.map((a) => [a.activityId, a.name]),
    );
    const activityModuleById = new Map(
        activities.map((a) => [a.activityId, a.moduleId]),
    );
    const activityDeadlineById = new Map(
        activities.map((a) => [a.activityId, a.deadline]),
    );
    const studentNameById = new Map(users.map((u) => [u.id, u.name]));
    const courseNameByModuleId = new Map(
        courses.flatMap((c) =>
            c.modules.map((m) => [m.moduleId, c.name] as const),
        ),
    );
    const courseIdByModuleId = new Map(
        courses.flatMap((c) =>
            c.modules.map((m) => [m.moduleId, c.courseId] as const),
        ),
    );

    return {
        activityName: (id) => activityNameById.get(id) ?? id,
        moduleIdForActivity: (id) => activityModuleById.get(id),
        deadlineForActivity: (id) => activityDeadlineById.get(id) ?? null,
        courseNameForActivity: (id) => {
            const moduleId = activityModuleById.get(id);
            return (moduleId && courseNameByModuleId.get(moduleId)) ?? "";
        },
        courseIdForActivity: (id) => {
            const moduleId = activityModuleById.get(id);
            return moduleId ? courseIdByModuleId.get(moduleId) : undefined;
        },
        studentName: (id) => studentNameById.get(id) ?? id,
    };
};

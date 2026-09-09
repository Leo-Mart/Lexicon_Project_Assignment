import type { ActivityResponse } from "../activity/ActivityResponse";
import type { CourseResponse } from "../course/CourseResponse";

export interface ModuleResponse {
    moduleId: string;
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    activities: ActivityResponse[];
    course: CourseResponse;
}

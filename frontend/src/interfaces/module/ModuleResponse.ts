import type { ActivityResponse } from "../activity/ActivityResponse";
import type { CourseResponse } from "../course/CourseResponse";
import type { ResourceResponse } from "../resource/ResourceResponse";

export interface ModuleResponse {
    moduleId: string;
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    activities: ActivityResponse[];
    course: CourseResponse;
    moduleResources: ResourceResponse[];
}

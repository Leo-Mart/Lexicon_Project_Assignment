import type { ActivityResponse } from "../activity/ActivityResponse";

export interface ModuleResponse {
    moduleId: string;
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    activities: ActivityResponse[];
}

import type { ModuleResponse } from "../module/ModuleResponse";

export interface CourseResponse {
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    modules: ModuleResponse[];
}

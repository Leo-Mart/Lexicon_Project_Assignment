import type { ModuleResponse } from "../module/ModuleResponse";
import type { ResourceResponse } from "../resource/ResourceResponse";

export interface CourseResponse {
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    modules: ModuleResponse[];
    courseResources: ResourceResponse[];
}

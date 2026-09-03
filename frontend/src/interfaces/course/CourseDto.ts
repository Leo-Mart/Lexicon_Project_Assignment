import type { ModuleDto } from "../module/ModuleResponse";

export interface CourseDto {
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    modules: ModuleDto[];
}

import type { Activity } from "../Activity";
export interface ModuleDto {
    moduleId: string;
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    activities: Activity[];
}

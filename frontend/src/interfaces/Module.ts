import type { Activity } from "./Activity";

export interface Module {
    name: string;
    startDate: string;
    endDate: string;
    moduleId: string;
    activities: Activity[];
}

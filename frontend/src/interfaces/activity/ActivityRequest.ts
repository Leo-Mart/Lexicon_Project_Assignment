import type { ActivityType } from "../../constants/ActivityType";

export interface ActivityRequest {
    moduleId: string;
    type: ActivityType;
    name: string;
    description: string;
    startAt: string;
    endAt: string;
    deadline: string | null;
}

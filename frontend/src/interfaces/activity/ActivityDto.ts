import type { ActivityType } from "../../constants/ActivityType";

export interface ActivityDto {
    activityId: string;
    moduleId: string;
    type: ActivityType;
    name: string;
    description: string;
    startAt: string;
    endAt: string;
    createdAt: string;
    updatedAt: string;
    deadline: string | null;
}

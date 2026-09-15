import type { ActivityType } from "../../constants/ActivityType";
import type { ResourceResponse } from "../resource/ResourceResponse";

export interface ActivityResponse {
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
    activityResources: ResourceResponse[];
}

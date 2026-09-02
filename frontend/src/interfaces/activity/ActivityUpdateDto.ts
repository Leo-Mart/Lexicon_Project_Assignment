import type { ActivityType } from "../../constants/ActivityType";

export interface ActivityUpdateDto {
    type: ActivityType;
    name: string;
    description: string;
    startAt: string;
    endAt: string;
    deadline: string | null;
}
import type { UserResponse } from "../user/UserResponse";

export interface ResourceResponse {
    resourceId: string;
    createdByTeacherId: string;
    name: string;
    description: string;
    content?: string | null;
    uri?: string | undefined;
    createdAt: string;
    updatedAt: string;
    createdByTeacher: UserResponse;
}

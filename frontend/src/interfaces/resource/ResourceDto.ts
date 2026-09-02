export interface ResourceDto {
    resourceId: string;
    createdByTeacherId: string;
    name: string;
    description: string;
    content?: string | null;
    uri?: string | null;
    createdAt: string;
    updatedAt: string;
}
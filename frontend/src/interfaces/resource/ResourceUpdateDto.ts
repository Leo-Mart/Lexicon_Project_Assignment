export interface ResourceUpdateDto {
    name: string;
    description: string;
    content?: string | null;
    uri?: string | null;
}

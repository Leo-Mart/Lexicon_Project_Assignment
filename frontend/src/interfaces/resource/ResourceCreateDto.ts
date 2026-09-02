export interface ResourceCreateDto {
    name: string;
    description: string;
    content?: string | null;
    uri?: string | null;
}
import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import type { OverdueSubmission } from "../interfaces/submission/OverdueSubmission";

const API_URL = API_BASE_URL + "/submissions";

export const fetchAllSubmissions = async (): Promise<SubmissionResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch submissions: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse[];
};

export const fetchSubmissionById = async (
    id: string,
): Promise<SubmissionResponse> => {
    const response = await authFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch submission: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse;
};

export const getCurrentUserSubmissions = async (): Promise<
    SubmissionResponse[]
> => {
    const response = await authFetch(`${API_URL}/me`);

    if (!response.ok) {
        throw new Error(`Failed to fetch submissions: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse[];
};

export const fetchOverdueByActivityId = async (
    activityId: string,
): Promise<OverdueSubmission[]> => {
    const response = await authFetch(
        `${API_URL}/activity/${activityId}/overdue`,
    );

    if (!response.ok) {
        throw new Error(
            `Failed to fetch overdue submissions: ${response.status}`,
        );
    }

    return (await response.json()) as OverdueSubmission[];
};

export const createSubmission = async (
    newSubmission: SubmissionRequest,
): Promise<SubmissionResponse> => {
    const response = await authFetch(API_URL, {
        method: HttpMethod.POST,
        headers: JSON_HEADERS,
        body: JSON.stringify(newSubmission),
    });

    if (!response.ok) {
        throw new Error(`Could not create the submission: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse;
};

//  [HttpPut("{submissionId:guid}/feedback")]
export const setFeedback = async (
    submissionId: string,
    feedback: FeedbackRequest,
): Promise<SubmissionResponse> => {
    const response = await authFetch(`${API_URL}/${submissionId}/feedback`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(feedback),
    });

    if (!response.ok) {
        throw new Error(`Could not set the feedback: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse;
};

export const fetchSubmissionsByActivityId = async (
    activityId: string,
): Promise<SubmissionResponse[]> => {
    const response = await authFetch(`${API_URL}/activity/${activityId}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch submissions: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse[];
};

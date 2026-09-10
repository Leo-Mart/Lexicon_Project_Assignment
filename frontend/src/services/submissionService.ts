import { authFetch } from "./authService";
import { API_BASE_URL, HttpMethod, JSON_HEADERS } from "../constants/Constants";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { FeedbackRequest } from "../interfaces/submission/FeedbackRequest";
import type { OverdueSubmission } from "../interfaces/submission/OverdueSubmission";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import type { PagedResponse } from "../interfaces/common/PagedResponse";
import type { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";

const API_URL = API_BASE_URL + "/submissions";

// Full, unpaginated list - used for the overview stats and the Overdue tab,
// which both need every submission, not just one page of them.
export const fetchAllSubmissions = async (): Promise<SubmissionResponse[]> => {
    const response = await authFetch(API_URL);

    if (!response.ok) {
        throw new Error(`Failed to fetch submissions: ${response.status}`);
    }

    return (await response.json()) as SubmissionResponse[];
};

// One page of submissions, searched/sorted server-side - for the Submissions table.
export const fetchSubmissionsPage = async (
    query: QueryParameters,
    reviewStatus?: SubmissionReviewStatus | null,
): Promise<PagedResponse<SubmissionResponse>> => {
    const params = new URLSearchParams({
        search: query.search,
        sortBy: query.sortBy,
        direction: query.direction,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
    });
    // Omitting the param means "not reviewed" server-side, so only set it when filtering to a status.
    if (reviewStatus != null) {
        params.set("reviewStatus", String(reviewStatus));
    }

    const response = await authFetch(`${API_URL}/paged?${params.toString()}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch submissions: ${response.status}`);
    }

    return (await response.json()) as PagedResponse<SubmissionResponse>;
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

//  [HttpPut("{submissionId:guid}")]
export const updateSubmission = async (
    submissionId: string,
    data: { text: string },
): Promise<SubmissionResponse> => {
    const response = await authFetch(`${API_URL}/${submissionId}`, {
        method: HttpMethod.PUT,
        headers: JSON_HEADERS,
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        throw new Error(`Could not update the submission: ${response.status}`);
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

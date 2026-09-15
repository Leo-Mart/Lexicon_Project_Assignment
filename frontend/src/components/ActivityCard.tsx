import { useState } from "react";
import { ActivityTime, ActivityDate } from "../utils/ActivityTimeConverter";
import { ActivityType, ActivityTypeNames } from "../constants/ActivityType";
import Button from "../components/Button";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
import {
    createSubmission,
    updateSubmission,
} from "../services/submissionService";
import type { SubmissionRequest } from "../interfaces/submission/SubmissionRequest";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import { useAuth } from "../hooks/useAuth";
import ConfirmDialog from "./ConfirmDialog";
import SubmissionViewModal from "./SubmissionViewModal";
import {
    createActivityFormConfig,
    createResourceFormConfig,
} from "../types/formSchemas";
import { createPortal } from "react-dom";
import ModalActivityDetails from "./ModalActivityDetails";
import StatusBadge from "./StatusBadge";
import ResourceCard from "./ResourceCard";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import {
    addResourceToActivity,
    createResource,
} from "../services/resourceService";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";

// Shared with SubmissionViewModal so the char count there matches this limit.
export const SUBMISSION_MAX_LENGTH = 2000;

const submissionFormConfig: EntityFormConfig<SubmissionRequest> = {
    title: "Add submission",
    fields: [
        {
            name: "text",
            label: "Submission",
            type: "textarea",
            required: true,
            maxLength: SUBMISSION_MAX_LENGTH,
        },
    ],
    widthClass: "w-full sm:w-1/2",
    heightClass: "h-[75vh]",
};

// Same fields as adding a submission, just a different modal title.
const resubmitFormConfig: EntityFormConfig<SubmissionRequest> = {
    ...submissionFormConfig,
    title: "Resubmit",
};

export default function ActivityCard({
    activity,
    submission,
    onSubmitted,
    editActivity,
    deleteActivity,
    addNewResource,
    editResource,
    deleteResource,
}: {
    activity: ActivityResponse;
    courseName: string;
    submission?: SubmissionResponse;
    onSubmitted?: (submission: SubmissionResponse) => void;
    editActivity: (activityId: string, payload: ActivityRequest) => void;
    deleteActivity: (activityId: string) => void;
    addNewResource: (resource: ResourceResponse) => void;
    editResource: (resourcerId: string, payload: ResourceRequest) => void;
    deleteResource: (resourceId: string) => void;
}) {
    const [isExpanded, setIsExpanded] = useState(false);
    const [addingSubmission, setAddingSubmission] = useState(false);
    const [viewingSubmission, setViewingSubmission] = useState(false);
    const [resubmitting, setResubmitting] = useState(false);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState(false);
    const [showEditActivityForm, setShowEditActivityForm] = useState(false);
    const [showDetailsModal, setShowDetailsModal] = useState(false);
    const [showCreateResourceForm, setShowCreateResourceForm] = useState(false);

    const { isAuthenticated, role } = useAuth();

    // Submissions are a student-only concern: teachers get none of this.
    const isStudent = role === "Student";

    // Only hand-in work types take a submission - not lectures/e-learning/other.
    const isSubmittable =
        activity.type === ActivityType.Task ||
        activity.type === ActivityType.Practice;

    // Nothing to submit before the activity has even started.
    const hasStarted = new Date() >= new Date(activity.startAt);

    // Same color scheme as ModuleSideViewPart: past/current/upcoming.
    // Compared by calendar day, not exact time - a lecture later today is
    // still "today", not "upcoming".
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const startDay = new Date(activity.startAt);
    startDay.setHours(0, 0, 0, 0);
    const endDay = new Date(activity.endAt);
    endDay.setHours(0, 0, 0, 0);
    const deadlineDay =
        activity.deadline != null ? new Date(activity.deadline) : null;
    deadlineDay?.setHours(0, 0, 0, 0);
    const isPastActivity = endDay < today;
    const isFutureActivity = startDay > today;
    const dateColor = isPastActivity
        ? "bg-gray-300 text-gray-600"
        : isFutureActivity
          ? "bg-accent-blue text-black"
          : "bg-btn-confirm text-black";

    // No submission row yet: derive lateness from the deadline instead.
    const isPastDeadline =
        activity.deadline != null && new Date() > new Date(activity.deadline);
    const missingAndLate = !submission && isPastDeadline;
    const submissionStatusText = submission
        ? submission.submittedLate
            ? "Submitted (Late)"
            : "Submitted"
        : isPastDeadline
          ? "Overdue"
          : "Not submitted";

    // Shown after the activity name in the Add submission/Resubmit title bar.
    const deadlineSuffix =
        activity.deadline != null
            ? ` · Deadline: ${ActivityDate(activity.deadline)} ${ActivityTime(activity.deadline)}`
            : "";

    // Corner badge, shown even collapsed: graded > overdue > submitted >
    // due soon > not submitted.
    const daysUntilDeadline =
        activity.deadline != null
            ? Math.round(
                  (deadlineDay!.getTime() - today.getTime()) /
                      (1000 * 60 * 60 * 24),
              )
            : null;
    let statusText: string | null = null;
    if (isSubmittable && isStudent) {
        // Teachers, and non-submittable activity types, see no badge.
        if (
            submission != null &&
            submission.reviewStatus === SubmissionReviewStatus.Approved
        ) {
            statusText = "Approved";
        } else if (
            submission != null &&
            submission.reviewStatus === SubmissionReviewStatus.NeedsCompletion
        ) {
            statusText = "Incomplete";
        } else if (missingAndLate) {
            statusText = "Overdue";
        } else if (submission) {
            statusText = submissionStatusText;
        } else if (
            hasStarted &&
            daysUntilDeadline != null &&
            daysUntilDeadline >= 0 &&
            daysUntilDeadline <= 5
        ) {
            statusText =
                daysUntilDeadline === 0
                    ? "Due today"
                    : `Due in ${daysUntilDeadline}d`;
        } else if (hasStarted) {
            statusText = "Not submitted";
        }
    }

    return (
        <div key={activity.activityId} className="relative w-80% m-3">
            <div className="rounded overflow-hidden shadow-lg bg-bg">
                <div
                    className="bg-bg-header w-full p-4 flex flex-row justify-between items-center gap-2 cursor-pointer"
                    role="button"
                    tabIndex={0}
                    aria-expanded={isExpanded}
                    onClick={() => setIsExpanded(!isExpanded)}
                    onKeyDown={(e) => {
                        if (e.key === "Enter" || e.key === " ") {
                            e.preventDefault();
                            setIsExpanded(!isExpanded);
                        }
                    }}
                >
                    <div className="flex flex-row items-center gap-2 min-w-0 flex-1">
                        <span
                            className={`text-3xl text-text-light transition-transform shrink-0 ${isExpanded ? "rotate-180" : ""}`}
                            aria-hidden="true"
                        >
                            ▾
                        </span>
                        <h3 className="font-bold w-18 text-l text-center bg-bg-window text-text-dark p-1.5 rounded shrink-0">
                            {ActivityTypeNames[activity.type]}
                        </h3>
                        <h2
                            className="font-bold text-text-light text-xl truncate min-w-0"
                            title={activity.name}
                        >
                            {activity.name}
                        </h2>
                    </div>
                    <div className="flex flex-row justify-end items-center gap-2">
                        {activity.type === ActivityType.Lecture && (
                            <span
                                className={`font-bold text-l p-1.5 rounded ${dateColor}`}
                            >
                                {ActivityDate(activity.startAt)}{" "}
                                {ActivityTime(activity.startAt)}
                            </span>
                        )}
                        {statusText && StatusBadge(statusText)}
                        {isAuthenticated && role === "Teacher" ? (
                            <div className="flex gap-1">
                                <Button
                                    variant="confirm"
                                    onClick={() =>
                                        setShowEditActivityForm(true)
                                    }
                                    className="hover:cursor-pointer"
                                >
                                    Edit
                                </Button>
                                <Button
                                    variant="cancel"
                                    onClick={() => setConfirmDeleteOpen(true)}
                                    className="hover:cursor-pointer"
                                >
                                    Delete
                                </Button>
                            </div>
                        ) : (
                            ""
                        )}
                    </div>
                </div>
                {isExpanded && (
                    <div className="bg-bg-white w-full">
                        <p className="text-m text-text-dark  p-3">
                            {activity.description}
                        </p>
                        <div className="flex flex-row justify-between items-center">
                            <p className="text-sm text-text-dark p-3 pt-0">
                                {activity.deadline != null ? (
                                    <>
                                        Deadline:{" "}
                                        {ActivityDate(activity.deadline)}{" "}
                                        {ActivityTime(activity.deadline)}
                                    </>
                                ) : ActivityDate(activity.startAt) ===
                                  ActivityDate(activity.endAt) ? (
                                    <>
                                        Scheduled:{" "}
                                        {ActivityDate(activity.startAt)}
                                        {" | "}
                                        {ActivityTime(activity.startAt)}-
                                        {ActivityTime(activity.endAt)}
                                    </>
                                ) : (
                                    <>
                                        Scheduled:{" "}
                                        {ActivityDate(activity.startAt)}{" "}
                                        {ActivityTime(activity.startAt)}
                                        {" - "}
                                        {ActivityDate(activity.endAt)}{" "}
                                        {ActivityTime(activity.endAt)}
                                    </>
                                )}
                            </p>
                            {isStudent && missingAndLate && (
                                <p className="text-sm text-red-600 px-3">
                                    Overdue - not yet submitted
                                </p>
                            )}
                        </div>
                        <div className="flex flex-row justify-between items-center p-2">
                            <div>
                                {isAuthenticated && role === "Teacher" ? (
                                    <Button
                                        onClick={() =>
                                            setShowCreateResourceForm(true)
                                        }
                                    >
                                        Add resource
                                    </Button>
                                ) : (
                                    ""
                                )}
                                {activity.activityResources?.map((resource) => (
                                    <ResourceCard
                                        key={resource.resourceId}
                                        resource={resource}
                                        editResource={editResource}
                                        deleteResource={deleteResource}
                                    />
                                ))}
                            </div>
                            <div className="flex gap-2">
                                {isStudent &&
                                    isSubmittable &&
                                    hasStarted &&
                                    !submission && (
                                        <Button
                                            variant="primary"
                                            onClick={() =>
                                                setAddingSubmission(true)
                                            }
                                        >
                                            Add submission
                                        </Button>
                                    )}
                                {isStudent &&
                                    isSubmittable &&
                                    hasStarted &&
                                    submission && (
                                        <>
                                            <Button
                                                variant="primary"
                                                onClick={() =>
                                                    setViewingSubmission(true)
                                                }
                                            >
                                                View submission
                                            </Button>
                                            {submission.reviewStatus ===
                                                SubmissionReviewStatus.NeedsCompletion && (
                                                <Button
                                                    variant="primary"
                                                    onClick={() =>
                                                        setResubmitting(true)
                                                    }
                                                >
                                                    Resubmit
                                                </Button>
                                            )}
                                        </>
                                    )}
                            </div>
                        </div>
                    </div>
                )}
            </div>
            {addingSubmission && (
                <FormModal
                    config={{
                        ...submissionFormConfig,
                        title: "Add submission for",
                    }}
                    titleBadge={ActivityTypeNames[activity.type]}
                    titleSuffix={`${activity.name}${deadlineSuffix}`}
                    initialValue={{ activityId: activity.activityId, text: "" }}
                    onSave={async (data) => {
                        const created = await createSubmission(data);
                        onSubmitted?.(created);
                    }}
                    onClose={() => setAddingSubmission(false)}
                />
            )}
            {viewingSubmission && submission && (
                <SubmissionViewModal
                    submission={submission}
                    activityName={activity.name}
                    activityType={activity.type}
                    onClose={() => setViewingSubmission(false)}
                />
            )}
            {resubmitting && submission && (
                <FormModal
                    config={{
                        ...resubmitFormConfig,
                        title: "Resubmit for",
                    }}
                    titleBadge={ActivityTypeNames[activity.type]}
                    titleSuffix={`${activity.name}${deadlineSuffix}`}
                    initialValue={{
                        activityId: activity.activityId,
                        text: submission.text,
                    }}
                    onSave={async (data) => {
                        const updated = await updateSubmission(
                            submission.submissionId,
                            { text: data.text },
                        );
                        onSubmitted?.(updated);
                    }}
                    onClose={() => setResubmitting(false)}
                />
            )}
            {showDetailsModal &&
                createPortal(
                    <ModalActivityDetails
                        open={showDetailsModal}
                        activity={activity}
                        onClose={() => setShowDetailsModal(false)}
                    />,
                    document.body!,
                )}
            {confirmDeleteOpen && (
                <ConfirmDialog
                    open={confirmDeleteOpen}
                    title="Delete Activity"
                    message={`Are you sure you want to delete the activity: ${activity.name}`}
                    onCancel={() => setConfirmDeleteOpen(false)}
                    onConfirm={() => deleteActivity(activity.activityId)}
                />
            )}
            {showEditActivityForm && (
                <FormModal
                    config={createActivityFormConfig}
                    initialValue={{
                        moduleId: activity.moduleId ?? "",
                        name: activity.name,
                        description: activity.description,
                        startAt: activity.startAt,
                        endAt: activity.endAt,
                        deadline: activity.deadline,
                        type: activity.type,
                    }}
                    onSave={async (data) =>
                        editActivity(activity.activityId, data)
                    }
                    onClose={() => setShowEditActivityForm(false)}
                />
            )}
            {showCreateResourceForm && (
                <FormModal
                    config={{
                        ...createResourceFormConfig,
                        title: `Add resource`,
                    }}
                    titleBadge={ActivityTypeNames[activity.type]}
                    titleSuffix={activity.name}
                    initialValue={{
                        name: "",
                        description: "",
                        content: "",
                        uri: undefined,
                    }}
                    onSave={async (data) => {
                        const resp = await createResource(data);
                        await addResourceToActivity(
                            resp.resourceId,
                            activity.activityId,
                        );
                        addNewResource(resp);
                    }}
                    onClose={() => setShowCreateResourceForm(false)}
                />
            )}
        </div>
    );
}

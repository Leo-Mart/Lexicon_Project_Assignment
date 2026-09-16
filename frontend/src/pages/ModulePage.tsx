import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ModuleSideView from "../components/ModuleSideView";
import { fetchModuleById } from "../services/moduleService";
import { getCurrentUserSubmissions } from "../services/submissionService";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import ActivityCard from "../components/ActivityCard";
import { useAuth } from "../hooks/useAuth";
import FormModal from "../components/FormModal";
import {
    createActivity,
    deleteActivity,
    updateActivity,
} from "../services/activityService";
import { ActivityType, ActivityTypeNames } from "../constants/ActivityType";
import { SubmissionReviewStatus } from "../constants/SubmissionReviewStatus";
import {
    addResourceToModule,
    createResource,
    deleteResource,
    updateResource,
} from "../services/resourceService";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import ResourceCard from "../components/ResourceCard";
import {
    createActivityFormConfig,
    createResourceFormConfig,
} from "../types/formSchemas";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import Button from "../components/Button";
import ActivitySchedule from "../components/ActivitySchedule";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import ErrorDisplay from "../components/ErrorDisplay";
import toast, { Toaster } from "react-hot-toast";

export default function ModulePage() {
    const { moduleId } = useParams<{ moduleId: string }>();
    const [module, setModule] = useState<ModuleResponse | undefined>(undefined);
    const [submissionsByActivityId, setSubmissionsByActivityId] = useState<
        Map<string, SubmissionResponse>
    >(new Map());
    const [moduleResources, setModuleResources] = useState<
        ResourceResponse[] | undefined
    >(undefined);
    const [moduleActivities, setModuleActivities] = useState<
        ActivityResponse[] | undefined
    >(undefined);
    const [activityTypeFilter, setActivityTypeFilter] = useState<
        ActivityType | "all"
    >("all");

    const [sortAscending, setSortAscending] = useState(true);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showCreateActivityForm, setShowCreateActivityForm] = useState(false);
    const [showCreateResourceForm, setShowCreateResourceForm] = useState(false);

    const { isAuthenticated, role } = useAuth();

    useEffect(() => {
        const fetchModule = async () => {
            setLoading(true);
            setError(null);
            try {
                if (moduleId === undefined) {
                    throw new Error("Could not find Id for module");
                }
                const moduleData = await fetchModuleById(moduleId);
                setModule(moduleData);
                setModuleActivities(moduleData.activities);
                setModuleResources(moduleData.moduleResources);

                if (role === "Student") {
                    const submissions = await getCurrentUserSubmissions();

                    setSubmissionsByActivityId(
                        new Map(
                            submissions.map((submission) => [
                                submission.activityId,
                                submission,
                            ]),
                        ),
                    );
                } else {
                    setSubmissionsByActivityId(new Map());
                }
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch module",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchModule();
    }, [moduleId, role]);

    const handleResourceEdit = async (
        resourceId: string,
        payload: ResourceRequest,
    ) => {
        await updateResource(resourceId, payload);
        const updatedResources: ResourceResponse[] = moduleResources!.map(
            (resource) => {
                if (resource.resourceId === resourceId) {
                    resource.name = payload.name;
                    resource.description = payload.description;
                    resource.content = payload.content;
                    resource.uri = payload.uri ?? undefined;

                    return resource;
                } else {
                    return resource;
                }
            },
        );
        setModuleResources(updatedResources);
        toast.success("Resource updated!");
    };
    const handleRemoveResource = async (resourceId: string) => {
        await deleteResource(resourceId);
        setModuleResources(
            moduleResources!.filter(
                (resource) => resource.resourceId !== resourceId,
            ),
        );
        toast.success("Resource Removed!");
    };

    const handleAddResourceToActivity = (resource: ResourceResponse) => {
        setModuleActivities(
            moduleActivities?.map((activity) => {
                activity.activityResources = [
                    ...activity.activityResources,
                    resource,
                ];
                return activity;
            }),
        );
        toast.success("Resource added to activity!");
    };

    const handleResourceEditForActivity = async (
        resourceId: string,
        payload: ResourceRequest,
    ) => {
        await updateResource(resourceId, payload);
        const updatedActivites: ActivityResponse[] = moduleActivities!.map(
            (activity) => {
                activity.activityResources.map((resource) => {
                    if (resource.resourceId === resourceId) {
                        resource.name = payload.name;
                        resource.description = payload.description;
                        resource.content = payload.content;
                        resource.uri = payload.uri ?? undefined;

                        return resource;
                    } else {
                        return resource;
                    }
                });
                return activity;
            },
        );
        setModuleActivities(updatedActivites);
        toast.success("Resource updated!");
    };

    const handleRemoveResourceFromActivity = async (resourceId: string) => {
        await deleteResource(resourceId);
        setModuleActivities(
            moduleActivities?.map((activity) => {
                activity.activityResources = activity.activityResources.filter(
                    (resource) => resource.resourceId !== resourceId,
                );
                return activity;
            }),
        );
        toast.success("Resource Removed from activity!");
    };

    const handleActivityEdit = async (
        activityId: string,
        payload: ActivityRequest,
    ) => {
        await updateActivity(activityId, payload);
        const updateActivities: ActivityResponse[] = moduleActivities!.map(
            (activity) => {
                if (activity.activityId === activityId) {
                    activity.name = payload.name;
                    activity.description = payload.description;
                    activity.startAt = payload.startAt;
                    activity.endAt = payload.endAt;
                    activity.type = payload.type;
                    activity.deadline = payload.deadline;

                    return activity;
                } else {
                    return activity;
                }
            },
        );
        setModuleActivities(updateActivities);
        toast.success("Activity Updated!");
    };

    const handleRemoveActivity = async (activityId: string) => {
        try {
            await deleteActivity(activityId);
            setModuleActivities(
                moduleActivities!.filter(
                    (activity) => activity.activityId !== activityId,
                ),
            );
            toast.success("Activity Removed!");
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message);
            }
        }
    };

    // Overdue < due today < needs completion < due soon < submitted/other <
    // approved. Flipped together with the deadline order when sortAscending
    // is off.
    const urgency = (activity: ActivityResponse) => {
        if (
            role !== "Student" ||
            (activity.type !== ActivityType.Task &&
                activity.type !== ActivityType.Practice)
        ) {
            return 4;
        }
        const submission = submissionsByActivityId.get(activity.activityId);
        const isPastDeadline =
            activity.deadline != null &&
            new Date() > new Date(activity.deadline);
        if (!submission && isPastDeadline) return 0;
        if (submission?.reviewStatus === SubmissionReviewStatus.Approved)
            return 5;
        if (submission?.reviewStatus === SubmissionReviewStatus.NeedsCompletion)
            return 2;

        // Calendar-day gap to the deadline, same as ActivityCard's badge.
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const deadlineDay =
            activity.deadline != null ? new Date(activity.deadline) : null;
        deadlineDay?.setHours(0, 0, 0, 0);
        const daysUntilDeadline = deadlineDay
            ? Math.round(
                  (deadlineDay.getTime() - today.getTime()) /
                      (1000 * 60 * 60 * 24),
              )
            : null;
        if (!submission && daysUntilDeadline === 0) return 1;
        if (
            !submission &&
            daysUntilDeadline != null &&
            daysUntilDeadline > 0 &&
            daysUntilDeadline <= 5
        )
            return 3;

        return 4;
    };

    if (loading) return <div>Loading...</div>;
    if (!module)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Module not found
                </h1>
            </div>
        );

    return (
        <>
            <div className="flex flex-row-reverse">
                <ModuleSideView module={module} />
            </div>

            <div className="flex flex-col items-center text-text-dark dark:text-text-light mt-8">
                <h1 className="text-4xl pt-5">{module.name}</h1>
                <div className="flex gap-5 pt-5">
                    <p className="text-2xl">Start: {module.startDate}</p>
                    <p className="text-2xl">End: {module.endDate}</p>
                </div>
                <Link className="mt-2" to={`/courses/${module.courseId}`}>
                    <Button className="hover:cursor-pointer">
                        Back to {module.course.name}
                    </Button>
                </Link>
            </div>
            <div className="w-full text-center">
                {error && <ErrorDisplay errorResp={error} />}
            </div>
            <div className="bg-bg dark:bg-bg-dark p-10 grid grid-flow-col grid-rows-[auto_1fr_1fr] grid-cols-2 gap-8 m-8">
                {moduleActivities && (
                    <ActivitySchedule activities={moduleActivities} />
                )}
                <div className="row-span-2 max-h-[70vh] overflow-auto rounded-md px-4 py-2 bg-bg-window  text-text-dark">
                    <div className="flex">
                        <div className="flex w-full">
                            <h1 className="text-4xl grow text-center">
                                Module Resources
                            </h1>
                        </div>
                    </div>
                    <div className="h-10 mt-3 mx-3 flex justify-end">
                        {isAuthenticated && role === "Teacher" ? (
                            <Button
                                onClick={() => setShowCreateResourceForm(true)}
                            >
                                Add
                            </Button>
                        ) : (
                            ""
                        )}
                    </div>
                    {moduleResources?.length ? (
                        <div className="mt-3">
                            {moduleResources.map(
                                (resource: ResourceResponse) => (
                                    <ResourceCard
                                        key={resource.resourceId}
                                        resource={resource}
                                        editResource={handleResourceEdit}
                                        deleteResource={handleRemoveResource}
                                    />
                                ),
                            )}
                        </div>
                    ) : (
                        "Module has no activities"
                    )}
                </div>
                <div className="row-span-2 max-h-[70vh] overflow-auto rounded-md px-4 py-2 bg-bg-window  text-text-dark ">
                    <div className="flex">
                        <div className="flex w-full ">
                            <h1 className="text-4xl grow text-center">
                                Activities
                            </h1>
                        </div>
                    </div>
                    {moduleActivities?.length ? (
                        <>
                            <div className="h-10 flex justify-between gap-2 mt-3 mx-3">
                                <div className="flex items-center gap-1">
                                    <button
                                        onClick={() =>
                                            activityTypeFilter === "all"
                                                ? setSortAscending(
                                                      !sortAscending,
                                                  )
                                                : setActivityTypeFilter("all")
                                        }
                                        className="relative overflow-hidden font-bold text-l bg-bg-window text-text-dark p-1.5 rounded hover:cursor-pointer"
                                    >
                                        {activityTypeFilter === "all" && (
                                            <span
                                                className={`absolute left-0 right-0 h-1.5 ${
                                                    sortAscending
                                                        ? "bottom-0 bg-accent-blue"
                                                        : "top-0 bg-accent-blue"
                                                }`}
                                            />
                                        )}
                                        All ({moduleActivities.length})
                                    </button>
                                    <span
                                        className={`text-xl font-black ${
                                            activityTypeFilter === "all"
                                                ? ""
                                                : "invisible"
                                        } ${
                                            sortAscending
                                                ? "text-text-dark"
                                                : "text-text-dark"
                                        }`}
                                    >
                                        {sortAscending ? "▲" : "▼"}
                                    </span>
                                </div>
                                {Object.entries(ActivityTypeNames).map(
                                    ([typeValue, typeName]) => {
                                        const type = Number(
                                            typeValue,
                                        ) as ActivityType;
                                        const count = moduleActivities.filter(
                                            (activity) =>
                                                activity.type === type,
                                        ).length;
                                        const isActive =
                                            activityTypeFilter === type;

                                        return count > 0 ? (
                                            <div
                                                key={type}
                                                className="flex items-center gap-1"
                                            >
                                                <button
                                                    onClick={() =>
                                                        isActive
                                                            ? setSortAscending(
                                                                  !sortAscending,
                                                              )
                                                            : setActivityTypeFilter(
                                                                  type,
                                                              )
                                                    }
                                                    className="relative overflow-hidden font-bold text-l bg-bg-window text-text-dark p-1.5 rounded hover:cursor-pointer"
                                                >
                                                    {isActive && (
                                                        <span
                                                            className={`absolute left-0 right-0 h-1.5 ${
                                                                sortAscending
                                                                    ? "bottom-0 bg-accent-blue"
                                                                    : "top-0 bg-accent-blue"
                                                            }`}
                                                        />
                                                    )}
                                                    {typeName} ({count})
                                                </button>
                                                <span
                                                    className={`text-xl font-black ${
                                                        isActive
                                                            ? ""
                                                            : "invisible"
                                                    } ${
                                                        sortAscending
                                                            ? "text-text-dark"
                                                            : "text-text-dark"
                                                    }`}
                                                >
                                                    {sortAscending ? "▲" : "▼"}
                                                </span>
                                            </div>
                                        ) : null;
                                    },
                                )}
                                <div className="flex justify-end">
                                    {isAuthenticated && role === "Teacher" ? (
                                        <Button
                                            onClick={() =>
                                                setShowCreateActivityForm(true)
                                            }
                                        >
                                            Add
                                        </Button>
                                    ) : (
                                        ""
                                    )}
                                </div>
                            </div>
                            <div className="mt-3">
                                {moduleActivities
                                    .filter(
                                        (activity) =>
                                            activityTypeFilter === "all" ||
                                            activity.type ===
                                                activityTypeFilter,
                                    )
                                    .sort((a, b) => {
                                        const urgencyDiff =
                                            urgency(a) - urgency(b);

                                        // Lectures/practices have no deadline - fall back to startAt.
                                        const aKey = new Date(
                                            a.deadline ?? a.startAt,
                                        ).getTime();
                                        const bKey = new Date(
                                            b.deadline ?? b.startAt,
                                        ).getTime();
                                        const diff =
                                            urgencyDiff !== 0
                                                ? urgencyDiff
                                                : aKey - bKey;
                                        return sortAscending ? diff : -diff;
                                    })
                                    .map((activity: ActivityResponse) => (
                                        <ActivityCard
                                            key={activity.activityId}
                                            activity={activity}
                                            courseName={module.course.name}
                                            editActivity={handleActivityEdit}
                                            deleteActivity={
                                                handleRemoveActivity
                                            }
                                            addNewResource={
                                                handleAddResourceToActivity
                                            }
                                            deleteResource={
                                                handleRemoveResourceFromActivity
                                            }
                                            editResource={
                                                handleResourceEditForActivity
                                            }
                                            submission={submissionsByActivityId.get(
                                                activity.activityId,
                                            )}
                                            onSubmitted={(submission) =>
                                                setSubmissionsByActivityId(
                                                    (prev) =>
                                                        new Map(prev).set(
                                                            submission.activityId,
                                                            submission,
                                                        ),
                                                )
                                            }
                                        />
                                    ))}
                            </div>
                        </>
                    ) : (
                        <div>
                            <div className="flex justify-end">
                                {isAuthenticated && role === "Teacher" ? (
                                    <Button
                                        onClick={() =>
                                            setShowCreateActivityForm(true)
                                        }
                                    >
                                        Add
                                    </Button>
                                ) : (
                                    ""
                                )}
                            </div>
                            Module has no activities
                        </div>
                    )}
                </div>
            </div>
            {showCreateActivityForm && (
                <FormModal
                    config={createActivityFormConfig}
                    initialValue={{
                        moduleId: moduleId ?? "",
                        name: "",
                        description: "",
                        startAt: "",
                        endAt: "",
                        deadline: null,
                        type: ActivityType.Other,
                    }}
                    onSave={async (data) => {
                        if (moduleActivities === undefined) {
                            throw new Error("Error loading module activities");
                        }
                        const resp = await createActivity(data);
                        setModuleActivities([...moduleActivities, resp]);
                        toast.success("Activity Created!");
                    }}
                    onClose={() => setShowCreateActivityForm(false)}
                />
            )}
            {showCreateResourceForm && (
                <FormModal
                    config={createResourceFormConfig}
                    initialValue={{
                        name: "",
                        description: "",
                        content: "",
                        uri: undefined,
                    }}
                    onSave={async (data) => {
                        const resp = await createResource(data);
                        if (moduleId === undefined) {
                            throw new Error("Id not found");
                        }
                        await addResourceToModule(resp.resourceId, moduleId);
                        setModuleResources([...moduleResources!, resp]);
                        toast.success("Resource Created!");
                    }}
                    onClose={() => setShowCreateResourceForm(false)}
                />
            )}
            <Toaster />
        </>
    );
}

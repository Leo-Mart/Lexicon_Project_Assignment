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
import { ActivityType } from "../constants/ActivityType";
import {
    addResourceToModule,
    createResource,
    deleteResource,
    fetchResourcesForModule,
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

                // Only students have submissions; teachers get a 403 here, so ignore failures.
                try {
                    const submissions = await getCurrentUserSubmissions();
                    setSubmissionsByActivityId(
                        new Map(submissions.map((s) => [s.activityId, s])),
                    );
                } catch {
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

        const getResourcesForModule = async () => {
            setLoading(true);
            setError(null);
            try {
                if (moduleId === undefined) {
                    throw new Error("Could not find Id for module");
                }
                const resourceData = await fetchResourcesForModule(moduleId);
                setModuleResources(resourceData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch resources",
                );
                console.error("Error fetching resources: ", err);
            } finally {
                setLoading(false);
            }
        };

        fetchModule();
        getResourcesForModule();
    }, [moduleId]);

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
    };
    const handleRemoveResource = async (resourceId: string) => {
        await deleteResource(resourceId);
        setModuleResources(
            moduleResources!.filter(
                (resource) => resource.resourceId !== resourceId,
            ),
        );
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
    };

    const handleRemoveActivity = async (activityId: string) => {
        await deleteActivity(activityId);
        setModuleActivities(
            moduleActivities!.filter(
                (activity) => activity.activityId !== activityId,
            ),
        );
    };

    if (loading) return <div>Loading...</div>;
    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
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
            <ModuleSideView module={module} />
            <div className="flex flex-col items-center text-text-dark dark:text-text-light">
                <h1 className="text-4xl pt-5">Current Module: {module.name}</h1>
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
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 grid-cols-2 gap-8 m-8">
                {moduleActivities && (
                    <ActivitySchedule activities={moduleActivities} />
                )}
                <div className="row-span-2 overflow-scroll rounded-md px-4 py-2 bg-buttons text-text-light">
                    <div className="flex">
                        <div className="flex w-full">
                            <h1 className="text-4xl grow text-center">
                                Module Resources
                            </h1>
                            {isAuthenticated && role === "Teacher" ? (
                                <button
                                    onClick={() =>
                                        setShowCreateResourceForm(true)
                                    }
                                    className="rounded-md p-2 w-10 bg-buttons border-text-light border hover:cursor-pointer"
                                >
                                    +
                                </button>
                            ) : (
                                ""
                            )}
                        </div>
                    </div>
                    {moduleResources?.length ? (
                        <div className="mt-5">
                            <ul className="flex flex-col gap-2">
                                {moduleResources.map(
                                    (resource: ResourceResponse) => (
                                        <ResourceCard
                                            key={resource.resourceId}
                                            resource={resource}
                                            editResource={handleResourceEdit}
                                            removeResource={
                                                handleRemoveResource
                                            }
                                        />
                                    ),
                                )}
                            </ul>
                        </div>
                    ) : (
                        "Module has no activities"
                    )}
                </div>
                <div className="row-span-2 overflow-scroll rounded-md px-4 py-2 bg-buttons text-text-dark dark:text-text-light">
                    <div className="flex">
                        <div className="flex w-full ">
                            <h1 className="text-4xl grow text-center">
                                Activities
                            </h1>
                            {isAuthenticated && role === "Teacher" ? (
                                <button
                                    onClick={() =>
                                        setShowCreateActivityForm(true)
                                    }
                                    className="rounded-md p-2 w-10 bg-buttons border-text-light border hover:cursor-pointer"
                                >
                                    +
                                </button>
                            ) : (
                                ""
                            )}
                        </div>
                    </div>
                    {moduleActivities?.length ? (
                        <div className="mt-5">
                            {moduleActivities!.map(
                                (activity: ActivityResponse) => (
                                    <ActivityCard
                                        key={activity.activityId}
                                        activity={activity}
                                        editActivity={handleActivityEdit}
                                        deleteActivity={handleRemoveActivity}
                                        submission={submissionsByActivityId.get(
                                            activity.activityId,
                                        )}
                                        onSubmitted={(submission) =>
                                            setSubmissionsByActivityId((prev) =>
                                                new Map(prev).set(
                                                    submission.activityId,
                                                    submission,
                                                ),
                                            )
                                        }
                                    />
                                ),
                            )}
                        </div>
                    ) : (
                        "Module has no activities"
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
                    }}
                    onClose={() => setShowCreateResourceForm(false)}
                />
            )}
        </>
    );
}

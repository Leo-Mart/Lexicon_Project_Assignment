import { useEffect, useState } from "react";
import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import { ActivityTypeNames } from "../constants/ActivityType";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import ModalWrapper from "./ModalWrapper";
import {
    addResourceToActivity,
    createResource,
    deleteResource,
    fetchResourcesForActivity,
    updateResource,
} from "../services/resourceService";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import Spinner from "./Spinner";
import ResourceCard from "./ResourceCard";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import { useAuth } from "../hooks/useAuth";
import Divider from "./Divider";
import FormModal from "./FormModal";
import { createResourceFormConfig } from "../types/formSchemas";

interface ModalActivityDetailsProps {
    open: boolean;
    onClose: () => void;
    activity: ActivityResponse;
}

const ModalActivityDetails = (props: ModalActivityDetailsProps) => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | undefined>(undefined);
    const [activityResources, setActivityResources] = useState<
        ResourceResponse[] | undefined
    >(undefined);
    const [showCreateResourceForm, setShowCreateResourceForm] = useState(false);

    const { isAuthenticated, role } = useAuth();

    useEffect(() => {
        const getResourcesForActivity = async () => {
            setLoading(true);
            setError(undefined);
            try {
                const resourceData = await fetchResourcesForActivity(
                    props.activity.activityId,
                );
                setActivityResources(resourceData);
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
        getResourcesForActivity();
    }, [props.activity.activityId]);

    const handleResourceEdit = async (
        resourceId: string,
        payload: ResourceRequest,
    ) => {
        await updateResource(resourceId, payload);
        const updatedResources: ResourceResponse[] = activityResources!.map(
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
        setActivityResources(updatedResources);
    };

    const handleRemoveResource = async (resourceId: string) => {
        await deleteResource(resourceId);
        setActivityResources(
            activityResources!.filter(
                (activity) => activity.resourceId !== resourceId,
            ),
        );
    };

    return (
        <ModalWrapper
            open={props.open}
            onClose={props.onClose}
            title="Activity Details"
        >
            <div className="bg-bg py-3 px-3">
                <div>
                    <h2 className="font-bold text-2xl">
                        {props.activity.name}
                    </h2>
                    <time className="text-sm text-text-dark  p-3 pt-0">
                        {ActivityDate(props.activity.startAt)}
                        {" | "}
                        {ActivityTime(props.activity.startAt)}-
                        {ActivityTime(props.activity.endAt)}
                    </time>
                    <div className="p-3 font-semibold">
                        {ActivityTypeNames[props.activity.type]}
                    </div>
                </div>
                <Divider />

                <div className="mt-2">
                    <div className="flex justify-between">
                        <h2 className="text-xl">Resources</h2>

                        {isAuthenticated && role === "Teacher" ? (
                            <button
                                onClick={() => setShowCreateResourceForm(true)}
                                className="rounded-md p-2 w-10 border hover:cursor-pointer"
                            >
                                +
                            </button>
                        ) : (
                            ""
                        )}
                    </div>

                    {loading ? (
                        <Spinner />
                    ) : (
                        <div>
                            {activityResources?.map((resource) => (
                                <ResourceCard
                                    key={resource.resourceId}
                                    resource={resource}
                                    editResource={handleResourceEdit}
                                    deleteResource={handleRemoveResource}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
            {error && <div className="text-red-600">{error}</div>}
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
                        await addResourceToActivity(
                            resp.resourceId,
                            props.activity.activityId,
                        );
                        setActivityResources([...activityResources!, resp]);
                    }}
                    onClose={() => setShowCreateResourceForm(false)}
                />
            )}
        </ModalWrapper>
    );
};

export default ModalActivityDetails;

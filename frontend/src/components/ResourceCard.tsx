import { useState } from "react";
import { useAuth } from "../hooks/useAuth";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import Button from "./Button";
import ConfirmDialog from "./ConfirmDialog";
import FormModal from "./FormModal";
import { createResourceFormConfig } from "../types/formSchemas";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";

interface ResourceCardProps {
    resource: ResourceResponse;
    editResource: (resourceId: string, payload: ResourceRequest) => void;
    removeResource: (resourceId: string) => void;
}

const ResourceCard = ({
    resource,
    editResource,
    removeResource,
}: ResourceCardProps) => {
    const [confirmDelete, setConfirmDeleteOpen] = useState(false);
    const [showEditResourceForm, setShowEditResourceForm] = useState(false);
    const { isAuthenticated, role } = useAuth();
    return (
        <li key={resource.resourceId} className="flex w-full">
            <div className="grow">
                <h1>{resource.name}</h1>
                <p>{resource.description}</p>
                {resource.content ? <p>{resource.content}</p> : ""}
                {resource.uri ? <p>{resource.uri}</p> : ""}
            </div>
            {isAuthenticated && role === "Teacher" ? (
                <div className="flex flex-col gap-1">
                    <Button
                        onClick={() => setShowEditResourceForm(true)}
                        variant="confirm"
                        className=" hover:cursor-pointer"
                    >
                        Edit Resource
                    </Button>
                    <Button
                        onClick={() => setConfirmDeleteOpen(true)}
                        variant="cancel"
                        className=" hover:cursor-pointer"
                    >
                        Delete Resource
                    </Button>
                </div>
            ) : (
                ""
            )}
            {confirmDelete && (
                <ConfirmDialog
                    open={confirmDelete}
                    title="Delete Resource"
                    message={`Are you sure you want to delete the resource: ${resource.name}`}
                    onCancel={() => setConfirmDeleteOpen(false)}
                    onConfirm={() => removeResource(resource.resourceId)}
                />
            )}
            {showEditResourceForm && (
                <FormModal
                    config={createResourceFormConfig}
                    initialValue={{
                        name: resource.name,
                        description: resource.description,
                        content: resource.content,
                        uri: resource.uri,
                    }}
                    onSave={async (data) =>
                        editResource(resource.resourceId, data)
                    }
                    onClose={() => setShowEditResourceForm(false)}
                />
            )}
        </li>
    );
};

export default ResourceCard;

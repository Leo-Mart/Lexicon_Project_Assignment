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
    const [isExpanded, setIsExpanded] = useState(false);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState(false);
    const [showEditResourceForm, setShowEditResourceForm] = useState(false);
    const { isAuthenticated, role } = useAuth();
    return (
        <>
            <div className="relative w-80% m-3">
                <div className="rounded overflow-hidden shadow-lg bg-white">
                    <div
                        className="bg-bg-header text-text-light w-full p-4 grid grid-cols-3 items-center cursor-pointer"
                        role="button"
                        tabIndex={0}
                        aria-expanded={isExpanded}
                        onClick={() => setIsExpanded(!isExpanded)}
                    >
                        <div className="flex items-center gap-2">
                            <span
                                className={`text-xl transition-transform ${isExpanded ? "rotate-180" : ""}`}
                            >
                                ▾
                            </span>
                            <h2 className="font-bold text-nowrap text-xl">
                                {resource.name}
                            </h2>
                        </div>
                        <div className="flex flex-row col-start-3 justify-end items-center gap-2 justify-self-end">
                            {isAuthenticated && role === "Teacher" ? (
                                <div className="flex gap-1">
                                    <Button
                                        onClick={() =>
                                            setShowEditResourceForm(true)
                                        }
                                        variant="confirm"
                                        className=" hover:cursor-pointer"
                                    >
                                        Edit
                                    </Button>
                                    <Button
                                        onClick={() =>
                                            setConfirmDeleteOpen(true)
                                        }
                                        variant="cancel"
                                        className=" hover:cursor-pointer"
                                    >
                                        Delete
                                    </Button>
                                </div>
                            ) : (
                                ""
                            )}
                            <button
                                className="border-2 border-bg-header-dark dark:text-text-light p-1"
                                onClick={() => setIsExpanded(!isExpanded)}
                            >
                                {isExpanded ? "Show Less" : "Show More"}
                            </button>
                        </div>
                    </div>
                    {isExpanded && (
                        <div className="bg-bg-window text-text-dark w-full">
                            <p className="text-m p-3 text-center">
                                {resource.description}
                            </p>
                            <hr className="h-px border-t-0 bg-linear-to-r from-transparent via-accent-blue to-transparent opacity-75"></hr>
                            <div className="flex flex-col justify-between">
                                {resource.content && (
                                    <p className="p-3 text-left">
                                        {resource.content}
                                    </p>
                                )}

                                {resource.uri && (
                                    <div className="flex p-3 text-text-dark gap-1">
                                        <span>Resource URL: </span>
                                        <a
                                            className="text-accent-blue hover:underline"
                                            href={resource.uri}
                                        >
                                            {resource.uri}
                                        </a>
                                    </div>
                                )}
                            </div>
                        </div>
                    )}
                </div>
                {confirmDeleteOpen && (
                    <ConfirmDialog
                        open={confirmDeleteOpen}
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
            </div>
        </>
    );
};

export default ResourceCard;

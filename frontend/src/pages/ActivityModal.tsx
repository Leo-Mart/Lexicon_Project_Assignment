import { useState } from "react";
import "../index.css";
import Button from "../components/Button";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import { createActivity } from "../services/activityService";
import { updateActivity } from "../services/activityService";
import { ActivityType } from "../constants/ActivityType";

interface ModalProps {
    selectedActivity: ActivityResponse;
    onClose: () => void;
    onSubmit: (returnData: ActivityResponse) => void;
}

const today = new Date();

export default function ActivityModal({
    selectedActivity,
    onClose,
    onSubmit,
}: ModalProps) {
    const [formData, setFormData] = useState<ActivityResponse>({
        activityId: selectedActivity.activityId,
        moduleId: "40000000-0000-0000-0000-000000000004",
        name: selectedActivity.name,
        description: selectedActivity.description,
        type: ActivityType.Other,
        startAt: selectedActivity.startAt,
        endAt: selectedActivity.endAt,
        deadline: selectedActivity.endAt,
        createdAt: selectedActivity.endAt,
        updatedAt: selectedActivity.endAt,
    });

    const activityId = selectedActivity.activityId;

    const handleOnSubmit = async (e: React.SubmitEvent) => {
        e.preventDefault();

        try {
            if (activityId != "") {
                await updateActivity(activityId, formData).then(() => {
                    onSubmit(formData);
                });
            } else {
                await createActivity(formData).then((response) => {
                    onSubmit(response);
                });
            }
            //onClose();
        } catch (error) {
            console.error("Error on saving:", error);
        }
    };

    function resetForm() {
        setFormData({
            activityId: "",
            moduleId: "",
            name: "",
            description: "",
            type: ActivityType.Other,
            startAt: "",
            endAt: "",
            deadline: "",
            createdAt: "",
            updatedAt: "",
        });
    }

    return (
        <>
            <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center backdrop-blur-xs">
                <div className="bg-white rounded-md overflow-hidden max-w-md w-full mx-4">
                    <nav className="bg-bg-header text-white flex justify-between px-4 py-2">
                        <h2 className="text-lg">
                            {formData.activityId == ""
                                ? "Create Activity"
                                : "Update Activity"}
                        </h2>
                        <button
                            className="bg-btn-cancel py-1 px-2 hover:brightness-110 rounded-full text-sm"
                            onClick={onClose}
                        >
                            &#10005;
                        </button>
                    </nav>
                    <div className="bg-bg py-3 px-3">
                        <form
                            className="px-8 pt-6 pb-8 mb-4"
                            onSubmit={handleOnSubmit}
                        >
                            <div className="mb-4">
                                <label htmlFor="name">Name</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    type="text"
                                    id="name"
                                    name="name"
                                    placeholder="Activity name"
                                    maxLength={50}
                                    value={formData.name}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            name: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="description">Description</label>
                                <textarea
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    id="description"
                                    name="description"
                                    placeholder="Activity description"
                                    maxLength={200}
                                    rows={5}
                                    value={formData.description}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            description: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="startAt">Start date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    min={today.toLocaleDateString()}
                                    type="date"
                                    id="startAt"
                                    name="startAt"
                                    value={formData.startAt}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            startAt: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="endAt">End date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    min={formData.startAt}
                                    type="date"
                                    id="endAt"
                                    name="endAt"
                                    value={formData.endAt}

                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            endAt: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mt-6">
                                <Button
                                    type="submit"
                                    className="mr-3"
                                    variant="confirm"
                                >
                                    Save
                                </Button>
                                <Button
                                    type="button"
                                    className="mr-3"
                                    onClick={resetForm}
                                    variant="primary"
                                >
                                    Clear
                                </Button>
                                <Button
                                    type="button"
                                    onClick={onClose}
                                    variant="cancel"
                                >
                                    Cancel
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </>
    );
}

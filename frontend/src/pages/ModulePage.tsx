// src/pages/ModulePage.tsx
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Button from "../components/Button";
import Lecture from "../components/Lecture";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ModuleSideView from "../components/ModuleSideView";
import { fetchModuleById } from "../services/moduleService";
import { getCurrentUserSubmissions } from "../services/submissionService";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import ActivityCard from "../components/ActivityCard";
import { useAuth } from "../hooks/useAuth";
import type { EntityFormConfig } from "../components/FormModal";
import FormModal from "../components/FormModal";
import { createActivity } from "../services/activityService";
import { ActivityType } from "../constants/ActivityType";

const createActivityFormConfig: EntityFormConfig<ActivityRequest> = {
    title: "Create new Activity",
    fields: [
        {
            name: "name",
            label: "Name",
            type: "text",
            required: true,
            maxLength: 100,
        },
        {
            name: "type",
            label: "Activity Type",
            type: "select",
            required: true,
            maxLength: 100,
            options: [
                {
                    value: ActivityType.Task.toString(),
                    label: "Task",
                },
                {
                    value: ActivityType.Lecture.toString(),
                    label: "Lecture",
                },
                {
                    value: ActivityType.ELearning.toString(),
                    label: "E-learning",
                },
                {
                    value: ActivityType.Practice.toString(),
                    label: "Practice",
                },
                {
                    value: ActivityType.Other.toString(),
                    label: "Other",
                },
            ],
        },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 100,
        },
        {
            name: "startAt",
            label: "Start Date",
            type: "datetime-local",
            required: true,
        },
        {
            name: "endAt",
            label: "End Date",
            type: "datetime-local",
            required: true,
        },
        {
            name: "deadline",
            label: "Set a deadline",
            type: "datetime-local",
        },
    ],
};

export default function ModulePage() {
    const { id } = useParams<{ id: string }>();
    const moduleId = id || "40000000-0000-0000-0000-000000000004";
    const [module, setModule] = useState<ModuleResponse | null>(null);
    const [submissionsByActivityId, setSubmissionsByActivityId] = useState<
        Map<string, SubmissionResponse>
    >(new Map());
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showCreateActivityForm, setShowCreateActivityForm] = useState(false);

    const { isAuthenticated, role } = useAuth();

    useEffect(() => {
        const fetchModule = async () => {
            setLoading(true);
            setError(null);
            try {
                const moduleData = await fetchModuleById(moduleId);
                setModule(moduleData);

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

        fetchModule();
    }, [moduleId]);

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
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Current Module: {module.name}
                </h1>
                <div className="flex gap-5 pt-5">
                    <p className="text-2xl text-text-dark">
                        Start: {module.startDate}
                    </p>
                    <p className="text-2xl text-text-dark">
                        End: {module.endDate}
                    </p>
                </div>
            </div>
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 grid-cols-2 gap-8 m-8">
                <Lecture
                    lectureName="Dependency Injection"
                    lectureTime="13:30"
                    teacher="Michael"
                />
                <Button className="row-span-2">Course Material</Button>
                <div className="row-span-2 rounded-md px-4 py-2 bg-buttons text-text-light">
                    <div className="flex flex-row justify-between">
                        <div></div>
                        <h1 className="text-4xl text-center">Activities</h1>
                        {isAuthenticated && role === "Teacher" ? (
                            <button
                                onClick={() => setShowCreateActivityForm(true)}
                                className="rounded-md p-2 w-10 bg-buttons border-text-light border-3 hover:cursor-pointer"
                            >
                                +
                            </button>
                        ) : (
                            ""
                        )}
                    </div>
                    {module.activities?.length ? (
                        <div className="mt-5">
                            {module.activities.map(
                                (activity: ActivityRequest) => (
                                    <ActivityCard
                                        key={activity.name}
                                        activity={activity}
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
                        moduleId: moduleId,
                        name: "",
                        description: "",
                        startAt: "",
                        endAt: "",
                        deadline: null,
                        type: ActivityType.Task,
                    }}
                    onSave={async (data) => {
                        await createActivity(data);
                    }}
                    onClose={() => setShowCreateActivityForm(false)}
                />
            )}
        </>
    );
}

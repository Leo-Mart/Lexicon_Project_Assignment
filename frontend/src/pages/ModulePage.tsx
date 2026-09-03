// src/pages/ModulePage.tsx
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Button from "../components/Button";
import Lecture from "../components/Lecture";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ModuleSideView from "../components/ModuleSideView";
import { fetchModuleById } from "../services/moduleService";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";

export default function ModulePage() {
    const { id } = useParams<{ id: string }>();
    const moduleId = id || "40000000-0000-0000-0000-000000000004";
    const [module, setModule] = useState<ModuleResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchModule = async () => {
            setLoading(true);
            setError(null);
            try {
                const moduleData = await fetchModuleById(moduleId);
                setModule(moduleData);
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
                    <h1 className="text-4xl text-center">Activities</h1>
                    {module.activities?.length ? (
                        <div>
                            {module.activities.map(
                                (activity: ActivityRequest) => (
                                    <div
                                        key={activity.activityId}
                                        className="max-w-sm rounded overflow-hidden shadow-lg bg-white m-3"
                                    >
                                        <div className="bg-bg-header w-full p-4 flex flex-row gap-20">
                                            <h2 className="font-bold text-xl mb-2 ">
                                                {activity.name}
                                            </h2>
                                            <p className="border-2 border-bg-dark p-1">
                                                Show More
                                            </p>
                                        </div>

                                        <p className="font-bold text-m mb-2 bg-bg-window text-text-dark w-full p-4">
                                            {activity.description}
                                        </p>
                                    </div>
                                ),
                            )}
                        </div>
                    ) : (
                        "Module has no activities"
                    )}
                </div>
            </div>
        </>
    );
}

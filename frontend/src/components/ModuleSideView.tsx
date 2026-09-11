import { useEffect, useState } from "react";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ModuleSideViewPart from "./ModuleSideViewPart";
import { fetchModulesForCourse } from "../services/courseService";

export default function ModuleSideView({ module }: { module: ModuleResponse }) {
    const [modules, setModules] = useState<ModuleResponse[]>();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isExpanded, setIsExpanded] = useState(false);

    useEffect(() => {
        const fetchModules = async () => {
            setLoading(true);
            setError(null);
            try {
                const moduleData = await fetchModulesForCourse(module.courseId);
                setModules(moduleData);
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

        const sortModules = () => {};

        fetchModules();
        sortModules();
    }, [module.courseId]);
    //
    const sortedModules = modules?.sort(
        (a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime(),
    );

    const pastModules =
        sortedModules?.filter((m) => new Date(m.endDate) < new Date()) ?? [];
    const currentModule =
        sortedModules?.filter(
            (m) =>
                new Date() >= new Date(m.startDate) &&
                new Date() <= new Date(m.endDate),
        ) ?? [];
    const upcomingModules =
        sortedModules?.filter((m) => new Date() < new Date(m.startDate)) ?? [];

    if (loading) return <div>Loading...</div>;
    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (!modules)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Module not found
                </h1>
            </div>
        );

    return (
        <>
            <div className="flex flex-row absolute mt-1 h-full">
                {isExpanded && (
                    <div className="bg-bg-window h-auto w-55 flex flex-col mx-1 z-50">
                        <div className="border-b-4 border-dotted py-4">
                            <h3 className="px-2">Currently viewing module:</h3>
                            <ModuleSideViewPart module={module} />
                        </div>

                        <div className="border-b-4 border-dotted py-4">
                            <h3 className="px-2">Upcoming Modules:</h3>
                            {upcomingModules.map((m) => (
                                <ModuleSideViewPart
                                    module={m}
                                    key={m.moduleId}
                                />
                            ))}
                        </div>
                        <div className="border-b-4 border-dotted py-4">
                            <h3 className="px-2">Current Module:</h3>
                            {currentModule.map((m) => (
                                <ModuleSideViewPart
                                    module={m}
                                    key={m.moduleId}
                                />
                            ))}
                        </div>
                        {pastModules.length > 0 ? (
                            <div className="border-b-4 border-dotted py-4">
                                <h3 className="px-2">Completed Modules:</h3>
                                {pastModules &&
                                    pastModules.map((m) => (
                                        <ModuleSideViewPart
                                            module={m}
                                            key={m.moduleId}
                                        />
                                    ))}
                            </div>
                        ) : (
                            ""
                        )}
                    </div>
                )}
                <button
                    className={`bg-bg-window rotate-45 transition-transform duration-300 ease-in-out w-20 h-20 m-5 ${isExpanded ? "rotate-90" : "rotate-45"}`}
                    onClick={() => setIsExpanded(!isExpanded)}
                >
                    <p className="-rotate-45">Module Side View</p>
                </button>
            </div>
        </>
    );
}

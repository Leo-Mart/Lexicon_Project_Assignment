import { useEffect, useState } from "react";
import type { ModuleDto } from "../interfaces/module/ModuleResponse";
import ModuleSideViewPart from "./ModuleSideViewPart";
import { fetchModules } from "../services/moduleService";

export default function ModuleSideView({ module }: { module: ModuleDto }) {
    const [modules, setModules] = useState<ModuleDto[]>();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchModule = async () => {
            setLoading(true);
            setError(null);
            try {
                const moduleData = await fetchModules();
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

        fetchModule();
    }, []);

    const [isExpanded, setIsExpanded] = useState(false);
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
            <div className="flex flex-row absolute">
                {isExpanded && (
                    <div className="bg-bg-window h-[calc(100vh-1rem)] w-50 border-2 border-bg-header flex flex-col gap-3">
                        <ModuleSideViewPart module={module} />
                        <p>---------------</p>
                        {[...modules]
                            .sort(
                                (a, b) =>
                                    new Date(a.endDate).getTime() -
                                    new Date(b.endDate).getTime(),
                            )
                            .map((m) => (
                                <ModuleSideViewPart module={m} key={m.name} />
                            ))}
                    </div>
                )}
                <button
                    className="bg-bg-window m-20px rotate-45 w-20 h-20 m-5 "
                    onClick={() => setIsExpanded(!isExpanded)}
                >
                    Module Side View
                </button>
            </div>
        </>
    );
}

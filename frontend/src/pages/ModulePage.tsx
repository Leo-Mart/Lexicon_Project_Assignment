// src/pages/ModulePage.tsx
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Button from "../components/Button";
import Lecture from "../components/Lecture";
import type { Module } from "../interfaces/Module";
import ModuleSideView from "../components/ModuleSideView";
import { fetchModuleById } from "../services/moduleService";

export default function ModulePage() {
    const { id } = useParams<{ id: string }>();
    const moduleId = id || "40000000-0000-0000-0000-000000000004";
    const [module, setModule] = useState<Module | null>(null);
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
                <Button className="row-span-2">Activities</Button>
            </div>
        </>
    );
}

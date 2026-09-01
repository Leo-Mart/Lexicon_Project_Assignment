import { useEffect, useState } from "react";
import Button from "../components/Button";
import Lecture from "../components/Lecture";

const API_URL = "https://localhost:7250/api/modules/";
const TEST_MODULE_ID = "40000000-0000-0000-0000-000000000003";

interface Module {
    name: string;
    startDate: string;
    endDate: string;
}

export default function Module() {
    const [module, setModule] = useState<Module | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchData = async () => {
        setLoading(true);
        try {
            const response = await fetch(`${API_URL}${TEST_MODULE_ID}`);

            const moduleData: Module = await response.json();
            setModule(moduleData);
        } catch (error) {
            console.error("Error fetching data:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        fetchData();
    }, []);

    if (loading) return <div>Loading...</div>;
    if (!module) return <div>Module not found</div>;

    return (
        <>
            <div className="flex flex-col items-center">
                <h1 className="align-middle text-4xl text-text-dark pt-5">
                    Current Module: {module.name}
                </h1>
                <div className="flex flex-row items-center gap-5">
                    <p className="align-middle text-2xl text-text-dark pt-5">
                        Start: {module.startDate}
                    </p>
                    <p className="align-middle text-2xl text-text-dark pt-5">
                        End: {module.endDate}
                    </p>
                </div>
            </div>
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 grid-cols-2 gap-8">
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

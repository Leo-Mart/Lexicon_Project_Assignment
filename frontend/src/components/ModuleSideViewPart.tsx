import { Link } from "react-router-dom";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";

export default function ModuleSideViewPart({
    module,
}: {
    module: ModuleResponse;
}) {
    // Parse the end date and check if it's in the past or in the future
    const dateString: string = new Date().toString();
    const isPast = new Date(module.endDate) < new Date(dateString);
    const isFuture = new Date(module.startDate) > new Date(dateString);

    return (
        <>
            <Link to={`/module/${module.moduleId}`} className="details-button">
                <div
                    className={`text-l  text-black font-semibold rounded p-2 m-1.5 ${isPast ? "bg-gray-300" : isFuture ? "bg-accent-blue" : "bg-btn-confirm"}`}
                >
                    <h1 className="text-center">{module.name}</h1>
                    <p className="text-center">
                        {module.startDate} - {module.endDate}
                    </p>
                </div>
            </Link>
        </>
    );
}

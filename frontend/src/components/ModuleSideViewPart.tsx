import { Link } from "react-router-dom";
import type { ModuleDto } from "../interfaces/module/ModuleDto";

export default function ModuleSideViewPart({ module }: { module: ModuleDto }) {
    // Parse the end date and check if it's in the past
    const dateString: string = "2026-10-01T00:00:00Z";
    const isPast = new Date(module.endDate) < new Date(dateString);

    return (
        <>
            <Link to={`/module/${module.moduleId}`} className="details-button">
                <div
                    className={`border-2 border-buttons text-l ${isPast ? "opacity-50 grayscale" : ""}`}
                >
                    <h1 className="text-center">{module.name}</h1>
                    <p>
                        {module.startDate} - {module.endDate}
                    </p>
                </div>
            </Link>
        </>
    );
}

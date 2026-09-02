import type { Module } from "../interfaces/Module";

export default function ModuleSideViewPart({
    name,
    startDate,
    endDate,
}: Module) {
    // Parse the end date and check if it's in the past
    const dateString: string = "2026-10-01T00:00:00Z";
    const isPast = new Date(endDate) < new Date(dateString);

    return (
        <>
            <div
                className={`border-2 border-buttons text-l ${isPast ? "opacity-50 grayscale" : ""}`}
            >
                <h1 className="text-center">{name}</h1>
                <p>
                    {startDate} - {endDate}
                </p>
            </div>
        </>
    );
}

import type { Module } from "../interfaces/Module";

//TODO, rename this to something smarter
export default function ModuleSideViewPart({
    name,
    startDate,
    endDate,
}: Module) {
    return (
        <>
            <div className="border-2 border-buttons text-l">
                <h1 className="text-center">{name}</h1>
                <p>
                    {startDate} - {endDate}
                </p>
            </div>
        </>
    );
}

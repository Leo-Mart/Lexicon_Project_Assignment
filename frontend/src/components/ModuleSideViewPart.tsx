import type { Module } from "../interfaces/Module";

//TODO, rename this to something smarter
export default function ModuleSideViewPart({
    name,
    startDate,
    endDate,
}: Module) {
    return (
        <>
            <div>
                <h1>{name}</h1>
                <p>
                    {startDate} - {endDate}
                </p>
            </div>
        </>
    );
}

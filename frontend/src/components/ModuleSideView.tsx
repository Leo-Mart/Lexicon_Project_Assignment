import { useState } from "react";
import type { Module } from "../interfaces/Module";
import ModuleSideViewPart from "./ModuleSideViewPart";

export default function ModuleSideView({ name, startDate, endDate }: Module) {
    const [isExpanded, setIsExpanded] = useState(false);
    return (
        <>
            <div className="flex flex-row absolute">
                {isExpanded && (
                    <div className="bg-bg-window h-[calc(100vh-1rem)] w-40 border-2 border-bg-header flex flex-col gap-5">
                        <ModuleSideViewPart
                            name={name}
                            startDate={startDate}
                            endDate={endDate}
                        />
                        <p>---------------</p>
                        <div>
                            {/* TODO Fetch all modules, and create a ModuleSideViewPart for each */}
                            <h1>{name}</h1>
                            <p>
                                {startDate} - {endDate}
                            </p>
                        </div>
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

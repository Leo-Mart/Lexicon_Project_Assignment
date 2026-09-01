import { useState } from "react";
import type { Module } from "../interfaces/Module";

export default function ModuleSideView({ name, startDate, endDate }: Module) {
    const [isExpanded, setIsExpanded] = useState(false);
    return (
        <>
            <div className="flex flex-row absolute">
                {isExpanded && (
                    <div className="bg-bg-window h-[calc(100vh-1rem)] w-30 border-2 border-b-accent-student">
                        <div>
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

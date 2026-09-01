import { useState } from "react";

export default function ModuleSideView() {
    const [isExpanded, setIsExpanded] = useState(false);
    return (
        <>
            <div className="flex flex-row absolute">
                {isExpanded && (
                    <div className="bg-bg-window h-100 w-30">Expanded div</div>
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

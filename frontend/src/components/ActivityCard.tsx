import { useState } from "react";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";

export default function ActivityCard({
    activity,
}: {
    activity: ActivityRequest;
}) {
    const [isExpanded, setIsExpanded] = useState(false);
    return (
        <div
            key={activity.activityId}
            className="max-w-sm rounded overflow-hidden shadow-lg bg-white m-3"
        >
            <div className="bg-bg-header w-full p-4 flex flex-row justify-between items-center">
                <h2 className="font-bold text-xl">{activity.name}</h2>
                <button
                    className="border-2 border-bg-header-dark p-1"
                    onClick={() => setIsExpanded(!isExpanded)}
                >
                    {isExpanded ? "Show Less" : "Show More"}
                </button>
            </div>
            {isExpanded && (
                <div className="bg-bg-window w-full">
                    <p className="font-bold text-m text-text-dark  p-3">
                        {activity.description}
                    </p>
                    <p className="font-bold text-sm text-text-dark  p-3 pt-0">
                        {activity.startAt} - {activity.endAt}
                    </p>
                </div>
            )}
        </div>
    );
}

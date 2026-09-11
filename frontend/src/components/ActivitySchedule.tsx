import { ActivityDate, ActivityTime } from "../utils/ActivityTimeConverter";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import { ClipboardCheck } from "lucide-react";

interface ActivityScheduleProps {
    activities: ActivityResponse[];
    // Quick-glance strip, not the full list - cap it so a module with many
    // activities doesn't blow up the layout.
    limit?: number;
}

const ActivitySchedule = ({ activities, limit = 5 }: ActivityScheduleProps) => {
    const now = new Date();

    // Only what's ongoing or still to come - past activities aren't a
    // "schedule" anymore.
    const shown = (activities ?? [])
        .filter((activity) => new Date(activity.endAt) >= now)
        .sort(
            (a, b) =>
                new Date(a.startAt).getTime() - new Date(b.startAt).getTime(),
        )
        .slice(0, limit);

    return (
        <div className="bg-buttons rounded-md col-span-2 p-2 overflow-x-auto self-start">
            <ol className="items-center sm:flex">
                {shown.map((activity) => (
                    <li
                        key={activity.activityId}
                        className="relative mb-6 sm:mb-0"
                    >
                        <div className="flex items-center">
                            <div className="z-10 flex items-center justify-center w-6 h-6 ring-0 ring-buttons sm:ring-8  rounded-md">
                                <ClipboardCheck />
                            </div>
                            <div className="hiddensm:flex w-full bg-bg-dark h-px"></div>
                        </div>
                        <div className="mt-3 sm:pe-8 text-text-dark dark:text-text-light ">
                            <time className="text-heading text-xs font-medium px-1.5 py-0.5 rounded">
                                <span className="text-sm">
                                    {ActivityDate(activity.startAt)}
                                    {" | "}
                                    {ActivityTime(activity.startAt)}-
                                    {ActivityTime(activity.endAt)}
                                </span>
                            </time>
                            <div className="px-1.5">
                                <h3 className="text-lg font-semibold text-heading my-2">
                                    {activity.name}
                                </h3>
                                <p className="text-body mb-4">
                                    {activity.description}
                                </p>
                            </div>
                        </div>
                    </li>
                ))}
            </ol>
        </div>
    );
};

export default ActivitySchedule;

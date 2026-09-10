import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import { ClipboardCheck } from "lucide-react";

interface ActivityScheduleProps {
    activities: ActivityResponse[];
}

const ActivitySchedule = ({ activities }: ActivityScheduleProps) => {
    return (
        <div className="bg-buttons rounded-md col-span-2 p-2">
            <ol className="items-center sm:flex">
                {activities?.map((activity) => (
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

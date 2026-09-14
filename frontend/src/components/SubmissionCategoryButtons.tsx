import Button from "./Button";
import type { SubmissionTabs } from "../types/SubmissionTabs";

interface SubmissionCategoryButtonsProps {
    notReviewedCount: number;
    overdueChecked: boolean;
    totalOverdue: number;
    needsCompletionCount: number;
    doneCount: number;
    tab: SubmissionTabs;
    handleTabChange: (value: SubmissionTabs) => void;
}

export default function SubmissionCategoryButtons({
    notReviewedCount,
    overdueChecked,
    totalOverdue,
    needsCompletionCount,
    doneCount,
    tab,
    handleTabChange,
}: SubmissionCategoryButtonsProps) {
    return (
        <div className="flex flex-wrap items-center gap-2">
            <Button
                variant={tab === "not-reviewed" ? "confirm" : "primary"}
                onClick={() => handleTabChange("not-reviewed")}
            >
                Not reviewed
                <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                    {notReviewedCount}
                </span>
            </Button>
            <Button
                variant={tab === "overdue" ? "confirm" : "primary"}
                className={tab === "overdue" ? "bg-accent-blue" : ""}
                onClick={() => handleTabChange("overdue")}
            >
                Overdue
                <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                    {overdueChecked ? totalOverdue : "…"}
                </span>
            </Button>
            <Button
                variant={tab === "needs-completion" ? "confirm" : "primary"}
                onClick={() => handleTabChange("needs-completion")}
            >
                Needs completion
                <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                    {needsCompletionCount}
                </span>
            </Button>
            <Button
                variant={tab === "done" ? "confirm" : "primary"}
                onClick={() => handleTabChange("done")}
            >
                Done
                <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                    {doneCount}
                </span>
            </Button>
        </div>
    );
}

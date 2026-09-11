import Button from "../Button";

export type SubmissionTab =
    | "not-reviewed"
    | "overdue"
    | "needs-completion"
    | "done";

interface SubmissionTabsProps {
    tab: SubmissionTab;
    onChange: (tab: SubmissionTab) => void;
    notReviewedCount: number;
    overdueCount: number | null;
    needsCompletionCount: number;
    doneCount: number;
}

// The four filter tabs above the submissions table, each with a live count.
export default function SubmissionTabs({
    tab,
    onChange,
    notReviewedCount,
    overdueCount,
    needsCompletionCount,
    doneCount,
}: SubmissionTabsProps) {
    const tabs: { value: SubmissionTab; label: string; count: number | null }[] = [
        { value: "not-reviewed", label: "Not reviewed", count: notReviewedCount },
        { value: "overdue", label: "Overdue", count: overdueCount },
        {
            value: "needs-completion",
            label: "Needs completion",
            count: needsCompletionCount,
        },
        { value: "done", label: "Done", count: doneCount },
    ];

    return (
        <div className="flex flex-wrap items-center gap-2">
            {tabs.map(({ value, label, count }) => (
                <Button
                    key={value}
                    variant="primary"
                    className={tab === value ? "bg-accent-blue" : ""}
                    onClick={() => onChange(value)}
                >
                    {label}
                    <span className="ml-2 rounded-full bg-white/30 px-2 text-xs">
                        {count ?? "…"}
                    </span>
                </Button>
            ))}
        </div>
    );
}

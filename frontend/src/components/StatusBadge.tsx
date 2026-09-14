export default function StatusBadge(
    text: string,
    Outer?: string,
    Inner?: string,
) {
    let bgColor: string;

    if (text === "Approved") {
        bgColor = "bg-btn-confirm";
    } else if (text === "Incomplete") {
        bgColor = "bg-bg-warning";
    } else if (text === "Needs completion") {
        bgColor = "bg-bg-warning";
    } else if (text == "Overdue") {
        bgColor = "bg-btn-cancel";
    } else if (text == "Not submitted") {
        bgColor = "bg-accent-blue";
    } else if (text.includes("Due in")) {
        bgColor = "bg-bg-warning";
    } else if (text.includes("Due today")) {
        bgColor = "bg-btn-cancel";
    } else {
        bgColor = "bg-accent-blue";
    }

    return (
        <span className={`${Outer} ${bgColor} text-sm  px-2 py-2 rounded  `}>
            <span
                className={`${Inner} bg-bg-window font-bold px-1 py-2 rounded text-text-dark `}
            >
                {text}
            </span>
        </span>
    );
}

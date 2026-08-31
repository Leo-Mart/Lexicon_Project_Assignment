import type { ComponentProps } from "react";
import { Link } from "react-router-dom"; // Import Link from React Router

type Variant = "primary" | "confirm" | "cancel";

const variants: Record<Variant, string> = {
    primary: "bg-buttons text-text-light",
    confirm: "bg-btn-confirm text-text-dark",
    cancel: "bg-btn-cancel text-text-light",
};

type ButtonProps = ComponentProps<"button"> & {
    variant?: Variant;
    asLink?: boolean; // New prop to indicate if it should render as a Link
    to?: string; // New prop for the link destination
};

export default function Button({
    variant = "primary",
    className = "",
    to = "#",
}: ButtonProps) {
    return (
        <Link
            to={to}
            className={`rounded-md px-4 py-2 font-semibold hover:brightness-110 disabled:opacity-50 ${variants[variant]} ${className}`}
        />
    );
}

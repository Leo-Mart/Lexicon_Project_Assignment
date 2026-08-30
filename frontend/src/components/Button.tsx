import type { ComponentProps } from "react";

type Variant = "primary" | "confirm" | "cancel";

const variants: Record<Variant, string> = {
    primary: "bg-buttons text-text-light",
    confirm: "bg-btn-confirm text-text-dark",
    cancel: "bg-btn-cancel text-text-light",
};

type ButtonProps = ComponentProps<"button"> & {
    variant?: Variant;
};

export default function Button({
    variant = "primary",
    className = "",
    type = "button",
    ...props
}: ButtonProps) {
    return (
        <button
            type={type}
            className={`rounded-md px-4 py-2 font-semibold hover:brightness-110 disabled:opacity-50 ${variants[variant]} ${className}`}
            {...props}
        />
    );
}

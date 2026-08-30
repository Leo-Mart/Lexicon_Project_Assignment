import type { ComponentProps } from "react";

type Variant = "primary" | "confirm" | "cancel" | "warning" | "ghost";
type Size = "sm" | "md" | "lg";

const variants: Record<Variant, string> = {
    primary: "bg-buttons text-text-light hover:brightness-110",
    confirm: "bg-btn-confirm text-text-dark hover:brightness-105",
    cancel: "bg-btn-cancel text-text-light hover:brightness-110",
    warning: "bg-bg-warning text-text-dark hover:brightness-105",
    ghost: "bg-transparent text-text-dark border border-bg-header hover:bg-bg-window",
};

const sizes: Record<Size, string> = {
    sm: "h-8 px-3 text-sm",
    md: "h-10 px-4 text-base",
    lg: "h-12 px-6 text-lg",
};

type ButtonProps = ComponentProps<"button"> & {
    variant?: Variant;
    size?: Size;
    fullWidth?: boolean;
};

export default function Button({
    variant = "primary",
    size = "md",
    fullWidth = false,
    className = "",
    type = "button",
    ...props
}: ButtonProps) {
    const classes = [
        "inline-flex items-center justify-center gap-2 rounded-md font-semibold",
        "cursor-pointer transition select-none",
        "focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-buttons",
        "disabled:cursor-not-allowed disabled:opacity-50 disabled:brightness-100",
        variants[variant],
        sizes[size],
        fullWidth ? "w-full" : "",
        className,
    ]
        .filter(Boolean)
        .join(" ");

    return <button type={type} className={classes} {...props} />;
}

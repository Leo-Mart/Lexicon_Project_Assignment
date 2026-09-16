import type { ComponentProps } from "react";
import FormField from "./FormField";

type TextareaInputProps = ComponentProps<"textarea"> & {
    label: string;
    error?: string;
};

export default function TextareaInput({
    label,
    error,
    id,
    name,
    className = "",
    ...props
}: TextareaInputProps) {
    const textareaId = id ?? name ?? "";

    return (
        <FormField label={label} htmlFor={textareaId} error={error}>
            <textarea
                id={textareaId}
                name={name}
                className={`shadow appearance-none border rounded w-full bg-white p-2 ${className}`}
                {...props}
            />
        </FormField>
    );
}

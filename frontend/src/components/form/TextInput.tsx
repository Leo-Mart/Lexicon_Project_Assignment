import type { ComponentProps } from "react";
import FormField from "./FormField";

type TextInputProps = ComponentProps<"input"> & {
    label: string;
    error?: string;
};

export default function TextInput({
    label,
    error,
    id,
    name,
    className = "",
    ...props
}: TextInputProps) {
    const inputId = id ?? name ?? "";

    return (
        <FormField label={label} htmlFor={inputId} error={error}>
            <input
                id={inputId}
                name={name}
                className={`shadow appearance-none border rounded w-full bg-white p-2 ${className}`}
                {...props}
            />
        </FormField>
    );
}

import type { ComponentProps } from "react";
import FormField from "./FormField";

interface SelectOption {
    value: string | number;
    label: string;
}

type SelectInputProps = ComponentProps<"select"> & {
    label: string;
    options: SelectOption[];
    error?: string;
};

export default function SelectInput({
    label,
    options,
    error,
    id,
    name,
    className = "",
    ...props
}: SelectInputProps) {
    const selectId = id ?? name ?? "";

    return (
        <FormField label={label} htmlFor={selectId} error={error}>
            <select
                id={selectId}
                name={name}
                className={`shadow appearance-none border rounded w-full bg-white p-2 ${className}`}
                {...props}
            >
                {options.map((option) => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
        </FormField>
    );
}

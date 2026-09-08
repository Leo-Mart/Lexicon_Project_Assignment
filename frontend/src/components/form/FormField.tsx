import type { ReactNode } from "react";

interface FormFieldProps {
    label: string;
    htmlFor: string;
    children: ReactNode;
    error?: string;
}

export default function FormField({
    label,
    htmlFor,
    children,
    error,
}: FormFieldProps) {
    return (
        <div className="mb-4">
            <label htmlFor={htmlFor} className="block mb-1 font-medium">
                {label}
            </label>

            {children}

            {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
        </div>
    );
}

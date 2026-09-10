import { useState } from "react";
import Button from "./Button";

export type FieldType =
    "text" | "textarea" | "date" | "datetime-local" | "select" | "url";

export interface FieldOption {
    value: string | number;
    label: string;
}

export interface FieldConfig<T> {
    // Must match a real property name of T, e.g. "text" for SubmissionRequest.
    name: keyof T & string;
    label: string;
    type: FieldType;
    required?: boolean;
    maxLength?: number;
    options?: FieldOption[];
    readOnly?: boolean;
}

export interface EntityFormConfig<T> {
    title: string;
    fields: FieldConfig<T>[];
    // Both optional - unset means the small, content-sized modal every other
    // form already uses. Set both to make it read like a text editor: a real
    // size, with the one maxLength textarea growing to fill the space.
    widthClass?: string;
    heightClass?: string;
}

// Shared styling for every input/textarea/select rendered below.
// Background isn't included here - textarea needs to switch it when read-only.
const inputClass = "shadow appearance-none border rounded w-full p-2";

interface FormModalProps<T> {
    config: EntityFormConfig<T>;
    initialValue: T;
    onSave: (data: T) => Promise<void>;
    onClose: () => void;
}

// One modal for creating/editing any entity - it only knows the field list
// from `config`, not whether it's a course, a module, or a user.
// T must be an object with string keys, so formData[field.name] type-checks.
export default function FormModal<T extends Record<string, unknown>>({
    config,
    initialValue,
    onSave,
    onClose,
}: FormModalProps<T>) {
    const [formData, setFormData] = useState<T>(initialValue);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    // Only show a count when there's one obvious field for it - a form with
    // several maxLength fields would make "1230/2000" ambiguous.
    const textareasWithMax = config.fields.filter(
        (field) => field.type === "textarea" && field.maxLength != null,
    );
    const charCountField =
        textareasWithMax.length === 1 ? textareasWithMax[0] : null;

    // Copy every existing field, then overwrite just the one named `name`.
    const setField = (name: string, value: string, type: FieldType) => {
        if (name === "url" && value === "") {
            setFormData({ ...formData, ["url"]: null });
            return;
        }
        if (type === "select") {
            setFormData({ ...formData, ["type"]: +value });
            return;
        }
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        try {
            await onSave(formData);
            setSuccess(true);
            setTimeout(onClose, 500);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Could not save.");
        } finally {
            setSaving(false);
        }
    };

    // One input element per field, picked by field.type.
    const renderInput = (field: FieldConfig<T>) => {
        // formData's value could be missing or non-string; inputs need a string.
        const value = String(formData[field.name] ?? "");
        const onChange = (
            e: React.ChangeEvent<
                HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
            >,
        ) => setField(field.name, e.target.value, field.type);

        if (field.type === "textarea") {
            // The one field the modal was sized around fills the space instead
            // of staying its default few rows.
            const grows = config.heightClass && field === charCountField;
            return (
                <textarea
                    className={`${inputClass} ${field.readOnly ? "bg-bg-light" : "bg-white"} ${grows ? "flex-1 min-h-0 resize-none" : ""}`}
                    id={field.name}
                    maxLength={field.maxLength}
                    value={value}
                    onChange={onChange}
                    required={field.required}
                    readOnly={field.readOnly}
                />
            );
        }

        if (field.type === "select") {
            return (
                <select
                    className={`${inputClass} bg-white`}
                    id={field.name}
                    value={value}
                    onChange={onChange}
                    required={field.required}
                >
                    {field.options?.map((opt) => (
                        <option key={opt.value} value={opt.value}>
                            {opt.label}
                        </option>
                    ))}
                </select>
            );
        }

        if (
            field.type === "text" ||
            field.type === "url" ||
            field.type === "date" ||
            field.type === "datetime-local"
        ) {
            return (
                <input
                    className={`${inputClass} bg-white`}
                    type={field.type}
                    id={field.name}
                    maxLength={field.maxLength}
                    value={value}
                    onChange={onChange}
                    required={field.required}
                />
            );
        }
    };

    return (
        <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center backdrop-blur-xs z-20">
            <div
                className={`bg-white rounded-md overflow-hidden mx-4 flex flex-col ${config.widthClass ?? "max-w-md w-full"} ${config.heightClass ?? ""}`}
            >
                <nav className="bg-bg-header text-white flex justify-between px-4 py-2">
                    <h2 className="text-lg">{config.title}</h2>
                    <button
                        className="bg-btn-cancel py-1 px-2 hover:brightness-110 rounded-full text-sm"
                        onClick={onClose}
                    >
                        &#10005;
                    </button>
                </nav>
                <form
                    className={`bg-bg py-3 px-3 text-text-dark ${config.heightClass ? "flex-1 flex flex-col overflow-y-auto min-h-0" : ""}`}
                    onSubmit={handleSubmit}
                >
                    {config.fields.map((field) => (
                        <div
                            className={`mb-4 ${config.heightClass && field === charCountField ? "flex-1 flex flex-col min-h-0" : ""}`}
                            key={field.name}
                        >
                            <label htmlFor={field.name}>{field.label}</label>
                            {renderInput(field)}
                        </div>
                    ))}
                    {error && <p className="text-red-700 mb-2">{error}</p>}
                    {success && (
                        <p className="text-green-700 font-semibold text-base mb-2">
                            Saved.
                        </p>
                    )}
                    <div className="flex items-center justify-between">
                        <div>
                            <Button
                                type="submit"
                                variant="confirm"
                                className="mr-3"
                                disabled={saving}
                            >
                                Save
                            </Button>
                            <Button
                                type="button"
                                variant="cancel"
                                onClick={onClose}
                            >
                                Cancel
                            </Button>
                        </div>
                        {charCountField && (
                            <span className="text-sm text-text-dark">
                                {
                                    String(formData[charCountField.name] ?? "")
                                        .length
                                }
                                /{charCountField.maxLength} chars
                            </span>
                        )}
                    </div>
                </form>
            </div>
        </div>
    );
}

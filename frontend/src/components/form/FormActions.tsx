import Button from "../Button";

interface FormActionsProps {
    submitLabel?: string;
    onClear?: () => void;
    onCancel?: () => void;
}

export default function FormActions({
    submitLabel = "Save",
    onClear,
    onCancel,
}: FormActionsProps) {
    return (
        <div className="mt-6">
            <Button type="submit" className="mr-3" variant="confirm">
                {submitLabel}
            </Button>

            {onClear && (
                <Button
                    type="button"
                    className="mr-3"
                    variant="primary"
                    onClick={onClear}
                >
                    Clear
                </Button>
            )}

            {onCancel && (
                <Button type="button" variant="cancel" onClick={onCancel}>
                    Cancel
                </Button>
            )}
        </div>
    );
}

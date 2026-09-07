import ModalWrapper from "./ModalWrapper";
import Button from "./Button";

interface ConfirmDialogProps {
    open: boolean;
    title: string;
    message: string;
    onConfirm: () => void;
    onCancel: () => void;
}

export default function ConfirmDialog({
    open,
    title,
    message,
    onConfirm,
    onCancel,
}: ConfirmDialogProps) {
    return (
        <ModalWrapper open={open} onClose={onCancel} title={title}>
            <p className="mb-6">{message}</p>

            <div className="flex justify-end gap-3">
                <Button type="button" variant="primary" onClick={onCancel}>
                    Cancel
                </Button>

                <Button type="button" variant="cancel" onClick={onConfirm}>
                    Delete
                </Button>
            </div>
        </ModalWrapper>
    );
}

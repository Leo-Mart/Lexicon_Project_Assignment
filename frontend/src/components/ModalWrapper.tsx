import type { ReactNode } from "react";

interface ModalProps {
    open: boolean;
    onClose: () => void;
    title?: string;
    // Pill next to the title, e.g. an activity type - same look as the
    // badge in FormModal and SubmissionViewModal.
    titleBadge?: string;
    // Rendered after titleBadge, e.g. the activity's name.
    titleSuffix?: string;
    children: ReactNode;
    footer?: ReactNode;
}

const ModalWrapper = (props: ModalProps) => {
    return (
        <>
            {props.open && (
                <div className="fixed inset-0 z-40 backdrop-blur-xs transition-opacity"></div>
            )}
            <dialog
                className={`fixed top-1/2 left-1/2 -translate-x-1/2 z-50 flex w-full max-w-2xl flex-col bg-bg rounded-lg ${props.open ? "-translate-y-1/2" : "top-0"}`}
            >
                <nav className="bg-bg-header dark:bg-bg-header-dark text-text-light rounded-t-md flex items-center justify-between gap-2 px-4 py-2">
                    <h2 className="flex min-w-0 items-center gap-2 text-lg text-text-light">
                        <span className="truncate">{props.title}</span>
                        {props.titleBadge && (
                            <span className="shrink-0 font-bold text-l bg-bg-window text-text-dark p-1.5 rounded">
                                {props.titleBadge}
                            </span>
                        )}
                        {props.titleSuffix && (
                            <span className="truncate">
                                {props.titleSuffix}
                            </span>
                        )}
                    </h2>
                    <button
                        className="bg-btn-cancel py-1 px-2 hover:brightness-110 hover:cursor-pointer rounded-full text-sm shrink-0"
                        onClick={props.onClose}
                    >
                        &#10005;
                    </button>
                </nav>
                <div className="bg-bg py-3 px-3">{props.children}</div>
                {props.footer && <div>{props.footer}</div>}
            </dialog>
        </>
    );
};

export default ModalWrapper;

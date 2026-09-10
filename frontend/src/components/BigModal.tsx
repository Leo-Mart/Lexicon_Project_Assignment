import type { ReactNode } from "react";

interface BigModalProps {
    title: string;
    onClose: () => void;
    heightClass: string;
    // Rendered next to the close button - e.g. a status badge.
    headerExtra?: ReactNode;
    // If set, the body is a <form> that submits on this instead of a plain div.
    onSubmit?: (e: React.FormEvent<HTMLFormElement>) => void;
    children: ReactNode;
}

// Shared chrome for the two modals that need far more room than ModalWrapper's
// small forms give - the student's submission view and the teacher's review.
export default function BigModal({
    title,
    onClose,
    heightClass,
    headerExtra,
    onSubmit,
    children,
}: BigModalProps) {
    return (
        <>
            <div className="fixed inset-0 z-40 backdrop-blur-xs transition-opacity"></div>
            <dialog
                className={`fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 z-50 flex w-[95%] max-w-3xl ${heightClass} flex-col bg-bg rounded-lg`}
            >
                <nav className="bg-bg-header dark:bg-bg-header-dark text-text-light rounded-t-md flex items-center justify-between px-4 py-2">
                    <h2 className="text-lg text-text-light">{title}</h2>
                    <div className="flex items-center gap-2">
                        {headerExtra}
                        <button
                            className="bg-btn-cancel py-1 px-2 hover:brightness-110 hover:cursor-pointer rounded-full text-sm"
                            onClick={onClose}
                        >
                            &#10005;
                        </button>
                    </div>
                </nav>
                <form
                    className="bg-bg py-3 px-3 flex-1 flex flex-col min-h-0"
                    onSubmit={onSubmit ?? ((e) => e.preventDefault())}
                >
                    {children}
                </form>
            </dialog>
        </>
    );
}

import type { ReactNode } from "react";

interface DialogProps {
    title: string;
    onClose: () => void;
    children: ReactNode;
}

// Generic modal chrome - title bar and close button. Doesn't know or care
// what's inside; that's up to whatever renders as children.
export default function Dialog({ title, onClose, children }: DialogProps) {
    return (
        <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center backdrop-blur-xs z-10">
            <div className="bg-white rounded-md overflow-hidden max-w-2xl w-full mx-4 max-h-[85vh] flex flex-col">
                <nav className="bg-bg-header text-white flex justify-between items-center px-4 py-2 flex-shrink-0">
                    <h2 className="text-lg">{title}</h2>
                    <button
                        className="bg-btn-cancel py-1 px-2 hover:brightness-110 rounded-full text-sm"
                        onClick={onClose}
                    >
                        &#10005;
                    </button>
                </nav>
                <div className="bg-bg py-3 px-3 overflow-y-auto">{children}</div>
            </div>
        </div>
    );
}

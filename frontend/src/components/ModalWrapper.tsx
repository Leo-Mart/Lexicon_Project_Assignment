import type { ReactNode } from "react";

interface ModalProps {
    open: boolean;
    onClose: () => void;
    title?: string;
    children: ReactNode;
    footer?: ReactNode;
}

const ModalWrapper = (props: ModalProps) => {
    return (
        <>
            {props.open && (
                <div className="fixed inset-0 z-40 bg-bg-header-dark/40 backdrop-blur-xs transition-opacity"></div>
            )}
            <dialog
                className={`fixed top-1/2 left-1/2 -translate-x-1/2 z-50 flex w-full max-w-md flex-col bg-bg shadow-2xl rounded-xl transition-transform duration-300 ease-in-out ${props.open ? "-translate-y-1/2" : "top-0"}`}
            >
                <nav className="bg-bg-header dark:bg-bg-header-dark text-text-light rounded-t-lg flex justify-between px-4 py-2">
                    <h2 className="text-lg text-text-light">{props.title}</h2>
                    <button
                        className="bg-btn-cancel py-1 px-2 hover:brightness-110 hover:cursor-pointer rounded-full text-sm"
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

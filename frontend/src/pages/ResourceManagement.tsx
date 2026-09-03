import { useState } from "react";
import Button from "../components/Button";
import { createPortal } from "react-dom";
import ModalCreateResource from "../components/ModalCreateResource";

const ResourceManagement = () => {
    const [modalOpen, setModalOpen] = useState(false);
    return (
        <>
            <div>This is is the ResourceManagement page</div>
            <Button variant="primary" onClick={() => setModalOpen(true)}>
                Open Modal
            </Button>
            {modalOpen &&
                createPortal(
                    <ModalCreateResource
                        open={modalOpen}
                        loading={false}
                        onClose={() => setModalOpen(false)}
                    />,
                    document.getElementById("root")!,
                )}
        </>
    );
};

export default ResourceManagement;

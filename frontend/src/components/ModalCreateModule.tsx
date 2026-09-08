import ModalWrapper from "./ModalWrapper";
import Button from "./Button";
import { useState } from "react";
import type { ModuleRequest } from "../interfaces/module/ModuleRequest";
import { createModule } from "../services/moduleService";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";

type CreateModuleModalProps = {
    open: boolean;
    onClose: () => void;
    handleUpdateState: (newModule: ModuleResponse) => void;
    courseId: string;
};
const today = Date.now();

const ModalCreateModule = (props: CreateModuleModalProps) => {
    const [error, setError] = useState("");
    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);

        const newModulePayload: ModuleRequest = {
            courseId: props.courseId,
            name: formData.get("name")!.toString(),
            description: formData.get("description")!.toString(),
            startDate: formData.get("startDate")!.toString(),
            endDate: formData.get("endDate")!.toString(),
        };

        try {
            const resp = await createModule(newModulePayload);
            props.handleUpdateState(resp);
            props.onClose();
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message);
            }
        }
    };

    return (
        <ModalWrapper
            open={props.open}
            onClose={props.onClose}
            title="Create new Resource"
        >
            <div className="bg-bg py-3 px-3">
                <form
                    className="px-8 pt-2 pb-8 mb-4 flex flex-col gap-2"
                    onSubmit={handleSubmit}
                >
                    <div>
                        <label htmlFor="name">Name</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            type="text"
                            id="name"
                            name="name"
                            placeholder="Module Name"
                            maxLength={50}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="description">Description</label>
                        <textarea
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            id="description"
                            name="description"
                            placeholder="Module description"
                            maxLength={200}
                            rows={5}
                            required
                        />
                    </div>
                    <div className="mb-4">
                        <label htmlFor="startDate">Start date</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            min={today.toLocaleString()}
                            type="date"
                            id="startDate"
                            name="startDate"
                            required
                        />
                    </div>
                    <div className="mb-4">
                        <label htmlFor="endDate">End date</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            min={today.toLocaleString()}
                            type="date"
                            id="endDate"
                            name="endDate"
                            required
                        />
                    </div>
                    {error && <span className="text-red-700">{error}</span>}
                    <div className="flex gap-2 justify-center">
                        <Button
                            type="submit"
                            className="hover:cursor-pointer"
                            variant="confirm"
                        >
                            Save
                        </Button>
                        <Button
                            type="reset"
                            className="hover:cursor-pointer"
                            variant="primary"
                        >
                            Clear
                        </Button>
                        <Button
                            type="button"
                            className="hover:cursor-pointer"
                            onClick={props.onClose}
                            variant="cancel"
                        >
                            Cancel
                        </Button>
                    </div>
                </form>
            </div>
        </ModalWrapper>
    );
};

export default ModalCreateModule;

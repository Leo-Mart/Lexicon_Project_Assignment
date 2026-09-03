import ModalWrapper from "./ModalWrapper";
import Button from "./Button";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import { createResource } from "../services/resourceService";
import { useState } from "react";

type CreateResouceModalProps = {
    open: boolean;
    onClose: () => void;
    loading: boolean;
};

const ModalCreateResource = (props: CreateResouceModalProps) => {
    const [error, setError] = useState("");
    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);

        const newResourcePayload: ResourceRequest = {
            name: formData.get("name")!.toString(),
            description: formData.get("description")!.toString(),
            content: formData.get("content")?.toString(),
            uri: formData.get("uri")?.toString(),
        };

        if (newResourcePayload.content === "") {
            newResourcePayload.content = null;
        }
        if (newResourcePayload.uri === "") {
            newResourcePayload.uri = null;
        }

        try {
            await createResource(newResourcePayload);
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
                            placeholder="Resource Name"
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
                            placeholder="Resource description"
                            maxLength={200}
                            rows={5}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="description">Content</label>
                        <textarea
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            id="content"
                            name="content"
                            placeholder="Resource Content"
                            maxLength={200}
                            rows={5}
                        />
                    </div>
                    <div>
                        <label htmlFor="name">URL</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white p-2"
                            type="url"
                            id="uri"
                            name="uri"
                            placeholder="url to resource"
                            maxLength={50}
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
                            type="button"
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

export default ModalCreateResource;

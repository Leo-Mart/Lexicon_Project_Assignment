import ModalWrapper from "./ModalWrapper";
import Button from "./Button";
import { useEffect, useState } from "react";
import type { ModuleRequest } from "../interfaces/module/ModuleRequest";
import { createModule, updateModule } from "../services/moduleService";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ErrorDisplay from "./ErrorDisplay";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";

type CreateModuleModalProps = {
    open: boolean;
    onClose: () => void;
    handleUpdateState: (newModule: ModuleResponse) => void;
    courseId?: string;
    isEditing?: boolean;
    moduleToEdit?: ModuleResponse;
};

//const today = Date.now();
const today = new Date();

interface ModuleForm {
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
}

const ModalCreateModule = (props: CreateModuleModalProps) => {
    const [error, setError] = useState("");
    const [courses, setCourses] = useState<CourseResponse[] | undefined>(
        undefined,
    );
    const [formData, setFormData] = useState<ModuleForm>({
        courseId: props.moduleToEdit?.courseId ?? "",
        name: props.moduleToEdit?.name ?? "",
        description: props.moduleToEdit?.description ?? "",
        startDate: props.moduleToEdit?.startDate ?? "",
        endDate: props.moduleToEdit?.endDate ?? "",
    });

    useEffect(() => {
        if (props.courseId === undefined) {
            const getCourses = async () => {
                try {
                    const resp = await fetchCourses({
                        search: "",
                        sortBy: "name",
                        direction: "asc",
                        page: 1,
                        pageSize: 200,
                    });
                    setCourses(resp.items);
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }
            };

            getCourses();
        }
    }, [props.courseId, props.moduleToEdit]);

    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();

        const newModulePayload: ModuleRequest = {
            courseId: formData.courseId,
            name: formData.name,
            description: formData.description,
            startDate: formData.startDate,
            endDate: formData.endDate,
        };

        if (props.isEditing) {
            try {
                const resp = await updateModule(
                    props.moduleToEdit!.moduleId,
                    newModulePayload,
                );
                props.handleUpdateState(resp);
                props.onClose();
            } catch (error) {
                if (error instanceof Error) {
                    setError(error.message);
                }
            }
            return;
        }

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
            title="Create new Module"
        >
            <div className="bg-bg dark:bg-bg-dark text-text-dark dark:text-text-light py-3 px-3">
                <form
                    className="px-8 pt-2 pb-8 mb-4 flex flex-col gap-2"
                    onSubmit={handleSubmit}
                >
                    <div>
                        <label htmlFor="name">Name</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
                            type="text"
                            id="name"
                            name="name"
                            placeholder="Module Name"
                            maxLength={50}
                            required
                            value={formData.name}
                            onChange={(event) =>
                                setFormData({
                                    ...formData,
                                    name: event.target.value,
                                })
                            }
                        />
                    </div>
                    <div>
                        <label htmlFor="description">Description</label>
                        <textarea
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
                            id="description"
                            name="description"
                            placeholder="Module description"
                            maxLength={200}
                            rows={5}
                            required
                            value={formData.description}
                            onChange={(event) =>
                                setFormData({
                                    ...formData,
                                    description: event.target.value,
                                })
                            }
                        />
                    </div>
                    <div className="mb-4">
                        <label htmlFor="startDate">Start date</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
                            min={today.toLocaleDateString()}
                            type="date"
                            id="startDate"
                            name="startDate"
                            required
                            value={formData.startDate}
                            onChange={(event) =>
                                setFormData({
                                    ...formData,
                                    startDate: event.target.value,
                                })
                            }
                        />
                    </div>
                    <div className="mb-4">
                        <label htmlFor="endDate">End date</label>
                        <input
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
                            min={today.toLocaleDateString()}
                            type="date"
                            id="endDate"
                            name="endDate"
                            required
                            value={formData.endDate}
                            onChange={(event) =>
                                setFormData({
                                    ...formData,
                                    endDate: event.target.value,
                                })
                            }
                        />
                    </div>
                    <fieldset className="flex mx-auto">
                        {courses && (
                            <div>
                                <h3>Choose course to tie resources to:</h3>
                                <select
                                    name="courseId"
                                    value={formData.courseId}
                                    onChange={(event) =>
                                        setFormData({
                                            ...formData,
                                            courseId: event.target.value,
                                        })
                                    }
                                    className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                >
                                    {courses.map((course) => (
                                        <option
                                            key={course.courseId}
                                            value={course.courseId}
                                            label={course.name}
                                        />
                                    ))}
                                </select>
                            </div>
                        )}
                    </fieldset>
                    {error && <ErrorDisplay errorResp={error} />}
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

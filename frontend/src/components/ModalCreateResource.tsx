import ModalWrapper from "./ModalWrapper";
import Button from "./Button";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import { useEffect, useState } from "react";
import {
    addResourceToActivity,
    addResourceToCourse,
    addResourceToModule,
    createResource,
} from "../services/resourceService";
import type { PagedResponse } from "../interfaces/common/PagedResponse";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import { fetchCourses } from "../services/courseService";
import { fetchModules } from "../services/moduleService";
import { fetchActivities } from "../services/activityService";
import ErrorDisplay from "./ErrorDisplay";

type createForEntity = "course" | "module" | "activity" | undefined;

type CreateResourceModalProps = {
    open: boolean;
    onClose: () => void;
    entityId?: string;
    createFor?: createForEntity;
};

const ModalCreateResource = (props: CreateResourceModalProps) => {
    const [error, setError] = useState("");
    const [forEntity, setForEntity] = useState<string>("course");
    const [isChecked, setIsChecked] = useState<boolean>(false);
    const [courses, setCourses] = useState<
        PagedResponse<CourseResponse> | undefined
    >(undefined);
    const [modules, setModules] = useState<ModuleResponse[] | undefined>(
        undefined,
    );
    const [activities, setActivites] = useState<ActivityResponse[] | undefined>(
        undefined,
    );

    useEffect(() => {
        const fetchChoseEntities = async () => {
            switch (forEntity) {
                case "course":
                    try {
                        const resp = await fetchCourses({
                            search: "",
                            sortBy: "name",
                            direction: "asc",
                            page: 1,
                            pageSize: 200,
                        });
                        setCourses(resp);
                    } catch (error) {
                        if (error instanceof Error) {
                            setError(error.message);
                        }
                    }

                    break;

                case "module":
                    try {
                        const resp = await fetchModules();
                        setModules(resp);
                    } catch (error) {
                        if (error instanceof Error) {
                            setError(error.message);
                        }
                    }
                    break;

                case "activity":
                    try {
                        const resp = await fetchActivities();
                        setActivites(resp);
                    } catch (error) {
                        if (error instanceof Error) {
                            setError(error.message);
                        }
                    }
                    break;

                default:
                    throw new Error("THat entity does not exist, try again.");
            }
        };
        fetchChoseEntities();
    }, [forEntity]);

    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);

        const newResourcePayload: ResourceRequest = {
            name: formData.get("name")!.toString(),
            description: formData.get("description")!.toString(),
            content:
                formData.get("content")?.toString() === ""
                    ? null
                    : formData.get("content")?.toString(),
            uri:
                formData.get("uri")?.toString() === ""
                    ? null
                    : formData.get("uri")?.toString(),
        };
        try {
            const resp = await createResource(newResourcePayload);

            if (props.createFor !== undefined && props.entityId !== undefined) {
                switch (props.createFor) {
                    case "course": {
                        await addResourceToCourse(
                            resp.resourceId,
                            props.entityId,
                        );
                        break;
                    }
                    case "module": {
                        await addResourceToModule(
                            resp.resourceId,
                            props.entityId,
                        );
                        break;
                    }
                    case "activity": {
                        await addResourceToActivity(
                            resp.resourceId,
                            props.entityId,
                        );
                        break;
                    }
                    default:
                        throw new Error("Invalid resource type");
                }
            } else {
                const entity = formData.get("forEntity");
                switch (entity) {
                    case "course":
                        try {
                            const courseId = formData.get("courseId");
                            if (courseId === null) {
                                throw new Error("CourseId missing");
                            }
                            await addResourceToCourse(
                                resp.resourceId,
                                courseId.toString(),
                            );
                        } catch (error) {
                            if (error instanceof Error) {
                                setError(error.message);
                            }
                        }

                        break;

                    case "module":
                        try {
                            const moduleId = formData.get("moduleId");
                            if (moduleId === null) {
                                throw new Error("moduleId missing");
                            }
                            await addResourceToModule(
                                resp.resourceId,
                                moduleId.toString(),
                            );
                        } catch (error) {
                            if (error instanceof Error) {
                                setError(error.message);
                            }
                        }
                        break;

                    case "activity":
                        try {
                            const activityId = formData.get("activityId");
                            if (activityId === null) {
                                throw new Error("ActivityId missing");
                            }
                            await addResourceToActivity(
                                resp.resourceId,
                                activityId.toString(),
                            );
                        } catch (error) {
                            if (error instanceof Error) {
                                setError(error.message);
                            }
                        }
                        break;

                    default:
                        throw new Error(
                            "THat entity does not exist, try again.",
                        );
                }
            }
            props.onClose();
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message);
            }
        }
    };
    const handleEntityChoice = (choice: string) => {
        if (isChecked) {
            setIsChecked(false);
        }

        setForEntity(choice);
    };
    return (
        <ModalWrapper
            open={props.open}
            onClose={props.onClose}
            title="Create new Resource"
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
                            placeholder="Resource Name"
                            maxLength={50}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="description">Description</label>
                        <textarea
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
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
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
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
                            className="shadow appearance-none border rounded w-full bg-white text-text-dark p-2"
                            type="url"
                            id="uri"
                            name="uri"
                            placeholder="url to resource"
                            maxLength={50}
                        />
                    </div>
                    {error && <ErrorDisplay errorResp={error} />}
                    {props.createFor === undefined &&
                    props.entityId === undefined ? (
                        <>
                            <fieldset>
                                <ul className="flex justify-between w-full gap-1">
                                    <li className="grow">
                                        <input
                                            className="hidden peer"
                                            type="radio"
                                            id="course"
                                            name="forEntity"
                                            value="course"
                                            checked={forEntity === "course"}
                                            onChange={() =>
                                                handleEntityChoice("course")
                                            }
                                        />
                                        <label
                                            htmlFor="course"
                                            className="inline-flex items-center justify-between rounded-md bg-buttons dark:bg-buttons-dark w-full p-2 hover:cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
                                        >
                                            Course
                                        </label>
                                    </li>
                                    <li className="grow">
                                        <input
                                            className="hidden peer"
                                            type="radio"
                                            id="module"
                                            name="forEntity"
                                            value="module"
                                            checked={forEntity === "module"}
                                            onChange={() =>
                                                handleEntityChoice("module")
                                            }
                                        />
                                        <label
                                            htmlFor="module"
                                            className="inline-flex items-center justify-between rounded-md bg-buttons dark:bg-buttons-dark w-full p-2 hover:cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
                                        >
                                            Module
                                        </label>
                                    </li>
                                    <li className="grow">
                                        <input
                                            className="hidden peer"
                                            type="radio"
                                            id="activity"
                                            name="forEntity"
                                            value="activity"
                                            checked={forEntity === "activity"}
                                            onChange={() =>
                                                handleEntityChoice("activity")
                                            }
                                        />
                                        <label
                                            htmlFor="activity"
                                            className="inline-flex items-center justify-between rounded-md bg-buttons dark:bg-buttons-dark w-full p-2 hover:cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
                                        >
                                            Activity
                                        </label>
                                    </li>
                                </ul>
                            </fieldset>
                            <fieldset className="flex mx-auto">
                                {courses !== undefined &&
                                forEntity === "course" ? (
                                    <div>
                                        <h3>
                                            Choose course to tie resources to:
                                        </h3>
                                        <select
                                            name="courseId"
                                            className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                        >
                                            {courses.items.map((course) => (
                                                <option
                                                    key={course.courseId}
                                                    value={course.courseId}
                                                    label={course.name}
                                                />
                                            ))}
                                        </select>
                                    </div>
                                ) : (
                                    ""
                                )}
                                {modules !== undefined &&
                                forEntity === "module" ? (
                                    <div>
                                        <h3>
                                            Choose module to tie resources to:
                                        </h3>
                                        <select
                                            name="moduleId"
                                            className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                        >
                                            {modules.map((module) => (
                                                <option
                                                    key={module.moduleId}
                                                    value={module.moduleId}
                                                    label={module.name}
                                                />
                                            ))}
                                        </select>
                                    </div>
                                ) : (
                                    ""
                                )}
                                {activities !== undefined &&
                                forEntity === "activity" ? (
                                    <div>
                                        <h3>
                                            Choose actvity to tie resources to:
                                        </h3>
                                        <select
                                            name="activityId"
                                            className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                        >
                                            {activities.map((activity) => (
                                                <option
                                                    key={activity.activityId}
                                                    value={activity.activityId}
                                                    label={activity.name}
                                                />
                                            ))}
                                        </select>
                                    </div>
                                ) : (
                                    ""
                                )}
                            </fieldset>
                        </>
                    ) : (
                        ""
                    )}
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

export default ModalCreateResource;

import { useEffect, useState } from "react";
import ModalWrapper from "./ModalWrapper";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import { fetchCourses } from "../services/courseService";
import { fetchModules } from "../services/moduleService";
import { fetchActivities } from "../services/activityService";
import type { PagedResponse } from "../interfaces/common/PagedResponse";
import Button from "./Button";

interface ModalAddResourceToEntityProps {
    open: boolean;
    resourceId: string;
    handleAddToEntity: (
        resourceId: string,
        entityId: string,
        forEntity: string,
    ) => void;
    onClose: () => void;
}

const ModalAddResourceToEntity = (props: ModalAddResourceToEntityProps) => {
    const [forEntity, setForEntity] = useState<string>("course");
    const [isChecked, setIsChecked] = useState<boolean>(false);
    const [error, setError] = useState<string>("");
    const [courses, setCourses] = useState<
        PagedResponse<CourseResponse> | undefined
    >(undefined);
    const [modules, setModules] = useState<
        PagedResponse<ModuleResponse> | undefined
    >(undefined);
    const [activities, setActivites] = useState<
        PagedResponse<ActivityResponse> | undefined
    >(undefined);

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
                        const resp = await fetchModules({
                            search: "",
                            sortBy: "name",
                            direction: "asc",
                            page: 1,
                            pageSize: 200,
                        });
                        setModules(resp);
                    } catch (error) {
                        if (error instanceof Error) {
                            setError(error.message);
                        }
                    }
                    break;

                case "activity":
                    try {
                        const resp = await fetchActivities({
                            search: "",
                            sortBy: "name",
                            direction: "asc",
                            page: 1,
                            pageSize: 200,
                        });
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
        setError("");

        const fd = new FormData(e.currentTarget);
        const entity = fd.get("forEntity");
        switch (entity) {
            case "course":
                try {
                    const courseId = fd.get("courseId");
                    if (courseId === null) {
                        throw new Error("CourseId missing");
                    }

                    props.handleAddToEntity(
                        props.resourceId,
                        courseId.toString(),
                        entity,
                    );
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }

                break;

            case "module":
                try {
                    const moduleId = fd.get("moduleId");
                    if (moduleId === null) {
                        throw new Error("moduleId missing");
                    }
                    props.handleAddToEntity(
                        props.resourceId,
                        moduleId.toString(),
                        entity,
                    );
                } catch (error) {
                    if (error instanceof Error) {
                        setError(error.message);
                    }
                }
                break;

            case "activity":
                try {
                    const activityId = fd.get("activityId");
                    if (activityId === null) {
                        throw new Error("ActivityId missing");
                    }
                    props.handleAddToEntity(
                        props.resourceId,
                        activityId.toString(),
                        entity,
                    );
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
            title="Add Resource to entity"
        >
            <div className="bg-bg py-3 px-3">
                <form
                    onSubmit={handleSubmit}
                    className="px-8 pt-2 pb-8 mb-4 flex flex-col gap-2"
                >
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
                                    className="inline-flex items-center justify-between bg-bg dark:bg-buttons-dark w-full p-2 cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
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
                                    className="inline-flex items-center justify-between bg-bg dark:bg-buttons-dark w-full p-2 cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
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
                                    className="inline-flex items-center justify-between bg-bg dark:bg-buttons-dark w-full p-2 cursor-pointer peer-checked:bg-accent-student hover:bg-accent-teacher"
                                >
                                    Activity
                                </label>
                            </li>
                        </ul>
                    </fieldset>
                    <fieldset className="flex mx-auto">
                        {courses !== undefined && forEntity === "course" ? (
                            <div>
                                <h3>Choose course to tie resources to:</h3>
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
                        {modules !== undefined && forEntity === "module" ? (
                            <div>
                                <h3>Choose module to tie resources to:</h3>
                                <select
                                    name="moduleId"
                                    className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                >
                                    {modules.items.map((module) => (
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
                                <h3>Choose actvity to tie resources to:</h3>
                                <select
                                    name="activityId"
                                    className="bg-bg-header text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
                                >
                                    {activities.items.map((activity) => (
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
                            onClick={props.onClose}
                            variant="cancel"
                        >
                            Cancel
                        </Button>
                    </div>
                </form>
                {error && <div className="text-red-600">{error}</div>}
            </div>
        </ModalWrapper>
    );
};

export default ModalAddResourceToEntity;

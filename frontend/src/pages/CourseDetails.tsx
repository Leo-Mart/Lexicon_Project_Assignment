import { useState, useEffect } from "react";
import { fetchCourse } from "../services/courseService";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import Button from "../components/Button";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import {
    addResourceToCourse,
    createResource,
    deleteResource,
    fetchResourcesForCourse,
    updateResource,
} from "../services/resourceService";
import { Link, useParams } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { createPortal } from "react-dom";
import ModalCreateModule from "../components/ModalCreateModule";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import ResourceCard from "../components/ResourceCard";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";
import FormModal from "../components/FormModal";
import { createResourceFormConfig } from "../types/formSchemas";
import UserSideView from "../components/UserSideView";
import Divider from "../components/Divider";
import { UserStatus } from "../constants/UserConstant";

export default function CoursesDetails() {
    const { courseId } = useParams<{ courseId: string }>();
    const { isAuthenticated, role } = useAuth();

    const newCourse = {
        courseId: "",
        name: "",
        description: "",
        startDate: "",
        endDate: "",
        modules: [],
        resources: [],
    };

    const emptyResource = {
        resourceId: "",
        createdByTeacherId: "",
        name: "",
        description: "",
        content: "",
        uri: "",
        createdAt: "",
        updatedAt: "",
        createdByTeacher: {
            id: "",
            name: "",
            email: "",
            status: UserStatus.Active,
        },
    };

    const [showCreateResourceForm, setShowCreateResourceForm] = useState(false);
    const [showCreateModuleModal, setShowCreateModuleModal] = useState(false);

    const [course, setCourse] = useState<CourseResponse>(newCourse);
    const [resources, setResources] = useState<ResourceResponse[]>([
        emptyResource,
    ]);

    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const handleUpdateCourseState = (newModule: ModuleResponse) => {
        setCourse({ ...course, modules: [...course.modules, newModule] });
    };

    // Get CourseDetails
    useEffect(() => {
        const fetchChosenCourse = async (courseId: string) => {
            setLoading(true);
            setError(null);
            try {
                const courseData = await fetchCourse(courseId);
                setCourse(courseData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch course",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        if (courseId === undefined) {
            return;
        }
        const fetchAllResourcesForCourse = async () => {
            setLoading(true);
            setError(null);
            try {
                const resourceData = await fetchResourcesForCourse(courseId);
                setResources(resourceData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch resources",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchChosenCourse(courseId);
        fetchAllResourcesForCourse();
    }, [courseId]);

    const handleResourceEdit = async (
        resourceId: string,
        payload: ResourceRequest,
    ) => {
        await updateResource(resourceId, payload);
        const updatedResources: ResourceResponse[] = resources!.map(
            (resource) => {
                if (resource.resourceId === resourceId) {
                    resource.name = payload.name;
                    resource.description = payload.description;
                    resource.content = payload.content;
                    resource.uri = payload.uri ?? undefined;

                    return resource;
                } else {
                    return resource;
                }
            },
        );
        setResources(updatedResources);
    };
    const handleRemoveResource = async (resourceId: string) => {
        await deleteResource(resourceId);
        setResources(
            resources!.filter((resource) => resource.resourceId !== resourceId),
        );
    };

    // RENDER
    if (loading) return <p>Loading...</p>;

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (!course)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Courses not found
                </h1>
            </div>
        );

    return (
        <>
            <div className="flex flex-row-reverse">
                <UserSideView courseId={courseId!}></UserSideView>
            </div>
            <div className="text-center text-text-dark dark:text-text-light">
                <h1 className="text-3xl font-bold p-3">{course.name}</h1>
                <Divider />
                <div className=" p-3">
                    <h2 className="text-l">{course.description}</h2>
                    <p>
                        {course.startDate} - {course.endDate}
                    </p>
                </div>
                <Divider />
                <div className="grid grid-cols-6 gap-2">
                    <div>
                        {isAuthenticated && role === "Teacher" ? (
                            <div className="fixed top-3/14 left-16 w-1/10">
                                <h3 className="text-text-dark dark:text-text-light">
                                    Teacher Control
                                </h3>
                                <div className="flex flex-col gap-3 justify-center">
                                    <Button
                                        className="hover:cursor-pointer"
                                        onClick={() =>
                                            setShowCreateModuleModal(true)
                                        }
                                    >
                                        Create new module
                                    </Button>
                                    <Button
                                        className="hover:cursor-pointer"
                                        onClick={() =>
                                            setShowCreateResourceForm(true)
                                        }
                                    >
                                        Create new resource
                                    </Button>
                                </div>
                            </div>
                        ) : (
                            ""
                        )}
                    </div>
                    <div className="p-2 m-2 col-span-2 rounded-md">
                        <h3 className="font-bold text-text-dark dark:text-text-light ">
                            Select module for more information
                        </h3>
                        <ul>
                            {course.modules.map((module) => (
                                <li className="p-3" key={module.moduleId}>
                                    <Link to={`/module/${module.moduleId}`}>
                                        <Button className="w-full hover:cursor-pointer">
                                            <h2 className="font-extrabold p-2">
                                                {module.name}
                                            </h2>
                                            <p>{module.description}</p>
                                            <p>
                                                {module.startDate} -{" "}
                                                {module.endDate}
                                            </p>
                                        </Button>
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </div>

                    <div className="flex flex-col items-center p-2 m-2 rounded-md col-span-2">
                        <div className="flex w-full">
                            <h2 className="font-bold grow text-center text-text-dark dark:text-text-light">
                                Course resources:{" "}
                            </h2>
                        </div>
                        <div className="">
                            {resources.map((resource) => (
                                <>
                                    <ResourceCard
                                        resource={resource}
                                        key={resource.resourceId}
                                        deleteResource={handleRemoveResource}
                                        editResource={handleResourceEdit}
                                    />
                                </>
                            ))}
                        </div>
                    </div>
                </div>

                {showCreateResourceForm && (
                    <FormModal
                        config={createResourceFormConfig}
                        initialValue={{
                            name: "",
                            description: "",
                            content: "",
                            uri: undefined,
                        }}
                        onSave={async (data) => {
                            const resp = await createResource(data);
                            await addResourceToCourse(
                                resp.resourceId,
                                course.courseId,
                            );
                            setResources([...resources!, resp]);
                        }}
                        onClose={() => setShowCreateResourceForm(false)}
                    />
                )}
                {showCreateModuleModal &&
                    createPortal(
                        <ModalCreateModule
                            open={showCreateModuleModal}
                            onClose={() => setShowCreateModuleModal(false)}
                            handleUpdateState={handleUpdateCourseState}
                            courseId={course.courseId}
                        />,
                        document.getElementById("root")!,
                    )}
            </div>
        </>
    );
}

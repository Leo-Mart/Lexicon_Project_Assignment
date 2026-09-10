import { useState, useEffect } from "react";
import { fetchCourse } from "../services/courseService";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import Button from "../components/Button";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import { fetchResourcesForCourse } from "../services/resourceService";
import { Link, useParams } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { createPortal } from "react-dom";
import ModalCreateResource from "../components/ModalCreateResource";
import ModalCreateModule from "../components/ModalCreateModule";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import UserSideView from "../components/UserSideView";

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
    };

    const [showCreateResourceModal, setShowCreateResourceModal] =
        useState(false);
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
            <div className="text-center">
                <h1 className="text-3xl font-bold p-3 bg-buttons text-white">
                    {course.name}
                </h1>
                <div className=" p-3 text-text-dark dark:text-text-light">
                    <h2 className="text-xl">{course.description}</h2>
                    <p>
                        {course.startDate} - {course.endDate}
                    </p>
                </div>
                <div className="">
                    <h2 className="font-bold">Course resources: </h2>
                    {resources.map((resource) => (
                        <li className="p-3" key={resource.resourceId}>
                            <a className="underline" href={resource.uri}>
                                {resource.name}
                            </a>
                        </li>
                    ))}
                </div>

                {course.modules.map((module) => (
                    <li className="p-3" key={module.moduleId}>
                        <Link to={`/module/${module.moduleId}`}>
                            <Button className="w-1/2 hover:cursor-pointer">
                                <h2 className="font-extrabold p-2">
                                    {module.name}
                                </h2>
                                <p>{module.description}</p>
                                <p>
                                    {module.startDate} - {module.endDate}
                                </p>
                            </Button>
                        </Link>
                    </li>
                ))}
                {isAuthenticated && role === "Teacher" ? (
                    <>
                        <h3 className="text-text-dark dark:text-text-light">
                            Teacher Control
                        </h3>
                        <div className="flex gap-3 justify-center">
                            <Button
                                className="hover:cursor-pointer"
                                onClick={() => setShowCreateResourceModal(true)}
                            >
                                Create resource
                            </Button>
                            <Button
                                className="hover:cursor-pointer"
                                onClick={() => setShowCreateModuleModal(true)}
                            >
                                Create new module
                            </Button>
                        </div>
                    </>
                ) : (
                    ""
                )}
                {showCreateResourceModal &&
                    createPortal(
                        <ModalCreateResource
                            open={showCreateResourceModal}
                            entityId={course.courseId}
                            createFor="course"
                            onClose={() => setShowCreateResourceModal(false)}
                        />,
                        document.getElementById("root")!,
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

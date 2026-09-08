import { useState, useEffect } from "react";
import { fetchCourse } from "../services/courseService";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import Button from "../components/Button";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import { fetchResourcesForCourse } from "../services/resourceService";

export default function CoursesDetails() {
    //TODO: byta ut mot anrop
    const courseId: string = "30000000-0000-0000-0000-000000000002";

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

    const [course, setCourse] = useState<CourseResponse>(newCourse);
    const [resources, setResources] = useState<ResourceResponse[]>([
        emptyResource,
    ]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

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

        fetchChosenCourse(courseId);
    }, []);

    // Get resources for course
    useEffect(() => {
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

        fetchAllResourcesForCourse();
    }, []);

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
            <div className="text-center">
                <h1 className="text-3xl font-bold p-3 bg-buttons text-white">
                    {course.name}
                </h1>
                <div className="p-3">
                    <h2 className="">{course.description}</h2>
                    <p>
                        {course.startDate} - {course.endDate}
                    </p>
                </div>
                <div>
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
                        <Button
                            onClick={() => alert("goTo(module.moduleId)")}
                            className="w-1/2"
                        >
                            <h2 className="font-extrabold p-2">
                                {module.name}
                            </h2>
                            <p>{module.description}</p>
                            <p>
                                {module.startDate} - {module.endDate}
                            </p>
                        </Button>
                    </li>
                ))}
            </div>
        </>
    );
}

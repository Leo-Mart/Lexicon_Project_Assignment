import { useState, useEffect } from "react";
import "../index.css";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";

export default function Courses() {
    // STATE
    const [courses, setCourses] = useState<CourseResponse[] | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    // READ ALL

    useEffect(() => {
        const fetchAllCourses = async () => {
            setLoading(true);
            setError(null);
            try {
                const courseData = await fetchCourses();
                setCourses(courseData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch module",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAllCourses();
    }, []);

    // RENDER
    if (loading) return <p>Loading...</p>;

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (!courses)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    Courses not found
                </h1>
            </div>
        );

    return (
        <>
            <h1 className="text-3xl font-bold px-3 pb-3 text-center bg-buttons text-white">
                Courses
            </h1>

            <div className="flex flex-row">
                {courses.map((course) => (
                    <div className="max-w-sm rounded overflow-hidden shadow-lg bg-white m-3">
                        <h2 className="font-bold text-xl mb-2 bg-bg-window w-full p-4">
                            {course.name}
                        </h2>
                        <div className="px-6 py-4">
                            <p className="text-base mb-2">
                                {course.description}
                            </p>
                            <p>Startdate: {course.startDate}</p>
                            <p>Enddate: {course.endDate}</p>
                        </div>
                    </div>
                ))}
            </div>
        </>
    );
}

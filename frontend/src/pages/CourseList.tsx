import { useState, useEffect } from "react";
import "../index.css";
import Button from "../components/Button";
import CourseModal from "./CourseModal";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";

export default function Courses() {
    // STATE

    const [courses, setCourses] = useState<CourseResponse[] | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const [isCourseModalVisible, setIsCourseModalVisible] = useState(false);

    const handleShowCourseModal = () => {
        setIsCourseModalVisible(true);
    };

    const handleCloseCourseModal = () => {
        setIsCourseModalVisible(false);
    };

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
            <div className="bg-bg border rounded m-3">
                <h1 className="text-3xl font-bold px-3 pb-3 text-center bg-bg-header text-white">
                    Courses
                </h1>
                <table className="w-full text-left">
                    <thead className="bg-bg-window h-10 border-b text-text-dark">
                        <tr>
                            <th className="p-3">Name</th>
                            <th className="p-3">Description</th>
                            <th className="p-3">Start date</th>
                            <th className="p-3">End date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {courses.map((course, index) => (
                            <tr
                                key={course.courseId}
                                className={
                                    index % 2 === 0 ? "bg-white" : "bg-bg"
                                }
                            >
                                <td className="p-3">{course.name}</td>
                                <td className="p-3">{course.description}</td>
                                <td className="p-3">{course.startDate}</td>
                                <td className="p-3">{course.endDate}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <div className="m-3">
                <Button onClick={handleShowCourseModal} className="col-span-2">
                    Create course
                </Button>
                {isCourseModalVisible && (
                    <CourseModal onClose={handleCloseCourseModal} />
                )}
            </div>
        </>
    );
}

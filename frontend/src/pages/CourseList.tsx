import { useState, useEffect } from "react";
import "../index.css";
import Button from "../components/Button";
import CourseModal from "./CourseModal";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";
import { deleteCourse } from "../services/courseService";

export default function Courses() {
    // STATE

    const [courses, setCourses] = useState<CourseResponse[] | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const newCourse = {
        courseId: "",
        name: "",
        description: "",
        startDate: "",
        endDate: "",
        modules: [],
    };

    const [isCourseModalVisible, setIsCourseModalVisible] = useState(false);
    const [selectedRow, setSelectedRow] = useState<CourseResponse>(newCourse);

    const handleCloseCourseModal = () => {
        setIsCourseModalVisible(false);

        if (selectedRow.courseId != "") {
            // Update the course
        }
    };

    const handleShowCourseModal = (course: CourseResponse) => {
        setSelectedRow(course);
        setIsCourseModalVisible(true);
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

    // DELETE
    async function handleDelete(course: CourseResponse) {
        if (
            !window.confirm(
                'Är du säker på att du vill radera kursen "' +
                    course.name +
                    '"?',
            )
        ) {
            return;
        }

        try {
            deleteCourse(course.courseId);
            // Filter the deleted course from state
            setCourses(courses!.filter((c) => c.courseId !== course.courseId));
        } catch (error) {
            alert(error);
            console.error("Fel vid radering:", error);
        }
    }

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
                            <th className="p-3 w-2/10">Name</th>
                            <th className="p-3 w-4/10">Description</th>
                            <th className="p-3 w-1/10">Start date</th>
                            <th className="p-3 w-1/10">End date</th>
                            <th className="p-3 w-2/10"></th>
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
                                <td className="p-3">
                                    <Button
                                        onClick={() =>
                                            handleShowCourseModal(course)
                                        }
                                        className="col-span-2"
                                    >
                                        Update
                                    </Button>
                                    <Button
                                        onClick={() => handleDelete(course)}
                                        className="col-span-2 mx-2"
                                    >
                                        Delete
                                    </Button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <div className="m-3">
                <Button
                    onClick={() => handleShowCourseModal(newCourse)}
                    className="col-span-2"
                >
                    Create course
                </Button>
                {isCourseModalVisible && (
                    <CourseModal
                        selectedCourse={selectedRow}
                        onClose={handleCloseCourseModal}
                    />
                )}
            </div>
        </>
    );
}

import { useState, useEffect } from "react";
import "../index.css";
import Button from "../components/Button";
import CourseModal from "../components/CourseModal";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";
import { deleteCourse } from "../services/courseService";
import { createPortal } from "react-dom";
import ModalCreateResource from "../components/ModalCreateResource";
import { Link } from "react-router-dom";

export default function CourseList() {
    // STATE

    const newCourse = {
        courseId: "",
        name: "",
        description: "",
        startDate: "",
        endDate: "",
        modules: [],
    };

    const [courses, setCourses] = useState<CourseResponse[]>([newCourse]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const [isCourseModalVisible, setIsCourseModalVisible] = useState(false);
    const [isCreateResourceModalVisible, setIsCreateResourceModalVisible] =
        useState(false);
    const [selectedRow, setSelectedRow] = useState<CourseResponse>(newCourse);

    const handleSubmitCourseModal = (returnData: CourseResponse) => {
        setIsCourseModalVisible(false);

        if (selectedRow.courseId != "")
        // Update the course in the list
        {
            setCourses(
                courses.map((c) =>
                    c.courseId === returnData.courseId ? returnData : c,
                ),
            );
        } else
        //show added course in the list
        {
            setCourses([...courses, returnData]);
        }
    };

    const handleCloseCourseModal = () => {
        setIsCourseModalVisible(false);
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
                'Are you sure you want to delete the course "' +
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
            <div className="m-3 flex justify-end">
                <Button
                    onClick={() => handleShowCourseModal(newCourse)}
                    className=""
                >
                    Create course
                </Button>
                {isCourseModalVisible && (
                    <CourseModal
                        selectedCourse={selectedRow}
                        onClose={handleCloseCourseModal}
                        onSubmit={handleSubmitCourseModal}
                    />
                )}
            </div>
            <div className="bg-bg dark:bg-bg-dark border rounded m-3">
                <h1 className="text-3xl font-bold px-3 pb-3 text-center bg-bg-header dark:bg-bg-header-dark text-white dark:text-text-light">
                    Courses
                </h1>
                <table className="w-full text-left">
                    <thead className="bg-bg-window dark:bg-bg-window-dark h-10 border-b border-accent-blue text-text-dark dark:text-text-light">
                        <tr>
                            {/*  <SortableTh
                                field="name"
                                label="Name"
                                sortBy={sortBy}
                                onSortChange={onSortChange}
                            /> */}
                            <th className="p-3 w-2/10">Name</th>
                            <th className="p-3 w-4/10">Description</th>
                            <th className="p-3 w-1/10">Start date</th>
                            <th className="p-3 w-1/10">End date</th>
                            <th className="p-3 w-2/10"></th>
                        </tr>
                    </thead>
                    <tbody className="text-text-dark dark:text-text-light">
                        {courses.map((course, index) => (
                            <tr
                                key={course.courseId}
                                className={
                                    index % 2 === 0
                                        ? "bg-white dark:bg-bg-window-dark"
                                        : "bg-bg dark:bg-bg-dark"
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
                                        onClick={() =>
                                            setIsCreateResourceModalVisible(
                                                true,
                                            )
                                        }
                                        className="hover:cursor-pointer mx-2"
                                    >
                                        Create Resource
                                    </Button>
                                    {isCreateResourceModalVisible &&
                                        createPortal(
                                            <ModalCreateResource
                                                open={
                                                    isCreateResourceModalVisible
                                                }
                                                entityId={course.courseId}
                                                createFor="course"
                                                onClose={() =>
                                                    setIsCreateResourceModalVisible(
                                                        false,
                                                    )
                                                }
                                            />,
                                            document.getElementById("root")!,
                                        )}
                                    <Button
                                        onClick={() => handleDelete(course)}
                                        className="col-span-2"
                                    >
                                        Delete
                                    </Button>
                                    <Link to={`/courses/${course.courseId}`}>
                                        <Button>Go to course</Button>
                                    </Link>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </>
    );
}

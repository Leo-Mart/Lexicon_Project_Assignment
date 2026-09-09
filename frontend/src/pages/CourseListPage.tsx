import { useState, useEffect } from "react";
import "../index.css";
import Button from "../components/Button";
import CourseModal from "../components/CourseModal";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";
import { deleteCourse } from "../services/courseService";
import ModalCreateResource from "../components/ModalCreateResource";
import CourseTable from "../components/CourseTable";
import { createPortal } from "react-dom";

export default function CourseListPage() {
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
    const [resourceTarget, setResourceTarget] = useState<CourseResponse | null>(
        null,
    );
    const [selectedRow, setSelectedRow] = useState<CourseResponse>(newCourse);

    const [sortBy, setSortBy] = useState("name-asc"); // or const DEFAULT_SORT = "name-asc"

    const handleSortChange = (value: string) => {
        setSortBy(value);
    };

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

    const isFirstLoad = loading && courses.length === 0;

    // READ ALL
    useEffect(() => {
        const fetchAllCourses = async () => {
            setLoading(true);
            setError(null);
            try {
                const [sortField, sortDirection = "asc"] = sortBy.split("-");

                const courseData = await fetchCourses({
                    search: "",
                    sortBy: sortField,
                    direction: sortDirection,
                    page: 1,
                    pageSize: 200,
                });
                setCourses(courseData.items);
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

        fetchAllCourses();
    }, [sortBy]);

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
            console.error("Error on render:", error);
        }
    }

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (isFirstLoad) return <p>Loading...</p>;
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
                    className="border-4 border-accent-student"
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
                {resourceTarget &&
                    createPortal(
                        <ModalCreateResource
                            open={true}
                            entityId={resourceTarget.courseId}
                            createFor="course"
                            onClose={() => setResourceTarget(null)}
                        />,
                        document.getElementById("root")!,
                    )}
            </div>
            <div className="bg-bg dark:bg-bg-dark border rounded m-3">
                <h1 className="text-3xl font-bold px-3 pb-3 text-center bg-bg-header dark:bg-bg-header-dark text-white dark:text-text-light">
                    Courses
                </h1>
                <CourseTable
                    courses={courses}
                    sortBy={sortBy}
                    isLoading={loading}
                    onSortChange={handleSortChange}
                    onUpdate={handleShowCourseModal}
                    onCreateResource={(course) => setResourceTarget(course)}
                    onDelete={handleDelete}
                />
            </div>
        </>
    );
}

import { useState, useEffect } from "react";
import "../index.css";
import CourseModal from "../components/CourseModal";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { fetchCourses } from "../services/courseService";
import { deleteCourse } from "../services/courseService";
import ModalCreateResource from "../components/ModalCreateResource";
import { createPortal } from "react-dom";
import type { SortOption } from "../types/SortOption";
import TableToolbar from "../components/TableToolbar";
import DataTable from "../components/DataTable";
import type { Column } from "../types/Column";
import { Link } from "react-router-dom";
import Button from "../components/Button";

const COURSES_SORT_OPTIONS: SortOption[] = [
    { value: "name-asc", label: "Name A-Z" },
    { value: "name-desc", label: "Name Z-A" },
    { value: "description-asc", label: "Description A-Z" },
    { value: "description-desc", label: "Description Z-A" },
    { value: "start-asc", label: "Start Date Old-New" },
    { value: "start-desc", label: "Start Date New-Old" },
    { value: "end-asc", label: "End Date Old-New" },
    { value: "end-desc", label: "End Date New-Old" },
];

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

    const [courses, setCourses] = useState<CourseResponse[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const [search, setSearch] = useState("");
    const [isCourseModalVisible, setIsCourseModalVisible] = useState(false);
    const [resourceTarget, setResourceTarget] = useState<CourseResponse | null>(
        null,
    );
    const [selectedRow, setSelectedRow] = useState<CourseResponse>(newCourse);

    const [sortBy, setSortBy] = useState("name-asc"); // or const DEFAULT_SORT = "name-asc"

    const handleSortChange = (value: string) => {
        setSortBy(value);
    };

    const handleSearchChange = (value: string) => {
        setSearch(value);
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
                    search,
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
    }, [search, sortBy]);

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
            await deleteCourse(course.courseId);
            // Filter the deleted course from state
            setCourses(courses!.filter((c) => c.courseId !== course.courseId));
        } catch (error) {
            console.error("Error on render:", error);
        }
    }

    const courseColumns: Column<CourseResponse>[] = [
        {
            key: "name",
            field: "name",
            header: "Name",
            render: (course) => (
                <Link
                    className="font-bold underline text-buttons text-lg"
                    to={`/courses/${course.courseId}`}
                >
                    {course.name}
                </Link>
            ),
        },
        {
            key: "description",
            field: "description",
            header: "Description",
            render: (course) => course.description,
        },
        {
            key: "startDate",
            field: "startDate",
            header: "Start date",
            render: (course) => course.startDate,
        },
        {
            key: "endDate",
            field: "endDate",
            header: "End date",
            render: (course) => course.endDate,
        },
        {
            key: "actions",
            header: "Interact",
            className: "whitespace-nowrap",
            render: (course) => (
                <div className="flex items-center gap-2">
                    <Button onClick={() => handleShowCourseModal(course)}>
                        Update
                    </Button>
                    <Button onClick={() => setResourceTarget(course)}>
                        Create Resource
                    </Button>
                    <Button
                        variant="cancel"
                        onClick={() => handleDelete(course)}
                    >
                        Delete
                    </Button>
                </div>
            ),
        },
    ];

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
            <div className="m-3 flex justify-between">
                <TableToolbar
                    tableTitle="Courses"
                    search={search}
                    sortBy={sortBy}
                    sortOptions={COURSES_SORT_OPTIONS}
                    onSearchChange={handleSearchChange}
                    onSortChange={handleSortChange}
                    addAction={{
                        label: "Add course",
                        onAdd: () => handleShowCourseModal(newCourse),
                    }}
                />
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
            <DataTable
                items={courses}
                columns={courseColumns}
                getKey={(course) => course.courseId}
                sortBy={sortBy}
                isLoading={loading}
                onSortChange={handleSortChange}
            />
        </>
    );
}

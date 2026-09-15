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
import ConfirmDialog from "../components/ConfirmDialog";
import ErrorDisplay from "../components/ErrorDisplay";
import toast, { Toaster } from "react-hot-toast";

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
        courseResources: [],
    };

    const [courses, setCourses] = useState<CourseResponse[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | undefined>(undefined);

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
            toast.success("Course Updated!");
        } else
        //show added course in the list
        {
            setCourses([...courses, returnData]);
            toast.success("Course Created!");
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

    const [deletingCourse, setDeletingCourse] = useState<
        CourseResponse | undefined
    >(undefined);

    const handleDeleteCourse = async (courseId: string) => {
        try {
            await deleteCourse(courseId);
            setCourses(
                courses!.filter((course) => course.courseId !== courseId),
            );
            setDeletingCourse(undefined);
            toast.success("Course Deleted!");
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message);
                toast.error(`Error deleting course: ${error.message}`);
            }
        } finally {
            setDeletingCourse(undefined);
        }
    };

    // READ ALL
    useEffect(() => {
        const fetchAllCourses = async () => {
            setLoading(true);
            setError(undefined);
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

    const courseColumns: Column<CourseResponse>[] = [
        {
            key: "name",
            field: "name",
            header: "Name",
            render: (course) => (
                <Link
                    className="font-bold underline text-buttons dark:text-buttons-dark text-lg"
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
                        onClick={() => setDeletingCourse(course)}
                    >
                        Delete
                    </Button>
                </div>
            ),
        },
    ];

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
                {deletingCourse && (
                    <ConfirmDialog
                        open={true}
                        title="Delete Course"
                        message={`Are you sure you want to delete the course: ${deletingCourse.name}`}
                        onCancel={() => setDeletingCourse(undefined)}
                        onConfirm={() =>
                            handleDeleteCourse(deletingCourse.courseId)
                        }
                    />
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
            {error && <ErrorDisplay errorResp={error} />}
            <Toaster />
        </>
    );
}

import { Link } from "react-router-dom";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import Button from "./Button";
import SortableTh from "./SortableTableHead";

interface CoursesTableProps {
    courses: CourseResponse[];
    sortBy: string;
    onSortChange: (value: string) => void;
    onUpdate: (course: CourseResponse) => void;
    onCreateResource: (course: CourseResponse) => void;
    onDelete: (course: CourseResponse) => void;
}

export default function CourseTable({
    courses,
    onUpdate,
    onCreateResource,
    onDelete,
    sortBy,
    onSortChange,
}: CoursesTableProps) {
    return (
        <table className="w-full text-left">
            <thead className="bg-bg-window dark:bg-bg-window-dark h-10 border-b border-accent-blue text-text-dark dark:text-text-light">
                <tr>
                    <SortableTh
                        field="name"
                        label="Name"
                        sortBy={sortBy}
                        onSortChange={onSortChange}
                    />
                    <SortableTh
                        field="description"
                        label="Description"
                        sortBy={sortBy}
                        onSortChange={onSortChange}
                    />
                    <SortableTh
                        field="startDate"
                        label="Start date"
                        sortBy={sortBy}
                        onSortChange={onSortChange}
                    />
                    <SortableTh
                        field="endDate"
                        label="End date"
                        sortBy={sortBy}
                        onSortChange={onSortChange}
                    />
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
                            <Button onClick={() => onUpdate(course)}>
                                Update
                            </Button>
                            <Button
                                onClick={() => onCreateResource(course)}
                                className="hover:cursor-pointer mx-2"
                            >
                                Create Resource
                            </Button>
                            <Button onClick={() => onDelete(course)}>
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
    );
}

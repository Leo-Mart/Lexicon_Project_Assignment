import { Link } from "react-router-dom";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import Button from "./Button";

interface CoursesTableProps {
    courses: CourseResponse[];
    onUpdate: (course: CourseResponse) => void;
    onCreateResource: (course: CourseResponse) => void;
    onDelete: (course: CourseResponse) => void;
    // Ready for when you add sorting:
    // sortBy: string;
    // onSortChange: (value: string) => void;
}

export default function CourseTable({
    courses,
    onUpdate,
    onCreateResource,
    onDelete,
}: CoursesTableProps) {
    return (
        <table className="w-full text-left">
            <thead className="bg-bg-window dark:bg-bg-window-dark h-10 border-b border-accent-blue text-text-dark dark:text-text-light">
                <tr>
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

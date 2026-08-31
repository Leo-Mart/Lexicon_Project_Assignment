import { useState } from "react";
import "../index.css";

interface CourseDto {
    courseId: number; // hur gör man med UUID?
    name: string;
    description: string;
    startDate: Date;
    endDate: Date;
}

//const API_URL = "https://localhost:5173/api/courses";

export default function Courses() {
    // STATE
    const [courses, setCourses] = useState<CourseDto[]>([
        {
            courseId: 0,
            name: "First course",
            description: "The first course",
            startDate: new Date("2026-09-01"),
            endDate: new Date("2026-09-07"),
        },
        {
            courseId: 1,
            name: "Second course",
            description: "The second course",
            startDate: new Date("2026-09-08"),
            endDate: new Date("2026-09-15"),
        },
        {
            courseId: 2,
            name: "Third course",
            description: "The third course",
            startDate: new Date("2026-09-16"),
            endDate: new Date("2026-09-23"),
        },
    ]);

    return (
        <>
            <div className="bg-bg border rounded m-3">
                <h1 className="text-3xl font-bold px-3 pb-3 text-center bg-bg-header text-white">
                    Courses
                </h1>
                <table className="w-full">
                    <thead className="bg-bg-window h-12 border-b text-text-dark">
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Start date</th>
                            <th>End date</th>
                        </tr>
                    </thead>
                    <tbody className="text-center">
                        {courses.map((course, index) => (
                            <tr
                                key={course.courseId}
                                className={
                                    index % 2 === 0
                                        ? "bg-white h-10"
                                        : "bg-bg h-10"
                                }
                            >
                                <td>{course.name}</td>
                                <td>{course.description}</td>
                                <td>{course.startDate.toLocaleDateString()}</td>
                                <td>{course.endDate.toLocaleDateString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </>
    );
}

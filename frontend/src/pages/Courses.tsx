import { useState, useEffect } from "react";
import "../index.css";

interface CourseDto {
    courseId: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
}

const API_URL = "http://localhost:5068/api/courses";

export default function Courses() {
    // STATE
    const [courses, setCourses] = useState<CourseDto[]>([
        {
            courseId: "0",
            name: "",
            description: "",
            startDate: "",
            endDate: "",
        },
    ]);

    const [loading, setLoading] = useState<boolean>(true);

    // READ ALL

    useEffect(() => {
        async function loadData() {
            try {
                const response = await fetch(API_URL, {
                    method: "GET",
                    headers: { accept: "application/json; charset=utf-8" },
                });
                if (!response.ok) throw new Error("Couldn't fetch courses");

                const data: CourseDto[] = await response.json();

                setCourses(data);
            } catch (error) {
                console.error("Error reading:", error);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, []);

    // RENDER
    if (loading) return <p>Laddar kurser från API...</p>;

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

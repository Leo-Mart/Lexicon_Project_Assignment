import { useState, useEffect } from "react";
import "../index.css";
import Button from "../components/Button";
import type { CourseDto } from "../interfaces/CourseDto";
import CourseModal from "./CourseModal";

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

    const [isCourseModalVisible, setIsCourseModalVisible] = useState(false);

    const handleShowCourseModal = () => {
        setIsCourseModalVisible(true);
    };

    const handleCloseCourseModal = () => {
        setIsCourseModalVisible(false);
    };

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

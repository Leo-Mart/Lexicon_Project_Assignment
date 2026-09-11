import { useEffect, useState } from "react";
import type { EnrollmentUserResponse } from "../interfaces/enrollment/EnrollmentUserResponse";
import { UserStatus } from "../constants/UserConstant";
import { fetchUsersForCourse } from "../services/enrollmentService";
import { useAuth } from "../hooks/useAuth";

export default function UserSideView({ courseId }: { courseId: string }) {
    const { name } = useAuth();

    const emptyUser = {
        studentId: "",
        courseId: "",
        student: {
            id: "",
            name: "",
            email: "",
            status: UserStatus.Inactive,
        },
    };

    const [isExpanded, setIsExpanded] = useState(false);
    const [users, setUsers] = useState<EnrollmentUserResponse[]>([emptyUser]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchAllUsersForCourse = async () => {
            setLoading(true);
            setError(null);
            try {
                const userData = await fetchUsersForCourse(courseId);
                setUsers(userData);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to fetch users",
                );
                console.error("Fetch error:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAllUsersForCourse();
    }, [courseId]);

    // RENDER
    if (loading) return <p>Loading...</p>;

    if (error)
        return <div className="text-red-500 text-4xl">Error: {error}</div>;
    if (!users)
        return (
            <div className="flex flex-col items-center">
                <h1 className="text-4xl text-text-dark pt-5">
                    No people connected to this course
                </h1>
            </div>
        );

    return (
        <>
            <div className="flex flex-row absolute mt-1 h-auto">
                {isExpanded && (
                    <div className="bg-bg-window h-full w-55 flex flex-col mx-1 z-50">
                        <div className="p-4">
                            <h3 className="pb-3 font-semibold">
                                {users.length === 0
                                    ? "No people in this course"
                                    : "People in this course:"}
                            </h3>
                            <ul>
                                {users
                                    .filter(
                                        (user) => user.student.name === name,
                                    )
                                    .map((user) => (
                                        <li className="mb-3">
                                            {user.student.name}
                                        </li>
                                    ))}

                                {users
                                    .filter(
                                        (user) => user.student.name !== name,
                                    )
                                    .sort((a, b) =>
                                        a.student.name.localeCompare(
                                            b.student.name,
                                        ),
                                    )
                                    .map((user) => (
                                        <li className="" key={user.studentId}>
                                            {user.student.name}
                                        </li>
                                    ))}
                            </ul>
                        </div>
                    </div>
                )}
                <button
                    className={`bg-bg-window rotate-45 transition-transform duration-300 ease-in-out w-20 h-20 m-5 ${isExpanded ? "rotate-90" : "rotate-45"}`}
                    onClick={() => setIsExpanded(!isExpanded)}
                >
                    <p className="-rotate-45">Show people</p>
                </button>
            </div>
        </>
    );
}

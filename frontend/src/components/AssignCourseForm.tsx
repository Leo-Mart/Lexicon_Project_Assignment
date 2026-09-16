import { useState, type SubmitEvent } from "react";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import SelectInput from "./form/SelectInput";
import FormActions from "./form/FormActions";

interface AssignCourseFormProps {
    courses: CourseResponse[];
    onSubmit: (courseId: string) => void;
    onCancel: () => void;
    submitError?: string;
}

export default function AssignCourseForm({
    courses,
    onSubmit,
    onCancel,
    submitError,
}: AssignCourseFormProps) {
    const [courseId, setCourseId] = useState(courses[0]?.courseId ?? "");

    const handleSubmit = (event: SubmitEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!courseId) {
            return;
        }

        onSubmit(courseId);
    };

    return (
        <form className="px-8 pt-6 pb-8 mb-4" onSubmit={handleSubmit}>
            <SelectInput
                label="Course"
                name="course"
                value={courseId}
                options={courses.map((course) => ({
                    value: course.courseId,
                    label: course.name,
                }))}
                onChange={(event) => setCourseId(event.target.value)}
            />

            {submitError && (
                <p className="mb-4 text-sm text-red-600">{submitError}</p>
            )}

            <FormActions submitLabel="Assign course" onCancel={onCancel} />
        </form>
    );
}

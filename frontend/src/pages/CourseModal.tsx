import { useState } from "react";
import "../index.css";
import Button from "../components/Button";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import { createCourse } from "../services/courseService";
import { updateCourse } from "../services/courseService";

interface ModalProps {
    selectedCourse: CourseResponse;
    onClose: () => void;
    onSubmit: (returnData: CourseResponse) => void;
}

const today = new Date();

export default function CourseModal({
    selectedCourse,
    onClose,
    onSubmit,
}: ModalProps) {
    const [formData, setFormData] = useState<CourseResponse>({
        courseId: selectedCourse.courseId,
        name: selectedCourse.name,
        description: selectedCourse.description,
        startDate: selectedCourse.startDate,
        endDate: selectedCourse.endDate,
        modules: [],
    });

    const handleOnSubmit = async (e: React.SubmitEvent) => {
        e.preventDefault();

        try {
            if (formData.courseId != "") {
                await updateCourse(formData.courseId, formData).then(() => {
                    onSubmit(formData);
                });
            } else {
                await createCourse(formData).then((response) => {
                    onSubmit(response);
                });
            }
            //onClose();
        } catch (error) {
            alert(error);
            console.error("Fel vid sparning:", error);
        }
    };

    function resetForm() {
        setFormData({
            courseId: "",
            name: "",
            description: "",
            startDate: "",
            endDate: "",
            modules: [],
        });
    }

    return (
        <>
            <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center backdrop-blur-xs">
                <div className="bg-white rounded-md overflow-hidden max-w-md w-full mx-4">
                    <nav className="bg-bg-header text-white flex justify-between px-4 py-2">
                        <h2 className="text-lg">
                            {formData.courseId == ""
                                ? "Create Course"
                                : "Update Course"}
                        </h2>
                        <button
                            className="bg-btn-cancel py-1 px-2 hover:brightness-110 rounded-full text-sm"
                            onClick={onClose}
                        >
                            &#10005;
                        </button>
                    </nav>
                    <div className="bg-bg py-3 px-3">
                        <form
                            className="px-8 pt-6 pb-8 mb-4"
                            onSubmit={handleOnSubmit}
                        >
                            <div>
                                <input
                                    type="text"
                                    id="id"
                                    value={formData.courseId}
                                    hidden
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="name">Name</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    type="text"
                                    id="name"
                                    name="name"
                                    placeholder="Course name"
                                    maxLength={50}
                                    value={formData.name}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            name: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="description">Description</label>
                                <textarea
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    id="description"
                                    name="description"
                                    placeholder="Course description"
                                    maxLength={200}
                                    rows={5}
                                    value={formData.description}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            description: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="startDate">Start date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    min={today.toLocaleDateString()}
                                    type="date"
                                    id="startDate"
                                    name="startDate"
                                    value={formData.startDate}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            startDate: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="endDate">End date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white p-2"
                                    min={formData.startDate}
                                    type="date"
                                    id="endDate"
                                    name="endDate"
                                    value={formData.endDate}

                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            endDate: e.target.value,
                                        })
                                    }
                                    required
                                />
                            </div>
                            <div className="mt-6">
                                <Button
                                    type="submit"
                                    className="mr-3"
                                    variant="confirm"
                                >
                                    Save
                                </Button>
                                <Button
                                    type="button"
                                    className="mr-3"
                                    onClick={resetForm}
                                    variant="primary"
                                >
                                    Clear
                                </Button>
                                <Button
                                    type="button"
                                    onClick={onClose}
                                    variant="cancel"
                                >
                                    Cancel
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </>
    );
}

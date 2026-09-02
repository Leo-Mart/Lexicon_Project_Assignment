import "../index.css";
import Button from "../components/Button";

interface ModalProps {
    onClose: () => void;
}

export default function CourseModal({ onClose }: ModalProps) {
    return (
        <>
            <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center backdrop-blur-xs">
                <div className="bg-white rounded-md overflow-hidden max-w-md w-full mx-4">
                    <nav className="bg-bg-header text-white flex justify-between px-4 py-2">
                        <h2 className="text-lg">Create course</h2>
                        <button
                            className="bg-btn-cancel py-1 px-2 hover:brightness-110 rounded-full text-sm"
                            onClick={onClose}
                        >
                            &#10005;
                        </button>
                    </nav>
                    <div className="bg-bg py-3 px-3">
                        <form className="px-8 pt-6 pb-8 mb-4" action="">
                            <div className="mb-4">
                                <label htmlFor="courseName">Course Name</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white"
                                    type="text"
                                    id="courseName"
                                    name="courseName"
                                    placeholder="Course name"
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="courseDescription">
                                    Course Description
                                </label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white"
                                    type="text"
                                    id="courseDescription"
                                    name="courseDescription"
                                    placeholder="Course description"
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="startDate">Start date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white"
                                    type="date"
                                    id="startDate"
                                    name="startDate"
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="endDate">Start date</label>
                                <input
                                    className="shadow appearance-none border rounded w-full bg-white"
                                    type="date"
                                    id="endDate"
                                    name="endDate"
                                />
                            </div>
                            <div className="mt-6">
                                <Button className="mr-3" variant="confirm">
                                    Save
                                </Button>
                                <Button onClick={onClose} variant="cancel">
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

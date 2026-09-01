interface LectureProps {
    lectureName: string;
    lectureTime: string;
    teacher: string;
}

export default function Lecture({
    lectureName,
    lectureTime,
    teacher,
}: LectureProps) {
    return (
        <div
            className="bg-buttons rounded-md px-4 py-2 font-semibold hover:brightness-110
            col-span-2 grid place-items-center"
        >
            <p className="text-text-light text-2xl align-middle">
                Next Lecture {lectureTime} - {lectureName}
            </p>
            <span className="bg-purple-200 rounded-md px-2 py-1 text-sm ml-auto">
                Teacher: {teacher}
            </span>
        </div>
    );
}

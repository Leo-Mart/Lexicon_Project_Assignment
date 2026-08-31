import Button from "../components/Button";
import Lecture from "../components/Lecture";

export default function Dashboard() {
    return (
        <>
            <div className="flex justify-center">
                <h1 className="align-middle text-4xl text-text-dark pt-5">
                    Current Module: C# Programming
                </h1>
            </div>
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 gap-8 grid-cols-5">
                <Lecture
                    lectureName="Dependency Injection"
                    lectureTime="13:30"
                    teacher="Michael"
                />
                <Button className="col-span-2 row-span-2">
                    Course Material
                </Button>
                <div className="col-span-1 row-span-2" />
                <Button className="col-span-2 row-span-2">Assignments</Button>
            </div>
        </>
    );
}

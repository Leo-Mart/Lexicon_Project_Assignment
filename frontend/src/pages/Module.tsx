import Button from "../components/Button";
import Lecture from "../components/Lecture";

export default function Dashboard() {
    return (
        <>
            <div className="flex flex-col items-center">
                <h1 className="align-middle text-4xl text-text-dark pt-5">
                    Current Module: C# Programming
                </h1>
                <div className="flex flex-row items-center gap-5">
                    <p className="align-middle text-2xl text-text-dark pt-5">
                        Start: Data
                    </p>
                    <p className="align-middle text-2xl text-text-dark pt-5">
                        End: Data
                    </p>
                </div>
            </div>
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 grid-cols-2 gap-8">
                <Lecture
                    lectureName="Dependency Injection"
                    lectureTime="13:30"
                    teacher="Michael"
                />
                <Button className="row-span-2">Course Material</Button>
                <Button className="row-span-2">Activities</Button>
            </div>
        </>
    );
}

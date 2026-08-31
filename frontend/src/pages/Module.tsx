import Button from "../components/Button";

export default function Dashboard() {
    return (
        <>
            <div className="flex justify-center">
                <h1 className="align-middle">Current Module: C# Programming</h1>
            </div>
            <div className="bg-bg-light h-[calc(100vh-12rem)] p-10 grid grid-flow-col grid-rows-3 gap-8 grid-cols-5">
                <Button className="col-span-5">Next Lecture</Button>
                <Button className="col-span-2">User Control</Button>
                <Button className="col-span-2">Resource Manager</Button>
            </div>
        </>
    );
}

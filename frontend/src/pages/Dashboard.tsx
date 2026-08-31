import Button from "../components/Button";
import Schedule from "../components/Schedule";

export default function Dashboard() {
    return (
        <>
            <div className="bg-bg-light h-screen grid grid-flow-col grid-rows-3 gap-8 grid-cols-5 m-10">
                <Button className="col-span-2">Course Management</Button>
                <Button className="col-span-2">User Control</Button>
                <Button className="col-span-2">Resource Manager</Button>
                <Schedule />
            </div>
        </>
    );
}

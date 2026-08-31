import Button from "../components/Button";

export default function Dashboard() {
    return (
        <>
            <div className="bg-bg-light h-screen grid grid-flow-col grid-rows-4 gap-4 grid-cols-3">
                <p className="text-3xl font-bold underline text-text-dark">
                    This is the dashboard page
                </p>
                <Button>Course Management</Button>
                <Button>User Control</Button>
                <Button>Resource Manager</Button>
            </div>
        </>
    );
}

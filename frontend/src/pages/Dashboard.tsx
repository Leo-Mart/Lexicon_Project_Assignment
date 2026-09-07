import Button from "../components/Button";
import Schedule from "../components/Schedule";
import { Link, useNavigate } from "react-router-dom";

//import { routes } from "../routes/config"; // Adjust the import path

export default function Dashboard() {
    const navigate = useNavigate();

    const handleNavigation = () => {
        navigate("/courselist");
    };

    return (
        <div className="bg-bg-light h-[calc(100vh-8rem)] p-10 grid grid-flow-col grid-rows-3 gap-8 grid-cols-5">
            <Button onClick={handleNavigation} className="col-span-2">
                Course Management
            </Button>
            <Button className="col-span-2">User Control</Button>
            <Link to="/resources" className="col-span-2">
                <Button className="size-full hover:cursor-pointer">
                    Resource Manager
                </Button>
            </Link>
            <Schedule />
        </div>
    );
}

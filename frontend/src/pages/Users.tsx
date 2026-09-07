import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import { fetchUsersWithCourse } from "../services/userService";
import UsersTable from "../components/UsersTable";

export default function Users() {
    const [users, setUsers] = useState<UserWithCourseResponse[]>([]);

    useEffect(() => {
        const loadUsers = async () => {
            const data = await fetchUsersWithCourse();
            setUsers(data);
              

        };

        void loadUsers();
    }, []);

    return (
        <UsersTable
            users={users}
            onEdit={(id) => console.log("Edit", id)}
            onDelete={(id) => console.log("Delete", id)}
            onAssignCourse={(id) => console.log("Assign course", id)}
        />
    );
}
import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import { fetchUsersWithCourse } from "../services/userService";
import UsersTable from "../components/UsersTable";
import UsersToolbar from "../components/UsersToolbar";

export default function Users() {
    const [users, setUsers] = useState<UserWithCourseResponse[]>([]);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("name-asc");
    //const [direction, setDirection] = useState("asc");
    //const [page, setPage] = useState(1);
    // const [pageSize, setPageSize] = useState(20);

    useEffect(() => {
        const loadUsers = async () => {
            const data = await fetchUsersWithCourse();
            setUsers(data);
        };

        void loadUsers();
    }, []);

    return (
        <div className="p-4">
            <UsersToolbar
                search={search}
                sortBy={sortBy}
                onSearchChange={setSearch}
                onSortChange={setSortBy}
                onAddUser={() => console.log("Add user")}
            />

            <UsersTable
                users={users}
                onEdit={(id) => console.log("Edit", id)}
                onDelete={(id) => console.log("Delete", id)}
                onAssignCourse={(id) => console.log("Assign course", id)}
            />
        </div>
    );
}

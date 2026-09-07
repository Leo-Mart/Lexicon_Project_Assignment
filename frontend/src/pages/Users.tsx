import { useEffect, useState } from "react";
import type { UserWithCourseResponse } from "../interfaces/user/UserWithCourseResponse";
import { fetchUsersWithCourse } from "../services/userService";
import UsersTable from "../components/UsersTable";
import UsersToolbar from "../components/UsersToolbar";
import type { QueryParameters } from "../interfaces/common/QueryParameters";
import Pagination from "../components/Pagination";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 20;
const DEFAULT_SORT = "name-asc";

export default function Users() {
    const [users, setUsers] = useState<UserWithCourseResponse[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(DEFAULT_PAGE);
    const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState(DEFAULT_SORT);

    const handleSearchChange = (value: string) => {
        setSearch(value);
        setPage(DEFAULT_PAGE);
    };

    const handleSortChange = (value: string) => {
        setSortBy(value);
        setPage(DEFAULT_PAGE);
    };

    useEffect(() => {
        const loadUsers = async () => {
            const [sortField, sortDirection = "asc"] = sortBy.split("-");

            const query: QueryParameters = {
                search,
                sortBy: sortField,
                direction: sortDirection,
                page,
                pageSize,
            };

            const data = await fetchUsersWithCourse(query);

            setUsers(data.items);
            setTotalCount(data.totalCount);
        };

        void loadUsers();
    }, [search, sortBy, page, pageSize]);

    return (
        <div className="p-4">
            <UsersToolbar
                search={search}
                sortBy={sortBy}
                onSearchChange={handleSearchChange}
                onSortChange={handleSortChange}
                onAddUser={() => console.log("Add user")}
            />

            <UsersTable
                users={users}
                onEdit={(id) => console.log("Edit", id)}
                onDelete={(id) => console.log("Delete", id)}
                onAssignCourse={(id) => console.log("Assign course", id)}
            />

            <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={setPage}
            />
        </div>
    );
}

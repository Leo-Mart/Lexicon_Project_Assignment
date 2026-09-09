import Button from "./Button";

interface UsersToolbarProps {
    search: string;
    sortBy: string;
    onSearchChange: (value: string) => void;
    onSortChange: (value: string) => void;
    onAddUser: () => void;
}

export default function UsersToolbar({
    search,
    sortBy,
    onSearchChange,
    onSortChange,
    onAddUser,
}: UsersToolbarProps) {
    return (
        <div className="flex items-center gap-3 mb-4 px-2">
            <input
                type="search"
                value={search}
                onChange={(event) => onSearchChange(event.target.value)}
                placeholder="Search ..."
                className="bg-slate-700 text-white placeholder:text-slate-400 border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
            />

            <select
                value={sortBy}
                onChange={(event) => onSortChange(event.target.value)}
                className="bg-slate-700 text-white border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
            >
                <option value="name-asc">Name A-Z</option>
                <option value="name-desc">Name Z-A</option>
                <option value="course-asc">Course A-Z</option>
                <option value="course-desc">Course Z-A</option>
                <option value="role-asc">Role A-Z</option>
                <option value="role-desc">Role Z-A</option>
                <option value="status">Status</option>
            </select>

            <Button onClick={onAddUser} className="ml-auto cursor-pointer">
                Add user
            </Button>
        </div>
    );
}

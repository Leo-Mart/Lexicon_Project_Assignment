interface TableSearchBarProps {
    search: string;
    onSearchChange: (value: string) => void;
}

export default function TableSearchBar({
    search,
    onSearchChange,
}: TableSearchBarProps) {
    return (
        <input
            type="search"
            value={search}
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="Search ..."
            className="bg-bg-header-dark text-white placeholder:text-slate-400 border border-slate-500 rounded-md px-3 py-2 outline-none focus:border-slate-300"
        />
    );
}

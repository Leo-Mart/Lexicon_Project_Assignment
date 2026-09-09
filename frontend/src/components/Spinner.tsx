const Spinner = ({ className = "w-8 h-8" }: { className?: string }) => {
    return (
        <div className="inline-flex justify-center" role="status">
            <svg
                aria-hidden="true"
                className={`${className} text-text-dark dark:text-text-light fill-bg dark:fill-bg-dark animate-spin`}
                viewBox="0 0 100 101"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
            >
                {/* paths unchanged */}
            </svg>
            <span className="sr-only">Loading...</span>
        </div>
    );
};

export default Spinner;

import type { ErrorMessage } from "../interfaces/error/ErrorMessage";

interface ErrorDisplayProps {
    errorResp: ErrorMessage | string;
}

const ErrorDisplay = ({ errorResp }: ErrorDisplayProps) => {
    if (errorResp !== undefined && typeof errorResp === "string") {
        return <div className="text-red-600">{errorResp}</div>;
    }

    if (
        errorResp.errors !== undefined &&
        typeof errorResp === "object" &&
        errorResp.errors.length > 0 &&
        typeof errorResp.errors !== "string"
    ) {
        return (
            <div>
                <h3>{errorResp.message}</h3>
                <ul className="text-red-600">
                    {errorResp.errors.map((err) => (
                        <li key={err}>{err}</li>
                    ))}
                </ul>
            </div>
        );
    }
};

export default ErrorDisplay;

import { useState } from "react";
import Button from "../components/Button";
import {
    fetchAllSubmissions,
    fetchSubmissionById,
    fetchSubmissionsByActivityId,
    getCurrentUserSubmissions,
    setFeedback,
} from "../services/submissionService";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";

// Dev-only page: one button per submissions endpoint, output shown below.
export default function SubmissionsTest() {
    const [submissions, setSubmissions] = useState<SubmissionResponse[]>([]);
    const [output, setOutput] = useState<string>("");

    // Wraps an endpoint call so every button shares the same error handling.
    const run = async (label: string, call: () => Promise<unknown>) => {
        try {
            const result = await call();
            setOutput(`${label}:\n${JSON.stringify(result, null, 2)}`);
        } catch (err) {
            setOutput(`${label} failed:\n${(err as Error).message}`);
        }
    };

    return (
        <div className="p-10">
            <h1 className="text-xl font-bold mb-4">Submissions test panel</h1>

            <div className="flex flex-wrap gap-3 mb-6">
                <Button
                    onClick={() =>
                        run("GET /submissions", async () => {
                            const data = await fetchAllSubmissions();
                            setSubmissions(data);
                            return data;
                        })
                    }
                >
                    Get all (teacher)
                </Button>

                <Button
                    onClick={() =>
                        run("GET /submissions/me", getCurrentUserSubmissions)
                    }
                >
                    Get my submissions
                </Button>

                <Button
                    onClick={() => {
                        const id = window.prompt("Submission id?");
                        if (id)
                            run(`GET /submissions/${id}`, () =>
                                fetchSubmissionById(id),
                            );
                    }}
                >
                    Get by id
                </Button>

                <Button
                    onClick={() => {
                        const activityId = window.prompt("Activity id?");
                        if (activityId)
                            run(`GET /submissions/activity/${activityId}`, () =>
                                fetchSubmissionsByActivityId(activityId),
                            );
                    }}
                >
                    Get by activity
                </Button>

                <Button
                    onClick={() => {
                        const id = window.prompt("Submission id?");
                        const feedback = id
                            ? window.prompt("Feedback text?")
                            : null;
                        if (id && feedback)
                            run(`PUT /submissions/${id}/feedback`, () =>
                                setFeedback(id, { feedback }),
                            );
                    }}
                >
                    Set feedback
                </Button>
            </div>

            {submissions.length > 0 && (
                <table className="w-full text-left border-collapse mb-6">
                    <thead>
                        <tr className="border-b">
                            <th className="pr-4">Id</th>
                            <th className="pr-4">Student</th>
                            <th className="pr-4">Status</th>
                            <th>Feedback</th>
                        </tr>
                    </thead>
                    <tbody>
                        {submissions.map((s) => (
                            <tr key={s.submissionId} className="border-b">
                                <td className="pr-4">{s.submissionId}</td>
                                <td className="pr-4">{s.studentId}</td>
                                <td className="pr-4">{s.status}</td>
                                <td>{s.feedback ?? "-"}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <pre className="bg-bg-light p-4 rounded-md whitespace-pre-wrap">
                {output}
            </pre>
        </div>
    );
}

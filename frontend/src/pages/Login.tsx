import Button from "../components/Button";
import type { LoginDto } from "../interfaces/auth/LoginDto";
import { useAuth } from "../contexts/AuthContext";

export default function Login() {
    const { login, loginError } = useAuth();
    const handleSubmit = (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();

        const formData = new FormData(e.currentTarget);

        const loginPayload: LoginDto = {
            email: formData.get("email")!.toString(),
            password: formData.get("password")!.toString(),
        };

        login(loginPayload);
        // clear form
        // navigate to userpage/dashboard
    };
    return (
        <>
            <div className="flex flex-1 flex-col items-center justify-center">
                <h1 className="text-3xl mb-3 font-bold text-text-dark dark:text-text-light">
                    Login
                </h1>
                <form
                    onSubmit={handleSubmit}
                    className="w-1/3 h-full flex flex-col gap-5"
                >
                    <label
                        htmlFor="email"
                        className="block overflow-hidden shadow-sm text-text-dark dark:text-text-light bg-bg-window dark:bg-bg-window-dark rounded-lg p-2"
                    >
                        <span className="text-xs font-medium">Email</span>
                        <input
                            type="email"
                            name="email"
                            placeholder="Enter your email"
                            id="email"
                            className="mt-1 text-text-dark dark:text-text-light w-full border-none bg-transparent p-0 focus:border-transparent focus:outline-none focus:ring-0 "
                            required
                        />
                    </label>
                    <label
                        htmlFor="password"
                        className="block overflow-hidden shadow-sm text-text-dark dark:text-text-light bg-bg-window dark:bg-bg-window-dark rounded-lg p-2"
                    >
                        <span className="text-xs font-medium">Password</span>
                        <input
                            type="password"
                            name="password"
                            placeholder="Enter your password"
                            id="password"
                            className="mt-1 text-text-dark dark:text-text-light w-full border-none bg-transparent p-0 focus:border-transparent focus:outline-none focus:ring-0 "
                            required
                        />
                    </label>
                    {loginError && (
                        <span className="text-red-700">{loginError}</span>
                    )}
                    <Button
                        variant="confirm"
                        className="hover:cursor-pointer"
                        type="submit"
                    >
                        Login
                    </Button>
                </form>
            </div>
        </>
    );
}

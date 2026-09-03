import Button from "../components/Button";
import { useAuth } from "../hooks/useAuth";
import type { LoginDto } from "../interfaces/auth/LoginDto";

export default function Login() {
    const { loginUser, loginError } = useAuth();
    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();

        const formData = new FormData(e.currentTarget);

        const loginPayload: LoginDto = {
            email: formData.get("email")!.toString(),
            password: formData.get("password")!.toString(),
        };

        loginUser(loginPayload);
    };
    return (
        <>
            <div className="flex flex-1 flex-col items-center justify-center">
                <div className="flex flex-col items-center p-5 rounded-lg  bg-bg-window dark:bg-bg-window-dark w-1/3">
                    <h1 className="text-3xl mb-3 font-bold text-text-light">
                        Login
                    </h1>
                    <form
                        onSubmit={handleSubmit}
                        className="h-full flex flex-col gap-2 p-2 w-full"
                    >
                        <label
                            className="text-text-dark dark:text-text-light mx-auto"
                            htmlFor="email"
                        >
                            Email
                        </label>
                        <input
                            className="shadow appearance-none border p-2 rounded w-full bg-bg mx-auto"
                            type="email"
                            id="email"
                            name="email"
                            placeholder="Enter your email"
                            required
                        />
                        <label
                            className="text-text-dark dark:text-text-light mx-auto"
                            htmlFor="password"
                        >
                            Password
                        </label>
                        <input
                            className="shadow appearance-none border p-2 rounded w-full bg-bg mx-auto"
                            type="password"
                            id="password"
                            name="password"
                            placeholder="Enter your password"
                            required
                        />
                        {loginError && (
                            <span className="text-red-700">{loginError}</span>
                        )}
                        <Button
                            variant="confirm"
                            className="hover:cursor-pointer mx-auto w-full"
                            type="submit"
                        >
                            Login
                        </Button>
                    </form>
                </div>
            </div>
        </>
    );
}

// App.tsx
import { Routes, Route } from "react-router-dom";
import { Suspense } from "react";
import { routes } from "./routes/config.ts";
import Header from "./components/Header";
import Footer from "./components/Footer.tsx";
import ProtectedRoute from "./components/ProtectedRoute.tsx";
import AuthProvider from "./contexts/AuthProvider.tsx";

export default function App() {
    const publicRoutes = routes.filter((route) => !route.isProtected);
    const protectedRoutes = routes.filter((route) => route.isProtected);
    return (
        <AuthProvider>
            <div className="flex flex-col min-h-screen bg-bg dark:bg-bg-dark">
                <Header />
                <Suspense fallback={<div>Loading...</div>}>
                    <main className="flex flex-col min-h-screen overflow-auto">
                        <Routes>
                            {publicRoutes.map(
                                ({ path, component: Component }) => (
                                    <Route
                                        key={path}
                                        path={path}
                                        element={<Component />}
                                    />
                                ),
                            )}
                            <Route element={<ProtectedRoute />}>
                                {protectedRoutes.map(
                                    ({ path, component: Component }) => (
                                        <Route
                                            key={path}
                                            path={path}
                                            element={<Component />}
                                        />
                                    ),
                                )}
                            </Route>
                        </Routes>
                    </main>
                </Suspense>
                <Footer />
            </div>
        </AuthProvider>
    );
}

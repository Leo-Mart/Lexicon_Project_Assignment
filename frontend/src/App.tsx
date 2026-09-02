// App.tsx
import { Routes, Route } from "react-router-dom";
import { Suspense } from "react";
import { routes } from "./routes/config.ts";
import Header from "./components/Header";
import Footer from "./components/Footer.tsx";

export default function App() {
    return (
        <div className="flex flex-col min-h-screen bg-bg dark:bg-bg-dark">
            <Header />
            <Suspense fallback={<div>Loading...</div>}>
                <main className="flex flex-col min-h-screen overflow-auto">
                    <Routes>
                        {routes.map(({ path, component: Component }) => (
                            <Route
                                key={path}
                                path={path}
                                element={<Component />}
                            />
                        ))}
                    </Routes>
                </main>
            </Suspense>
            <Footer />
        </div>
    );
}

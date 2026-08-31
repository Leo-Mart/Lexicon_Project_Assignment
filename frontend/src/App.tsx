// App.tsx
import { Routes, Route } from "react-router-dom";
import { Suspense } from "react";
import { routes } from "./routes/config.ts";
import Header from "./components/Header";
import Footer from "./components/Footer.tsx";
/* import Footer from "./components/Footer.tsx"; */

export default function App() {
    return (
        <>
            <Header />
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    {routes.map(({ path, component: Component }) => (
                        <Route key={path} path={path} element={<Component />} />
                    ))}
                </Routes>
            </Suspense>
            <Footer />
        </>
    );
}

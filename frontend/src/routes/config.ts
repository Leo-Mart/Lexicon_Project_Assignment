import { lazy } from "react";

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Courses")),
    },
    {
        path: "/index",
        component: lazy(() => import("../pages/Courses")),
    },
    {
        path: "/login",
        component: lazy(() => import("../pages/Login")),
    },
];

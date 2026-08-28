import { lazy } from "react";

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Courses")),
        createHeader: false,
    },
    {
        path: "/index",
        component: lazy(() => import("../pages/Courses")),
        createHeader: true,
    },
    {
        path: "/login",
        component: lazy(() => import("../pages/Login")),
        createHeader: true,
    },
];

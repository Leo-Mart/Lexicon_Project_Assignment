import { lazy } from "react";

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: false,
    },
    {
        path: "/index",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: true,
    },
    {
        path: "/login",
        component: lazy(() => import("../pages/Login")),
        createHeader: true,
    },
    {
        path: "/courses",
        component: lazy(() => import("../pages/Courses")),
        createHeader: true,
    },
    {
        path: "/module",
        component: lazy(() => import("../pages/Module")),
        createHeader: true,
    },
    {
        path: "/courselist",
        component: lazy(() => import("../pages/CourseList")),
        createHeader: true,
    },
];

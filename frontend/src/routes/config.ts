import { lazy } from "react";

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: false,
    },
    {
        path: "/index",
        displayName: "Dashboard",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: true,
    },
    {
        path: "/login",
        displayName: "Login",
        component: lazy(() => import("../pages/Login")),
        createHeader: true,
    },
    {
        path: "/courses",
        displayName: "Courses",
        component: lazy(() => import("../pages/Courses")),
        createHeader: true,
    },
    {
        path: "/module/:id",
        displayName: "Module",
        component: lazy(() => import("../pages/ModulePage")),
        createHeader: false,
    },
    {
        path: "/module",
        displayName: "Module",
        component: lazy(() => import("../pages/ModulePage")),
        createHeader: true,
    },
    {
        path: "/courselist",
        displayName: "Course list",
        component: lazy(() => import("../pages/CourseList")),
        createHeader: true,
    },
];

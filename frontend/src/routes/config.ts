import { lazy } from "react";

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: false,
        isProtected: false,
    },
    {
        path: "/index",
        displayName: "Dashboard",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: true,
        isProtected: false,
    },
    {
        path: "/login",
        displayName: "Login",
        component: lazy(() => import("../pages/Login")),
        createHeader: true,
        isProtected: false,
    },
    {
        path: "/courses",
        displayName: "Courses",
        component: lazy(() => import("../pages/Courses")),
        createHeader: true,
        isProtected: false,
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
        isProtected: false,
    },
    {
        path: "/courselist",
        displayName: "Course list",
        component: lazy(() => import("../pages/CourseList")),
        createHeader: true,
        isProtected: false,
    },
];

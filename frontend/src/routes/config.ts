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
        path: "/module:id",
        displayName: "Module Default",
        component: lazy(() => import("../pages/ModulePage")),
        createHeader: true,
    },
    {
        path: "/module/40000000-0000-0000-0000-000000000001",
        displayName: "Module 1",
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

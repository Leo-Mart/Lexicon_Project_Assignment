import { lazy } from "react";
import type { UserRole } from "../constants/UserConstant";
const teacherOnly: UserRole[] = ["Teacher"];

export const routes = [
    {
        path: "",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: false,
        isProtected: true,
        allowedRoles: teacherOnly,
    },
    {
        path: "/index",
        displayName: "Dashboard",
        component: lazy(() => import("../pages/Dashboard")),
        createHeader: true,
        isProtected: true,
        allowedRoles: teacherOnly,
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
        path: "/module/:moduleId",
        displayName: "Module",
        component: lazy(() => import("../pages/ModulePage")),
        createHeader: false,
        isProtected: true,
    },
    {
        path: "/resources",
        displayName: "Resources",
        component: lazy(() => import("../pages/ResourceManagement")),
        createHeader: false,
        isProtected: true,
        allowedRoles: teacherOnly,
    },
    {
        path: "/module",
        displayName: "Module",
        component: lazy(() => import("../pages/ModulePage")),
        createHeader: false,
        isProtected: true,
        allowedRoles: teacherOnly,
    },
    {
        path: "/courselist",
        displayName: "Course list",
        component: lazy(() => import("../pages/CourseListPage")),
        createHeader: false,
        isProtected: true,
        allowedRoles: teacherOnly,
    },
    {
        path: "/courses/:courseId",
        displayName: "Course details",
        component: lazy(() => import("../pages/CourseDetails")),
        createHeader: false,
        isProtected: true,
    },
    {
        path: "/users",
        displayName: "Users",
        component: lazy(() => import("../pages/Users")),
        createHeader: true,
        isProtected: true,
        allowedRoles: teacherOnly,
    },
];

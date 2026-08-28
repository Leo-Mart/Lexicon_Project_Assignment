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
];

import { useState } from "react";
import Button from "../components/Button";
import Dialog from "../components/Dialog";
import ResourceTree, { type DrillItem, type ResourceConfig } from "../components/ResourceTree";
import FormModal, { type EntityFormConfig } from "../components/FormModal";
import {
    fetchCourses,
    fetchCourse,
    updateCourse,
    createCourse,
} from "../services/courseService";
import { fetchModuleById, updateModule, createModule } from "../services/moduleService";
// Aliased: activityService's update function is misnamed "updateCourse"
// (copy-paste bug) - not renaming that file right now, just working around it here.
import { updateCourse as updateActivity, createActivity } from "../services/activityService";
import {
    fetchAllSubmissions,
    fetchSubmissionsByActivityId,
    setFeedback,
} from "../services/submissionService";
// Aliased: userService's create function is misnamed "createCourse" (same
// copy-paste bug as activityService) - not renaming that file, working around it here.
import { fetchUsers, updateUser, createUser as createUserAccount } from "../services/userService";
import {
    fetchResourcesByActivityId,
    createResource,
    updateResource,
    addResourceToActivity,
} from "../services/resourceService";
import type { CourseResponse } from "../interfaces/course/CourseResponse";
import type { ModuleResponse } from "../interfaces/module/ModuleResponse";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import type { SubmissionResponse } from "../interfaces/submission/SubmissionResponse";
import type { UserResponse } from "../interfaces/user/UserResponse";
import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";
import type { UserCreateRequest } from "../interfaces/user/UserCreateRequest";
import { ActivityType, ActivityTypeNames } from "../constants/ActivityType";
import type { UserRole } from "../constants/UserConstant";

// Same FormModal, different shapes - this is the point.
const courseFormConfig: EntityFormConfig<CourseResponse> = {
    title: "Edit course",
    fields: [
        { name: "name", label: "Name", type: "text", required: true, maxLength: 50 },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 200,
        },
        { name: "startDate", label: "Start date", type: "date", required: true },
        { name: "endDate", label: "End date", type: "date", required: true },
    ],
};

const moduleFormConfig: EntityFormConfig<ModuleResponse> = {
    title: "Edit module",
    fields: [
        { name: "name", label: "Name", type: "text", required: true, maxLength: 50 },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 200,
        },
        { name: "startDate", label: "Start date", type: "date", required: true },
        { name: "endDate", label: "End date", type: "date", required: true },
    ],
};

const activityFormConfig: EntityFormConfig<ActivityResponse> = {
    title: "Edit activity",
    fields: [
        { name: "name", label: "Name", type: "text", required: true, maxLength: 50 },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 200,
        },
        {
            name: "type",
            label: "Type",
            type: "select",
            required: true,
            options: Object.entries(ActivityTypeNames).map(([value, label]) => ({
                value,
                label,
            })),
        },
        { name: "startAt", label: "Start at", type: "datetime-local", required: true },
        { name: "endAt", label: "End at", type: "datetime-local", required: true },
        { name: "deadline", label: "Deadline", type: "datetime-local" },
    ],
};

const submissionFormConfig: EntityFormConfig<SubmissionResponse> = {
    title: "Submission",
    fields: [
        { name: "text", label: "Submission", type: "textarea", readOnly: true },
        { name: "feedback", label: "Feedback", type: "textarea", maxLength: 200 },
    ],
};

const resourceFormConfig: EntityFormConfig<ResourceResponse> = {
    title: "Edit resource",
    fields: [
        { name: "name", label: "Name", type: "text", required: true, maxLength: 50 },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 200,
        },
        { name: "content", label: "Text", type: "textarea", maxLength: 2000 },
        { name: "uri", label: "Link", type: "text" },
    ],
};

const userFormConfig: EntityFormConfig<UserResponse> = {
    title: "Edit user",
    fields: [
        { name: "name", label: "Name", type: "text", required: true },
        { name: "email", label: "Email", type: "text", required: true },
    ],
};

// Separate from userFormConfig - creating a user needs a password and role,
// editing one doesn't. Different shape, same FormModal either way.
const userCreateFormConfig: EntityFormConfig<UserCreateRequest> = {
    title: "New user",
    fields: [
        { name: "name", label: "Name", type: "text", required: true },
        { name: "email", label: "Email", type: "text", required: true },
        { name: "password", label: "Password", type: "text", required: true },
        {
            name: "role",
            label: "Role",
            type: "select",
            required: true,
            options: [
                { value: "Student", label: "Student" },
                { value: "Teacher", label: "Teacher" },
            ],
        },
    ],
};

interface EditingState<T extends Record<string, unknown>> {
    config: EntityFormConfig<T>;
    value: T;
    onSave: (data: T) => Promise<void>;
}

type ResourceKey =
    | "root"
    | "course"
    | "module"
    | "activity"
    | "activityBranch"
    | "submission"
    | "resource"
    | "student"
    | "studentSubmission";

// One entry per controller/service - the only place that knows what a
// "course" or "student" is. ResourceTree just renders {id, label} rows.
const resources: Record<ResourceKey, ResourceConfig<ResourceKey>> = {
    root: {
        label: "Browse",
        child: null,
        editable: false,
        creatable: false,
        branch: true,
        load: async () => [
            { id: "course", label: "Courses", raw: null },
            { id: "student", label: "Students", raw: null },
        ],
    },
    course: {
        label: "Courses",
        child: "module",
        editable: true,
        creatable: true,
        branch: false,
        load: async () =>
            (await fetchCourses()).map((c) => ({
                id: c.courseId,
                label: c.name,
                raw: c,
            })),
    },
    module: {
        label: "Modules",
        child: "activity",
        editable: true,
        creatable: true,
        branch: false,
        // Hack: no "modules by course" endpoint, so we fetch the whole
        // course just to read its nested modules list.
        load: async (courseId) => {
            const course = await fetchCourse(courseId);
            return course.modules.map((m) => ({
                id: m.moduleId,
                label: m.name,
                raw: m,
            }));
        },
    },
    activity: {
        label: "Activities",
        child: "activityBranch",
        editable: true,
        creatable: true,
        branch: false,
        // Hack: same as modules - fetches the whole module for its
        // nested activities instead of an "activities by module" endpoint.
        load: async (moduleId) => {
            const module = await fetchModuleById(moduleId);
            return module.activities.map((a) => ({
                id: a.activityId,
                label: a.name,
                raw: a,
            }));
        },
    },
    // An activity holds two different kinds of children - this level just
    // picks which one, same idea as root picking Courses vs Students.
    activityBranch: {
        label: "View",
        child: null,
        editable: false,
        creatable: false,
        branch: true,
        load: async () => [
            { id: "submission", label: "Submissions", raw: null },
            { id: "resource", label: "Resources", raw: null },
        ],
    },
    submission: {
        label: "Submissions",
        child: null,
        editable: true,
        creatable: false,
        branch: false,
        load: async (activityId) => {
            const [submissions, users] = await Promise.all([
                fetchSubmissionsByActivityId(activityId).catch(() => []),
                fetchUsers(),
            ]);
            return submissions.map((s) => {
                const name = users.find((u) => u.id === s.studentId)?.name ?? s.studentId;
                return {
                    id: s.submissionId,
                    label: `${name} - ${s.feedback ? "reviewed" : "pending"}`,
                    raw: s,
                };
            });
        },
    },
    resource: {
        label: "Resources",
        child: null,
        editable: true,
        creatable: true,
        branch: false,
        load: async (activityId) =>
            (await fetchResourcesByActivityId(activityId)).map((r) => ({
                id: r.resourceId,
                label: r.name,
                raw: r,
            })),
    },
    // No roster/teacher-per-course endpoint yet, so "who's enrolled" isn't
    // wired up. This student list is derived from who has submitted something.
    student: {
        label: "Students",
        child: "studentSubmission",
        editable: true,
        creatable: true,
        branch: false,
        load: async () => {
            const [submissions, users] = await Promise.all([
                fetchAllSubmissions(),
                fetchUsers(),
            ]);
            const studentIds = [...new Set(submissions.map((s) => s.studentId))];
            return studentIds.map((id) => {
                const user = users.find((u) => u.id === id);
                return { id, label: user?.name ?? id, raw: user ?? null };
            });
        },
    },
    studentSubmission: {
        label: "Submissions",
        child: null,
        editable: true,
        creatable: false,
        branch: false,
        load: async (studentId) => {
            const submissions = await fetchAllSubmissions();
            return submissions
                .filter((s) => s.studentId === studentId)
                .map((s) => ({
                    id: s.submissionId,
                    label: `${s.text.slice(0, 30)} - ${s.feedback ? "reviewed" : "pending"}`,
                    raw: s,
                }));
        },
    },
};

export default function DrillDownDemo() {
    const [open, setOpen] = useState(false);
    // Hack: `any` here because one state slot has to hold whichever entity
    // type is being edited - course, module, or user.
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const [editing, setEditing] = useState<EditingState<any> | null>(null);

    const handleEdit = (resourceKey: ResourceKey, item: DrillItem, refresh: () => void) => {
        if (resourceKey === "course") {
            const course = item.raw as CourseResponse;
            setEditing({
                config: courseFormConfig,
                value: course,
                onSave: async (data: CourseResponse) => {
                    await updateCourse(data.courseId, data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "module") {
            const module = item.raw as ModuleResponse;
            setEditing({
                config: moduleFormConfig,
                value: module,
                onSave: async (data: ModuleResponse) => {
                    await updateModule(data.moduleId, data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "activity") {
            const activity = item.raw as ActivityResponse;
            setEditing({
                config: activityFormConfig,
                value: activity,
                onSave: async (data: ActivityResponse) => {
                    await updateActivity(data.activityId, data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "submission" || resourceKey === "studentSubmission") {
            const submission = item.raw as SubmissionResponse;
            setEditing({
                config: submissionFormConfig,
                value: submission,
                onSave: async (data: SubmissionResponse) => {
                    await setFeedback(data.submissionId, { feedback: data.feedback ?? "" });
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "resource") {
            const resource = item.raw as ResourceResponse;
            setEditing({
                config: resourceFormConfig,
                value: resource,
                onSave: async (data: ResourceResponse) => {
                    await updateResource(data.resourceId, data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "student" && item.raw) {
            const user = item.raw as UserResponse;
            setEditing({
                config: userFormConfig,
                value: user,
                onSave: async (data: UserResponse) => {
                    await updateUser(data.id, data);
                    setEditing(null);
                    refresh();
                },
            });
        }
    };

    const handleCreate = (resourceKey: ResourceKey, parentId: string, refresh: () => void) => {
        if (resourceKey === "course") {
            const empty: CourseResponse = {
                courseId: "",
                name: "",
                description: "",
                startDate: "",
                endDate: "",
                modules: [],
            };
            setEditing({
                config: { ...courseFormConfig, title: "New course" },
                value: empty,
                onSave: async (data: CourseResponse) => {
                    await createCourse(data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "module") {
            const empty: ModuleResponse = {
                moduleId: "",
                courseId: parentId,
                name: "",
                description: "",
                startDate: "",
                endDate: "",
                activities: [],
            };
            setEditing({
                config: { ...moduleFormConfig, title: "New module" },
                value: empty,
                onSave: async (data: ModuleResponse) => {
                    await createModule(data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "activity") {
            // ActivityRequest has no moduleId field (a gap in that
            // interface), so moduleId rides along on this ActivityResponse-
            // shaped object instead - it's extra JSON the backend DTO wants.
            const empty: ActivityResponse = {
                activityId: "",
                moduleId: parentId,
                type: ActivityType.Task,
                name: "",
                description: "",
                startAt: "",
                endAt: "",
                createdAt: "",
                updatedAt: "",
                deadline: null,
            };
            setEditing({
                config: { ...activityFormConfig, title: "New activity" },
                value: empty,
                onSave: async (data: ActivityResponse) => {
                    await createActivity(data);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "resource") {
            const empty: ResourceResponse = {
                resourceId: "",
                createdByTeacherId: "",
                name: "",
                description: "",
                content: "",
                uri: "",
                createdAt: "",
                updatedAt: "",
            };
            setEditing({
                config: { ...resourceFormConfig, title: "New resource" },
                value: empty,
                // Create then link it to this activity - two calls, since
                // resources aren't owned by an activity, just attached to it.
                onSave: async (data: ResourceResponse) => {
                    const created = await createResource(data);
                    await addResourceToActivity(created.resourceId, parentId);
                    setEditing(null);
                    refresh();
                },
            });
        } else if (resourceKey === "student") {
            const empty: UserCreateRequest = {
                name: "",
                email: "",
                password: "",
                role: "Student" as UserRole,
            };
            setEditing({
                config: userCreateFormConfig,
                value: empty,
                onSave: async (data: UserCreateRequest) => {
                    await createUserAccount(data);
                    setEditing(null);
                    refresh();
                },
            });
        }
    };

    return (
        <div className="p-10">
            <Button onClick={() => setOpen(true)}>Browse</Button>
            {open && (
                <Dialog title="Browse" onClose={() => setOpen(false)}>
                    <ResourceTree
                        resources={resources}
                        rootKey="root"
                        onEdit={handleEdit}
                        onCreate={handleCreate}
                    />
                </Dialog>
            )}
            {editing && (
                <FormModal
                    config={editing.config}
                    initialValue={editing.value}
                    onSave={editing.onSave}
                    onClose={() => setEditing(null)}
                />
            )}
        </div>
    );
}

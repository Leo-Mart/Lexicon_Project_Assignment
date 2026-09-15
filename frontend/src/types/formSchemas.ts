import type { EntityFormConfig } from "../components/FormModal";
import { ActivityType } from "../constants/ActivityType";
import type { ActivityRequest } from "../interfaces/activity/ActivityRequest";
import type { ModuleRequest } from "../interfaces/module/ModuleRequest";
import type { ResourceRequest } from "../interfaces/resource/ResourceRequest";

export const createActivityFormConfig: EntityFormConfig<ActivityRequest> = {
    title: "Create new Activity",
    fields: [
        {
            name: "name",
            label: "Name",
            type: "text",
            required: true,
            maxLength: 100,
        },
        {
            name: "type",
            label: "Activity Type",
            type: "select",
            required: true,
            maxLength: 100,
            options: [
                {
                    value: ActivityType.Task,
                    label: "Task",
                },
                {
                    value: ActivityType.Lecture,
                    label: "Lecture",
                },
                {
                    value: ActivityType.ELearning,
                    label: "E-learning",
                },
                {
                    value: ActivityType.Practice,
                    label: "Practice",
                },
                {
                    value: ActivityType.Other,
                    label: "Other",
                },
            ],
        },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 100,
        },
        {
            name: "startAt",
            label: "Start Date",
            type: "datetime-local",
            required: true,
        },
        {
            name: "endAt",
            label: "End Date",
            type: "datetime-local",
            required: true,
        },
        {
            name: "deadline",
            label: "Set a deadline",
            type: "datetime-local",
        },
    ],
};

export const createModuleFormConfig: EntityFormConfig<ModuleRequest> = {
    title: "Create new Module",
    fields: [
        {
            name: "name",
            label: "Name",
            type: "text",
            required: true,
            maxLength: 100,
        },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 100,
        },
        {
            name: "startDate",
            label: "Start Date",
            type: "date",
            required: true,
        },
        {
            name: "endDate",
            label: "End Date",
            type: "date",
            required: true,
        },
    ],
};

export const createResourceFormConfig: EntityFormConfig<ResourceRequest> = {
    title: "Create new Resource",
    fields: [
        {
            name: "name",
            label: "Name",
            type: "text",
            required: true,
            maxLength: 100,
        },
        {
            name: "description",
            label: "Description",
            type: "textarea",
            required: true,
            maxLength: 100,
        },
        {
            name: "content",
            label: "Content",
            type: "textarea",
            required: false,
            maxLength: 100,
        },
        {
            name: "uri",
            label: "URL",
            type: "url",
            required: false,
            maxLength: 100,
        },
    ],
};

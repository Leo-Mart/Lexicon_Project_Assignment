import { useState, type FormEvent } from "react";
import {
    UserStatus,
    type UserRole,
    type UserStatus as UserStatusType,
} from "../constants/UserConstant";
import TextInput from "./form/TextInput";
import SelectInput from "./form/SelectInput";
import FormActions from "./form/FormActions";

interface UserFormValues {
    name: string;
    email: string;
    password: string;
    role: UserRole;
    status: UserStatusType;
}

interface UserFormProps {
    mode: "create" | "edit";
    initialValues?: Partial<UserFormValues>;
    onSubmit: (values: UserFormValues) => void;
    onCancel: () => void;
    submitError?: string;
}

export default function UserForm({
    mode,
    initialValues,
    onSubmit,
    onCancel,
    submitError,
}: UserFormProps) {
    const defaultValues: UserFormValues = {
        name: initialValues?.name ?? "",
        email: initialValues?.email ?? "",
        password: "",
        role: initialValues?.role ?? "Student",
        status: initialValues?.status ?? UserStatus.Active,
    };

    const [formData, setFormData] = useState<UserFormValues>(defaultValues);

    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        onSubmit(formData);
    };

    const resetForm = () => {
        setFormData(defaultValues);
    };

    return (
        <form className="px-8 pt-6 pb-8 mb-4" onSubmit={handleSubmit}>
            <TextInput
                label="Name"
                name="name"
                type="text"
                placeholder="Name"
                maxLength={100}
                value={formData.name}
                onChange={(event) =>
                    setFormData({
                        ...formData,
                        name: event.target.value,
                    })
                }
                required
            />

            <TextInput
                label="Email"
                name="email"
                type="email"
                placeholder="Email"
                value={formData.email}
                onChange={(event) =>
                    setFormData({
                        ...formData,
                        email: event.target.value,
                    })
                }
                required
            />

            {mode === "create" && (
                <TextInput
                    label="Password"
                    name="password"
                    type="password"
                    placeholder="Password"
                    value={formData.password}
                    onChange={(event) =>
                        setFormData({
                            ...formData,
                            password: event.target.value,
                        })
                    }
                    required
                />
            )}

            <SelectInput
                label="Role"
                name="role"
                value={formData.role}
                options={[
                    { value: "Student", label: "Student" },
                    { value: "Teacher", label: "Teacher" },
                ]}
                onChange={(event) =>
                    setFormData({
                        ...formData,
                        role: event.target.value as UserRole,
                    })
                }
            />

            {mode === "edit" && (
                <SelectInput
                    label="Status"
                    name="status"
                    value={formData.status}
                    options={[
                        {
                            value: UserStatus.Active,
                            label: "Active",
                        },
                        {
                            value: UserStatus.Inactive,
                            label: "Inactive",
                        },
                        {
                            value: UserStatus.Suspended,
                            label: "Suspended",
                        },
                    ]}
                    onChange={(event) =>
                        setFormData({
                            ...formData,
                            status: Number(
                                event.target.value,
                            ) as UserStatusType,
                        })
                    }
                />
            )}

            {submitError && (
                <p className="mb-4 text-sm text-red-600">{submitError}</p>
            )}

            <FormActions
                submitLabel={mode === "create" ? "Create user" : "Save"}
                onClear={resetForm}
                onCancel={onCancel}
            />
        </form>
    );
}

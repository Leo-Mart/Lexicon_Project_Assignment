import { useEffect, useState } from "react";

export interface DrillItem<T = unknown> {
    id: string;
    label: string;
    raw: T;
}

export interface ResourceConfig<K extends string> {
    label: string;
    load: (parentId: string) => Promise<DrillItem[]>;
    child: K | null;
    editable: boolean;
    creatable: boolean;
    // Branch resources (root; activityBranch) aren't real entities - their
    // items are just picks between other resource keys, rendered as
    // labeled groups instead of cards.
    branch: boolean;
}

interface ResourceTreeProps<K extends string> {
    resources: Record<K, ResourceConfig<K>>;
    rootKey: K;
    onEdit: (resourceKey: K, item: DrillItem, refresh: () => void) => void;
    onCreate: (resourceKey: K, parentId: string, refresh: () => void) => void;
}

// Renders a tree of cards, each collapsed until clicked - opening one
// fetches and shows only its own children, not the whole tree at once.
// `resources` is the only thing that knows what a "course" or "student" is;
// this component just recurses through it.
export default function ResourceTree<K extends string>(props: ResourceTreeProps<K>) {
    return <ResourceList {...props} resourceKey={props.rootKey} parentId="" />;
}

interface ResourceListProps<K extends string> {
    resources: Record<K, ResourceConfig<K>>;
    resourceKey: K | null;
    parentId: string;
    onEdit: ResourceTreeProps<K>["onEdit"];
    onCreate: ResourceTreeProps<K>["onCreate"];
}

function ResourceList<K extends string>({
    resources,
    resourceKey,
    parentId,
    onEdit,
    onCreate,
}: ResourceListProps<K>) {
    const config = resourceKey ? resources[resourceKey] : null;
    const [items, setItems] = useState<DrillItem[] | null>(null);
    const [version, setVersion] = useState(0);
    const reload = () => setVersion((v) => v + 1);

    useEffect(() => {
        if (!config) {
            return;
        }
        let cancelled = false;
        config.load(parentId).then((result) => {
            if (!cancelled) {
                setItems(result);
            }
        });
        return () => {
            cancelled = true;
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [resourceKey, parentId, version]);

    if (!config) {
        return null;
    }

    if (items === null) {
        return <p className="text-sm text-text-dark/60">Loading...</p>;
    }

    if (config.branch) {
        return (
            <div className="flex flex-col gap-3">
                {items.map((item) => (
                    <div key={item.id}>
                        <p className="text-xs font-semibold uppercase tracking-wide text-buttons mb-1">
                            {item.label}
                        </p>
                        <ResourceList
                            resources={resources}
                            resourceKey={item.id as K}
                            parentId={parentId}
                            onEdit={onEdit}
                            onCreate={onCreate}
                        />
                    </div>
                ))}
            </div>
        );
    }

    return (
        <div className="flex flex-col gap-2">
            {config.creatable && (
                <button
                    className="text-left text-sm px-3 py-1.5 rounded border border-dashed hover:bg-bg-light hover:cursor-pointer"
                    onClick={() => onCreate(resourceKey as K, parentId, reload)}
                >
                    + New
                </button>
            )}
            {items.length === 0 && <p className="text-sm text-text-dark/60">Nothing here.</p>}
            {items.map((item) => (
                <EntityCard
                    key={item.id}
                    resources={resources}
                    resourceKey={resourceKey as K}
                    item={item}
                    onEdit={onEdit}
                    onCreate={onCreate}
                    reload={reload}
                />
            ))}
        </div>
    );
}

function EntityCard<K extends string>({
    resources,
    resourceKey,
    item,
    onEdit,
    onCreate,
    reload,
}: {
    resources: Record<K, ResourceConfig<K>>;
    resourceKey: K;
    item: DrillItem;
    onEdit: ResourceTreeProps<K>["onEdit"];
    onCreate: ResourceTreeProps<K>["onCreate"];
    reload: () => void;
}) {
    const config = resources[resourceKey];
    // Collapsed by default - its children aren't fetched until opened.
    const [expanded, setExpanded] = useState(false);

    // Leaf: nothing nests under it, so it's just a row you can edit.
    if (!config.child) {
        return (
            <div className="flex items-center justify-between px-3 py-2 rounded bg-bg-light">
                <span className="text-sm">{item.label}</span>
                {config.editable && (
                    <button
                        className="text-xs px-2 py-1 rounded hover:bg-white hover:cursor-pointer"
                        onClick={() => onEdit(resourceKey, item, reload)}
                    >
                        Edit
                    </button>
                )}
            </div>
        );
    }

    return (
        <div className="rounded-md border p-3">
            <div className="flex items-center justify-between gap-2">
                <button
                    className="flex items-center gap-2 text-left hover:cursor-pointer"
                    onClick={() => setExpanded(!expanded)}
                >
                    <span className="text-xs w-3">{expanded ? "▾" : "▸"}</span>
                    <span className="text-sm font-semibold">{item.label}</span>
                </button>
                {config.editable && (
                    <button
                        className="text-xs px-2 py-1 rounded hover:bg-bg-light hover:cursor-pointer"
                        onClick={() => onEdit(resourceKey, item, reload)}
                    >
                        Edit
                    </button>
                )}
            </div>
            {expanded && (
                <div className="mt-2 pl-3 border-l-2">
                    <p className="text-xs font-semibold uppercase tracking-wide text-buttons mb-1">
                        {resources[config.child].label}
                    </p>
                    <ResourceList
                        resources={resources}
                        resourceKey={config.child}
                        parentId={item.id}
                        onEdit={onEdit}
                        onCreate={onCreate}
                    />
                </div>
            )}
        </div>
    );
}

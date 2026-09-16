import type { ResourceResponse } from "../interfaces/resource/ResourceResponse";

type ResourceListItemProps = {
    item: ResourceResponse;
    index: number;
};

const ResourceListItem = (props: ResourceListItemProps) => {
    return (
        <tr
            key={props.item.resourceId}
            className={
                props.index % 2 === 0
                    ? "bg-white dark:bg-bg-window-dark"
                    : "bg-bg dark:bg-bg-dark"
            }
        >
            <td className="p-3">{props.item.name}</td>
            <td className="p-3">{props.item.description}</td>
            <td className="p-3">{props.item.content}</td>
            <td className="p-3">{props.item.uri}</td>
            <td className="p-3">{props.item.createdAt}</td>
        </tr>
    );
};

export default ResourceListItem;

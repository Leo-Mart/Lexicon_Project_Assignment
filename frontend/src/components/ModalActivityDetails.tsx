import { ActivityDate, ActivityTime } from "../constants/ActivityTimeConverter";
import type { ActivityResponse } from "../interfaces/activity/ActivityResponse";
import ModalWrapper from "./ModalWrapper";

interface ModalActivityDetailsProps {
    open: boolean;
    onClose: () => void;
    activity: ActivityResponse;
}

const ModalActivityDetails = (props: ModalActivityDetailsProps) => {
    return (
        <ModalWrapper
            open={props.open}
            onClose={props.onClose}
            title="Activity Details"
        >
            <div className="bg-bg py-3 px-3">
                <div>
                    <h2>{props.activity.name}</h2>
                    <time className="text-sm text-text-dark  p-3 pt-0">
                        {ActivityDate(props.activity.startAt)}
                        {" | "}
                        {ActivityTime(props.activity.startAt)}-
                        {ActivityTime(props.activity.endAt)}
                    </time>
                </div>

                <div>
                    <h2>Resources</h2>
                </div>
            </div>
        </ModalWrapper>
    );
};

export default ModalActivityDetails;

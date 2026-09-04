export const ActivityDate = (dateString: string): string => {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = date.getMonth();
    const day = date.getDate();
    return `${day}/${month}/${year}`;
};

export const ActivityTime = (dateString: string): string => {
    const date = new Date(dateString);
    const hour = String(date.getHours()).padStart(2, "0");
    const minute = String(date.getMinutes()).padStart(2, "0");
    return `${hour}:${minute}`;
};

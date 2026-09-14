export class BadRequestError extends Error {
    readonly errors: string[] | undefined;
    constructor(message: string, errors: string[] | undefined) {
        super(message);
        this.name = "BadRequestError";
        this.errors = errors;

        Object.setPrototypeOf(this, BadRequestError.prototype);
    }
}

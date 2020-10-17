export type Dictionary<T> = { [key: string]: T };

export type Errors = { [key: string]: Array<string> };

export interface Response<T> {
    succeeded: boolean
    message: string
    errors?: Errors
    data: T
}
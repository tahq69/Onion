import {Dispatch, SetStateAction} from 'react';
import Axios, {AxiosResponse} from 'axios';
import {Errors, Response} from "../types";


const instance = Axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    timeout: 10000,
});

export type SetErrorState = Dispatch<SetStateAction<Errors | undefined | null>>;
export const handleError = (setErrors: SetErrorState) => (error: any) => {
    const body: Response<any> = error.response.data;
    setErrors(body.errors);
}

export type SetMessage = Dispatch<SetStateAction<string | null>>;
export type CheckSucceeded<T extends Response<U>, U> = (value: AxiosResponse<T>) => U | PromiseLike<U> | undefined;
export function checkSucceeded<T extends Response<U>, U = any>(setGlobalMessage: SetMessage): CheckSucceeded<T, U> {
    return (value: AxiosResponse<T>): U | PromiseLike<U> => {
        if (!value.data.succeeded) {
            setGlobalMessage(value.data.message);
        }

        return value.data.data
    }
}


export default instance;
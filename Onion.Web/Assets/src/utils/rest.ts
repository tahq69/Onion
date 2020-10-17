import {Dispatch, SetStateAction} from 'react';
import Axios, {AxiosInstance, AxiosResponse} from 'axios';
import {Dictionary, Errors, Response} from '../types';

const rest = Axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    timeout: 10000
});

export const setDefaultHeaders = (headers: Dictionary<string>) => {
    for (const key in Object.keys(headers)) {
        rest.defaults.headers.common[key] = headers[key];
    }
};

export const addResponseErrorHandler = (cb: (rest: AxiosInstance, error: any) => any) => {
    rest.interceptors.response.use(r => r, (error) => cb(rest, error));
}

export type SetErrorState = Dispatch<SetStateAction<Errors | undefined | null>>;
export const handleError = (setErrors: SetErrorState) => (error: any) => {
    if (!error.response) {
        console.error(error)
        return null;
    }

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


export default rest;
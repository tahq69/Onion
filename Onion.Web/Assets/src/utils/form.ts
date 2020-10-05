import {useState} from 'react';

import {Errors, Response} from '../types';
import {CheckSucceeded, checkSucceeded, handleError} from './rest';

export type ValidateStatus = (field: string) => "error" | "";
export const validateStatus = (errors: Errors | undefined | null): ValidateStatus => (field: string) =>
    errors && Object.keys(errors).map(k => k.toLowerCase()).indexOf(field.toLowerCase()) > -1 ? "error" : "";

export type ValidateMessage = (field: string) => string | undefined;
export const validateMessage = (errors: Errors | undefined | null) => (field: string) =>
    errors && errors.hasOwnProperty(field) ? errors[field].join(".") : undefined;

type OnReject = (reason: any) => (void | PromiseLike<void>) | null | undefined;
type Clear = (field: string | undefined) => void;
export const useErrors = (initialState: Errors | undefined | null = null)
    : [ValidateStatus, ValidateMessage, OnReject, Clear] => {
    const [errors, setErrors] = useState<Errors | undefined | null>(initialState);
    const status = validateStatus(errors);
    const message = validateMessage(errors);
    const errorHandler = handleError(setErrors);
    const clear = (field: string | undefined) => {
        if (field) {
            if (!errors || !errors.hasOwnProperty(field)) return;

            const clone = JSON.parse(JSON.stringify(errors));
            if (clone.hasOwnProperty(field)) {
                delete clone[field];
                setErrors(clone);
            }
        } else {
            setErrors(null);
        }
    }

    return [status, message, errorHandler, clear];
}

export const useResponseMessage = <T extends Response<U>, U = any>(): [string | null, CheckSucceeded<T, U>] => {
    const [message, setMessage] = useState<string | null>(null);
    const check = checkSucceeded<T, U>(setMessage);

    return [message, check];
}
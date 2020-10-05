import {Errors} from '../types';

export const validateStatus = (errors: Errors | undefined | null) => (field: string) =>
    errors && Object.keys(errors).map(k => k.toLowerCase()).indexOf(field.toLowerCase()) > -1 ? "error" : "";

export const validateMessage = (errors: Errors | undefined | null) => (field: string) =>
    errors && errors.hasOwnProperty(field) ? errors[field].join(".") : undefined;
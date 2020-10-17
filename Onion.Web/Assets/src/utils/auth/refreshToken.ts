export const refreshToken = (): Promise<string> => {
    // TODO: Call refresh token endpoint and get net token value.
    //       If refresh token call fails, we should logout user: logout();

    console.log('refresh token reject');

    return Promise.reject();
}

export default refreshToken;
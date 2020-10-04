import Axios from 'axios';

const instance = Axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    timeout: 10000,
});

export default instance;
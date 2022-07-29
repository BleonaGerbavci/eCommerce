import axios, { AxiosError, AxiosResponse } from "axios";

axios.defaults.baseURL = 'https://localhost:7196/api/';
const sleep = () => new Promise (resolve => setTimeout(resolve, 400))

const responseBody =(response: AxiosResponse) => response.data;

axios.interceptors.response.use( async response =>{
    await sleep();
    return response;
}, (error: AxiosError) => {
    console.log("caught by interceptor");
    return Promise.reject(error.response);
})

const requests ={
    get: (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody),
}

const Catalog = {
    list: () => requests.get('Product'),
    details: (id: number) => requests.get(`Product/${id}`)
}

const TestErrors ={
    get400Error: () => requests.get('Buggy/bad-request'),
    get401Error: () => requests.get('Buggy/unauthorised'),
    get404Error: () => requests.get('Buggy/not-found'),
    get500Error: () => requests.get('Buggy/server-error'),
    getValidationError: ()=> requests.get('Buggy/validation-error'),
}

const agent = {
    Catalog,
    TestErrors
}

export default agent;
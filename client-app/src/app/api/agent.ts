import axios, { AxiosResponse } from "axios";
import { IActivity, IActivitiesEnvelope } from "../models/activity";
import { IUserFormValues, IUser } from "../models/user";
import { history } from "../..";
import { toast } from "react-toastify";
import { IProfile, IPhoto } from "../models/profile";

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

axios.interceptors.request.use(
  config => {
    const token = window.localStorage.getItem("jwt");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

axios.interceptors.response.use(undefined, error => {
  const { status, data, config, headers } = error.response;

  if (error.message === "Network Error" && !error.response) {
    toast.error("Network Error - Make sure api is running!");
  }
  if(status === 401 && headers['www-authenticate'] === 'Bearer error="invalid_token", error_description="The token is expired"' ){
    window.localStorage.removeItem('jwt');
    history.push("/");
    toast.info("Your session has experied please login again!")
  }
  if (
    status === 404 ||
    (status === 400 &&
      config.method === "get" &&
      data.errors.hasOwnProperty("id"))
  ) {
    history.push("/notFound");
  }
  if (status === 500) {
    toast.error("Server error - check the terminal for more info!");
  }
  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

const request = {
  get: (url: string) =>
    axios
      .get(url)
      .then(responseBody),
  post: (url: string, body: {}) =>
    axios
      .post(url, body)
      .then(responseBody),
  put: (url: string, body: {}) =>
    axios
      .put(url, body)
      .then(responseBody),
  del: (url: string) =>
    axios
      .delete(url)
      .then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append("File", file);
    return axios
      .post(url, formData, {
        headers: { "Content-type": "multipart/form-data" }
      })
      .then(responseBody);
  }
};

const Activities = {
  list: (params: URLSearchParams): Promise<IActivitiesEnvelope> =>
    axios.get('/activities',{params: params}).then(responseBody),
  detail: (id: string) => request.get(`/activities/${id}`),
  create: (activity: IActivity) => request.post("/activities", activity),
  update: (activity: IActivity) =>
    request.put(`/activities/${activity.id}`, activity),
  delete: (id: string) => request.del(`/activities/${id}`),
  attend: (id: string) => request.post(`/activities/${id}/attend`, {}),
  unattend: (id: string) => request.del(`/activities/${id}/attend`)
};

const User = {
  current: (): Promise<IUser> => request.get(`/user`),
  login: (user: IUserFormValues): Promise<IUser> =>
    request.post("/user/login/", user),
  register: (user: IUserFormValues): Promise<IUser> =>
    request.post("/user/register", user)
};

const Profiles = {
  get: (userName: string): Promise<IProfile> =>
    request.get(`/profiles/${userName}`),
  uploadPhoto: (photo: Blob): Promise<IPhoto> =>
    request.postForm("/photos", photo),
  setMainPhoto: (id: string) => request.post(`/Photos/${id}/setmain`, {}),
  deletePhoto: (id: string) => request.del(`/Photos/${id}`),
  updateProfile: (profile: Partial<IProfile>) =>
    request.put(`/profiles`, profile),
  follow: (userName: string) =>
    request.post(`/profiles/${userName}/follow`, {}),
  unfollow: (userName: string) => request.del(`/profiles/${userName}/follow`),
  listFollowings: (userName: string, predicate: string) =>
    request.get(`/profiles/${userName}/follow?predicate=${predicate}`),
  listActivities: (userName:string, predicate: string) => 
    request.get(`/profiles/${userName}/activities?predicate=${predicate}`)
  };

export default {
  Activities,
  User,
  Profiles
};

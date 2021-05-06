import {Project} from '@/models/Project';
import axios from 'axios';
import {User} from '@/models/User';
import {ProfileResponse} from '@/models/ProfileResponse';
import {ChangePasswordData} from '@/models/ChangePasswordData';
import {RegistrationData} from '@/models/RegistrationData';
import {AuthenticationData} from '@/models/AuthenticationData';
import {LoginSuccess} from '@/models/LoginSuccess';
import {GenerateRequest} from '@/models/GenerateRequest';
import {GenerateResponse} from '@/models/GenerateResponse';

const dateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/;

function reviver(key: string, value: string) {
  if (dateFormat.test(value)) {
    return new Date(value);
  }
  return value;
}

function transformResponse(response: string) {
  if (response && response.trim()) {
    return JSON.parse(response, reviver);
  }
  return null;
}

function config(jwt: string) {
  return {headers: {'Authorization': 'Bearer ' + jwt}, transformResponse: transformResponse };
}

function userToDto(user: User): User {
  return {
    id: user.id,
    userName: user.userName,
    email: user.email,
    newsletter: user.newsletter,
  };
}

function projectToDto(project: Project): Project {
  return {
    id: project.id,
    name: project.name,
    json: project.json,
    ownerId: project.ownerId,
    backend: project.backend,
    frontend: project.frontend
  };
}

const api = {
  login: async function (data: AuthenticationData): Promise<LoginSuccess> {
    const response = await axios.post('authentication/login', data);
    return response.data;
  },
  register: async function (data: RegistrationData): Promise<ProfileResponse> {
    const response = await axios.post('registration/register', data);
    return response.data;
  },
  activate: async function (activationToken: string): Promise<boolean> {
    const response = await axios.post(`registration/activate?activationToken=${activationToken}`);
    return response.data;
  },
  profile: async function (jwt: string): Promise<User> {
    const response = await axios.get('profile', config(jwt));
    return response.data;
  },
  updateProfile: async function (user: User, jwt: string): Promise<ProfileResponse> {
    const response = await axios.post('profile', userToDto(user), config(jwt));
    return response.data;
  },
  changePassword: async function (data: ChangePasswordData, jwt: string): Promise<void> {
    const response = await axios.post('profile/change-password', data, config(jwt));
    return response.data;
  },
  getProjects: async function(jwt: string): Promise<Array<Project>> {
    const response = await axios.get('projects', config(jwt));
    return response.data;
  },
  getProject: async function(id: number, jwt: string): Promise<Project> {
    const response = await axios.get(`projects/${id}`, config(jwt));
    return response.data;
  },
  addProject: async function(project: Project, jwt: string): Promise<Project> {
    const response = await axios.post('projects', projectToDto(project), config(jwt));
    return response.data;
  },
  updateProject: async function(project: Project, jwt: string): Promise<Project> {
    const response = await axios.put(`projects/${project.id}`, projectToDto(project), config(jwt));
    return response.data;
  },
  deleteProject: async function(project: Project, jwt: string): Promise<void> {
    const response = await axios.delete(`projects/${project.id}`, config(jwt));
    return response.data;
  },
  generate: async function(request: GenerateRequest): Promise<GenerateResponse> {
    const response = await axios.post('generate/generate', request);
    return response.data;
  },
  download: async function(request: GenerateRequest): Promise<void> {
    const response = await axios.post('generate/download', request, {responseType: 'blob'});
    const fileURL = window.URL.createObjectURL(new Blob([response.data]));
    const fileLink = document.createElement('a');
    fileLink.href = fileURL;
    fileLink.target = '_blank';
    fileLink.setAttribute('download', `${request.nameSpace}.zip`);
    document.body.appendChild(fileLink);
    fileLink.click();
  }
}

export default api;

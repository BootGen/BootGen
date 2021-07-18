import {LoginSuccess} from "@/models/LoginSuccess";

export interface OAuthLoginSuccess extends LoginSuccess{
  accessToken: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  email: string;
  userId: string;
  roles: string[];
}
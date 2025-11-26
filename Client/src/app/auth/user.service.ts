import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = `${environment.apiUrl}/profile`;

  constructor(private http: HttpClient) {}

  getProfile() {
    return this.http.get(this.apiUrl);
  }

  updateProfile(data: any) {
    return this.http.put(this.apiUrl, data);
  }
}

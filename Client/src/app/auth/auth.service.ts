import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Login } from '../login/login';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth/login`;
  private tokenKey = 'auth_token';
  private rolesKey = 'auth_roles';
  public isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {
    const token = localStorage.getItem(this.tokenKey);
    this.isLoggedIn$.next(!!token);
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<{ token: string, roles?: string[] }>(
      this.apiUrl, credentials).pipe(
      tap((response) => {
        console.log(response);
        if (response && response.token) {
          localStorage.setItem(this.tokenKey, response.token);
          if(response.roles) {
            localStorage.setItem(this.rolesKey, JSON.stringify(response.roles))
          }
          this.isLoggedIn$.next(true);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.rolesKey);
    this.isLoggedIn$.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('auth_token');
  }

   getRoles(): string[] {
    const rolesJson = localStorage.getItem(this.rolesKey);
    return rolesJson ? JSON.parse(rolesJson) : [];
  }

  isAdmin(): boolean {
    return this.getRoles().includes('Admin');
  }

  isClient(): boolean {
    return this.getRoles().includes('Client');
  }
}

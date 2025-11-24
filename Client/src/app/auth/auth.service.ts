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
  public isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {
    const token = localStorage.getItem(this.tokenKey);
    this.isLoggedIn$.next(!!token);
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<{ token: string }>(this.apiUrl, credentials).pipe(
      tap((response) => {
        //console.log(response);
        if (response && response.token) {
          localStorage.setItem(this.tokenKey, response.token);
          this.isLoggedIn$.next(true);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn$.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}

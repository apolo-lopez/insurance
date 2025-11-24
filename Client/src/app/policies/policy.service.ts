import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
import { Policy } from "./policy.model";
import { environment } from "../../environments/environment";

@Injectable({
    providedIn: 'root',
})
export class PolicyService {
    private apiUrl = `${environment.apiUrl}/Policies`;

    constructor(private http: HttpClient) {}

    getAll(): Observable<Policy[]> {
        return this.http.get<Policy[]>(this.apiUrl);
    }

    getById(id: string): Observable<Policy> {
        return this.http.get<Policy>(`${this.apiUrl}/${id}`);
    }

    create(policy: any): Observable<any> {
        return this.http.post<Policy>(this.apiUrl, policy);
    }

    update(id: string, policy: any): Observable<any> {
        return this.http.put<Policy>(`${this.apiUrl}/${id}`, policy);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    search(term: string): Observable<Policy[]> {
        return this.http.get<Policy[]>(`${this.apiUrl}/search?term=${term}`);
    }
}
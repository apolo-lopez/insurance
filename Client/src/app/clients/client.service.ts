import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Client } from "./client.model";

@Injectable({
    providedIn: 'root'
})
export class ClientService {
    private apiUrl = 'http://localhost:5069/api/Client';

    constructor(private http: HttpClient) {}

    getAll(): Observable<Client[]> {
        
        return this.http.get<Client[]>(this.apiUrl);
    }

    getById(id: string): Observable<Client> {
        return this.http.get<Client>(`${this.apiUrl}/${id}`);
    }

    create(client: Client): Observable<Client> {
        return this.http.post<Client>(this.apiUrl, client);
    }

    update(id: string, client: Client): Observable<Client> {
        return this.http.put<Client>(`${this.apiUrl}/${id}`, client);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    search(term: string): Observable<Client[]> {
        return this.http.get<Client[]>(`${this.apiUrl}?search=${term}`);
    }
}
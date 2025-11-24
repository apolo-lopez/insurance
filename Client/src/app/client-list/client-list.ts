import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Client } from '../clients/client.model';
import { ClientService } from '../clients/client.service';
import { ClientFormComponent } from '../clients/client-form/client-form.component';
import { JsonPipe, NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-client-list',
  imports: [NgIf, NgForOf, ClientFormComponent],
  templateUrl: './client-list.html',
  styleUrl: './client-list.scss',
})
export class ClientList implements OnInit {
  clients: Client[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;

  showForm: boolean = false;
  selectedClient?: Client;

  constructor(private clientService: ClientService, private cd: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.fetchClients();
  }

  fetchClients(): void {
    this.isLoading = true;
    this.clientService.getAll().subscribe({
      next: (data) => {
        this.clients = Array.isArray(data) ? data : [data];
        this.isLoading = false;
        //console.log('Lo que asigno a clients:', this.clients);
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching clients', err);
        this.isLoading = false;
        this.clients = [];
        this.cd.detectChanges();
      },
    });
  }

  searchClients(): void {
    if (!this.searchTerm.trim()) {
      this.fetchClients();
      return;
    }

    this.isLoading = true;
    this.clientService.search(this.searchTerm).subscribe({
      next: (data) => {
        this.clients = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error searching clients', err);
        this.isLoading = false;
      },
    });
  }

  openForm(client?: Client): void {
    //console.log(client);
    this.selectedClient = client;
    this.showForm = true;
  }

  onSaved(client: Client): void {
    this.showForm = false;
    this.selectedClient = undefined;
    this.fetchClients();
  }

  closeClientForm(): void {
    this.selectedClient = undefined;
    this.showForm = false;
  }
}

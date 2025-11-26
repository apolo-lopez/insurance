import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Client } from '../clients/client.model';
import { ClientService } from '../clients/client.service';
import { ClientFormComponent } from '../clients/client-form/client-form.component';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { NgIf, NgFor } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [
    NgIf,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    FormsModule,
  ],
  templateUrl: './client-list.html',
  styleUrl: './client-list.scss',
})
export class ClientList implements OnInit {
  clients: Client[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;
  searchName: string = '';
  searchEmail: string = '';
  searchIdentification: string = '';
  searchPhone: string = '';
  filtersOpen: boolean = false;

  displayedColumns: string[] = [
    'identificationNumber',
    'name',
    'email',
    'phoneNumber',
    'address',
    'actions',
  ];

  showForm: boolean = false;
  selectedClient?: Client;

  constructor(
    private clientService: ClientService,
    private cd: ChangeDetectorRef,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.fetchClients();
  }

  fetchClients(): void {
    this.isLoading = true;
    this.clientService.getAll().subscribe({
      next: (data) => {
        this.clients = Array.isArray(data) ? data : [data];
        this.isLoading = false;
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
    this.isLoading = true;

    this.clientService
      .search(
        this.searchName.trim() || undefined,
        this.searchEmail.trim() || undefined,
        this.searchIdentification.trim() || undefined,
        this.searchPhone.trim() || undefined
      )
      .subscribe({
        next: (data) => {
          this.clients = data;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          this.cd.detectChanges();
        },
      });
  }

  clearFilters(): void {
    this.searchName = '';
    this.searchEmail = '';
    this.searchIdentification = '';
    this.searchPhone = '';
    this.fetchClients(); // vuelve a cargar todos
  }

  onSaved(): void {
    this.showForm = false;
    this.selectedClient = undefined;
    this.fetchClients();
  }

  deleteClient(clientId: string): void {
    this.clientService.delete(clientId).subscribe({
      next: () => {
        console.log('Client deleted successfully');
        this.fetchClients(); // refresca la lista
      },
      error: (err) => {
        console.error('Error deleting client', err);
      },
    });
  }

  openCreate(): void {
    const dialogRef = this.dialog.open(ClientFormComponent, {
      width: '600px',
      data: null,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.fetchClients();
    });
  }

  openEdit(client: Client): void {
    const dialogRef = this.dialog.open(ClientFormComponent, {
      width: '600px',
      data: client,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.fetchClients();
    });
  }
}

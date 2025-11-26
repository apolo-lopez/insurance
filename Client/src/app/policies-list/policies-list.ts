import { Component, OnInit, ViewChild } from '@angular/core';
import { Policy } from '../policies/policy.model';
import { PolicyService } from '../policies/policy.service';

import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { PolicyFormComponent } from '../policies/policy-form/policy-form.component';
import { DatePipe, CurrencyPipe, NgIf, NgFor } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { Client } from '../clients/client.model';
import { ClientService } from '../clients/client.service';
import { MatOption, MatSelect, MatSelectModule } from '@angular/material/select';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-policies-list',
  standalone: true,
  templateUrl: './policies-list.html',
  styleUrls: ['./policies-list.scss'],
  imports: [
    NgIf,
    NgFor,
    DatePipe,
    CurrencyPipe,
    // Angular Material
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatTableModule,
    MatPaginatorModule,
    MatCardModule,
    MatOption,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
  ],
})
export class PoliciesList implements OnInit {
  displayedColumns: string[] = ['policyNumber', 'startDate', 'endDate', 'insuredAmount', 'actions'];

  dataSource = new MatTableDataSource<Policy>([]);
  isLoading = false;
  filtersOpen = false;

  clients: Client[] = [];

  filterClientId: string | null = null;
  filterType: string | null = null;
  filterStatus: string | null = null;
  filterFrom: string | null = null;
  filterTo: string | null = null;
  filterPolicyNumber: string = '';

  page = 1;
  pageSize = 20;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private policyService: PolicyService,
    private clientService: ClientService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.fetchClients();
    this.fetchPolicies();
  }

  fetchClients(): void {
    if (this.authService.isAdmin()) {
      this.clientService.getAll().subscribe({
        next: (c) => (this.clients = c),
        error: (err) => console.error('Error loading clients', err),
      });
    }
  }

  applyFilters(): void {
    this.isLoading = true;

    // Creamos el objeto de filtros sin nulls
    const params: any = {};

    if (this.filterClientId) params.clientId = this.filterClientId;
    if (this.filterType) params.type = this.filterType;
    if (this.filterStatus) params.status = this.filterStatus;
    if (this.filterFrom) params.from = this.filterFrom;
    if (this.filterTo) params.to = this.filterTo;
    if (this.filterPolicyNumber) params.policyNumber = this.filterPolicyNumber;

    // paginación
    params.page = this.page;
    params.pageSize = this.pageSize;

    this.policyService.search(params).subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.isLoading = false;

        if (this.paginator) {
          this.dataSource.paginator = this.paginator;
        }
      },
      error: (err) => {
        console.error('Error searching policies', err);
        this.isLoading = false;
      },
    });
  }

  fetchPolicies(): void {
    this.isLoading = true;
    if (this.authService.isAdmin()) {
      this.policyService.getAll().subscribe({
        next: (policies) => {
          this.dataSource.data = policies;
          this.isLoading = false;

          if (this.paginator) {
            this.dataSource.paginator = this.paginator;
          }
        },
        error: (err) => {
          console.error('Error loading policies', err);
          this.isLoading = false;
        },
      });
    } else {
      this.policyService.getMine().subscribe({
        next: (policies) => {
          this.dataSource.data = policies;
          this.isLoading = false;

          if (this.paginator) {
            this.dataSource.paginator = this.paginator;
          }
        },
        error: (err) => {
          console.error('Error loading policies', err);
          this.isLoading = false;
        },
      })
    }
  }

  openForm(policy?: Policy): void {
    const dialogRef = this.dialog.open(PolicyFormComponent, {
      width: '600px',
      data: { policy: policy ?? null, isAdmin: this.authService.isAdmin() },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchPolicies(); // refresh list
      }
    });
  }

  deletePolicy(policyId: string): void {
    this.policyService.delete(policyId).subscribe({
      next: () => {
        console.log('Client deleted successfully');
        this.fetchPolicies(); // refresca la lista
      },
      error: (err) => {
        console.error('Error deleting client', err);
      },
    });
  }

  clearFilters(): void {
    this.filterClientId = null;
    this.filterType = null;
    this.filterStatus = null;
    this.filterFrom = null;
    this.filterTo = null;
    this.filterPolicyNumber = '';

    this.fetchPolicies();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }
}

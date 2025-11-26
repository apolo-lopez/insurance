import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Policy } from '../policy.model';
import { PolicyService } from '../policy.service';

import { MatDialogActions, MatDialogContent, MatDialogTitle } from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { NgFor, NgIf, NgSwitch } from '@angular/common';
import { ClientService } from '../../clients/client.service';
import { Client } from '../../clients/client.model';

export interface PolicyDialogData {
  policy: Policy | null;
  isAdmin: boolean;
}

@Component({
  selector: 'app-policy-form',
  standalone: true,
  templateUrl: './policy-form.component.html',
  styleUrl: './policy-form.component.css',
  imports: [
    ReactiveFormsModule,
    NgFor,
    MatDialogModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    NgIf,
],
})
export class PolicyFormComponent implements OnInit {
  policyForm!: FormGroup;
  isSaving = false;

  types = [
    { value: 1, label: 'Life' },
    { value: 2, label: 'Health' },
    { value: 3, label: 'Auto' },
    { value: 4, label: 'Home' },
  ];

  status = [
    { value: 1, label: 'Active' },
    { value: 2, label: 'Inactive' },
    { value: 3, label: 'Pending' },
    { value: 4, label: 'Cancelled' },
  ];

  clients: Client[] = [];
  selectedClientId: string = '';

  constructor(
    private fb: FormBuilder,
    private policyService: PolicyService,
    private clientService: ClientService,
    private dialogRef: MatDialogRef<PolicyFormComponent>,
    @Inject(MAT_DIALOG_DATA) public policyData: PolicyDialogData
  ) {}

  ngOnInit(): void {
    if (this.policyData.isAdmin) this.loadClients();

    const isAdmin = this.policyData.isAdmin;

    this.policyForm = this.fb.group({
      clientId: ['', Validators.required],
      policyNumber: [{value: '', disabled: !isAdmin}, Validators.required],
      policyType: [{value: '', disabled: !isAdmin}, Validators.required],
      startDate: [{value: '', disabled: !isAdmin}, Validators.required],
      endDate: [{value: '', disabled: !isAdmin}],
      policyStatus: [{value: '', disabled: !isAdmin}, Validators.required],
      insuredAmount: [{value: '', disabled: !isAdmin}, [Validators.required, Validators.min(0)]],
    });

    if (this.policyData.policy) {
      this.policyForm.patchValue({
        clientId: this.policyData.policy.clientId,
        policyNumber: this.policyData.policy.policyNumber,
        policyType: Number(this.policyData.policy.policyType),
        startDate: this.formatDate(this.policyData.policy.startDate),
        endDate: this.formatDate(this.policyData.policy.endDate),
        policyStatus: Number(this.policyData.policy.policyStatus),
        insuredAmount: this.policyData.policy.insuredAmount,
      });
    }
  }

  loadClients(): void {
    this.clientService.getAll().subscribe({
      next: (data) => {
        this.clients = data;
      },
      error: (err) => {
        console.error('Error loading clients', err);
      },
    });
  }

  formatDate(date: any): string {
    if (!date) return '';
    return new Date(date).toISOString().substring(0, 10);
  }

  submit(): void {
    if (this.policyForm.invalid) return;

    this.isSaving = true;

    const values = this.policyForm.value;

    const policyRequest = {
      Id: this.policyData.policy?.id ?? '',
      ClientId: values.clientId,
      PolicyNumber: values.policyNumber,
      Type: Number(values.policyType),
      Status: Number(values.policyStatus),
      StartDate: values.startDate,
      EndDate: values.endDate,
      InsuredAmount: Number(values.insuredAmount),
      CreatedAt: this.policyData.policy?.createdAt ?? new Date().toISOString(),
    };

    const request = !this.policyData.policy
      ? this.policyService.create(policyRequest)
      : this.policyService.update(this.policyData.policy.id, policyRequest);

    request.subscribe({
      next: (result) => {
        this.isSaving = false;
        this.dialogRef.close(result);
      },
      error: () => (this.isSaving = false),
    });
  }

  close(): void {
    this.dialogRef.close(null);
  }
}

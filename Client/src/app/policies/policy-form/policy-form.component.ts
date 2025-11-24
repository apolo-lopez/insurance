import { Component, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { PolicyService } from '../policy.service';
import { Policy } from '../policy.model';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-policy-form',
  standalone: true,
  templateUrl: './policy-form.component.html',
  imports: [ReactiveFormsModule, NgFor],
})
export class PolicyFormComponent implements OnChanges {
  @Input() policyData?: Policy;
  @Output() formSubmit = new EventEmitter<Policy>();
  @Output() closeForm = new EventEmitter<void>();

  policyForm: FormGroup;
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

  constructor(private fb: FormBuilder, private policyService: PolicyService) {
    this.policyForm = this.fb.group({
      policyNumber: ['', Validators.required],
      clientId: ['', Validators.required],
      policyType: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: [''],
      policyStatus: ['', Validators.required],
      insuredAmount: ['', [Validators.required, Validators.min(0)]],
    });
  }

  ngOnChanges(): void {
    if (this.policyData) {
        //console.log(this.policyData);
        
      this.policyForm.patchValue({
        clientId: this.policyData.clientId ?? '',
        policyNumber: this.policyData.policyNumber ?? '',
        policyType: this.policyData.policyType ?? '',
        startDate: this.formatDate(this.policyData.startDate), // format 'YYYY-MM-DD'
        endDate: this.formatDate(this.policyData.endDate),
        policyStatus: this.getStatusNumber(this.policyData.policyStatus ?? ''),
        insuredAmount: this.policyData.insuredAmount ?? '',
      });
    }
  }

  formatDate(date: any): string {
    if (!date) return '';
    // Admite formato Date y string "MM/DD/YYYY HH:mm:ss ..."
    if (typeof date === 'string') {
      const match = date.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})/);
      if (match) return `${match[3]}-${match[1].padStart(2, '0')}-${match[2].padStart(2, '0')}`;
      // Si ya es ISO, solo toma los 10 primeros caracteres
      if (date.length > 10) return date.substring(0, 10);
      return date;
    }
    // Si es objeto Date
    const d = new Date(date);
    return d.toISOString().substring(0, 10);
  }

  getStatusNumber(status: string): number | '' {
    if (status === 'Active') return 1;
    if (status === 'Cancelled') return 2;
    if (status === 'Pending') return 3;
    return '';
  }

  onSubmit(): void {
    if (this.policyForm.invalid) return;

    this.isSaving = true;
    const policyRequest = {
      PolicyNumber: this.policyForm.value.policyNumber,
      Type: Number(this.policyForm.value.policyType),
      Status: Number(this.policyForm.value.policyStatus),
      StartDate: this.policyForm.value.startDate,
      EndDate: this.policyForm.value.endDate,
      InsuredAmount: Number(this.policyForm.value.insuredAmount),
      ClientId: this.policyForm.value.clientId,
      // Si usas Id y CreatedAt al actualizar:
      Id: this.policyData?.id ?? '',
      CreatedAt: this.policyData?.createdAt || new Date().toISOString(),
    };

    if (!this.policyData) {
      this.policyService.create(policyRequest).subscribe({
        next: (result) => {
          this.formSubmit.emit(result);
          this.isSaving = false;
        },
        error: (err) => {
          console.error('Error creating policy', err);
          this.isSaving = false;
        },
      });
    } else {
      //console.log('policy type:', policyRequest);

      this.policyService.update(this.policyData.id, policyRequest).subscribe({
        next: (result) => {
          this.formSubmit.emit(result);
          this.isSaving = false;
        },
        error: (err) => {
          console.error('Error updating policy', err);
          this.isSaving = false;
        },
      });
    }
  }

  onClose(): void {
    this.closeForm.emit();
  }
}

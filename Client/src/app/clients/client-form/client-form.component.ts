import { Component, Input, Output, EventEmitter, Inject } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { ClientService } from '../client.service';
import { Client } from '../client.model';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-client-form',
  standalone: true,
  templateUrl: './client-form.component.html',
  styleUrl: './client-form.component.css',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
  ],
})
export class ClientFormComponent {
  clientForm: FormGroup;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService,
    private dialogRef: MatDialogRef<ClientFormComponent>,
    @Inject(MAT_DIALOG_DATA) public clientData: Client | null
  ) {
    this.clientForm = this.fb.group({
      identificationNumber: ['', [Validators.required, Validators.maxLength(10)]],
      name: ['', [Validators.required, Validators.maxLength(200)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(250)]],
      password: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required, Validators.maxLength(15)]],
      address: ['', [Validators.maxLength(500)]],
    });

    // 👉 AQUÍ cargamos los datos
    if (clientData) {
      this.clientForm.patchValue(clientData);
    }
  }

  submit(): void {
    if (this.clientForm.invalid) return;

    this.isSaving = true;

    const formValues = this.clientForm.value;
    const client: Client = {
      ...this.clientData,
      id: this.clientData?.id ?? '',
      ...formValues,
      createdAt: this.clientData?.createdAt ?? new Date().toISOString(),
    };

    const request = this.clientData
      ? this.clientService.update(client.id!, client)
      : this.clientService.create(client);

    request.subscribe({
      next: (result) => {
        setTimeout(() => {
          this.isSaving = false;
          this.dialogRef.close(result);
        });
      },
      error: (err) => {
        console.error('Error saving client', err);
        this.isSaving = false;
      },
    });
  }

  close(): void {
    this.dialogRef.close(null);
  }
}

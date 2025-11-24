import { Component, Input, Output, EventEmitter } from "@angular/core";
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { ClientService } from '../client.service';
import { Client, IdentificationNumber } from '../client.model';
import { NgIf } from "@angular/common";

@Component({
    selector: 'app-client-form',
    templateUrl: './client-form.component.html',
    imports: [NgIf, ReactiveFormsModule],
})
export class ClientFormComponent {
    @Input() clientData?: Client;
    @Output() formSubmit: EventEmitter<Client> = new EventEmitter<Client>();
    @Output() closeForm: EventEmitter<void> = new EventEmitter<void>();

    clientForm: FormGroup;
    isSaving: boolean = false;

    constructor(private fb: FormBuilder, private ClientService: ClientService) {
        this.clientForm = this.fb.group({
            identificationNumber: ['', [Validators.required, Validators.maxLength(10)]],
            name: ['', [Validators.required, Validators.maxLength(200)]],
            email: ['', [Validators.required, Validators.email, Validators.maxLength(250)]],
            phoneNumber: ['', [Validators.required, Validators.maxLength(15)]],
            address: ['', [Validators.maxLength(500)]],
        })
    }

    ngOnChanges(): void {
        if(this.clientData) {
            this.clientForm.patchValue({
                identificationNumber: this.clientData.identificationNumber,
                name: this.clientData.name,
                email: this.clientData.email,
                phoneNumber: this.clientData.phoneNumber,
                address: this.clientData.address,
            });
        }
    }

    onSubmit(): void {
        if(this.clientForm.invalid) return;

        this.isSaving = true;
        const formValues = this.clientForm.value;
        const client: Client = {
            ...this.clientData,
            id: this.clientData?.id ?? '',
            identificationNumber: formValues.identificationNumber,
            name: formValues.name,
            email: formValues.email,
            phoneNumber: formValues.phoneNumber,
            address: formValues.address,
            createdAt: this.clientData?.createdAt || new Date().toISOString()
        };

        if(!this.clientData) {
            this.ClientService.create(client).subscribe({
                next: (result) => {
                    this.formSubmit.emit(result);
                    this.isSaving = false;
                },
                error: (err) => {
                    console.error('Error creating client', err);
                    this.isSaving = false;
                }
            });
        } else {
            this.ClientService.update(this.clientData.id, client).subscribe({
                next: (result) => {
                    this.formSubmit.emit(result);
                    this.isSaving = false;
                },
                error: (err) => {
                    console.error('Error updating client', err);
                    this.isSaving = false;
                }
            });
        }
    }

    onClose(): void {
        this.closeForm.emit();
    }
}
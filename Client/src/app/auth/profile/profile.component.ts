import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../user.service';
import { AuthService } from '../auth.service';
import { MatFormFieldControl, MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
  imports: [
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule
  ]
})
export class ProfilePageComponent implements OnInit {
  profileForm: FormGroup;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService
  ) {
    this.profileForm = this.fb.group({
      phoneNumber: ['', Validators.required],
      address: ['']
    });
  }

  ngOnInit(): void {
    this.userService.getProfile().subscribe(profile => {
      this.profileForm.patchValue(profile);
    });
  }

  submit(): void {
    if (this.profileForm.invalid) return;
    this.isSaving = true;
    this.userService.updateProfile(this.profileForm.value).subscribe({
      next: () => {
        this.isSaving = false;
      },
      error: () => this.isSaving = false
    });
  }
}

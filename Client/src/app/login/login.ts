import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-login',
  imports: [
    NgIf,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule,
    MatIcon,
    MatProgressSpinnerModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  email: string = '';
  password: string = '';
  error: string = '';

  form: FormGroup;
  loading = false;
  hidePassword = true;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router) {
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onLogin(event: Event): void {
    if (event) event.preventDefault();

    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { email, password } = this.form.value;

    this.authService.login({email: email, password: password})
      .subscribe({
        next: () => {
          this.loading = false;
          if(this.authService.isAdmin()) {
            this.router.navigate(['/clients']);
          } else {
            this.router.navigate(['/policies']);
          }
        },
        error: (err) => {
          console.log("Error", err);
          this.loading = false;
          this.errorMessage = 'No se pudo iniciar sesión. Intente nuevamente.';
        }
      })
  }
}

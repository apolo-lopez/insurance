import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [NgIf],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  email: string = '';
  password: string = '';
  error: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(event: Event): void {
    if (event) event.preventDefault();

    this.authService.login({email: this.email, password: this.password})
      .subscribe({
        next: () => {
          this.error = '';
          this.router.navigate(['/clients']);
        },
        error: (err) => {
          console.log("Error", err);
          this.error = 'Incorrect credentials, please check'
        }
      })
  }
}

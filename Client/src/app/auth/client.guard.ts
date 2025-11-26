import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ClientGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isClient()) {
      return true;
    } else {
      // Opcional: redirige a home/login si no es cliente
      this.router.navigate(['/']);
      return false;
    }
  }
}

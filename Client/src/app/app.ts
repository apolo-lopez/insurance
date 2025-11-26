import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from './auth/auth.service';
import { NgIf } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    NgIf,
    RouterModule,
    //NavbarComponent,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatListModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly title = signal('evaluacion_fronted');

  isMobile = false;

  constructor(private router: Router, private authService: AuthService, private cd: ChangeDetectorRef) {}

  ngOnInit() {
    this.checkMobile();
    window.addEventListener('resize', () => this.checkMobile());
    this.cd.detectChanges();
  }

  get isLoggedIn(): boolean {
    const logged = this.authService.isLoggedIn();
    return logged;
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  checkMobile(): void {
    this.isMobile = window.innerWidth < 768;
  }

  closeOnMobile(sidenav: any): void {
    if (this.isMobile) sidenav.close();
  }
}

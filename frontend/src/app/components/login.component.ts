import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [CommonModule, FormsModule]
})
export class LoginComponent {
  email = '';
  password = '';
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  login(): void {
    this.error = null;
    this.authService.login({ email: this.email, password: this.password })
      .subscribe({
        next: () => this.router.navigate(['/employees']),
        error: () => this.error = 'Invalid credentials'
      });
  }
}

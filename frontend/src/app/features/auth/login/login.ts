import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { ErrorService } from '../../../core/services/error';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);
  private errorService = inject(ErrorService);
  private fb = inject(FormBuilder);

  isSubmitting = false;

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.errorService.showSuccess('Login successful');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorService.showError('Login failed');
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      },
    });
  }
}

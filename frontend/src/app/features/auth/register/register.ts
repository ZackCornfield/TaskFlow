import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { Router, RouterLink } from '@angular/router';
import { ErrorService } from '../../../core/services/error';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);
  private errorService = inject(ErrorService);
  private fb = inject(FormBuilder);

  isSubmitting = false;

  registerForm: FormGroup = this.fb.group(
    {
      displayName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    {
      validators: this.passwordMatchValidator,
    }
  );

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (
      password &&
      confirmPassword &&
      password.value !== confirmPassword.value
    ) {
      return { passwordMismatch: true };
    }

    return null;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const { confirmPassword, ...registerData } = this.registerForm.value;

    this.authService.register(registerData).subscribe({
      next: () => {
        this.errorService.showSuccess('Registration successful');
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.errorService.showError('Registration failed');
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      },
    });
  }
}

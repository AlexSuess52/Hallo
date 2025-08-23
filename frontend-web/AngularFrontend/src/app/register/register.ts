import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { RegisterService } from '../services/register.service';
import {RefreshService} from '../services/refresh.service';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.html',
  styleUrl: './register.scss',
  imports: [ReactiveFormsModule, FormsModule]
})
export class Register implements OnInit {

  registerForm!: FormGroup;
  // flag to indicate form has been submitted (used for validation display)
  submitted = false;
  errorMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private registerService: RegisterService,
    private tokenService: TokenService,
    private router: Router,
    private refreshService: RefreshService,
  ) {}

  ngOnInit(): void {
    // initialize registration form with validation rules
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  get f() {
    return this.registerForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;

    // if form is invalid, show warning and exit
    if (this.registerForm.invalid) {
      console.warn('[Register] form is invalid. please complete all required fields.');
      return;
    }

    const { name, password } = this.registerForm.value;

    this.registerService.registerUser(name, password).subscribe({
      next: (response) => {
        console.log('[Register] registration successful:', response);

        this.tokenService.setAccessToken(response.accessToken);
        this.tokenService.setRefreshToken(response.refreshToken);

        const payload = this.tokenService.getDecodedToken();
        if (payload) {
          this.tokenService.setPlayerId(payload.player_id);
          console.log('[Register] player ID stored:', payload.player_id);
        }
        this.refreshService.refreshAllCaches().subscribe({
          next: () => {
            console.log('[Register] caches refresh successfully!');
            this.refreshService.startAutoRefresh(5000)
            this.router.navigate(['/main']);
          },
          error: (err) => {
            console.error('[Register] error during refreshing of caches', err);
            this.router.navigate(['/main']);
          }
        });

      },
      error: () => {
        console.warn('[Register] registration failed. username may already exist.');
        this.errorMessage = 'Registration failed. Please try a different username.';
      }
    });
  }

  redirectToLogin(): void {
    void this.router.navigate(['/login']);
  }
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LoginService } from '../services/login.service';
import { Router } from '@angular/router';
import {TokenService} from '../services/token.service';
import {RefreshService} from '../services/refresh.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {

  loginForm!: FormGroup;
  // flag to indicate form has been submitted (used for displaying validation errors)
  submitted = false;
  errorMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private loginService: LoginService,
    private router: Router,
    private tokenService: TokenService,
    private refreshService: RefreshService,
) {}

  ngOnInit(): void {
    // initialize the login form with validation rules
    this.loginForm = this.formBuilder.group({
      name: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  // easy access to form controls in template
  get f() {
    return this.loginForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;

    // stop if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    const { name, password } = this.loginForm.value;

    // attempt to retrieve token from backend
    this.loginService.getToken(name, password).subscribe({
      next: (response) => {
        // store tokens locally
        this.tokenService.setAccessToken(response.accessToken);
        this.tokenService.setRefreshToken(response.refreshToken);

        // decode token to extract player information
        const payload = this.tokenService.getDecodedToken();
        if (payload) {
          this.tokenService.setPlayerId(payload.player_id);
        }

        console.log('[Login] login successfully, loading players and leaderboards...');

        // refresh caches and navigate to main page
        this.refreshService.refreshAllCaches().subscribe({
          next: () => {
            console.log('[Login] caches refresh successfully!');
            this.refreshService.startAutoRefresh(5000)
            this.router.navigate(['/main']);
          },
          error: (err) => {
            console.error('[Login] error during refreshing of caches', err);
            this.router.navigate(['/main']);
          }
        });
      },
      error: () => {
        console.log('[Login] login error:');
        this.errorMessage = 'Login fehlgeschlagen.';
      }
    });

  }
}

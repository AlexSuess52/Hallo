import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TokenService } from '../services/token.service';
import { RefreshService} from '../services/refresh.service';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.html',
  imports: [RouterLink],
  styleUrl: './header.scss'
})
export class Header implements OnInit, OnDestroy {

  // initiate
  playerName = '';
  playerId = '';
  tokenExpiresInMinutes = 0;
  tokenExpiresInSeconds = 0;
  protected expTimestamp = 0;
  private intervalId: any;

  constructor(
    protected tokenService: TokenService,
    protected refreshService: RefreshService,
    protected router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // read token payload on component initialization
    const payload = this.tokenService.getDecodedToken();
    if (payload) {
      this.playerName = payload.player_name;
      this.playerId = payload.player_id;
      this.expTimestamp = payload.exp;
    }

    // listen for token updates (e.g., after login or refresh)
    this.tokenService.tokenChanged.subscribe(() => {
      const newPayload = this.tokenService.getDecodedToken();
      if (newPayload) {
        this.playerName = newPayload.player_name;
        this.playerId = newPayload.player_id;
        this.expTimestamp = newPayload.exp;
      } else {
        this.resetDisplay();
      }
    });

    // start interval to update remaining time every second
    this.intervalId = setInterval(() => {
      this.updateRemainingTime();
    }, 1000);
  }

  updateRemainingTime(): void {
    if (!this.expTimestamp) {
      this.resetDisplay();
      return;
    }

    const now = Math.floor(Date.now() / 1000);
    const remainingSeconds = this.expTimestamp - now;

    if (remainingSeconds <= 0) {
      console.warn('[header] access token expired, attempting to refresh...');

      // try to refresh token automatically
      this.tokenService.refreshToken().subscribe({
        next: (response) => {
          console.log('[Header] token successfully refreshed:', response);

          this.tokenService.setAccessToken(response.accessToken);
          this.tokenService.setRefreshToken(response.refreshToken);

          const payload = this.tokenService.getDecodedToken();
          if (payload) {
            this.tokenService.setPlayerId(payload.player_id);
            this.expTimestamp = payload.exp;
            console.log('[Header] new expiration timestamp (unix):', this.expTimestamp);

            const expDate = new Date(this.expTimestamp * 1000);
            console.log('[Header] access token expires at:', expDate.toLocaleString());
          }
        },
        error: () => {
          console.warn('[Header] failed to refresh token, user will be logged out.');
          this.tokenService.removeAccessToken();
          this.tokenService.removeRefreshToken();
          this.router.navigate(['/login']);
        }
      });

      return;
    }

    // update countdown display
    this.tokenExpiresInMinutes = Math.floor(remainingSeconds / 60);
    this.tokenExpiresInSeconds = remainingSeconds % 60;
    this.cdr.detectChanges();
  }

  // clear displayed user info and countdown
  resetDisplay(): void {
    this.playerName = '';
    this.playerId = '';
    this.tokenExpiresInMinutes = 0;
    this.tokenExpiresInSeconds = 0;
    this.expTimestamp = 0;
    this.cdr.detectChanges();
  }

  redirectToRegister(): void {
    void this.router.navigate(['/register']);
  }

  logout(): void {
    // full logout process: clear session, reset display, stop refresh, redirect to login
    this.tokenService.cleanSession();
    setTimeout(() => {
      this.resetDisplay();
      this.refreshService.stopAutoRefresh()
      this.router.navigate(['/login']);
    }, 0);
  }

  ngOnDestroy(): void {
    // cleanup: clear interval when component is destroyed
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
}

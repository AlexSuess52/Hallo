import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import {BehaviorSubject, Observable, throwError} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {environment} from '../enviroments/enviroments';

interface TokenPayload {
  player_name: string;
  player_id: string;
  exp: number;
  iss: string;
  aud: string;
}

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  // emits true/false when token status changes (login/logout)
  tokenChanged = new BehaviorSubject<boolean>(false);

  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.API_BACKEND_DOTNET;
  }

  getDecodedToken(): TokenPayload | null {
    const token = this.getAccessToken();

    if (!token) {
      return null;
    }

    const payload = jwtDecode<TokenPayload>(token);
    const now = Math.floor(Date.now() / 1000);

    // if token expired, clean up access token and return null
    if (payload.exp && payload.exp < now) {
      if (payload.player_id) {
        this.setPlayerId(payload.player_id);
      }
      this.removeAccessToken();
      return null;
    }

    return payload;
  }

  setAccessToken(token: string): void {
    sessionStorage.setItem('accessToken', token);
    this.tokenChanged.next(true);
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem('accessToken');
  }

  removeAccessToken(): void {
    sessionStorage.removeItem('accessToken');
    this.tokenChanged.next(false);
  }

  setRefreshToken(refreshToken: string): void {
    sessionStorage.setItem('refreshToken', refreshToken);
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem('refreshToken');
  }

  removeRefreshToken(): void {
    sessionStorage.removeItem('refreshToken');
    this.tokenChanged.next(false);
  }

  setPlayerId(playerId: string): void {
    sessionStorage.setItem('cachedPlayerId', playerId);
  }

  getPlayerId(): number | null {
    const raw = sessionStorage.getItem('cachedPlayerId');
    return raw !== null ? Number(raw) : null;
  }

  cleanSession(): void {
    // clean up everty session storage entry
    sessionStorage.removeItem('cachedPlayerId');
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');
    // resets the token
    this.tokenChanged.next(false);
  }

  refreshToken(): Observable<{ accessToken: string, refreshToken: string }> {
    const refreshToken = this.getRefreshToken();
    const playerId = this.getPlayerId();

    if (!playerId || !refreshToken) {
      return throwError(() => new Error('Keine gültigen Tokens vorhanden'));
    }

    const requestBody = { id: playerId, refreshToken };

    return this.http.post<{ accessToken: string, refreshToken: string }>(
      `${this.baseUrl}/api/auth/refresh-token`,
      requestBody
    );
  }

}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable, of, tap} from 'rxjs';
import { TokenService } from './token.service';
import { environment } from '../enviroments/enviroments';
import isEqual from 'lodash/isEqual';


export interface Player {
  id: number;
  name: string;
  userEmail: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  private baseUrl: string;
  private cachedPlayers: Player[] | null = null;

  constructor(private http: HttpClient , private tokenService: TokenService) {
    this.baseUrl = environment.API_BACKEND_DOTNET;
  }

  getAllPlayers(forceRefresh = false): Observable<Player[]> {
    const token = this.tokenService.getAccessToken();

    // no valid token -> return empty list
    if (!token) {
      console.warn('[PlayerService] access permitted, no valid access token');
      return of([]);
    }

    // return cached players if available and no forced refresh requested
    if (this.cachedPlayers && !forceRefresh) {
      return of(this.cachedPlayers);
    }

    return this.http.get<Player[]>(`${this.baseUrl}/api/auth/get-all-players`, {
      headers: {
        Authorization: `Bearer ${this.tokenService.getAccessToken()}`
      }
    }).pipe(
      tap(players => {
        // only update cache if data has changed
        if (!isEqual(players, this.cachedPlayers)) {
          console.log('[PlayerService] cache updated with new server data');
          this.cachedPlayers = players
        }else {
          console.log('[PlayerService] server data matches cache, no update');
        }
      })
    );
  }
}

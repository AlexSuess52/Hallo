import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable, of, tap, switchMap} from 'rxjs';
import { TokenService } from './token.service';
import { environment } from '../enviroments/enviroments';
import isEqual from 'lodash/isEqual';

export interface LeaderBoardEntry {
  rank: number;
  playerId: number;
  playerName: string;
  totalScore: number;
  totalTime: number;
  sessionsPlayed: number;
}

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {

  private baseUrl: string;
  private cachedLeaderboards: LeaderBoardEntry[] | null = null;

  constructor(private http: HttpClient, private tokenService: TokenService) {
    this.baseUrl = environment.API_BACKEND_DOTNET;
  }

  getLeaderBoard(forceRefresh = false): Observable<LeaderBoardEntry[]> {
    const token = this.tokenService.getAccessToken();

    // no valid token -> return empty list
    if (!token) {
      console.warn('[LeaderboardService] access permitted, no valid access token');
      return of([]);
    }

    // return cached leaderboard if available and no forced refresh requested
    if (this.cachedLeaderboards && !forceRefresh) {
      return of(this.cachedLeaderboards);
    }

    // fetch leaderboard from server
    return this.http.get<LeaderBoardEntry[]>(`${this.baseUrl}/api/leaderboard/get-current-ranking`, {
      headers: {
        Authorization: `Bearer ${this.tokenService.getAccessToken()}`
      }
    }).pipe(
      tap(entries => {
        // only update cache if data has changed
        if (!isEqual(entries, this.cachedLeaderboards)) {
          console.log('[LeaderboardService] cache updated with new server data');
          this.cachedLeaderboards = entries;
        } else {
          console.log('[LeaderboardService] server data matches cache, no update');
        }
      })
    );
  }

  private recalculateLeaderboard(): Observable<any> {
    return this.http.post(`${this.baseUrl}/api/leaderboard/recalculate-leaderboard`, {}, {
      headers: {
        Authorization: `Bearer ${this.tokenService.getAccessToken()}`
      }
    });
  }


  refreshLeaderboardWithRecalculation(forceRefresh = false): Observable<LeaderBoardEntry[]> {
    return this.recalculateLeaderboard().pipe(
      switchMap(() => this.getLeaderBoard(forceRefresh))
    );
  }

}

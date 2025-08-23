import { Injectable } from '@angular/core';
import { PlayerService } from './player.service';
import { LeaderboardService } from './leaderboard.service';
import { forkJoin, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RefreshService {

  // holds reference to the active interval for auto-refresh
  private refreshIntervalId: any;

  constructor(
    private playerService: PlayerService,
    private leaderboardService: LeaderboardService
  ) {}

  refreshAllCaches(): Observable<any> {
    return forkJoin([
      this.playerService.getAllPlayers(true),
      this.leaderboardService.refreshLeaderboardWithRecalculation(true)
    ]);
  }

  startAutoRefresh(intervalMs: number = 30000): void {
    // prevent multiple intervals from stacking
    if (this.refreshIntervalId) {
      clearInterval(this.refreshIntervalId);
    }
    console.log('[RefreshService] auto-refresh started');

    this.refreshIntervalId = setInterval(() => {
      this.refreshAllCaches().subscribe({
        next: () => {
          console.log('[RefreshService] all caches updated successfully');
        },
        error: (err) => {
          console.error('[RefreshService] error with refreshing caches', err);
        }
      });
    }, intervalMs);
  }

  stopAutoRefresh(): void {
    if (this.refreshIntervalId) {
      clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = null;
      console.log('[RefreshService] auto-Refresh stopped');
    }
  }
}

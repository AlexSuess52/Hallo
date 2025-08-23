import { Component, OnInit } from '@angular/core';
import { LeaderboardService, LeaderBoardEntry } from '../services/leaderboard.service';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './leaderboard.html',
  styleUrl: './leaderboard.scss'
})
export class Leaderboard implements OnInit {

  leaderboardEntries: LeaderBoardEntry[] = [];

  constructor(private leaderboardService: LeaderboardService) {}

  ngOnInit(): void {
    // load leaderboard data when component initializes
    this.loadLeaderboard();
  }

  loadLeaderboard(): void {
    // fetch leaderboard entries from the service
    this.leaderboardService.getLeaderBoard().subscribe({
      next: (entries) => {
        // sort and store leaderboard entries
        this.leaderboardEntries = [...this.sortByRank(entries)];
      },
      error: (err) => {
        // log error if fetching fails
        console.error('[Leaderboard] failed to load leaderboard', err);
      }
    });
  }

  private sortByRank(entries: LeaderBoardEntry[]): LeaderBoardEntry[] {
    // sort entries by rank, handling possible undefined/null ranks
    return entries.sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0));
  }
}

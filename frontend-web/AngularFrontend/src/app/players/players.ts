import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../services/player.service';

@Component({
  selector: 'app-players',
  standalone: true,
  templateUrl: './players.html',
  styleUrl: './players.scss'
})
export class Players implements OnInit {

  players: any[] = [];

  constructor(private playerService: PlayerService,) {}

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.playerService.getAllPlayers().subscribe({
      next: (players) => {
        // sort players by ID in ascending order
        this.players = players.sort((a, b) => (a.id ?? 0) - (b.id ?? 0));
      },
      error: (err) => {
        console.error('[Players] failed to load player list', err);
      }
    });
  }
}

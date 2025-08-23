import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-main',
  standalone: true,
  templateUrl: './main.html',
  styleUrl: './main.scss'
})
export class Main implements OnInit {

  playerName = '';
  playerId = '';

  constructor(
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const payload = this.tokenService.getDecodedToken();

    if (payload) {
      this.playerName = payload.player_name;
      this.playerId = payload.player_id;
    } else {
      console.warn('[Main] No valid token found, redirecting to login.');
      this.router.navigate(['/login']);
    }
  }
}

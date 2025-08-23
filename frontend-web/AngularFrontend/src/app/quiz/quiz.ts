import { Component, OnInit} from '@angular/core';
import { FormsModule} from "@angular/forms";
import { CommonModule } from '@angular/common';
import { QuizService } from '../services/quiz.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../enviroments/enviroments';
import { Router } from "@angular/router";

/**
 * Quiz-Komponente für die Auswahl und den Start eines Quiz.
 * Quiz-Component to start the Quiz.
 * When starting, a request to backend is sent that generates the quiz and passes the Quiz-Id to the Websocket (Quiz-Session)
 */

@Component({
  selector: 'app-quiz',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
  ],
  templateUrl: './quiz.html',
  styleUrl: './quiz.scss',
})

export class Quiz implements OnInit {
  quizzes: any[] = [];
  playerName: string | null = null;
  _quizId: number | null = null;
  selectedMethod: string = 'HARDCODED';
  difficulty: string = '';
  showDifficulty = false;

  constructor(
      private quizService: QuizService,
      private http: HttpClient,
      protected router: Router,
  ) {}

  ngOnInit(): void {
    this.quizService.getAllQuizzes().subscribe(data => {
      this.quizzes = data;
    });
    }
  toggleDifficulty(): void {
    this.showDifficulty = this.selectedMethod === 'API' || this.selectedMethod === 'AI';
  }

  getQuizId(): number | null {
    return this._quizId;
  }

  start(): void {
    if (this._quizId == null) {
      return
    }

    const loginPayload = {
      playerName: "player1",
      quizId: this._quizId,
      difficulty: this.difficulty,
      method: this.selectedMethod
    };

    this.http.post(`${environment.API_BACKEND_JAVA}/api/player/generateQuiz`, null, {
      params: {
        playerName: loginPayload.playerName,
        quizId: loginPayload.quizId,
        difficulty: loginPayload.difficulty,
        method: loginPayload.method
      },
      observe: 'response',
      responseType: 'json'
    }).subscribe({
      next: (response) => {
        const body: any = response.body; // type can be adjusted if you know the structure
        const quizId = body.quizId;      // Access quizId from JSON body
        console.log('Quiz ID:', quizId);

        this.quizService.setQuizId(quizId);

        this.router.navigate(['/quiz-session']);

      },
      error: (err) => {
        console.error('Login failed:', err);
      }
    });
  }
}

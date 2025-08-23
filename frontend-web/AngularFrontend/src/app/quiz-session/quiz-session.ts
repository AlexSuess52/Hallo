import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import {WebSocketService} from '../services/websocket.service';
import {AnswerMessage} from '../dto/AnswerMessage';
import {StartMessage} from '../dto/StartMessage';
import {QuizQuestion} from '../model/quizQuestion';
import {FormsModule} from '@angular/forms';
import {TokenService} from '../services/token.service';
import {QuizService} from '../services/quiz.service';
import {Router} from '@angular/router';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-quiz-session',
  templateUrl: './quiz-session.html',
  styleUrl: './quiz-session.scss',
  imports: [
    FormsModule
  ]
})

export class QuizSession implements OnInit, OnDestroy {

  view: 'start' | 'question' | 'answer' | 'score' = 'start'

  question: QuizQuestion | null = null;
  quizName: string | null = null;
  playerName: string | null = null;
  isCorrect: boolean | null = null;
  correctAnswer: string | null = null;
  score: number | null = null;
  count: number | null = null;
  time: number | null = null;
  submittedAnswer: string | null = null;

  private questionSubscription: Subscription | undefined;
  private checkSubscription: Subscription | undefined;
  private scoreSubscription: Subscription | undefined;

  constructor(private wsService: WebSocketService,
              private quizService: QuizService,
              private tokenService: TokenService,
              protected router: Router,
              private cdr: ChangeDetectorRef) {}

  // connect the websocket connection, init subscriptions and start the new quiz
  async ngOnInit() {
    this.wsService.connect();
    // init subscriptions
    this.questionSubscription = this.wsService.questionSubject.subscribe(question => {
      if(question != null) {
        this.question = question.quizQuestion;
        this.quizName = question.quizName;
        if(this.question == null && this.view == 'answer'){
          this.showScore();
        } else {
          this.view = 'question';
          this.cdr.detectChanges();
          console.log('got question');
        }
      }
    });
    this.checkSubscription = this.wsService.checkSubject.subscribe(res => {
      if (res != null) {
        this.isCorrect = res?.correct;
        this.correctAnswer = res.correctAnswer;
        this.view = 'answer';
        this.cdr.detectChanges();
      }
    });
    this.scoreSubscription = this.wsService.scoreSubject.subscribe(score => {
      if (score != null){
        this.score = score.score;
        this.count = score.count;
        this.time = (score.time / 1000.0);
        this.view = 'score';
        this.cdr.detectChanges();
      }
    });
    await this.wsService.waitForConnection();
    // auto start
    this.start();
  }

  // unsubscribe subscriptions
  ngOnDestroy() {
    if (this.questionSubscription) {
      this.questionSubscription.unsubscribe();
    }
    if (this.checkSubscription) {
      this.checkSubscription.unsubscribe();
    }
    if (this.scoreSubscription) {
      this.scoreSubscription.unsubscribe();
    }
  }

  // get player and quiz id, create a start message and start new quiz
  start(): void {
    let playerId = this.tokenService.getPlayerId();
    let quizId = this.quizService.getQuizId();
    if (playerId != null && quizId != null){
      const startMsg: StartMessage = {
        playerId: playerId,
        quizId: quizId
      }
      this.wsService.startQuiz(startMsg);
    }
  }

  // get user answer and send answer message
  answer(userAnswer: string): void {
    this.submittedAnswer = userAnswer
    const answerMsg: AnswerMessage = {answer: userAnswer}
    this.wsService.submitAnswer(answerMsg);
  }

  // request next question
  next(): void {
    this.wsService.requestNextQuestion();
  }

  // request the session scores
  showScore(): void {
    this.wsService.requestScore();
  }

  // finish session and navigate to selected page
  finish(navigator: string): void {
    this.reset();
    this.router.navigate([navigator]);
  }

  // reset session variables
  private reset(): void {
    this.quizService.setQuizId(0);
    this.view = 'start';
    this.question = null;
    this.playerName = null;
    this.quizName = null;
    this.isCorrect = null;
    this.score = null;
    this.cdr.detectChanges();
  }
}

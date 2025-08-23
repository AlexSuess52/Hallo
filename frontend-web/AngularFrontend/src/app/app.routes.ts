import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Quiz } from './quiz/quiz';
import { Main } from './main/main';
import { Layout } from './layout/layout';
import {QuizSession} from './quiz-session/quiz-session';
import {Register} from './register/register';
import {Players} from './players/players';
import {Leaderboard} from './leaderboard/leaderboard';

export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      { path: '', redirectTo: 'main', pathMatch: 'full' },
      { path: 'main', component: Main },
      { path: 'login', component: Login },
      { path: 'players', component: Players },
      { path: 'leaderboard', component: Leaderboard },
      { path: 'quiz', component: Quiz },
      { path: 'quiz-session', component: QuizSession },
      { path: 'register', component: Register },
    ]
  }
];


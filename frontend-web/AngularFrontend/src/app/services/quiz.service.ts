import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {Quiz} from '../model/quiz';
import {environment} from '../enviroments/enviroments';


@Injectable({
  providedIn: 'root'
})

export class QuizService {
  private baseUrl: string;
  private quizId: number = 0;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.API_BACKEND_JAVA;
  }

  getAllQuizzes(): Observable<Quiz[]> {
    return this.http.get<Quiz[]>(`${this.baseUrl}/api/quizzes/main`);
  }

  getQuizId(): number {
    return this.quizId;
  }

  setQuizId(value: number) {
    this.quizId = value;
  }

// You can add more methods like getQuizById(id: number), submitAnswer(), etc.
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {environment} from '../enviroments/enviroments';

@Injectable({ providedIn: 'root' })
export class LoginService {

  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.API_BACKEND_DOTNET;
  }

  getToken(name: string, password: string): Observable<{ accessToken: string, refreshToken: string }> {

    return this.http.post<{ accessToken: string, refreshToken: string }>(
      `${this.baseUrl}/api/auth/login`,
      { name, password }
    );
  }


}

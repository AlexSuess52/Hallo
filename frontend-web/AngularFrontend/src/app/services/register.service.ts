import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviroments/enviroments';

@Injectable({ providedIn: 'root' })
export class RegisterService {

  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.API_BACKEND_DOTNET;
  }

  registerUser(name: string, password: string): Observable<{ accessToken: string, refreshToken: string }> {
    return this.http.post<{ accessToken: string, refreshToken: string }>(
      `${this.baseUrl}/api/auth/register`,
      { name, password }
    );
  }
}

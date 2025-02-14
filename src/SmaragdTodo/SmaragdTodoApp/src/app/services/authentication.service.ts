declare const google: any;

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private clientId: string = import.meta.env.NG_APP_GOOGLE_CLIENT_ID;
  private apiBaseUrl: string = import.meta.env.NG_APP_API_BASE_URL;

  constructor(private router: Router, private http: HttpClient) {
    google.accounts.id.initialize({
      client_id: this.clientId,
      callback: this.handleCredentialResponse.bind(this)
    });
  }

  signIn(): void {
    google.accounts.id.prompt();
  }

  handleCredentialResponse(response: any): void {
    const idToken = response.credential;
    localStorage.setItem('jwt', idToken);

    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    this.http.post(`${this.apiBaseUrl}/authentication/login`, { token: idToken }, { headers })
      .subscribe({
        next: (res) => this.router.navigate(['/dashboard']),
        error: (err) => console.error(err)
    })
  }

  signOut() : void {
    localStorage.removeItem('jwt');
  }

  getToken(): string | null {
    return localStorage.getItem('jwt');
  }
}

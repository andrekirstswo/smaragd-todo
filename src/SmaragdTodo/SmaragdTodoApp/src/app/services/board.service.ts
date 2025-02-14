import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  constructor(private http: HttpClient) { }

  createBoard(data: {
    name: string
  }): Observable<any> {
    const apiBaseUrl = import.meta.env.NG_APP_API_BASE_URL;
    return this.http.post(`${apiBaseUrl}/board`, data)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    if (error.error instanceof ErrorEvent) {
      console.error('Clientseitiger Fehler:', error.error.message);
    } else {
      console.error('Serverseitiger Fehler:', error.status);
    }
    return throwError(() => new Error('Etwas ist schiefgelaufen. Bitte versuchen Sie es später noch einmal.'));
  }
}

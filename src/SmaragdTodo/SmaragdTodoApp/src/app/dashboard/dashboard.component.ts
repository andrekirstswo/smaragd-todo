import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BoardService } from '../services/board.service';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [JsonPipe, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  createBoardForm!: FormGroup;
  response: any | null = null;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private boardService: BoardService) {
    this.createBoardForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  onSubmit() {
    if(this.createBoardForm.valid) {
      const name = this.createBoardForm.value.name;

      this.boardService.createBoard({name}).subscribe({
        next: (response) => {
          this.response = response;
          this.errorMessage = null;
        },
        error: (error) => {
          console.error('Fehler bei der API-Abfrage', error);
          this.errorMessage = 'Ein Fehler ist aufgetreten. Bitte versuchen Sie es später noch einmal.';
          this.response = null;
        }
      });
    }
  }

}

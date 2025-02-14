declare const google: any;

import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../services/authentication.service";
import { MatButtonModule } from '@angular/material/button';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faGoogle } from '@fortawesome/free-brands-svg-icons';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [
    MatButtonModule,
    FontAwesomeModule]
})
export class LoginComponent implements OnInit {
  user: any = null;
  faGoogle = faGoogle;

  constructor(private authService: AuthenticationService) { }

  ngOnInit(): void {
    const googleLoginBtn = document.getElementById('google-login-btn');
    if (googleLoginBtn) {
      google.accounts.id.renderButton(googleLoginBtn, { theme: 'outline', size: 'large' });
    }
  }

  signInWithGoogle() {
    this.authService.signIn();
  }
}

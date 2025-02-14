declare const google: any;

import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../services/authentication.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
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

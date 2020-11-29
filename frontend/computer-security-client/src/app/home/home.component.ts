import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import { IUserModel } from '../core/models/user';
import { AuthenticationService } from '../core/services/auth.service';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  loading = false;
  user: IUserModel;

  constructor(
      private authenticationService: AuthenticationService
  ) {
      this.user = this.authenticationService.userValue;
  }

  ngOnInit() {
      this.loading = true;
  }
}
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faEdit } from '@fortawesome/free-solid-svg-icons';
import { IUserModel } from '../core/models/user';
import { AuthenticationService } from '../core/services/auth.service';
import { UserService } from '../core/services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user: IUserModel;
  faEdit = faEdit;

  constructor(
    private autService: AuthenticationService  
    ) {}

  ngOnInit(): void {
    this.user = this.autService.userValue;
  }
}

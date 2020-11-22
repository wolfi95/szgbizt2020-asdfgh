import { Component, OnInit } from '@angular/core';
import { IUserModel } from '../core/models/user';

@Component({
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  loading = false;
  users: IUserModel[] = [];

  constructor() { }

  ngOnInit() {
      this.loading = true;
  }
}

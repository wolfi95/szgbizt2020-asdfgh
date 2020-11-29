import { Component, OnInit } from '@angular/core';
import { IUserModel } from '../core/models/user';
import { UserService } from '../core/services/user.service';

@Component({
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  loading = false;
  dataSource: IUserModel[] = [];
  columnsToDisplay = ['email', 'firstName', 'lastName'];

  constructor(private service: UserService) { }

  ngOnInit() {
    this.loading = true;
    this.getAllUsers();
  }

  getAllUsers() {
    this.service.getAllUsers().subscribe(res => {
      this.dataSource = res;
      this.loading = false;
    })
  }
}

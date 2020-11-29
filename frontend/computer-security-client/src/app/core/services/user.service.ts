import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CaffEdituserdataClient, CaffGetallusersClient, CaffGetuserdatabyidClient, EditUserDto } from 'src/app/shared/clients';
import { Role } from '../models/role';
import { IUserEditModel, IUserModel } from '../models/user';
import { AuthenticationService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private getAllUserClient: CaffGetallusersClient,
    private getUserbyIdClient: CaffGetuserdatabyidClient,
    private editUserdataClient: CaffEdituserdataClient,
    public authService: AuthenticationService,
  ) { }

  getAllUsers(): Observable<IUserEditModel[]> {
    return this.getAllUserClient.getAllUsers().pipe(map(
      (dtos: EditUserDto[]): IUserEditModel[] => this.dtosToModels(dtos)
    ));
  }

  getUserDataById(userId: string): Observable<IUserEditModel> {
    return this.getUserbyIdClient.getUserData(userId).pipe(map(
      (dto: EditUserDto): IUserEditModel => this.dtoToModel(dto)
    ));
  }

  editUser(model: IUserEditModel): Observable<IUserModel> {
    const dto = new EditUserDto({
      id: model.id,
      email: model.email,
      firstName: model.firstName,
      lastName: model.lastName
    });
    return this.editUserdataClient.editUser(dto).pipe(map(dto => {
      const user: IUserModel = {
        id: dto.id,
        firstName: dto.firstName,
        lastName: dto.lastName,
        email: dto.email,
        role: (<any>Role)[dto.role],
        token: this.authService.userValue.token
      };
      localStorage.setItem('user', JSON.stringify(user));
      this.authService.setUserValue(user);
      return user;
    }));;
  }

  private dtosToModels(dtos: EditUserDto[]): IUserEditModel[] {
    let users: IUserEditModel[] = [];
    for (let dto of dtos) {
      let user: IUserEditModel = {
        email: dto.email,
        firstName: dto.firstName,
        lastName: dto.lastName,
        id: dto.id
      };
      users.push(user);
    }
    return users;
  }

  private dtoToModel(dto: EditUserDto): IUserEditModel {
    let user: IUserEditModel = {
      email: dto.email,
      firstName: dto.firstName,
      lastName: dto.lastName,
      id: dto.id
    };
    return user;
  }
}

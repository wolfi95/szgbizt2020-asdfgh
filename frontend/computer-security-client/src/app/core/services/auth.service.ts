import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IUserModel, IUserRegisterModel } from '../models/user';
import { environment } from 'src/environments/environment';
import { AuthLoginClient, AuthRegisterClient, LoginRequestDTO, RegistrationRequestDTO, UserDTO } from 'src/app/shared/clients';
import { Role } from '../models/role';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private userSubject: BehaviorSubject<IUserModel>;
    public user: Observable<IUserModel>;

    constructor(
        private router: Router,
        private authLoginClient: AuthLoginClient,
        private authRegisterClient: AuthRegisterClient
    ) {
        this.userSubject = new BehaviorSubject<IUserModel>(JSON.parse(localStorage.getItem('user')!));
        this.user = this.userSubject.asObservable();
    }

    public setUserValue(value: IUserModel) {
        this.userSubject.next(value);
    }

    public get userValue(): IUserModel {
        return this.userSubject.value;
    }

    login(email: string, password: string): Observable<IUserModel> {
        const dto = new LoginRequestDTO({
            email: email,
            password: password
        })
        return this.authLoginClient.login(dto)
            .pipe(map(dto => {
                const user: IUserModel = {
                    id: dto.id,
                    firstName: dto.firstName,
                    lastName: dto.lastName,
                    email: dto.email,
                    role: (<any>Role)[dto.role],
                    token: dto.token
                };
                localStorage.setItem('user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
            }));
    }

    register(model: IUserRegisterModel): Observable<IUserModel> {
        const dto = new RegistrationRequestDTO({
            firstName: model.firstName,
            lastName: model.lastName,
            email: model.email,
            password: model.password
        })
        return this.authRegisterClient.register(dto)
            .pipe(map(dto => {
                const user: IUserModel = {
                    id: dto.id,
                    firstName: dto.firstName,
                    lastName: dto.lastName,
                    role: (<any>Role)[dto.role],
                    token: dto.token
                };
                localStorage.setItem('user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
            }));
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('user');
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }
}
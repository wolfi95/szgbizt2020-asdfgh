import { compileComponentFromMetadata } from '@angular/compiler';
import { expressionType } from '@angular/compiler/src/output/output_ast';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { AuthenticationService } from '../core/services/auth.service';

import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  const authServiceMock = {
    userValue: {
      email: 'test@test.hu',
      firstName: 'firstName',
      lastName: 'lastName',
    },
    login: () =>
      of({
        id: 'id',
        firstName: 'firstName',
        lastName: 'lastName',
        role: [],
        token: 'token',
      }),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule, RouterTestingModule],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call server with valid form', () => {
    spyOn(authServiceMock, 'login').and.callThrough();
    component.loginForm.setValue({
      email: "test@test.com",
      password: "secret"
    })
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.login).toHaveBeenCalled();
  });

  it('should not call server with invalid form', () => {
    spyOn(authServiceMock, 'login').and.callThrough();
    component.loginForm.patchValue({
      email: "test@test.com",
    })
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.login).not.toHaveBeenCalled();
  });

  it('should not call server with invalid email', () => {
    spyOn(authServiceMock, 'login').and.callThrough();
    component.loginForm.patchValue({
      email: "enail address",
      password: "secret"
    })
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.login).not.toHaveBeenCalled();
  });
});

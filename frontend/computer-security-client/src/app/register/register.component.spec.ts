/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { RegisterComponent } from './register.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { AuthenticationService } from '../core/services/auth.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  const authServiceMock = {
    register: () =>
      of({
        id: 'id',
        firstName: 'firstName',
        lastName: 'lastName',
        role: [],
        token: 'token',
      }),
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterComponent],
      imports: [ReactiveFormsModule, RouterTestingModule],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not call server with partially empty form', () => {
    spyOn(authServiceMock, 'register').and.callThrough();
    component.registerForm.patchValue({
      firstName: 'first',
      lastName: 'second',
    });
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.register).not.toHaveBeenCalled();
  });

  it('should call server with valid form', () => {
    spyOn(authServiceMock, 'register').and.callThrough();
    component.registerForm.setValue({
      firstName: 'first',
      lastName: 'second',
      email: "test@test.com",
      password: "secret",
      confirmPassword: "secret",
    });
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.register).toHaveBeenCalled();
  });

  it('should not call server with invalid email', () => {
    spyOn(authServiceMock, 'register').and.callThrough();
    component.registerForm.setValue({
      firstName: 'first',
      lastName: 'second',
      email: "just an easy email",
      password: "secret",
      confirmPassword: "secret",
    });
    fixture.nativeElement.querySelector('button').click();
    expect(authServiceMock.register).not.toHaveBeenCalled();
  });
});

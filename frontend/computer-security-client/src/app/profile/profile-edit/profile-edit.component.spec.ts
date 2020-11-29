import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';

import { ProfileEditComponent } from './profile-edit.component';

describe('ProfileEditComponent', () => {
  let component: ProfileEditComponent;
  let fixture: ComponentFixture<ProfileEditComponent>;

  const authServiceMock = {
    userValue: {
      email: 'test@test.hu',
      firstName: 'firstName',
      lastName: 'lastName',
    },
  } as AuthenticationService;
  const userServiceMock = {
    editUser: () =>
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
      declarations: [ProfileEditComponent],
      imports: [ReactiveFormsModule],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        },
        {
          provide: UserService,
          useValue: userServiceMock,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set default email to field', () => {
    const emailInput: HTMLInputElement = fixture.nativeElement.querySelector(
      '[placeholder="Email"]'
    );
    expect(emailInput.value).toBe('test@test.hu');
  });

  it('should set default first name to field', () => {
    const emailInput: HTMLInputElement = fixture.nativeElement.querySelector(
      '[placeholder="First Name"]'
    );
    expect(emailInput.value).toBe('firstName');
  });

  it('should set default last name to field', () => {
    const emailInput: HTMLInputElement = fixture.nativeElement.querySelector(
      '[placeholder="Last Name"]'
    );
    expect(emailInput.value).toBe('lastName');
  });

  it('should not send data to user service on submit if the form is invalid', () => {
    spyOn(userServiceMock, 'editUser').and.callThrough();
    expect(userServiceMock.editUser).not.toHaveBeenCalled();
    component.editProfileForm.controls.firstName.setValue('');
    fixture.nativeElement.querySelector('button').click();
    expect(userServiceMock.editUser).not.toHaveBeenCalled();
  });

  it('should send data to user service on submit', () => {
    spyOn(userServiceMock, 'editUser').and.callThrough();
    expect(userServiceMock.editUser).not.toHaveBeenCalled();
    fixture.nativeElement.querySelector('button').click();
    expect(userServiceMock.editUser).toHaveBeenCalled();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthenticationService } from '../core/services/auth.service';

import { ProfileComponent } from './profile.component';

fdescribe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;

  const authServiceMock = {
    userValue: {
      email: 'test@test.hu',
      firstName: 'firstName',
      lastName: 'lastName',
    },
  } as AuthenticationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileComponent],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show name', () => {
    expect(fixture.nativeElement.textContent).toContain('firstName lastName');
  });

  it('should show email', () => {
    expect(fixture.nativeElement.textContent).toContain('test@test.hu');
  });
});

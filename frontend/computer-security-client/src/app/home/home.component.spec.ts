import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthenticationService } from '../core/services/auth.service';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;

  const authServiceMock = {
    userValue: {
      email: 'test@test.hu',
      firstName: 'firstName',
      lastName: 'lastName',
    },
  } as AuthenticationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeComponent ],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

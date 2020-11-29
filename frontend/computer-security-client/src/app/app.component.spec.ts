import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { AppComponent } from './app.component';
import { AuthenticationService } from './core/services/auth.service';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;

  const authServiceMock = {
    user: of({
      id: 'id',
      firstName: 'firstName',
      lastName: 'lastName',
      token: 'token',
    }),
    logout: () => {},
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [AppComponent],
      providers: [
        {
          provide: AuthenticationService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
  });

  it('should create the app', () => {
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should contain navbar', () => {
    expect(fixture.nativeElement.querySelector('nav')).toBeTruthy();
  });

  it('should show appName', () => {
    expect(fixture.nativeElement.textContent).toContain("CAFFStore");
  });

  it('should contain router-outlet', () => {
    expect(fixture.nativeElement.querySelector("router-outlet")).toBeTruthy()
  });

  it('should call logout', () => {
    spyOn(authServiceMock, "logout").and.callThrough();
    fixture.componentInstance.logout();
    expect(authServiceMock.logout).toHaveBeenCalled();
  });
});

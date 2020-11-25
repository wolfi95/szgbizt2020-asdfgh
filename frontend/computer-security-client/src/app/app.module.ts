import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin/admin.component';
import { API_BASE_URL } from './shared/clients';
import { RegisterComponent } from './register/register.component';
import { CafffilesComponent } from './cafffiles/cafffiles.component';
import { CafffileCreateComponent } from './cafffiles/cafffile-create/cafffile-create.component';
import { CafffileDetailsComponent } from './cafffiles/cafffile-details/cafffile-details.component';
import { CafffileListComponent } from './cafffiles/cafffile-list/cafffile-list.component';
import { ProfileComponent } from './profile/profile.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ProfileEditComponent } from './profile/profile-edit/profile-edit.component';

@NgModule({
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    FontAwesomeModule
  ],
  declarations: [	
    AppComponent,
    HomeComponent,
    LoginComponent,
    AdminComponent,
    RegisterComponent,
    CafffilesComponent,
    CafffileCreateComponent,
    CafffileDetailsComponent,
    CafffileListComponent,
    ProfileComponent,
    ProfileEditComponent,
   ],
  providers: [
    { provide: API_BASE_URL, useValue: 'https://localhost:44335'},
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

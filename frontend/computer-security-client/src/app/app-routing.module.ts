import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { CafffileCreateComponent } from './cafffiles/cafffile-create/cafffile-create.component';
import { CafffileDetailsComponent } from './cafffiles/cafffile-details/cafffile-details.component';
import { CafffileStartComponent } from './cafffiles/cafffile-start/cafffile-start.component';
import { CafffilesComponent } from './cafffiles/cafffiles.component';
import { AuthGuard } from './core/guards/auth.guard';
import { Role } from './core/models/role';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { ProfileEditComponent } from './profile/profile-edit/profile-edit.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: '', redirectTo: '/caffs', pathMatch: 'full' },
  {
    path: 'caffs', component: CafffilesComponent, children: [
      { path: '', component: CafffileStartComponent },
      { path: 'new', component: CafffileCreateComponent,    canActivate: [AuthGuard]    },
      { path: ':id', component: CafffileDetailsComponent },
      { path: ':id/edit', component: CafffileCreateComponent,    canActivate: [AuthGuard]    },

    ]
  },
  // {
  //     path: '',
  //     component: HomeComponent,
  //     canActivate: [AuthGuard]
  // },
  {
      path: 'admin',
      component: AdminComponent,
      canActivate: [AuthGuard],
      data: { roles: [Role.Admin] }
  },
  {
      path: 'login',
      component: LoginComponent
  },
  {
    path: 'profile',
    component: ProfileComponent
  },
  {
    path: 'profile/edit',
    component: ProfileEditComponent
  },
  {
    path: 'register',
    component: RegisterComponent
},

  // otherwise redirect to home
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

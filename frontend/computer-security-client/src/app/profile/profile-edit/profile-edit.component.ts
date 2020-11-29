import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IUserEditModel } from 'src/app/core/models/user';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.scss']
})
export class ProfileEditComponent implements OnInit {

  editProfileForm: FormGroup;
  loading = false;
  submitted = false;
  userId: string;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private autService: AuthenticationService,
    private service: UserService
  ) { }

  ngOnInit(): void {
    this.userId = this.autService.userValue.id;
    this.editProfileForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });

    this.f.firstName.setValue(this.autService.userValue.firstName);
    this.f.lastName.setValue(this.autService.userValue.lastName);
    this.f.email.setValue(this.autService.userValue.email);
  }

  get f() { return this.editProfileForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.editProfileForm.invalid) {
      return;
    }

    this.loading = true;
    const model: IUserEditModel = {
      id: this.userId,
      email: this.f.email.value,
      firstName: this.f.firstName.value,
      lastName: this.f.lastName.value,
    };
    this.service.editUser(model).subscribe(_ => {
      this.router.navigate(['/profile']);
      this.loading = false
    });
  }

}

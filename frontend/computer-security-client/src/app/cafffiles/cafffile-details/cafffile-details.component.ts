import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ICaffEditModel, ICaffFileDetails, IComment, ICommentCreate } from 'src/app/core/models/cafffile';
import { CafffileService } from 'src/app/core/services/cafffile.service';
import { saveAs } from 'file-saver';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { IUserModel } from 'src/app/core/models/user';
import { Role } from 'src/app/core/models/role';

@Component({
  selector: 'app-cafffile-details',
  templateUrl: './cafffile-details.component.html',
  styleUrls: ['./cafffile-details.component.scss']
})
export class CafffileDetailsComponent implements OnInit {
  caffForm: FormGroup;
  caffNameForm: FormGroup;
  caffId: string;
  caffFile: ICaffFileDetails;
  imageSource: any;
  comments: IComment[] = [];
  user: IUserModel;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cafffileService: CafffileService,
    private sanitizer: DomSanitizer,
    private formBuilder: FormBuilder,
    private authService: AuthenticationService
  ) {
    this.authService.user.subscribe(x => this.user = x);
  }

  ngOnInit(): void {
    this.caffForm = this.formBuilder.group({
      name: [''],
      comment: ['', Validators.required]
    });
    this.caffNameForm = this.formBuilder.group({
      name: ['', Validators.required]
    });
    this.route.params.subscribe((params: Params) => {
      this.caffId = params['id'];
      this.cafffileService.getCaffFilebyId(this.caffId).subscribe(res => {
        this.caffForm.controls.name.setValue(res.name);
        this.caffFile = res;
        this.comments = res.comments;
        this.imageSource = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${res.imageData}`);
      });
    });
  }

  get isAdmin() {
    return this.user && this.user.role === Role.Admin;
  }

  get f() { return this.caffForm.controls; }
  get fName() { return this.caffNameForm.controls; }

  downloadCaffFile() {
    this.cafffileService.downloadCaffFile(this.caffFile.id).subscribe(res => {
      saveAs(res.data, this.caffFile.name + '.caff');
    })
  }

  onSubmit() {
    const model: ICommentCreate = {
      message: this.f.comment.value,
      caffFileId: this.caffId
    }
    this.cafffileService.comment(model).subscribe();
  }

  editName() {
    const model: ICaffEditModel = {
      name: this.fName.name.value,
      id: this.caffId
    }
    this.cafffileService.editName(model).subscribe();
  }
}

import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ICaffFileListItemModel } from 'src/app/core/models/cafffile';
import { Role } from 'src/app/core/models/role';
import { IUserModel } from 'src/app/core/models/user';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { CafffileService } from 'src/app/core/services/cafffile.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-cafffile-list-item',
  templateUrl: './cafffile-list-item.component.html',
  styleUrls: ['./cafffile-list-item.component.scss']
})
export class CafffileListItemComponent implements OnInit {
  imageSource: any;
  user: IUserModel;

  @Input() caffFile: ICaffFileListItemModel;

  constructor(
    private sanitizer: DomSanitizer,
    private service: CafffileService,
    private authService: AuthenticationService
  ) {
    this.authService.user.subscribe(x => this.user = x);
  }

  ngOnInit(): void {
    this.imageSource = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${this.caffFile.imageData}`);
  }

  get isAdmin() {
    return this.user && this.user.role === Role.Admin;
  }

  deleteCaffFile() {
    this.service.deleteCaffFile(this.caffFile.id).subscribe(res => {
      this.service.getAllCaffFiles().subscribe(res => {
        this.service.cafffileListChanged.next(res);
      });
    }
    );
  }

}

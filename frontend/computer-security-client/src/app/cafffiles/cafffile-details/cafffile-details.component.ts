import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ICaffFileDetails, IComment } from 'src/app/core/models/cafffile';
import { CafffileService } from 'src/app/core/services/cafffile.service';

@Component({
  selector: 'app-cafffile-details',
  templateUrl: './cafffile-details.component.html',
  styleUrls: ['./cafffile-details.component.scss']
})
export class CafffileDetailsComponent implements OnInit {

  caffId: string;
  caffFile: ICaffFileDetails;
  imageSource: any;
  comments: IComment[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cafffileService: CafffileService,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      this.caffId = params['id'];
      this.cafffileService.getCaffFilebyId(this.caffId).subscribe(res => {
        this.caffFile = res;
        this.comments = res.comments;
        this.imageSource = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${res.imageData}`);
      });
  });
}

}

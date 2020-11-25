import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ICaffFileListItemModel } from 'src/app/core/models/cafffile';

@Component({
  selector: 'app-cafffile-list-item',
  templateUrl: './cafffile-list-item.component.html',
  styleUrls: ['./cafffile-list-item.component.scss']
})
export class CafffileListItemComponent implements OnInit {
  imageSource: any;
  @Input() caffFile: ICaffFileListItemModel;
  
  constructor(
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    this.imageSource = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${this.caffFile.imageData}`);
  }

}

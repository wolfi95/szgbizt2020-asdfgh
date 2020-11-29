import { Component, OnInit } from '@angular/core';
import { ICaffFileListItemModel } from '../core/models/cafffile';
import { CafffileService } from '../core/services/cafffile.service';

@Component({
  selector: 'app-cafffiles',
  templateUrl: './cafffiles.component.html',
  styleUrls: ['./cafffiles.component.scss']
})
export class CafffilesComponent implements OnInit {
  caffList: ICaffFileListItemModel[] = [];
  isLoading = true;

  constructor(private cafffileService: CafffileService) { }

  ngOnInit(): void {
    this.cafffileService.getAllCaffFiles().subscribe(res => {
      this.caffList = res;
      this.isLoading = false;
    })
  }

}

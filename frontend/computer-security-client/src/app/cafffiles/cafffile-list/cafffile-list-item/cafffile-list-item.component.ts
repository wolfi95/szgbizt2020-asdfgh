import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ICaffFileListItemModel } from 'src/app/core/models/cafffile';

@Component({
  selector: 'app-cafffile-list-item',
  templateUrl: './cafffile-list-item.component.html',
  styleUrls: ['./cafffile-list-item.component.scss']
})
export class CafffileListItemComponent implements OnInit {

  @Input() caffFile: ICaffFileListItemModel;
  
  constructor() { }

  ngOnInit(): void {
  }

}

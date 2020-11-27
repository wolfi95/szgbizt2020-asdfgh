import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { ICaffFileListItemModel } from 'src/app/core/models/cafffile';
import { CafffileService } from 'src/app/core/services/cafffile.service';

@Component({
  selector: 'app-cafffile-list',
  templateUrl: './cafffile-list.component.html',
  styleUrls: ['./cafffile-list.component.scss']
})
export class CafffileListComponent implements OnInit {

  faSearch = faSearch;
  caffList: ICaffFileListItemModel[] = [];

  constructor(
    private cafffileService: CafffileService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.cafffileService.getAllCaffFiles().subscribe(res => {
      this.caffList = res;
    })
  }

  onNewCaff(){
    this.router.navigate(['new'], { relativeTo: this.route });
  }

}

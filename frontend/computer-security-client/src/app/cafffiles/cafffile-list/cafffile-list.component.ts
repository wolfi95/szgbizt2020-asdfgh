import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { Subscription } from 'rxjs';
import { ICaffFileListItemModel } from 'src/app/core/models/cafffile';
import { CafffileService } from 'src/app/core/services/cafffile.service';

@Component({
  selector: 'app-cafffile-list',
  templateUrl: './cafffile-list.component.html',
  styleUrls: ['./cafffile-list.component.scss']
})
export class CafffileListComponent implements OnInit {

  faSearch = faSearch;
  searchText: any;
  @Input() caffList: ICaffFileListItemModel[];
  subscriptionChange: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: CafffileService
  ) { }

  ngOnInit(): void {
    this.subscriptionChange = this.service.cafffileListChanged.subscribe(list => { this.caffList = list; });
  }

  onNewCaff(){
    this.router.navigate(['new'], { relativeTo: this.route });
  }

}

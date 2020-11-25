import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CaffCafffilesClient, CaffHeader } from 'src/app/shared/clients';
import { map } from 'rxjs/operators';
import { ICaffFileListItemModel } from '../models/cafffile';

@Injectable({
  providedIn: 'root'
})
export class CafffileService {

  constructor(
    private caffCafffilesClient: CaffCafffilesClient
    ) { }

  getAllCaffFiles(): Observable<ICaffFileListItemModel[]>{
    //return this.caffCafffilesClient.getAllCaffFiles().pipe(map(this.caffHeaderToModel));
    return this.caffCafffilesClient.getAllCaffFiles().pipe(map(
      (dtos: CaffHeader[]):ICaffFileListItemModel[] => this.caffHeaderToModel(dtos)
    ));
  }

  private caffHeaderToModel(dtos: CaffHeader[]): ICaffFileListItemModel[] {
    let cafffiles: ICaffFileListItemModel[] = [];
    for (let caffDto of dtos) {
      let caff: ICaffFileListItemModel = {
        name: caffDto.name,
        imageData: caffDto.imageData,
        id: caffDto.id
      };
      cafffiles.push(caff);
    }
    return cafffiles;
  }
}

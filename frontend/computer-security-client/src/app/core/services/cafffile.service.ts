import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CaffCafffilesClient, CaffHeader, CaffUploadClient, FileParameter } from 'src/app/shared/clients';
import { map } from 'rxjs/operators';
import { ICaffFileListItemModel, ICaffFileUploadModel } from '../models/cafffile';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CafffileService {

  constructor(
    private caffCafffilesClient: CaffCafffilesClient,
    private caffUploadClient: CaffUploadClient,
    private httpClient: HttpClient
    ) { }

  getAllCaffFiles(): Observable<ICaffFileListItemModel[]>{
    //return this.caffCafffilesClient.getAllCaffFiles().pipe(map(this.caffHeaderToModel));
    return this.caffCafffilesClient.getAllCaffFiles().pipe(map(
      (dtos: CaffHeader[]):ICaffFileListItemModel[] => this.caffHeaderToModel(dtos)
    ));
  }

  uploadCaffFile(model: ICaffFileUploadModel): Observable<void> {
    const name = model.name;
    const fileParam : FileParameter = {
      data: model.data,
      fileName: model.name
    }
    console.log(name);

    console.log(fileParam);
    return this.caffUploadClient.uploadCaffFile(name, fileParam);
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

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CaffCafffilesClient, CaffHeader, CaffUploadClient, CaffCommentClient, FileParameter, CaffGetcafffilebyidClient, CaffFileDto, CaffDownloadClient } from 'src/app/shared/clients';
import { map } from 'rxjs/operators';
import { ICaffFileDetails, ICaffFileListItemModel, ICaffFileUploadModel, IComment, ICommentCreate } from '../models/cafffile';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CafffileService {

  constructor(
    private caffCafffilesClient: CaffCafffilesClient,
    private caffUploadClient: CaffUploadClient,
    private caffDownloadClient: CaffDownloadClient,
    private caffCommentClient: CaffCommentClient,
    private caffGetbyidClient: CaffGetcafffilebyidClient,
    private httpClient: HttpClient
  ) { }

  getAllCaffFiles(): Observable<ICaffFileListItemModel[]> {
    //return this.caffCafffilesClient.getAllCaffFiles().pipe(map(this.caffHeaderToModel));
    return this.caffCafffilesClient.getAllCaffFiles().pipe(map(
      (dtos: CaffHeader[]): ICaffFileListItemModel[] => this.caffHeaderToModel(dtos)
    ));
  }

  uploadCaffFile(model: ICaffFileUploadModel): Observable<void> {
    const name = model.name;
    const fileParam: FileParameter = {
      data: model.data,
      fileName: model.name
    }
    return this.caffUploadClient.uploadCaffFile(name, fileParam);
  }

  downloadCaffFile(id: string): Observable<void> {
    return this.caffDownloadClient.downloadCaffFileById(id);
  }

  comment(model: ICommentCreate): Observable<void> {
    const message = model.message;
    const id = model.caffFileId;
    return this.caffCommentClient.comment(message, id);
  }

  getCaffFilebyId(id: string): Observable<ICaffFileDetails> {
    return this.caffGetbyidClient.getFCaffFileById(id).pipe(map(
      (dto: CaffFileDto): ICaffFileDetails => this.caffDetailsToModel(dto)
    ));
  }

  private caffDetailsToModel(dto: CaffFileDto): ICaffFileDetails {
    let comments: IComment[] = [];
    for (let comment of dto.comments) {
      let comm: IComment = {
        userId: comment.userId,
        userName: comment.userName,
        content: comment.content
      };
      comments.push(comm);
    }
    let caff: ICaffFileDetails = {
      id: dto.id,
      name: dto.name,
      imageData: dto.imageData,
      comments: comments
    };
    return caff;

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

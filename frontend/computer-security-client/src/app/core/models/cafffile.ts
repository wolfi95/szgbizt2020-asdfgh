import { FileParameter } from 'src/app/shared/clients';

export interface ICaffFileListItemModel {
    id: string;
    name: string;
    imageData: string;
    
}

export interface ICaffFileUploadModel {
    name: string;
    data: Blob;
}

export interface ICommentCreate {
    message: string;
    caffFileId: string;
}

export interface IComment {
    userId: string;
    userName: string;
    content: string;
}

export interface ICaffFileDetails {
    id: string;
    name: string;
    imageData: string;
    comments: IComment[];
}
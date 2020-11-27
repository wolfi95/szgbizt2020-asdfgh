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
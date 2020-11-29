import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ICaffFileUploadModel } from 'src/app/core/models/cafffile';
import { CafffileService } from 'src/app/core/services/cafffile.service';

@Component({
  selector: 'app-cafffile-create',
  templateUrl: './cafffile-create.component.html',
  styleUrls: ['./cafffile-create.component.scss']
})
export class CafffileCreateComponent implements OnInit {
 
  uploadForm: FormGroup;
  loading: boolean = false; // Flag variable 
  file: Blob = null; // Variable to store file 
  
  constructor(
    private caffFileService: CafffileService,
    private formBuilder: FormBuilder,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.uploadForm = this.formBuilder.group({
      name: ['', Validators.required],
      file: ['', Validators.required],
    });
  }

  get f() { return this.uploadForm.controls; }

  onFileSelected(event: any) {
    if(event.target.files[0]) {
      this.file = event.target.files[0];
    }
  }

  onSubmit() {
    if (this.uploadForm.invalid) {
      return;
    }
    this.loading = !this.loading; 
    const model: ICaffFileUploadModel = {
      name: this.f.name.value,
      data: this.file as Blob
    }
    this.caffFileService.uploadCaffFile(model).subscribe(_ => { 
      this.loading = false;
      this.router.navigate(['/caffs']);
    });
  }

}

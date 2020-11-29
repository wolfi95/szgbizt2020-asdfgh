import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { CafffileService } from 'src/app/core/services/cafffile.service';

import { CafffileCreateComponent } from './cafffile-create.component';

describe('CafffileCreateComponent', () => {
  let component: CafffileCreateComponent;
  let fixture: ComponentFixture<CafffileCreateComponent>;

  const caffFileServiceMock = {
    uploadCaffFile: () => of,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CafffileCreateComponent],
      imports: [ReactiveFormsModule],
      providers: [
        {
          provide: CafffileService,
          useValue: caffFileServiceMock,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call upload function with valid form', () => {
    spyOn(caffFileServiceMock, 'uploadCaffFile').and.callThrough();
    component.uploadForm.setValue({
      name: 'caff image name',
      file: '',
    });
    let input = fixture.nativeElement.querySelector('input[type=file]');
    const dataTransfer = new DataTransfer();
    dataTransfer.items.add(new File(['content'], 'caffFile'));
    input.files = dataTransfer.files;
    input.dispatchEvent(new Event('change'));
    input.dispatchEvent(new Event('input'));
    fixture.nativeElement.querySelector('button').click();
    expect(caffFileServiceMock.uploadCaffFile).toHaveBeenCalled();
  });

  it('should not call upload function with missing file', () => {
    spyOn(caffFileServiceMock, 'uploadCaffFile').and.callThrough();
    component.uploadForm.patchValue({
      name: 'caff image name',
    });
    expect(caffFileServiceMock.uploadCaffFile).not.toHaveBeenCalled();
  });

  it('should not call upload function with missing file name', () => {
    spyOn(caffFileServiceMock, 'uploadCaffFile').and.callThrough();
    component.uploadForm.patchValue({
      file: ''
    });
    expect(caffFileServiceMock.uploadCaffFile).not.toHaveBeenCalled();
  });
});

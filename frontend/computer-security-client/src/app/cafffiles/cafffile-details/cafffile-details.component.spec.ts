import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { CafffileService } from 'src/app/core/services/cafffile.service';

import { CafffileDetailsComponent } from './cafffile-details.component';

describe('CafffileDetailsComponent', () => {
  let component: CafffileDetailsComponent;
  let fixture: ComponentFixture<CafffileDetailsComponent>;

  const caffFileServiceMock = {
    getCaffFilebyId: () => of({
      id: 'id',
      name: "image name",
      imageData: "data",
      comments: []
    })
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileDetailsComponent ],
      imports: [RouterTestingModule],
      providers: [
        {
          provide: CafffileService,
          useValue: caffFileServiceMock
        },
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should contain image', () => {
    expect(fixture.nativeElement.querySelector("img")).toBeTruthy()
  });

  it('should show image name', () => {
    expect(fixture.nativeElement.textContent).toContain('image name');
  });
});

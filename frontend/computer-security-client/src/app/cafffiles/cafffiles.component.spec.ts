import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffilesComponent } from './cafffiles.component';

describe('CafffilesComponent', () => {
  let component: CafffilesComponent;
  let fixture: ComponentFixture<CafffilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffilesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffileDetailsComponent } from './cafffile-details.component';

describe('CafffileDetailsComponent', () => {
  let component: CafffileDetailsComponent;
  let fixture: ComponentFixture<CafffileDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileDetailsComponent ]
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
});

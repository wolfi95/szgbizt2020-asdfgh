import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffileStartComponent } from './cafffile-start.component';

describe('CafffileStartComponent', () => {
  let component: CafffileStartComponent;
  let fixture: ComponentFixture<CafffileStartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileStartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileStartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

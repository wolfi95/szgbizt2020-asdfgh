import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffileCreateComponent } from './cafffile-create.component';

describe('CafffileCreateComponent', () => {
  let component: CafffileCreateComponent;
  let fixture: ComponentFixture<CafffileCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileCreateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

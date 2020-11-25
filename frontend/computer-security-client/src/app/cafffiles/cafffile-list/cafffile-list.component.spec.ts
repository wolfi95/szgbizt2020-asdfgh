import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffileListComponent } from './cafffile-list.component';

describe('CafffileListComponent', () => {
  let component: CafffileListComponent;
  let fixture: ComponentFixture<CafffileListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

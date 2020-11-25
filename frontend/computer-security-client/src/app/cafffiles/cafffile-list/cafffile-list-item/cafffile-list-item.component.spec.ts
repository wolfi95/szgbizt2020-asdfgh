import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CafffileListItemComponent } from './cafffile-list-item.component';

describe('CafffileListItemComponent', () => {
  let component: CafffileListItemComponent;
  let fixture: ComponentFixture<CafffileListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CafffileListItemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

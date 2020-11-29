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
    component.caffFile = {
      id: 'id',
      imageData: 'data',
      name: 'image name'
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show image name', () => {
    expect(fixture.nativeElement.textContent).toContain("image name")
  });

  it('should contain image', () => {
    expect(fixture.nativeElement.querySelector("img")).toBeTruthy()
  });
});

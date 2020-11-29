import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/compiler';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { CafffileService } from 'src/app/core/services/cafffile.service';
import { FilterPipe } from 'src/app/shared/filter.pipe';

import { CafffileListComponent } from './cafffile-list.component';

describe('CafffileListComponent', () => {
  let component: CafffileListComponent;
  let fixture: ComponentFixture<CafffileListComponent>;

  const files = [{}, {}, {}];
  const caffFileServiceMock = {
    getAllCaffFiles: () => of(files),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CafffileListComponent, FilterPipe],
      imports: [RouterTestingModule],
      providers: [
        {
          provide: CafffileService,
          useValue: caffFileServiceMock,
        },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CafffileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render all returned elements', () => {
    const listItems = fixture.debugElement.queryAll(
      By.css('app-cafffile-list-item')
    );
    expect(listItems.length).toBe(3);
  });

  it('should render all expected elements', () => {
    const listItems = fixture.debugElement.queryAll(
      By.css('app-cafffile-list-item')
    );
    expect(
      listItems.every((x, i) => x.properties.caffFile == files[i])
    ).toBeTrue();
  });
});

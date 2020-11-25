import { TestBed } from '@angular/core/testing';

import { CafffileService } from './cafffile.service';

describe('CafffileService', () => {
  let service: CafffileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CafffileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { UrlManagerService } from './url-manager.service';

describe('Service: UrlManager', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UrlManagerService]
    });
  });

  it('should ...', inject([UrlManagerService], (service: UrlManagerService) => {
    expect(service).toBeTruthy();
  }));
});

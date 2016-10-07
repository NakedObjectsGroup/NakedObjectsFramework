/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RepLoaderService } from './rep-loader.service';

describe('Service: RepLoader', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RepLoaderService]
    });
  });

  it('should ...', inject([RepLoaderService], (service: RepLoaderService) => {
    expect(service).toBeTruthy();
  }));
});

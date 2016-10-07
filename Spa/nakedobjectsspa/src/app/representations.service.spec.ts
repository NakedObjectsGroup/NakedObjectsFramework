/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RepresentationsService } from './representations.service';

describe('Service: Representations', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RepresentationsService]
    });
  });

  it('should ...', inject([RepresentationsService], (service: RepresentationsService) => {
    expect(service).toBeTruthy();
  }));
});

/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { MaskConfigService } from './mask-config.service';

describe('Service: MaskConfig', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MaskConfigService]
    });
  });

  it('should ...', inject([MaskConfigService], (service: MaskConfigService) => {
    expect(service).toBeTruthy();
  }));
});

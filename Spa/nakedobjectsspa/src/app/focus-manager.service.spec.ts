/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { FocusManagerService } from './focus-manager.service';

describe('Service: FocusManager', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FocusManagerService]
    });
  });

  it('should ...', inject([FocusManagerService], (service: FocusManagerService) => {
    expect(service).toBeTruthy();
  }));
});

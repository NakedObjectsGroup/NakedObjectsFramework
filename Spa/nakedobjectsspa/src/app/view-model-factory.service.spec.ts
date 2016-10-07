/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ViewModelFactoryService } from './view-model-factory.service';

describe('Service: ViewModelFactory', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ViewModelFactoryService]
    });
  });

  it('should ...', inject([ViewModelFactoryService], (service: ViewModelFactoryService) => {
    expect(service).toBeTruthy();
  }));
});

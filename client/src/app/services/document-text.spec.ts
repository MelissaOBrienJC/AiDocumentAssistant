import { TestBed } from '@angular/core/testing';

import { DocumentText } from './document-text';

describe('DocumentText', () => {
  let service: DocumentText;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DocumentText);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

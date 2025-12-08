import { TestBed } from '@angular/core/testing';

import { BoardMember } from './board-member';

describe('BoardMember', () => {
  let service: BoardMember;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardMember);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

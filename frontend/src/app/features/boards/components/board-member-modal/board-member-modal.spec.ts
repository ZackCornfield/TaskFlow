import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMemberModal } from './board-member-modal';

describe('BoardMemberModal', () => {
  let component: BoardMemberModal;
  let fixture: ComponentFixture<BoardMemberModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardMemberModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMemberModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

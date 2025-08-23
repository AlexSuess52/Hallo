import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuizSession } from './quiz-session';

describe('QuizSession', () => {
  let component: QuizSession;
  let fixture: ComponentFixture<QuizSession>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizSession]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuizSession);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

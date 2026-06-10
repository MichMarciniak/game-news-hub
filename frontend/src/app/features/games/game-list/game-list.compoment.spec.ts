import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameListCompoment } from './game-list.compoment';

describe('GameListCompoment', () => {
  let component: GameListCompoment;
  let fixture: ComponentFixture<GameListCompoment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameListCompoment],
    }).compileComponents();

    fixture = TestBed.createComponent(GameListCompoment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

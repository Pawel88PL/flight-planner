import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsertFlyDataComponent } from './insert-fly-data.component';

describe('InsertFlyDataComponent', () => {
  let component: InsertFlyDataComponent;
  let fixture: ComponentFixture<InsertFlyDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsertFlyDataComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InsertFlyDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

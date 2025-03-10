import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AircraftsListComponent } from './aircrafts-list.component';

describe('AircraftsListComponent', () => {
  let component: AircraftsListComponent;
  let fixture: ComponentFixture<AircraftsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AircraftsListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AircraftsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

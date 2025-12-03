import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlansAndPricing } from './plans-and-pricing';

describe('PlansAndPricing', () => {
  let component: PlansAndPricing;
  let fixture: ComponentFixture<PlansAndPricing>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlansAndPricing]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlansAndPricing);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

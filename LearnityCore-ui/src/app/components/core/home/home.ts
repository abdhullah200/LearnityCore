import { Component } from '@angular/core';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { PlansAndPricing } from "../plans-and-pricing/plans-and-pricing";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CarouselModule, PlansAndPricing],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {

}

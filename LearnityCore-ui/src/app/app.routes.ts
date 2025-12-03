import { Routes } from '@angular/router';
import { Home } from './components/core/home/home';
import { AboutUs } from './components/core/about-us/about-us';

export const routes: Routes = [
	{ path: '', component: Home },
	{ path: 'about', component: AboutUs }
];

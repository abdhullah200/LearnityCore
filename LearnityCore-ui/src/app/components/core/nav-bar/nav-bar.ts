import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ThemeService } from '../../../shared/theme/theme.service';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './nav-bar.html',
  styleUrl: './nav-bar.css',
})
export class NavBar {

  private readonly themeService = inject(ThemeService);
  protected readonly isDarkMode = this.themeService.isDarkMode;
  protected readonly themeLabel = computed(() => this.isDarkMode() ? 'Light mode' : 'Dark mode');

  protected loginDisplay = false;
  protected profilePictureUrl: string | null = null;

  protected toggleTheme(): void {
    this.themeService.toggle();
  }

  protected loginRedirect(): void {
    console.info('Redirect to login');
  }

  protected logout(): void {
    console.info('Perform logout');
  }

}

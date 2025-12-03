import { DOCUMENT } from '@angular/common';
import { Injectable, computed, effect, inject, signal } from '@angular/core';

type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly documentRef = inject(DOCUMENT);
  private readonly storageKey = 'learnity-theme';
  private readonly modeSignal = signal<ThemeMode>('light');

  readonly mode = this.modeSignal.asReadonly();
  readonly isDarkMode = computed(() => this.modeSignal() === 'dark');

  constructor() {
    const initial = this.documentRef?.defaultView?.localStorage.getItem(this.storageKey) as ThemeMode | null;
    if (initial === 'dark') {
      this.modeSignal.set('dark');
    }

    effect(() => {
      const mode = this.modeSignal();
      this.applyMode(mode);
      this.documentRef?.defaultView?.localStorage.setItem(this.storageKey, mode);
    });
  }

  toggle(): void {
    this.modeSignal.update((current) => (current === 'dark' ? 'light' : 'dark'));
  }

  private applyMode(mode: ThemeMode): void {
    const root = this.documentRef?.documentElement;
    if (!root) {
      return;
    }

    root.setAttribute('data-theme', mode);
  }
}

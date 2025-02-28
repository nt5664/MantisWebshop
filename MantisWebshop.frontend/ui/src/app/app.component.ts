import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NxWelcomeComponent } from './nx-welcome.component';
import { ProductListComponent } from '../lib/shop/product-list.component';
import { Menubar } from 'primeng/menubar';
import { Avatar } from 'primeng/avatar';
import { NgClass, NgIf } from '@angular/common';
import { Badge } from 'primeng/badge';
import { Ripple } from 'primeng/ripple';
import { TokenService } from '../lib/services/token-service.service';
import { StateComponent } from '../lib/auth/state/state.component';

interface MenuItem {
  label: string;
  path: string;
  items: MenuItem[];
  requireAuth: boolean;
}

@Component({
  imports: [
    NxWelcomeComponent,
    RouterModule,
    ProductListComponent,
    Menubar,
    Avatar,
    NgIf,
    NgClass,
    Badge,
    Ripple,
    StateComponent,
  ],
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'Mantis Shop';

  constructor(private tokenService: TokenService) {}

  menuItems = [
    {
      label: 'Shop',
      path: '/'
    },
    {
      label: 'Cart',
      path: '/cart',
      requireAuth: true
    },
    {
      label: 'Orders',
      path: '/orders',
      requireAuth: true
    }
  ];

  get isLoggedIn(): boolean {
    return this.tokenService.isLoggedIn();
  }
}

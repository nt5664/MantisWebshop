import { Component, OnInit } from '@angular/core';
import { CartItem } from '../../models/cart-item';
import { HttpClient } from '@angular/common/http';
import { TokenService } from '../../services/token-service.service';
import { Router } from '@angular/router';
import { ActionResponse } from '../../models/action-response';
import { Message } from 'primeng/message';
import { NgClass, NgIf, NgOptimizedImage } from '@angular/common';
import { OrderList } from 'primeng/orderlist';
import { flatMap, mergeMap } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [Message, NgIf, OrderList, NgClass, NgOptimizedImage],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent implements OnInit {
  protected items: CartItem[] | null = null;

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.http
      .get<ActionResponse>('https://localhost:7153/shop/cart')
      .subscribe({
        next: (res) => (this.items = res.data),
        error: (error) => console.log('***** ERROR *****', error),
      });
  }

  removeFromCart(id: string) {
    const body = {
      id: id,
      quantity: 0,
      override: true
    };

    this.http.post('https://localhost:7153/shop/cart', body).subscribe({})
  }
}

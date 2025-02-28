import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionResponse } from '../../models/action-response';
import { Product } from '../../models/product';
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';
import { TokenService } from '../../services/token-service.service';
import { FormsModule } from '@angular/forms';
import { InputNumber } from 'primeng/inputnumber';
import { Message } from 'primeng/message';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [Card, Button, FormsModule, InputNumber, Message, NgIf],
  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  private productObj: Product | null = null;

  quantity = 1;
  message: string | null = null;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    const prodId = this.route.snapshot.params['id'];
    this.http
      .get<ActionResponse>(`https://localhost:7153/shop/${prodId}`, {
        observe: 'response',
      })
      .subscribe({
        next: (res) => {
          if (res.status !== 200) {
            return this.router.navigate(['/404']);
          }

          return (this.productObj = res.body?.data || null);
        },
      });
  }

  get product(): Product | null {
    return this.productObj;
  }

  onAddToCart() {
    if (!this.productObj) {
      return this.router.navigate(['/404']);
    }

    if (!this.tokenService.isLoggedIn()) {
      return this.router.navigate(['/login']);
    }

    const body = {
      id: this.productObj.id,
      quantity: this.quantity,
      override: false,
    };
    return this.http
      .post<ActionResponse>('https://localhost:7153/shop/cart', body)
      .subscribe({
        next: (res) => (this.message = res.message),
        error: (error) => (this.message = error.message),
      });
  }
}

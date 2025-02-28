import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataView, DataViewModule } from 'primeng/dataview';
import { Tag } from 'primeng/tag';
import { Button } from 'primeng/button';
import { Product } from '../models/product';
import { HttpClient } from '@angular/common/http';
import { ActionResponse } from '../models/action-response';
import { Router } from '@angular/router';
import { TokenService } from '../services/token-service.service';

@Component({
  selector: 'app-shop-list',
  standalone: true,
  imports: [CommonModule, Tag, DataView, Button],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css',
})
export class ProductListComponent implements OnInit {
  public products: Product[] = [];

  constructor(private http: HttpClient, private router: Router, private tokenService: TokenService) {}

  ngOnInit() {
    this.getProducts();
  }

  getProducts() {
    this.http.get<ActionResponse>('https://localhost:7153/shop').subscribe({
      next: res=> this.products = res.data,
      error: error => console.log('***** ERROR *****', error),
    });
  }

  onAddToCart(id: string) {
    if (!this.tokenService.isLoggedIn()) {
      return this.router.navigate(['/login']);
    }

    const body = {
      id: id,
      quantity: 1,
      override: false
    };
    return this.http.post<ActionResponse>('https://localhost:7153/shop/cart', body).subscribe({
      next: res=> this.products = res.data,
      error: error => console.log(error)
    })
  }
}

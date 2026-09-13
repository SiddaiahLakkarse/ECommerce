import { Component, OnInit } from '@angular/core';
import { CartItem, CartService } from '../core/cart/cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  items: CartItem[] = [];
  total = 0;

  constructor(private cartService: CartService) { }

  ngOnInit(): void {
    this.cartService.items$.subscribe(items => {
      this.items = items;
      this.total = this.cartService.getTotal(items);
    });
  }

  updateQuantity(item: CartItem, event: Event): void {
    const quantity = Number((event.target as HTMLInputElement).value);
    this.cartService.updateQuantity(item.product.id, quantity);
  }

  removeItem(productId: number): void {
    this.cartService.removeItem(productId);
  }

  clearCart(): void {
    this.cartService.clear();
  }
}

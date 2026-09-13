import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from '../../shared/models/product';

export interface CartItem {
  product: Product;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly storageKey = 'ecommerce-cart';
  private readonly itemsSubject = new BehaviorSubject<CartItem[]>(this.loadItems());
  readonly items$ = this.itemsSubject.asObservable();

  addItem(product: Product, quantity = 1): void {
    const items = [...this.itemsSubject.value];
    const existingItem = items.find(item => item.product.id === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      items.push({ product, quantity });
    }

    this.updateItems(items);
  }

  updateQuantity(productId: number, quantity: number): void {
    const items = this.itemsSubject.value
      .map(item => item.product.id === productId ? { ...item, quantity } : item)
      .filter(item => item.quantity > 0);

    this.updateItems(items);
  }

  removeItem(productId: number): void {
    this.updateItems(this.itemsSubject.value.filter(item => item.product.id !== productId));
  }

  clear(): void {
    this.updateItems([]);
  }

  getItemCount(items = this.itemsSubject.value): number {
    return items.reduce((total, item) => total + item.quantity, 0);
  }

  getTotal(items = this.itemsSubject.value): number {
    return items.reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  private updateItems(items: CartItem[]): void {
    this.itemsSubject.next(items);
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private loadItems(): CartItem[] {
    try {
      const storedItems = localStorage.getItem(this.storageKey);
      return storedItems ? JSON.parse(storedItems) : [];
    } catch {
      return [];
    }
  }
}

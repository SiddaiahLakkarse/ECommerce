import { Component, OnInit } from '@angular/core';
import { ShopService } from './shop.service';
import { Pagination } from '../shared/models/pagination';
import { Product } from '../shared/models/product';
import { Brand } from '../shared/models/brand';
import { Type } from '../shared/models/type';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {

  products: Product[] = [];
  brands: Brand[] = [];
  types: Type[] = [];

  constructor(private shopService: ShopService) { }

  ngOnInit(): void {
    this.shopService.getProducts().subscribe({
      next: response => this.products = response.data,
      error: error => console.error()
    });

    this.getProducts();
    this.getBrands();
    this.getTypes();

  }

  onSortSelected(event: any) {
  }

  onSearch() {
  }

  onReset() {
  }

  getProducts() {
    this.shopService.getProducts().subscribe({
      next: response => {
        this.products = response.data;
      },
      error: error => console.log(error)
    })
  }

  getBrands() {
    this.shopService.getBrands().subscribe({
      next: response => this.brands = [{ id: 0, name: 'All' }, ...response],
      error: error => console.log(error)
    })
  }

  getTypes() {
    this.shopService.getTypes().subscribe({
      next: response => this.types = [{ id: 0, name: 'All' }, ...response],
      error: error => console.log(error)
    })
  }

  onBrandSelected(brandId: number) {
  }

  onTypeSelected(typeId: number) {
  }
}

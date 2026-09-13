import { Component, OnInit } from '@angular/core';
import { ShopService } from '../shop/shop.service';
import { Product } from '../shared/models/product';
import { ShopParams } from '../shared/models/shopParams';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  featuredProducts: Product[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private shopService: ShopService) { }

  ngOnInit(): void {
    const params = new ShopParams();
    params.pageSize = 4;

    this.shopService.getProducts(params).subscribe({
      next: response => {
        this.featuredProducts = response.data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Featured products could not be loaded.';
      }
    });
  }

}

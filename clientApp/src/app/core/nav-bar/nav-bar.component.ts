import { Component } from '@angular/core';
import { CartService } from '../cart/cart.service';
import { map } from 'rxjs';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent {
  cartItemCount$ = this.cartService.items$.pipe(
    map(items => this.cartService.getItemCount(items))
  );

  constructor(private cartService: CartService) { }

}

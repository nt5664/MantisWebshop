import { Route } from '@angular/router';
import { LoginComponent } from '../lib/auth/login/login.component';
import { ProductListComponent } from '../lib/shop/product-list.component';
import { loggedOutGuard } from '../lib/guards/logged-out.guard';
import { RegisterComponent } from '../lib/auth/register/register.component';
import { ProductComponent } from '../lib/shop/product/product.component';
import { FourOFourComponent } from '../lib/errors/four-o-four/four-o-four.component';
import { CartComponent } from '../lib/shop/cart/cart.component';

export const appRoutes: Route[] = [
  { path: '', component: ProductListComponent },
  { path: 'login', component: LoginComponent, canActivate: [loggedOutGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [loggedOutGuard] },
  { path: 'products/:id', component: ProductComponent },
  { path: 'cart', component: CartComponent },
  { path: '404', component: FourOFourComponent }
];

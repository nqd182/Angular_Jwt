import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { authGuard } from './auth-guard';
import { Home } from './pages/home/home';
import { Product } from './pages/product/product';

export const routes: Routes = [
    {path: 'login', component: Login},
    {path: 'register', component: Register},
    {path: 'home', component: Home, canActivate: [authGuard]},
    {path: 'product', component: Product, canActivate: [authGuard]},
    {path: '', redirectTo: 'login', pathMatch: 'full'}
];

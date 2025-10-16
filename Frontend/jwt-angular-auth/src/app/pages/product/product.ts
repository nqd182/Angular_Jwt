import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MyProduct } from '../../services/product';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AuthService } from '../../services/auth';

declare var bootstrap: any;

@Component({
  selector: 'app-product',
  imports: [CommonModule, FormsModule, CurrencyPipe],
  standalone: true,
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product implements OnInit{
  products: any[] = []
  newProduct = {
    name: '', description: '', price: 0, imageUrl: '', maLoai: ''
  }
  editProduct: any = {}; // dữ liệu của sản phẩm đang sửa
  constructor(private productService: MyProduct, private authService: AuthService) {};
  ngOnInit() {
    this.loadProducts();
  }
  loadProducts() {
    this.productService.getAll().subscribe(res => this.products = res.data);
  }
  addProduct() {
    this.productService.create(this.newProduct).subscribe({

      next: (res) =>{
      this.loadProducts();
      this.newProduct = { name: '', description: '', price: 0, imageUrl: '', maLoai: '' };// update lại form nhâp 
      },
      error: (err) => {
        alert('Thêm thất bại')
      }
    })
  }
  deleteProduct(id: number) {
    this.productService.delete(id).subscribe(() => this.loadProducts()); 
  }
  //  Mở modal sửa
  openEdit(p: any) {
    this.editProduct = { ...p }; // clone dữ liệu
    const modal = new bootstrap.Modal(document.getElementById('editModal'));
    modal.show();
  }

  //  Cập nhật sản phẩm
  updateProduct() {
    this.productService.update(this.editProduct.id, this.editProduct).subscribe(() => {
      this.loadProducts();
      const modal = bootstrap.Modal.getInstance(document.getElementById('editModal'));
      modal.hide();
    });
  }
  // đăng xuất
  clickLogout(){
    this.authService.logout();
  }
}

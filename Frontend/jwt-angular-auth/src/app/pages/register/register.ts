import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, FormsModule, CommonModule], // thêm router link để chuyển trang
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  hoTen = '';
  userName = '';
  email = '';
  password = '';
  constructor(private authService: AuthService, private router: Router){}
  onRegister(){
    const userData = {hoTen: this.hoTen, userName: this.userName, password: this.password, email: this.email};
    this.authService.register(userData).subscribe({
      next: (res) =>{
        alert('Tạo tài khoán thành công');
        this.router.navigate(['/login']);
      },
      error: (err) =>{
        alert('Tạo tài khoản thất bại, hãy thử lại sau');

      }
    })
  }
}

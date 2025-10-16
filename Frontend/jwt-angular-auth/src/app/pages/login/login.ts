import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  userName = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) {}  
  
  onLogin() {
    const userData = {userName: this.userName, password: this.password};

    this.auth.login(userData).subscribe({
      next: (res) => {
        localStorage.setItem('accessToken', res.data.accessToken);
        localStorage.setItem('refreshToken', res.data.refreshToken);
        alert('Đăng nhập thành công');
        setTimeout(() =>  this.router.navigate(['/product']), 200);
      },
      error: (err) =>{
        console.log(err);
        alert('Sai tài khoản hoặc mật khẩu');
      }
    })

  }
}

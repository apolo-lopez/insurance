import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Login } from '../login/login';



@NgModule({
  imports: [
    CommonModule,
    Login
  ]
})
export class AuthModule { }

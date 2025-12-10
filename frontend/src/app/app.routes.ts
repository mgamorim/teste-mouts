import { Routes } from '@angular/router';
import { LoginComponent } from './components/login.component';
import { EmployeesComponent } from './components/employees.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'employees', component: EmployeesComponent },
  { path: '', pathMatch: 'full', redirectTo: 'login' }
];

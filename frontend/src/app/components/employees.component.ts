import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService, Employee } from '../services/employee.service';

@Component({
  standalone: true,
  selector: 'app-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.css'],
  imports: [CommonModule]
})
export class EmployeesComponent implements OnInit {
  employees: Employee[] = [];

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.employeeService.getEmployees().subscribe({
      next: emps => this.employees = emps,
      error: err => console.error('Error getting employees', err)
    });
  }
}

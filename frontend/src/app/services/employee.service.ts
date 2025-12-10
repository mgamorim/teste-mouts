import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  dateOfBirth: string;
  role: number;
  managerId?: number;
  managerName?: string;
  phones: string[];
}

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private apiUrl = 'http://localhost:8080/api/employees';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : ''
    });
  }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl, { headers: this.getAuthHeaders() });
  }
}

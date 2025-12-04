import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DateService {
  constructor() {}

  // Converts a Date object to a string in 'YYYY-MM-DD' format
  toDateString(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  // Converts a Date object to a string in 'MM/DD/YYYY' format
  toLocaleDateString(date: Date): string {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const year = date.getFullYear();
    return `${month}/${day}/${year}`;
  }

  // Converts a string in 'YYYY-MM-DD' format to a Date object
  fromDateString(dateString: string): Date {
    const [year, month, day] = dateString.split('-').map(Number);
    return new Date(year, month - 1, day);
  }

  // Converts a string in 'MM/DD/YYYY' format to a Date object
  fromLocaleDateString(dateString: string): Date {
    const [month, day, year] = dateString.split('/').map(Number);
    return new Date(year, month - 1, day);
  }
}

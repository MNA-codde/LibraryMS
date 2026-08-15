export interface User {
  fullName: string;
  role: 'Admin' | 'Member';
}

export interface AuthContextType {
  user: User | null;
  login: (token: string, fullName: string, role: string) => void;
  logout: () => void;
}

export interface Book {
  id: string;
  title: string;
  isbn: string;
  authorName: string;
  categoryName: string;
  totalCopies: number;
  copiesAvailable: number;
}

export interface Author {
  id: string;
  fullName: string;
}

export interface BookPopularity {
  bookId: string;
  title: string;
  borrowCount: number;
}


export interface Category {
  id: string;
  name: string;
}

export interface Location {
  id: string;
  name: string;
  latitude: number;
  longitude: number;
}

export interface LocationStats {
  locationId: string;
  name: string;
  latitude: number;
  longitude: number;
  bookCount: number;
}


export interface BorrowRecord {
  id: string;
  bookTitle: string;
  borrowDate: string;
  dueDate: string;
  status: 'Borrowed' | 'Returned' | 'Overdue' | 'Lost';
}
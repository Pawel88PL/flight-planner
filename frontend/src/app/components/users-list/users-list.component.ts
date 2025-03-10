import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltip } from '@angular/material/tooltip';
import { MatPaginatorIntlPolish } from '../../classes/mat-paginator-polish';

import { AuthService } from '../../services/auth.service';
import { JwtService } from '../../services/jwt.service';
import { ToastrService } from 'ngx-toastr';
import { UsersService } from '../../services/users.service';

import { User } from '../../models/user-model';
import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatTableModule,
    MatTooltip,

    ReactiveFormsModule
  ],
  providers: [
    { provide: MatPaginatorIntl, useClass: MatPaginatorIntlPolish }
  ],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.css'
})

export class UsersListComponent implements OnInit {

  displayedColumns: string[] = ['index', 'firstName', 'lastName', 'role', 'email', 'lastSuccessfulLogin', 'isActive', 'actions'];
  dataSource = new MatTableDataSource<User>([]);
  pageIndex: number = 0;
  pageSize: number = 5;
  rowNumber: number = 0;
  totalRecords: number = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort!: MatSort;

  errorMessage: string | null = null;
  isLoading: boolean = true;
  isSortInitialized: boolean = false;

  searchForm!: FormGroup;
  searchQuery: string = '';

  constructor(
    private authService: AuthService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private usersService: UsersService,
    private jwtService: JwtService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.initialeSearchForm();
    this.loadUsers(0, this.pageSize);
    this.searchQueryChanges();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  initialeSearchForm() {
    this.searchForm = this.formBuilder.group({
      query: [''],
    })
  }

  initializeSort() {
    if (!this.isSortInitialized) {
      setTimeout(() => {
        this.dataSource.sort = this.sort;
        this.sort.sortChange.subscribe((event) => this.onSortChange(event));
      }, 500);
      this.isSortInitialized = true;
    }
  }

  loadUsers(pageIndex: number, pageSize: number, sortColumn?: string, sortDirection?: string, searchQuery?: string): void {
    const params = {
      pageNumber: pageIndex + 1,
      pageSize: pageSize,
      sortColumn: sortColumn || 'role',
      sortDirection: sortDirection || 'asc',
      searchQuery: searchQuery || ''
    };

    this.usersService.getUsersPaged(params).subscribe({
      next: (response) => {
        this.dataSource.data = response.data;
        this.totalRecords = response.totalRecords;
        this.initializeSort();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error.message ?? 'Wystąpił błąd podczas pobierania danych';
        this.toastr.error(this.errorMessage!, 'Błąd');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  deleteUser(userId: string) {
    this.authService.deleteUser(userId).subscribe({
      next: () => {
        this.toastr.success('Użytkownik został usunięty', 'Sukces');
        this.loadUsers(this.paginator.pageIndex, this.paginator.pageSize);
      },
      error: (error) => {
        this.toastr.error('Wystąpił błąd podczas usuwania użytkownika', 'Błąd');
        console.error(error);
      }
    });
  }

  onDeleteUser(userId: string): void {
    if (userId === this.jwtService.getUserId()) {
      this.toastr.error('Nie możesz usunąć swojego konta', 'Błąd');
      return;
    }

    let message = 'Czy na pewno chcesz usunąć tego użytkownika?';

    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '400px',
      height: '180px',
      data: { message: message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteUser(userId);
      }
    });
  }

  onPageChange(event: any): void {
    this.loadUsers(event.pageIndex, event.pageSize);
    this.rowNumber = event.pageIndex * event.pageSize;
  }

  onSearch(query: string): void {
    const searchQuery = query.trim();
    this.loadUsers(0, this.pageSize, undefined, undefined, searchQuery);
  }

  onSortChange(event: any): void {
    this.loadUsers(this.paginator.pageIndex, this.paginator.pageSize, event.active, event.direction);
  }

  openEditUserComponent(user: User) {
    // this.adminService.setSelectedUser(user);
    // this.adminService.setComponent('userEdit');
  }


  selectUser(user: User) {
    this.openEditUserComponent(user);
  }

  searchQueryChanges() {
    this.searchForm.get('query')?.valueChanges.pipe(
      debounceTime(100),
      distinctUntilChanged()
    ).subscribe(query => {
      this.onSearch(query);
    });
  }
}
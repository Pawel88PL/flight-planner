import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subscription } from 'rxjs';
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

import { AircraftService } from '../../services/aircraft.service';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

import { AircraftModel } from '../../models/aircraft.model';
import { User } from '../../models/user-model';
import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';
import { Router } from '@angular/router';
import { DataService } from '../../services/data.service';


@Component({
  selector: 'app-aircrafts-list',
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
  templateUrl: './aircrafts-list.component.html',
  styleUrl: './aircrafts-list.component.css'
})

export class AircraftsListComponent implements OnInit {

  displayedColumns: string[] = ['index', 'name', 'manufacturer', 'model', 'cruiseSpeed', 'range', 'maxCrosswind', 'dateAdded', 'actions'];
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

  subscriptions: Subscription = new Subscription();

  constructor(
    private aircraftService: AircraftService,
    private authService: AuthService,
    private dataService: DataService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.initialeSearchForm();
    this.loadAircrafts(0, this.pageSize);
    this.messageSubscription();
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

  loadAircrafts(pageIndex: number, pageSize: number, sortColumn?: string, sortDirection?: string, searchQuery?: string): void {
    const params = {
      pageNumber: pageIndex + 1,
      pageSize: pageSize,
      sortColumn: sortColumn || 'dateAdded',
      sortDirection: sortDirection || 'asc',
      searchQuery: searchQuery || ''
    };

    this.aircraftService.getAircraftsPaged(params).subscribe({
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

  deleteAircraft(userId: string) {
    this.authService.deleteUser(userId).subscribe({
      next: () => {
        this.toastr.success('Samolot został usunięty', 'Sukces');
        this.loadAircrafts(this.paginator.pageIndex, this.paginator.pageSize);
      },
      error: (error) => {
        this.toastr.error('Wystąpił błąd podczas usuwania samolotu', 'Błąd');
        console.error(error);
      }
    });
  }

  messageSubscription(): void {
    this.subscriptions.add(
      this.dataService.errorMessage$.subscribe(message => {
        if (message)
          this.toastr.error(message, 'Błąd');
      })
    );

    this.subscriptions.add(
      this.dataService.successMessage$.subscribe(message => {
        if (message)
          this.toastr.success(message, 'Sukces');
      })
    );
  }

  onDeleteAircraft(userId: string): void {
    let message = 'Czy na pewno chcesz usunąć ten samolot?';

    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '400px',
      height: '180px',
      data: { message: message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteAircraft(userId);
      }
    });
  }

  onPageChange(event: any): void {
    this.loadAircrafts(event.pageIndex, event.pageSize);
    this.rowNumber = event.pageIndex * event.pageSize;
  }

  onSearch(query: string): void {
    const searchQuery = query.trim();
    this.loadAircrafts(0, this.pageSize, undefined, undefined, searchQuery);
  }

  onSortChange(event: any): void {
    this.loadAircrafts(this.paginator.pageIndex, this.paginator.pageSize, event.active, event.direction);
  }

  openAircraftEditComponent(id: number) {
    this.router.navigate(['/aircraft-edit', id]);
  }

  openAircraftAddComponent() {
    this.router.navigate(['/aircraft-add']);
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

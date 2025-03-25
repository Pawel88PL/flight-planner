import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subscription } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

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

import { DataService } from '../../services/data.service';
import { FlightPlanService } from '../../services/flight-plan.service';
import { JwtService } from '../../services/jwt.service';
import { ToastrService } from 'ngx-toastr';

import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-flight-plans-list',
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
  templateUrl: './flight-plans-list.component.html',
  styleUrl: './flight-plans-list.component.css'
})

export class FlightPlansListComponent implements OnInit {

  displayedColumns: string[] = ['index', 'id', 'createdAt', 'departureAirport', 'arrivalAirport', 'departureTime', 'aircraftName', 'actions'];

  dataSource = new MatTableDataSource<any>([]);
  pageIndex: number = 0;
  pageSize: number = 5;
  rowNumber: number = 0;
  totalRecords: number = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort!: MatSort;

  errorMessage: string | null = null;
  isLoading: boolean = true;
  isSortInitialized: boolean = false;
  userId: string | null = null;

  searchForm!: FormGroup;
  searchQuery: string = '';

  subscriptions: Subscription = new Subscription();

  constructor(
    private dataService: DataService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private jwtService: JwtService,
    private router: Router,
    private toastr: ToastrService,
    private flightPlanService: FlightPlanService
  ) { }

  ngOnInit() {
    this.getUserId();
    this.initialeSearchForm();
    this.messageSubscription();
    this.searchQueryChanges();
    this.loadFlightPlans(0, this.pageSize);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  getUserId(): void {
    this.userId = this.jwtService.getUserId();
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

  loadFlightPlans(pageIndex: number, pageSize: number, sortColumn?: string, sortDirection?: string, searchQuery?: string): void {
    const params = {
      pageNumber: pageIndex + 1,
      pageSize: pageSize,
      sortColumn: sortColumn || 'createdAt',
      sortDirection: sortDirection || 'asc',
      searchQuery: searchQuery || '',
      userId: this.userId
    };

    this.flightPlanService.getFlightPlansPaged(params).subscribe({
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

  deleteFlightPlan(id: number): void {
    this.flightPlanService.deleteFlightPlan(id).subscribe({
      next: () => {
        this.toastr.success('Plan lotu został usunięty', 'Sukces');
        this.loadFlightPlans(this.paginator.pageIndex, this.paginator.pageSize);
      },
      error: (error) => {
        this.toastr.error('Wystąpił błąd podczas usuwania planu lotu', 'Błąd');
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

  onDeleteFlightPlan(id: number): void {
    let message = 'Czy na pewno chcesz usunąć ten plan lotu?';

    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '400px',
      height: '180px',
      data: { message: message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteFlightPlan(id);
      }
    });
  }

  onPageChange(event: any): void {
    this.loadFlightPlans(event.pageIndex, event.pageSize);
    this.rowNumber = event.pageIndex * event.pageSize;
  }

  onSearch(query: string): void {
    const searchQuery = query.trim();
    this.loadFlightPlans(0, this.pageSize, undefined, undefined, searchQuery);
  }

  onSortChange(event: any): void {
    this.loadFlightPlans(this.paginator.pageIndex, this.paginator.pageSize, event.active, event.direction);
  }

  searchQueryChanges() {
    this.searchForm.get('query')?.valueChanges.pipe(
      debounceTime(100),
      distinctUntilChanged()
    ).subscribe(query => {
      this.onSearch(query);
    });
  }

  showFlightPlanDetails(flightPlanId: number): void {
    this.router.navigate(['/response', flightPlanId]);
  }
}
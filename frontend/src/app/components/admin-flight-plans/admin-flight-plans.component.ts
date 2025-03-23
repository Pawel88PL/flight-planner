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

import { AircraftService } from '../../services/aircraft.service';
import { DataService } from '../../services/data.service';
import { FlightPlanService } from '../../services/flight-plan.service';
import { ToastrService } from 'ngx-toastr';

import { AircraftModel } from '../../models/aircraft.model';
import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';


@Component({
  selector: 'app-admin-flight-plans',
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
  templateUrl: './admin-flight-plans.component.html',
  styleUrl: './admin-flight-plans.component.css'
})
export class AdminFlightPlansComponent {

  displayedColumns: string[] = ['index', 'id', 'createdAt', 'departureAirport', 'arrivalAirport', 'departureTime', 'aircraftName', 'userFullName', 'actions'];

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

  searchForm!: FormGroup;
  searchQuery: string = '';

  subscriptions: Subscription = new Subscription();

  constructor(
    private aircraftService: AircraftService,
    private dataService: DataService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private router: Router,
    private toastr: ToastrService,
    private flightPlanService: FlightPlanService
  ) { }

  ngOnInit() {
    this.initialeSearchForm();
    this.messageSubscription();
    this.searchQueryChanges();
    this.loadFlightPlans(0, this.pageSize);
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

  loadFlightPlans(pageIndex: number, pageSize: number, sortColumn?: string, sortDirection?: string, searchQuery?: string): void {
    const params = {
      pageNumber: pageIndex + 1,
      pageSize: pageSize,
      sortColumn: sortColumn || 'createdAt',
      sortDirection: sortDirection || 'asc',
      searchQuery: searchQuery || ''
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

  deleteAircraft(aircraftId: number): void {
    this.aircraftService.deleteAircraft(aircraftId).subscribe({
      next: () => {
        this.toastr.success('Samolot został usunięty', 'Sukces');
        this.loadFlightPlans(this.paginator.pageIndex, this.paginator.pageSize);
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

  onDeleteAircraft(aircraftId: number): void {
    let message = 'Czy na pewno chcesz usunąć ten samolot?';

    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '400px',
      height: '180px',
      data: { message: message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteAircraft(aircraftId);
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
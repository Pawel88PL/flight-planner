import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

import { AdminComponent } from './components/admin/admin.component';
import { FlightPlansListComponent } from './components/flight-plans-list/flight-plans-list.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ResponseComponent } from './components/response/response.component';
import { UsersListComponent } from './components/users-list/users-list.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'admin', component: AdminComponent },
    { path: 'flight-plans', component: FlightPlansListComponent },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'response/:id', component: ResponseComponent },
    { path: 'users-list', component: UsersListComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})

export class AppRoutingModule { }
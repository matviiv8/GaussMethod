import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { GaussMethodSolverComponent } from './components/gauss-method-solver/gauss-method-solver.component';

import { AppComponent } from './components/app/app.component';

@NgModule({
  declarations: [
    AppComponent,
    GaussMethodSolverComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

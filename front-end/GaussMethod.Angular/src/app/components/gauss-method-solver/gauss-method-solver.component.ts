import { Component } from '@angular/core';
import { GaussMethodService } from '../../services/gauss-method.service';
import { GaussMethodResult } from '../../models/gaussMethodResult';
import { GaussMethodInput } from 'src/app/models/gaussMethodInput';

@Component({
  selector: 'gauss-method-solver',
  templateUrl: './gauss-method-solver.component.html',
  styleUrls: ['./gauss-method-solver.component.css']
})
export class GaussMethodSolverComponent{
  precision = 4;
  n = 7;
  selectedOption = 'manualInput';
  selectedSize = 3;
  matrix: number[][] = Array.from({ length: this.selectedSize }, () => Array(this.selectedSize).fill(0));
  vector: number[] = Array(this.selectedSize).fill(0);
  cardSize = '400px';
  result: GaussMethodResult | null = null;
  errorText = '';
  selectedFile: File | null = null;

  constructor(private gaussMethodService: GaussMethodService) { }

  updateMatrixAndVector() {
      this.matrix = Array.from({ length: this.selectedSize }, () => Array(this.selectedSize).fill(0));
      this.vector = Array.from({ length: this.selectedSize }).fill(0) as number[];
  
      this.cardSize = (this.selectedSize * 40 + 300) + 'px';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] || null;
  }
  
  onOptionChange(option: string) {
    this.selectedOption = option;
    this.result = null;
    this.resetErrorText();
    this.resetMatrixAndVector();
  }

  resetErrorText(){
    this.errorText = '';
  }
  
  resetMatrixAndVector() {
    this.cardSize = '400px';
    this.selectedSize = 3;
    this.updateMatrixAndVector();
  }

  calculate() {
    this.resetErrorText();
    
    if (this.selectedOption === 'manualInput') {
      const model: GaussMethodInput = {
        A: this.matrix,
        b: this.vector,
      };

      this.gaussMethodService.solveManually(model, this.precision)
        .subscribe(
          (response) => this.result = response,
          (error) => this.errorText = error.error
        );
    } else if (this.selectedOption === 'fileInput' && this.selectedFile) {
      this.gaussMethodService.solveFromFile(this.selectedFile, this.precision)
        .subscribe(
          (response) => this.result = response,
          (error) => this.errorText = error.error
        );
    } else if (this.selectedOption === 'generateSystem') {
      this.gaussMethodService.generateAndSimplifySystem(this.n, this.precision)
        .subscribe(
          (response) => this.result = response,
          (error) => this.errorText = error.error
        );
    }
  }
}
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Policy } from '../policies/policy.model';
import { PolicyService } from '../policies/policy.service';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { PolicyFormComponent } from "../policies/policy-form/policy-form.component";

@Component({
  selector: 'app-policies-list',
  imports: [NgIf, NgFor, CurrencyPipe, DatePipe, PolicyFormComponent],
  templateUrl: './policies-list.html',
  styleUrl: './policies-list.scss',
})
export class PoliciesList implements OnInit {
  policies: Policy[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;

  showForm = false;
  selectedPolicy?: Policy;

  constructor(private policyService: PolicyService, private cd: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.fetchPolicies();
  }

  fetchPolicies(): void {
    this.isLoading = true;
    this.policyService.getAll().subscribe({
      next: (data) => {
        this.policies = data;
        this.isLoading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching policies', err);
        this.isLoading = false;
        this.cd.detectChanges();
      },
    });
  }

  searchPolicies(): void {
    if (!this.searchTerm.trim()) {
      this.policyService.search(this.searchTerm).subscribe({
        next: (data) => {
          this.policies = data;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error searching policies', err);
          this.isLoading = false;
        }
      });
    }
  }

  openForm(policy?: Policy): void {
    this.selectedPolicy = policy;
    this.showForm = true;
  }

  onSaved(policy: Policy): void {
    this.showForm = false;
    this.selectedPolicy = undefined;
    this.fetchPolicies();
  }

  closePolicyForm(): void {
    this.selectedPolicy = undefined;
    this.showForm = false;
  }
}

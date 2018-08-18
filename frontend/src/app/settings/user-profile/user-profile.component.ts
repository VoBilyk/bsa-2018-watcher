import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../shared/models/user.model';
import { ToastrService } from '../../core/services/toastr.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.sass']
})
export class UserProfileComponent implements OnInit {

  constructor(private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private toastrService: ToastrService) { }

  public user: User;
  private userId: string;

  public userForm = this.fb.group({
    displayName: new FormControl({ value: '', disabled: true }, Validators.required),
    firstName: new FormControl({ value: '', disabled: true }, Validators.required),
    lastName: new FormControl({ value: '', disabled: true }, Validators.required),
    bio: new FormControl({ value: '', disabled: true })
  });

  ngOnInit() {
    this.authService.currentUser.subscribe(
      (userData) => {
        this.user = { ...userData };
        this.userId = userData.id;
        this.setUserData();
      }
    );
  }

  setUserData(): void {
    Object.keys(this.userForm.controls).forEach(field => {
      const control = this.userForm.get(field);
      control.setValue(this.user[field]);
      control.enable();
    });

    this.userForm.valueChanges.subscribe(value => {
      Object.keys(this.userForm.controls).forEach(field => {
        this.user[field] = this.userForm.get(field).value;
      });
    });
  }

  onImageSelected(files: FileList) {
    const reader = new FileReader();
    reader.onload = (event: any) => {
      this.user.photoURL = event.target.result;
    };

    reader.readAsDataURL(files.item(0));
  }

  onSubmit() {
    if (this.userForm.valid) {
      this.userService.update(this.userId, this.user).subscribe(value => {
        this.authService.updateCurrentUser(this.user);
        this.toastrService.success('Profile was updated');
      },
        err => {
          this.toastrService.error('Profile was not updated');
        });
    } else {
      Object.keys(this.userForm.controls).forEach(field => {
        const control = this.userForm.get(field);
        control.markAsDirty({ onlySelf: true });
      });
    }
  }
}

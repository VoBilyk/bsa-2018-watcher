import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';

import { AccordionModule, TabViewModule } from 'primeng/primeng';
import { PanelModule } from 'primeng/primeng';
import { ButtonModule } from 'primeng/components/button/button';
import { RadioButtonModule } from 'primeng/primeng';
import { DialogModule } from 'primeng/primeng';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';

import { LandingComponent } from './landing/landing.component';
import { CoreModule } from './core/core.module';
import { AuthorizationComponent } from './authorization/authorization.component';
import { AdminModule } from './admin/admin.module';
import { UserModule } from './user/user.module';
import { DashboardsModule } from './dashboards/dashboards.module';
import {NotificationsModule} from './notifications/notifications.module';

import { AngularFireModule } from 'angularfire2';
import { environment } from '../environments/environment';
import { AngularFireDatabaseModule } from 'angularfire2/database';
import { AngularFireAuthModule } from 'angularfire2/auth';
import { AuthService } from './core/services/auth.service';

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    AuthorizationComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    CoreModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    AccordionModule,
    PanelModule,
    ButtonModule,
    RadioButtonModule,
    DialogModule,
    AdminModule,
    UserModule,
    DashboardsModule,
    AngularFireModule.initializeApp(environment.firebase, 'watcherapp'),
    AngularFireDatabaseModule,
    AngularFireAuthModule,
    NotificationsModule,
    TabViewModule
  ],
  providers: [AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }

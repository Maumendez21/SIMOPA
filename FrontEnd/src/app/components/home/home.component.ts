import { Component } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  constructor(private app: AppComponent, private router: Router, private shared: SharedService) {
    if (this.app.auth) {
      this.app.setTitle("Inicia - SEMOPA");
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Inicio");
  }

}

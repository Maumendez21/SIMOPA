import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent {

  activePage: string;
  parts:any [] = [];

  constructor(private shared: SharedService, private route: Router) {

    this.shared.activePage.subscribe(data => {
      this.activePage = data;
    });

    this.shared.parts.subscribe(data => {
      this.parts = data;
    });
    
  }

  irUbicacion(ruta: string) {
    this.route.navigate([ruta]);
  }

}

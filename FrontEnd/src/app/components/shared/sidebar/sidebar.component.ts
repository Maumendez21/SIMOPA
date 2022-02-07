import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import { AppComponent } from '../../../app.component';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  municipio: string;
  ejercicio: number;
  rol: string;
  permisos: string;

  constructor(private shared: SharedService, private route: Router, private app: AppComponent) {
    this.prepareMunicipioShared();
    this.prepareAuditorEjercicio();
    this.prepareRol();
    this.preparePermisos();
  }

  logout() {

    this.app.auth = false;

    this.shared.clearSharedSession();

    this.route.navigate(['/login']);
  }

  prepareRol() {
    this.shared.roleStream$.subscribe(response => {
      this.rol = response;
    });

    if (!this.rol) {
      this.rol = localStorage.getItem('role');
    }
  }

  preparePermisos() {
    this.shared.permisosStream$.subscribe(response => {
      console.log(response);
      this.permisos = response;
    });

    if (!this.permisos) {
      this.permisos = localStorage.getItem('permits');
    }
  }

  prepareMunicipioShared() {

    this.shared.municipioStream$.subscribe(data => this.municipio = data);

    if (!this.municipio) {
      this.municipio = localStorage.getItem("municipio");
    }
  }

  prepareAuditorEjercicio() {

    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
    });
  }

  ngOnInit(): void {
  }

  toggleListgroup(id: string) {

    const element = document.getElementById(id);

    if (element.style.display == "block") {
      element.style.display = "none";
    } else {
      element.style.display = "block";
    }
  }

}

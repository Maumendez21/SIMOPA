import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  municipio: string;
  nombreUsuario: string;
  ejercicio: number = 2021;
  auditor: string = "auditorestodosdq37iq84burwwjdbsd3";
  auditores: any;
  rol: string;
  showSelectAuditores: boolean;
  showSelectEjercicio: boolean;

  constructor(private app: AppComponent, private route: Router, private router: ActivatedRoute, private shared: SharedService, private pbrService: PbrService) {
    this.prepareUsernameShared();
    this.prepareMunicipioShared();
    this.prepareShowSelectAuditores();
    this.prepareShowSelectEjercicio();
    this.prepareRol();
  }

  prepareRol() {
    this.shared.roleStream$.subscribe(response => {
      this.rol = response;
    });

    if (!this.rol) {
      this.rol = localStorage.getItem('role');
    }
  }

  prepareShowSelectAuditores() {
    this.shared.showSelectAuditoresStream$.subscribe(response => {
      this.showSelectAuditores = response;
      if (this.app.auth) { this.getAuditores(); }
    });
  }

  prepareShowSelectEjercicio() {
    this.shared.showSelectEjercicioStream$.subscribe(response => {
      this.showSelectEjercicio = response;
    });
  }

  prepareUsernameShared() {

    this.shared.usernameStream$.subscribe(data => this.nombreUsuario = data);

    if (!this.nombreUsuario) {
      this.nombreUsuario = localStorage.getItem("name");
    }
  }

  prepareMunicipioShared() {

    this.shared.municipioStream$.subscribe(data => this.municipio = data);

    if (!this.municipio) {
      this.municipio = localStorage.getItem("municipio");
    }
  }

  logout() {

    this.app.auth = false;

    this.shared.clearSharedSession();

    this.route.navigate(['/login']);
  }

  cambiarEjercicio() {
    this.shared.broadcastEjercicioStream(this.ejercicio);
  }

  toggleMenu() {
    document.getElementById("wrapper").classList.toggle("toggled");
  }

  getAuditores() {
    this.pbrService.getAuditores()
        .subscribe(data => {
          this.auditores = data;
        });
  }

  cambiarAuditor() {
    this.shared.broadcastAuditorStream(this.auditor);
  }

}

import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, RouterEvent, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { SharedService } from './services/shared.service';
import { PbrService } from './services/pbr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  municipio: string;
  nombreUsuario: string;

  loading:boolean = false;
  auth: boolean;

  showSelectEjercicio: boolean;
  ejercicio: number = 2021;
  rol: string;
  showSelectAuditores: boolean;
  auditor: string = "auditorestodosdq37iq84burwwjdbsd3";
  auditores: any;

  public constructor(private titleService: Title, private router: Router,  private shared: SharedService, private pbrService: PbrService) {
    this.auth = (localStorage.getItem("token") && localStorage.getItem("tokenRefresh")) ? true : false;


    this.prepareUsernameShared();
    this.prepareMunicipioShared();
    this.prepareShowSelectAuditores();
    this.prepareShowSelectEjercicio();
    this.prepareRol();
  }

  public setTitle(newTitle: string) {
    this.titleService.setTitle(newTitle);
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
      if (this.auth) { this.getAuditores(); }
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

  getAuditores() {
    this.pbrService.getAuditores()
        .subscribe(data => {
          this.auditores = data;
        });
  }

  cambiarEjercicio() {
    this.shared.broadcastEjercicioStream(this.ejercicio);
  }
  cambiarAuditor() {
    this.shared.broadcastAuditorStream(this.auditor);
  }
}

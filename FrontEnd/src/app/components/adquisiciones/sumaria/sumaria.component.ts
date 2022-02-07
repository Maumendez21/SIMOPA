import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { PbrService } from 'src/app/services/pbr.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sumaria',
  templateUrl: './sumaria.component.html',
  styleUrls: ['./sumaria.component.css']
})
export class SumariaComponent implements OnInit {

  tiposAdjudicacion: any = [
    {
      name: 'INVITACIÓN A CUANDO MENOS TRES PERSONAS',
      route: 'invitacion/tres/'+localStorage.getItem("municipio"),
      class: 'primary'
    },
    {
      name: 'CONCURSO POR INVITACIÓN',
      route: 'consurso/invitacion/'+localStorage.getItem("municipio"),
      class: 'success'
    },
    {
      name: 'ADJUDICACIÓN DIRECTA',
      route: 'adjudicacion/directa/'+localStorage.getItem("municipio"),
      class: 'danger'
    },
    {
      name: 'LICITACIÓN PÚBLICA',
      route: 'licitacion/publica/'+localStorage.getItem("municipio"),
      class: 'warning'
    },
    {
      name: 'REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL',
      route: '33/'+localStorage.getItem("municipio"),
      class: 'info'
    }
  ];

  linkSumaria: string;
  ejercicio: number;

  constructor(private app: AppComponent, private shared: SharedService, private pbrService: PbrService, private router: Router) {
    if (this.app.auth) {
      //this.app.loading = true;
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Analítica - Adquisiciones");
      this.prepareBreadcrumb();
      this.prepareAuditorEjercicio();
    } else {
      this.router.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Analítica");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  prepareAuditorEjercicio() {

    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
    });
  }

  ngOnInit(): void {
  }
}
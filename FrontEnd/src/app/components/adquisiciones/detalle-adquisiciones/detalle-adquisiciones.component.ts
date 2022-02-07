import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { PbrService } from 'src/app/services/pbr.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';

@Component({
  selector: 'app-detalle-adquisiciones',
  templateUrl: './detalle-adquisiciones.component.html',
  styleUrls: ['./detalle-adquisiciones.component.css']
})
export class DetalleAdquisicionesComponent {

  adjudicaciones: any[] = [];
  busqueda: string;
  estatus: number = 2;
  estatusExpediente: string;
  orderDate: string = "All";
  resource: string = "All";
  ejercicio: number;

  constructor(private app: AppComponent, private pbrService: PbrService, private route: Router, private router: ActivatedRoute, private shared: SharedService) {

    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Detalle - Adquisiciones");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
      this.prepareEjercicio();
      this.getAdquisiciones();
    } else {
      this.route.navigate(['/login']);
    }

  }

  verifyParamsRoute() {
    this.router.params.subscribe(params => {
      this.estatusExpediente = params['estatusExpediente'];
    });
  }

  prepareEjercicio() {
    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
      this.getAdquisiciones();
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Detalle de Adquisiciones");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  getAdquisiciones(orden: string = "") {
    
    this.app.loading = true;

    this.pbrService.getAdquisiciones(this.ejercicio, orden)
      .subscribe((data:any) => {
        this.adjudicaciones = data;
        this.app.loading = false;
      }, error => {
        this.app.auth = false;
        this.shared.clearSharedSession();
        this.route.navigate(["/login"]);
      });
  }

  changeStatus = (id:number) => {
    if (id === 2) {
      this.estatusExpediente = null;
    }

    this.estatus = id;
  }

  changeEstatusExpediente = (estatus: string) => {
    this.estatusExpediente = estatus;
    console.log(this.adjudicaciones);
  }

  changeOrderDate(order:string) {
    this.orderDate = order;
  }

  changeResource(resource:string) {
    this.resource = resource;
  }

  buscar = (termino:string) => {
    this.busqueda = termino;
  }

  changeOrder = (orden: string) => {
    this.getAdquisiciones(orden);
  }
}

import { Component } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { PbrService } from 'src/app/services/pbr.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';

@Component({
  selector: 'app-listado-obras',
  templateUrl: './listado-obras.component.html',
  styleUrls: ['./listado-obras.component.css']
})
export class ListadoObrasComponent {
  
  obras: any[] = [];
  busqueda: string;
  estatusExpediente: string;
  orderDate: string = "All";
  resource: string = "All";
  ejercicio: number;

  constructor(private app: AppComponent, private pbrService: PbrService, private route: Router, private router: ActivatedRoute, private shared: SharedService) {

    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Listado - Obras Publicas");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
      this.prepareEjercicio();
    } else {
      this.route.navigate(['/login']);
    }

  }

  verifyParamsRoute() {
    this.router.params.subscribe(params => {
      this.estatusExpediente = params['estatusExpediente'];
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Detalle de Obras");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  prepareEjercicio() {
    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
      this.getObras();
    });
  }

  getObras(order: string = "") {

    this.app.loading = true;
    
    this.pbrService.getObras(this.ejercicio, order)
      .subscribe(data => {
        this.obras = data;
        this.app.loading = false;
      }, error => {
        this.app.auth = false;
        this.shared.clearSharedSession();
        this.route.navigate(["/login"]);
      });
  }

  changeEstatusExpediente(estatus: string) {
    if (estatus != 'Todos') {
      this.estatusExpediente = estatus;
    } else {
      this.estatusExpediente = null;
    }
  }

  changeOrderDate(order:string) {
    this.orderDate = order;
  }

  changeResource(resource:string) {
    this.resource = resource;
  }

  buscar(termino:string) {
    this.busqueda = termino;
  }

  changeOrder(orden: string) {
    this.getObras(orden);
  }

}

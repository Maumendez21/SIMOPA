import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AppComponent } from 'src/app/app.component';
import { PbrService } from 'src/app/services/pbr.service';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-evidencia-fotografica',
  templateUrl: './evidencia-fotografica.component.html',
  styleUrls: ['./evidencia-fotografica.component.css']
})
export class EvidenciaFotograficaComponent {

  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();

  idVisita: string;
  numeroExpediente: string;
  evidencias: any;

  tituloEvidencia: string;
  descripcionEvidencia: string;
  fileEvidencia: any;


  constructor(private app: AppComponent, private toastr: ToastrService, private pbrService: PbrService, private http: HttpClient, private router: Router, private route: ActivatedRoute, private shared: SharedService) {
    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Evidencia Fotografica - Obras");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  verifyParamsRoute() {
    this.route.params.subscribe(params => {
      this.idVisita = params['idVisita'];
      this.numeroExpediente = params['idExpediente'];

      this.getFotografias();
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Evidencia Fotografica");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" },
      { name: "Detalle de Obras", route: '/obras/detalle' }
    ]);
  }

  getFotografias() {

    this.pbrService.getFotografiasVisitaObra(this.idVisita)
      .subscribe(data => {
        this.evidencias = data;
        this.app.loading = false;
      });
  }

  onFileChanged(event) {
    this.fileEvidencia = event.target.files[0];
  }

  registrarEvidenciaFotografica() {

    if (this.verifyEmpty(this.fileEvidencia) && this.verifyEmpty(this.tituloEvidencia) && this.verifyEmpty(this.descripcionEvidencia)) {

      let formData = new FormData();

      formData.append('ImageFile', this.fileEvidencia, this.fileEvidencia.name);
      formData.append('idExpediente', this.numeroExpediente);
      formData.append('idVisita', this.idVisita);
      formData.append('titulo', this.tituloEvidencia);
      formData.append('descripcion', this.descripcionEvidencia);
      
      this.pbrService.setFotografiasVisitaObra(formData)
        .subscribe(response => {
          this.getFotografias();
          this.toastr.success("Se registro exitosamente la evidencia fotografica", "Exito");
        });
    } else {
      this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
    }
  }

  verifyEmpty(input: string) {
    return input != null && input != "";
  }

}

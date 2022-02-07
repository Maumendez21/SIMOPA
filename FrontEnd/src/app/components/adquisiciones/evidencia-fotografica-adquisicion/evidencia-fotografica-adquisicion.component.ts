import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PbrService } from 'src/app/services/pbr.service';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-evidencia-fotografica-adquisicion',
  templateUrl: './evidencia-fotografica-adquisicion.component.html',
  styleUrls: ['./evidencia-fotografica-adquisicion.component.css']
})
export class EvidenciaFotograficaAdquisicionComponent {

  rol: boolean = (localStorage.getItem("role") == "Coordinador" || localStorage.getItem("role") == "Auditor") ? true : false;
  numeroAdjudicacion: string;
  evidencias: any;

  tituloEvidencia: string;
  descripcionEvidencia: string;
  fileEvidencia: any;

  constructor(private app: AppComponent, private toastr: ToastrService, private pbrService: PbrService, private http: HttpClient, private router: Router, private route: ActivatedRoute, private shared: SharedService) {
    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Evidencia Fotografica - Adquisiciones");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  verifyParamsRoute() {
    this.route.params.subscribe(params => {
      this.numeroAdjudicacion = params['id'];
      this.getFotografias(params['id']);
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Evidencia Fotografica");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" },
      { name: "Detalle de Adquisiciones", route: "/adquisiciones/detalle" }
    ]);
  }

  getFotografias(expediente: string) {
    this.pbrService.getMosaicoExpediente(expediente)
      .subscribe(data => {
        console.log(data);
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
      formData.append('idExpediente', this.numeroAdjudicacion);
      formData.append('titulo', this.tituloEvidencia);
      formData.append('descripcion', this.descripcionEvidencia);
      
      this.pbrService.setEvidenciaFotografica("adquisiciones", formData)
        .subscribe(response => {
          this.getFotografias(this.numeroAdjudicacion);
          this.toastr.success("Se registro exitosamente la evidencia fotografica", "Exito");
        });
    } else {
      this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
    }
  }

  eliminarEvidenciaFotografica(idFotografia: string, index: number) {

    Swal.fire({
      title: '¿Seguro que quieres eliminar la fotografia?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.eliminarFotografia(idFotografia, index);
        document.getElementById("openModalButton").click();
      }
    });
  }

  eliminarFotografia(idFotografia: string, index: number) {
    this.pbrService.deleteFotografia(idFotografia)
      .subscribe(data => {
        console.log(data);
        if (data['success']) {
          this.evidencias = this.evidencias.filter(({ id }) => id !== idFotografia);
        }
      });
  }

  verifyEmpty(input: string) {
    return input != null && input != "";
  }

}

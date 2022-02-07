import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';

@Component({
  selector: 'app-documentos-adquisiciones',
  templateUrl: './documentos-adquisiciones.component.html',
  styleUrls: ['./documentos-adquisiciones.component.css']
})
export class DocumentosAdquisicionesComponent {

  info: any[] = [];
  documentos: any[] = [];
  doc: string;
  idExpediente: number;
  claveExpediente: number;
  observacionGeneral: string;
  estatus: string;
  estatusColor: string;
  observaciones: any = [];
  anexos: any;
  indexDocument: number;
  document: any;
  index: number;

  constructor(private app: AppComponent, private route: ActivatedRoute, private router: Router, private pbrService: PbrService, private shared: SharedService) {

    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Documentos - Adquisiciones");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  /**
   * Esta función obtiene el id que viene en la Url y lo asigna a la variable idExpediente, 
   * al mismo tiempo invoca la función getDocuments para obtener todos los documentos del expediente
   */
  verifyParamsRoute() {
    this.route.params.subscribe(params => {
      this.idExpediente = params['id'];
      this.getDocuments(params['id']);
    });
  }

  /**
   * Esta función prepara el breadcrumb
   */
  prepareBreadcrumb() {
    this.shared.activePage.emit("Documentos de Adjudicacion");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" },
      { name: "Detalle de Adquisiciones", route: "/adquisiciones/detalle" }
    ]);
  }

  /**
   * Esta función obtiene los documentos del expediente actual
   * @param id - id del expediente
   */
  getDocuments(id:number) {

    this.pbrService.getDocumentsAdjudicacion(id)
        .subscribe((data:any) => {
          this.info = data;
          this.info["ExpedienteOwner"] = this.info["NombreAuditor"] == localStorage.getItem("name");
          this.documentos = data['documentos'];
          this.estatus = data['estatusExpediente'];
          this.verificarEstatusColor();

          this.app.loading = false;

          this.observacionGeneral = data['ObservacionesGenerales'];

          console.log(this.info);

        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
          this.router.navigate(["/login"]);
        });
  }

  /**
   * Esta función verifica el estatus del expediente para asignarle un tema personalizado en CSS
   */
  verificarEstatusColor() {
    switch (this.estatus) {
      case "ALTA EN SISTEMA":
        this.estatusColor = "blue";
        break;
      case "CARGA EN PROCESO":
        this.estatusColor = "gray";
        break;
      case "CARGADO":
        this.estatusColor = "blue-cargado";
        break;
      case "EN REVISION":
        this.estatusColor = "yellow";
        break;
      case "EN SOLVENTACION":
        this.estatusColor = "purple";
        break;
      case "REVISADO CON OBSERVACIONES":
        this.estatusColor = "danger";
        break;
      default:
        this.estatusColor = "success";
    }
  }

  /**
   * Esta función carga los anexos del expediente
   */
  cargarAnexos = () => {
    this.pbrService.getCargarAnexos(this.idExpediente.toString())
      .subscribe(data => {
        console.log(data);
        this.anexos = data;
      });
  };

  /**
   * Esta función inserta la info en el documento para ser utilizado en otras funciones
   * @param index - Indice del documento
   * @param clave - Clave del documento
   */
  setDataDocument = (index: number, clave: string) => {
    this.claveExpediente = +clave;
    this.indexDocument = index;
  };

  /**
   * Esta función obtiene las obversaciones del documento
   * @param clave - Clave del documento
   * @param index - Indice del documento
   */
  getObservaciones = (clave: string, index: number) => {

    this.claveExpediente = +clave;
    this.indexDocument = index;

    this.pbrService.getObservacionesExpediente("adquisiciones", this.idExpediente, clave)
      .subscribe(data => {
        console.log(data);
        this.observaciones = data;
      });
  }

  /**
   * Esta función inserta la información del documento PDF
   * @param document - array con la inforamción del documento
   * @param i - indice del documento
   */
  setInfoDocumentoPdf = (document, i) => {
    this.document = document;
    this.index = i;
  }

  /**
   * Esta función inserta la url del documento para poder visualizarlo en el modal
   * @param document - link del PDF
   */
  setViewerDoc = (document: string) => {
    this.doc = document;
  }

}

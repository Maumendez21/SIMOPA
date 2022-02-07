import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';
import { AppComponent } from 'src/app/app.component';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { SharedService } from 'src/app/services/shared.service';
import { ToastrService }  from 'ngx-toastr';
import Swal from 'sweetalert2';
import { Location } from '@angular/common';

declare var $:any;

@Component({
  selector: 'app-detalle-obras',
  templateUrl: './detalle-obras.component.html',
  styleUrls: ['./detalle-obras.component.css']
})

export class DetalleObrasComponent {

  rol: string = localStorage.getItem("role");

  @ViewChild('fileDocument')
  fileDocument: ElementRef;

  @ViewChild('fileAnexo')
  fileAnexo: ElementRef;

  urlDownload: string = "https://apipbrdevelolop.azurewebsites.net/api/expedients/obrapublica/adownload";
  
  info: any[] = [];
  documentos: any[] = [];
  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();
  doc: string;
  
  idExpediente: number;
  edicionComentarioRecomendacion: boolean = false;
  claveExpediente: number;
  nuevoComentario: string;
  nuevaRecomendacion: string;
  estatusComentarioRecomendacion: number = 1;
  estatus: string;
  estatusColor: string;
  observaciones: any = [];
  estatusChangeDocument: string = "N/A";
  indexDocument: number;
  idObservacion: string;
  observacionGeneral: string;

  role: string = localStorage.getItem("role");

  filePdf: any;
  document: any;
  index: number;

  anexos: any;
  tituloAnexo: string;
  descripcionAnexo: string;
  fileAnexoForm: any;

  latitud: string
  longitud: string;
  estatusExpediente: string;
  coordsMap: boolean = false;
  detailsVisible: boolean = false;

  centerCords = {
    lat: 18.967548,
    lng: -97.901011
  };

  iconBase:string = "assets/global/images/pins/";

  icons: any = {
    //'REVISADO SIN OBSERVACIONES': { 'icon': this.iconBase + 'marker_verde.png' },
    //'CORREGIDO': { 'icon': this.iconBase + 'marker_verde.png' },
    'ALTA EN SISTEMA': { 'icon': this.iconBase + 'marker_azul.png' },
    'CARGA EN PROCESO': { 'icon': this.iconBase + 'marker_gris.png' },
    'CARGADO': { 'icon': this.iconBase + 'marker_azul_cargado.png' },
    'EN REVISION': { 'icon': this.iconBase + 'marker_amarillo.png' },
    'EN SOLVENTACION': { 'icon': this.iconBase + 'marker_morado.png' },
    'REVISADO CON OBSERVACIONES': { 'icon': this.iconBase + 'marker_rojo.png' },
    'REVISADO SIN OBSERVACIONES CORREGIDO': { 'icon': this.iconBase + 'marker_verde.png' }
  };

  markersOnMap: any;

  //Toast Carga Documento
  description: string = "N/D";
  show: boolean = false;

  disabledBtnObservacionesRecomendaciones: boolean = false;

  constructor(private app: AppComponent, private route: ActivatedRoute, private router: Router, private pbrService: PbrService, private http: HttpClient, private shared: SharedService, private toastr: ToastrService, private _location: Location) {

    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Detalle - Obras Publicas");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  verifyParamsRoute() {
    this.route.params.subscribe(params => {
      this.idExpediente = params['id'];
      this.getDocuments(params['id']);
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Documentos de Obras");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" },
      { name: "Detalle de Obras", route: '/obras/detalle' }
    ]);
  }

  getDocuments(id:string) {

    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      ordering: false,
      responsive: true,
      drawCallback(settings) {
        var api = this.api();
        var rows = api.rows({ page: 'current' }).nodes();
        var last = null;
        const columIndex = 1;
        api.column(columIndex, { page: 'current' }).data().each(function (group, i) {
          if (last !== group) {
            $(rows).eq(i).before(
              '<tr style="color: black !important; font-weight: bold; background-color: #eeeeee;" class="groupColumns"><td style="display: none;"></td><td colspan="8">' + group + '</td></tr>'
            );
            last = group;
          }
        });
      },
      language: {
        'search': "Buscar",
        'paginate': {
          'first': 'Primero',
          'previous': 'Anterior',
          'next': 'Siguiente',
          'last': 'Ultimo'
        },
        'lengthMenu': 'Mostrar _MENU_ documentos',
        'info': 'Mostrando _PAGE_ de _PAGES_'
      },
      initComplete: function () {
        this.api().columns(".filtersearch").every( function () {
            const column = this;
            const select = $("<select> <option value=''>Todos</option> <option value='N/A'>N/A</option> <option value='SI'>SI</option> <option value='NO'>NO</option> <option value='DOC. ERRONEO'>DOC. ERRONEO</option> <option value='POR REVISAR'>POR REVISAR</option> </select>")
                .appendTo( $(column.header()).empty() )
                .on( 'change', function () {
                    let val = $.fn.dataTable.util.escapeRegex(
                        $(this).val().toString()
                    );

                    column.search( val ? '^'+val+'$' : '', true, false ).draw();
                } );

            column.data().unique().sort().each( function ( d, j ) {
                //select.append( '<option value="'+j+'">'+d+'</option>' )
            } );
        } );
      }
    }

    this.pbrService.getDocumentsObras(id)
        .subscribe((data:any) => {
          this.info = data;
          this.info["ExpedienteOwner"] = this.info["NombreAuditor"] == localStorage.getItem("name");
          this.documentos = data['documentosObraPublicaV1'];
          this.estatus = data['estatusExpediente'];
          this.observacionGeneral = data['observacionesGenerales'];

          if (data['latitud'] && data['longitud']) {
            this.coordsMap = true;
            this.latitud = data['latitud'];
            this.longitud = data['longitud'];
          }

          this.estatusExpediente = data['estatusExpediente'];
          this.verificarEstatusColor();
          this.dtTrigger.next();
          this.app.loading = false;

          console.log(this.info);
        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
          this.router.navigate(["/login"]);
        });
  }

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
  }

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

  modal(clave:number) {

    console.log(clave);

    const expediente = this.documentos.find(x => x.clave === clave);

    this.claveExpediente = expediente.clave;
    this.nuevoComentario = expediente.comentario;
    this.nuevaRecomendacion = expediente.recomendacion;
  }

  registrarObservacion() {

    if (this.nuevoComentario != "" && 
        this.nuevaRecomendacion != "" && 
        this.nuevoComentario != null && 
        this.nuevaRecomendacion != null) {

      this.disabledBtnObservacionesRecomendaciones = true;

      const data = {
        'id': '',
        'idExpediente': this.idExpediente,
        'TipoExpediente': 'ObraPublica',
        'Clave': this.claveExpediente,
        'Observacion': this.nuevoComentario,
        'Recomendacion': this.nuevaRecomendacion,
        'Estatus': 2
      };

      this.pbrService.setObservacionesExpediente("obrapublica", this.idExpediente, this.claveExpediente, data)
        .subscribe(response => {

          this.getObservaciones(this.claveExpediente.toString(), this.indexDocument);

          this.nuevoComentario = "";
          this.nuevaRecomendacion = "";

          this.documentos[this.indexDocument].comentario = this.newObservaciones();

          this.disabledBtnObservacionesRecomendaciones = false;
        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
          this.router.navigate(["/login"]);
        });

    }
  }

  newObservaciones() : string {
    let atentidas: number = 0;
    let noAtendidas: number = 0;
    let total: number = this.observaciones.length + 1;

    this.observaciones.map(function(x) {
      (x.Estatus == 1) ? atentidas = atentidas+1 : noAtendidas = noAtendidas+1;
    });

    return atentidas+"/"+total;
  }

  prepareCargaDocumento() {

    if (this.documentos[this.index].downloadLinkDocument) {
      Swal.fire({
        title: '¿Seguro que quieres reemplazar el documento existente?',
        text: "Esta acción no se puede revertir",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'Cancelar',
      }).then((result) => {
        if (result.value) {
          this.cargarDocumento();
        }
      });
    } else {
      this.cargarDocumento();
    }
  }

  cargarDocumento() {

    this.setToastInfo("Cargando 1 elemento", true);

    let formData = new FormData();

    formData.append('File', this.filePdf, this.filePdf.name);
    formData.append('idExpediente', this.idExpediente.toString());
    formData.append('clave', this.document.clave);

    this.pbrService.setDocumentoExpedienteObra(formData)
      .subscribe(data => {
        
        const documentoInfo = this.documentos[this.index]; 

        documentoInfo.downloadLinkDocument = `${this.urlDownload}?id=${this.idExpediente}&clave=${documentoInfo.clave}`;
        
        if (this.role == "Coordinador") {
          documentoInfo.estatus = "POR REVISAR";
        } else {
          documentoInfo.estatus = "SI";
        }

        Swal.fire(
          'Carga exitosa!',
          'El archivo se ha subido correctamente.',
          'success'
        );

        this.setToastInfo("N/D", false);

        this.fileDocument.nativeElement.value = "";
      });
  }

  setToastInfo(_description: string, _show: boolean) {
    this.description = _description;
    this.show = _show;
  }

  setInfoDocumentoPdf(document, i) {
    this.document = document;
    this.index = i;
  }

  modalEliminarDocumento(data: any, index: number) {

    this.claveExpediente = data.clave;
    this.indexDocument = index;

    Swal.fire({
      title: '¿Seguro que quieres eliminar el documento?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.eliminarDocumento(data, index);
        //document.getElementById("openModalButton").click();
      }
    });
  }

  setDataDocument(index: number, clave: string) {
    this.claveExpediente = +clave;
    this.indexDocument = index;
  }

  eliminarDocumento(data: any, index: number) {
    this.pbrService.deleteDocumentoExpedienteObra(this.idExpediente, data.clave)
      .subscribe(response => {
        this.documentos[index].estatus = "NO";
        this.documentos[index].downloadLinkDocument = "";
        Swal.fire(
          'Eliminado!',
          'El archivo ha sido eliminado exitosamente.',
          'success'
        );
      });
  }

  onFileChanged(event) {
    this.filePdf = event.target.files[0];
  }

  onFileChangedAnexo(event) {
    this.fileAnexoForm = event.target.files[0];
  }

  registrarUbicacion() {
    
    const data = {
      "latitud": this.latitud,
      "longitud": this.longitud
    };

    this.pbrService.setCoordenadasObras(this.idExpediente, data)
      .subscribe(data => {
        if (data['success']) {
          Swal.fire(
            'Exito!',
            'Ubicación agregada exitosamente',
            'success'
          );

          this.coordsMap = true;
        } else {
          Swal.fire(
            'Error',
            'Al parecer hubo un error al tratar de agregar la nueva ubicación',
            'error'
          );
        }
      });
  }

  habilitarCarga(index: number, clave: string) {
    
    const data = {
      "estatusDocumento": "NO"
    };

    this.pbrService.setUpdateEstatus("obrapublica", this.idExpediente, clave, data)
      .subscribe(data => {
        console.log(data);
        this.documentos[index].estatus = "NO";
      });
  }

  cambiarEstatusDocumento() {
    const data = {
      "estatusDocumento": this.estatusChangeDocument
    };

    this.pbrService.setUpdateEstatus("obrapublica", this.idExpediente, this.claveExpediente.toString(), data)
      .subscribe(data => {
        this.documentos[this.indexDocument].estatus = this.estatusChangeDocument;
      });
  }

  actualizarEstatus(dot) {

    this.estatus = dot;

    const data = {
      'estatusExpediente': dot
    }

    this.verificarEstatusColor();

    this.pbrService.setEstatusExpediente("obrapublica", this.idExpediente, data)
      .subscribe(data => {
        if (data['success']) {
          Swal.fire(
            'Exito!',
            'Estatus actualizado exitosamente!',
            'success'
          );
        } else {
          Swal.fire(
            'Error',
            'Al parecer hubo un error al tratar de agregar la actualizar el estatus',
            'error'
          );
        }
      });
  }

  setViewerDoc(document: string) {
    this.doc = document;
  }

  getObservaciones(clave: string, index: number) {

    this.claveExpediente = +clave;
    this.indexDocument = index;

    this.pbrService.getObservacionesExpediente("obrapublica", this.idExpediente, clave)
      .subscribe(data => {
        this.observaciones = data;
      });
  }

  setDataUpdateComentarios(data: any) {
    this.changeEstatusUpdateComentarios(true);
    this.idObservacion = data.Id;
    this.nuevoComentario = data.Observacion;
    this.nuevaRecomendacion = data.Recomendacion;
    this.estatusComentarioRecomendacion = data.Estatus;
  }

  actualizarObservacion() {

    this.disabledBtnObservacionesRecomendaciones = true;

    const data = {
      'id': this.idObservacion,
      'idExpediente': this.idExpediente,
      'TipoExpediente': 'ObraPublica',
      'Clave': this.claveExpediente,
      'Observacion': this.nuevoComentario,
      'Recomendacion': this.nuevaRecomendacion,
      'Estatus': this.estatusComentarioRecomendacion
    };

    this.pbrService.updateObservacionesExpediente("obrapublica", this.idExpediente, this.claveExpediente, data)
      .subscribe(response => {

        this.documentos[this.indexDocument].comentario = this.calcularObservaciones();

        this.getObservaciones(this.claveExpediente.toString(), this.indexDocument);
        this.changeEstatusUpdateComentarios(false);
        this.nuevoComentario = "";
        this.nuevaRecomendacion = "";
        this.disabledBtnObservacionesRecomendaciones = false;
      }, error => {
        this.app.auth = false;
        this.shared.clearSharedSession();
        this.router.navigate(["/login"]);
      });
  }

  calcularObservaciones() : string {

    let conteo: number = 0;

    this.observaciones.map(function(x) {
      conteo = (x.Estatus == 1) ? conteo + 1 : conteo;
    });

    conteo = (this.estatusComentarioRecomendacion == 1) ? conteo + 1 : conteo - 1;

    return conteo+"/"+this.observaciones.length;
  }

  changeEstatusUpdateComentarios(estatus: boolean) {
    this.edicionComentarioRecomendacion = estatus;
  }

  registrarObservacionGeneral() {
    if (this.observacionGeneral) {
      const data = {
        'observacionGeneral': this.observacionGeneral
      }

      this.pbrService.setObservacionGeneral(this.idExpediente, "obrapublica", data)
        .subscribe(data => {

          console.log(data);

          this.info['observacionesGenerales'] = this.observacionGeneral;

          Swal.fire(
            'Exito!',
            'Observación registrada exitosamente!',
            'success'
          );
        });

    } else {
      this.toastr.error("Es necesario llenar los campos", "Error");
    }
  }

  cargarAnexos() {
    this.pbrService.getCargarAnexos(this.idExpediente.toString())
      .subscribe(data => {
        console.log(data);
        this.anexos = data;
      });
  }

  cargarAnexo() {

    if (this.tituloAnexo != "" && this.descripcionAnexo != "") {
      let formData = new FormData();

      formData.append('idExpediente', this.idExpediente.toString());
      formData.append('titulo', this.tituloAnexo);
      formData.append('descripcion', this.descripcionAnexo);
      formData.append('File', this.fileAnexoForm, this.fileAnexoForm.name);

      this.pbrService.setCargarAnexo("obrapublica", formData)
        .subscribe(response => {

          console.log(response);

          this.cargarAnexos();

          if (response['success']) {

            Swal.fire(
              'Carga exitosa!',
              'El archivo se ha subido correctamente.',
              'success'
            );
            
            this.tituloAnexo = "";
            this.descripcionAnexo = "";
            this.fileAnexo.nativeElement.value = "";
          }
        });
    }
  }

  eliminarAnexo(id: string) {
    console.log(id);

    Swal.fire({
      title: '¿Seguro que quieres eliminar el Anexo?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.pbrService.deleteAnexo(id)
          .subscribe(data => {
            console.log(data);

            this.cargarAnexos();

            Swal.fire(
              'Eliminado exitosamente!',
              'El anexo se ha eliminado correctamente.',
              'success'
            );
          });
      }
    });
  }

  descargarObservacionesRecomendaciones(id: number) {
    return `https://apipbrdevelolop.azurewebsites.net/api/reporte/O/cedula/observaciones/recomendaciones/${id}`;
  }

  asignarmeExpediente() {
    Swal.fire({
      title: '¿Seguro que quieres reasignarte el Expediente?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.pbrService.setAsignarAuditorBitacora("Obra", this.info["Id"])
          .subscribe(response => {
            if (response["success"]) {

              Swal.fire(
                'Exito!',
                'Se reasigno exitosamente el Expediente.',
                'success'
              );

              this.info["ExpedienteOwner"] = !this.info["ExpedienteOwner"];
              this.info["NombreAuditor"] = localStorage.getItem("name");

            } else {

              Swal.fire(
                'Error!',
                'Al parecer hubo un error al tratar de reasignar el Expediente',
                'error'
              );
            }
          });
      }
    });
  }

}

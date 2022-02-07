import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import Swal from 'sweetalert2';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-carga-documento-modal-adq',
  templateUrl: './carga-documento-modal-adq.component.html',
  styleUrls: ['./carga-documento-modal-adq.component.css']
})
export class CargaDocumentoModalAdqComponent implements OnInit {

  urlDownload: string = "https://apipbrdevelolop.azurewebsites.net/api/expedients/adownload";

  @ViewChild('fileDocument')
  fileDocument: ElementRef;

  @Input() idExpediente: number;
  @Input() documentos: any;
  @Input() index: number;
  @Input() document: any;

  //Toast Carga Documento
  description: string = "N/D";
  show: boolean = false;

  filePdf: any;
  role: string = localStorage.getItem("role");

  constructor(private pbrService: PbrService) { }

  ngOnInit(): void {
  }

  onFileChanged(event) {
    this.filePdf = event.target.files[0];
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

    this.pbrService.setDocumentoExpedienteAdquisicion(formData)
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

}

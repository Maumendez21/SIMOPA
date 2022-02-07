import { Component, OnInit, Input } from '@angular/core';
import { Subject } from 'rxjs';
import Swal from 'sweetalert2';
import { PbrService } from 'src/app/services/pbr.service';
import { ValueTransformer } from '@angular/compiler/src/util';

@Component({
  selector: 'app-datatable-adq',
  templateUrl: './datatable-adq.component.html',
  styleUrls: ['./datatable-adq.component.css']
})
export class DatatableAdqComponent implements OnInit {

  rol: string = localStorage.getItem("role");

  @Input() idExpediente: number;
  @Input() documentos: any;

  @Input() setDataDocument: any;
  @Input() getObservaciones: any;
  @Input() setViewerDoc: any;
  @Input() setInfoDocumentoPdf:  any;

  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();

  constructor(private pbrService: PbrService) {}

  ngOnInit(): void {}

  ngOnChanges(): void {
    this.dtTrigger.next();
    this.generateDataTable();
  }

  generateDataTable() {

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
              //select.append( '<option value="'+val+'">'+val+' - '+j+'</option>' );
            } );
        } );
      }
    }

  }

  modalEliminarDocumento(data: any, index: number) {
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
        document.getElementById("openModalButton").click();
      }
    });
  }

  eliminarDocumento(data: any, index: number) {
    this.pbrService.deleteDocumentoExpedienteAdquisiciones(this.idExpediente, data.clave)
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

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
  }

}

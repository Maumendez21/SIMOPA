import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filtersListaObras'
})
export class FiltersListaObrasPipe implements PipeTransform {

  transform(data: any[], estatusExpediente: string, busqueda: string): unknown {

    if (busqueda != null) {
      
      data = data.filter(obra => (
        this.search(obra.procedimiento, busqueda) || 
        this.search(obra.programa, busqueda) || 
        this.search(obra.ejecutor, busqueda) || 
        this.search(obra.fechaRevision, busqueda) || 
        this.search(obra.localidad, busqueda) || 
        this.search(obra.nombreObra, busqueda) || 
        this.search(obra.modalidad, busqueda) || 
        this.search(obra.tipoAdjudicacion, busqueda) || 
        this.search(obra.tipoContrato, busqueda) || 
        this.search(obra.proveedor, busqueda) || 
        this.search(obra.numeroProcedimiento, busqueda) || 
        this.search(obra.fechaProcedimiento, busqueda) || 
        this.search(obra.numeroContrato, busqueda) || 
        this.search(obra.montoAsignado, busqueda)
        )
      )
    }

    if (estatusExpediente != null) {
      data = data.filter(adquisicion => (adquisicion.estatusExpediente === estatusExpediente));
    }

    return data;

  }

  search(object: any, search: string) : boolean {

    if (object) {
      return (object.search(search) > -1 || object.toLowerCase().search(search) > -1 || object.toUpperCase().search(search) > -1);
    } else {
      return false
    }
  }

}

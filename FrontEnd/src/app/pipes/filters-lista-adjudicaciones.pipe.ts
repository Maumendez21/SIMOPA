import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filtersListaAdjudicaciones'
})
export class FiltersListaAdjudicacionesPipe implements PipeTransform {

  transform(data: any[], status: number, estatusExpediente: string, busqueda: string): unknown {

    if (busqueda != null) {
      data = data.filter(adquisicion => (
        this.search(adquisicion.tipoAdjudicacion, busqueda) || 
        this.search(adquisicion.numeroAdjudicacion, busqueda) || 
        this.search(adquisicion.numeroContrato, busqueda) || 
        this.search(adquisicion.objetoContrato, busqueda) || 
        this.search(adquisicion.montoAdjudicado, busqueda) 
        )
      )
    }

    if (status != 2) {
      data = data.filter(adquisicion => (adquisicion.status === status));
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

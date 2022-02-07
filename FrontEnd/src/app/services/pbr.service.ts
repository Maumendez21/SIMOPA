import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PbrService {

  url = "https://apipbrdevelolop.azurewebsites.net/api/";

  constructor(private http: HttpClient) {}

  getQuery(query:string) {
    const url = `${this.url}${ query }`;
    
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("token")}`
    });

    return this.http.get(url, { headers });
  }

  postQuery(query:string, data:any = null) {

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("token")}`
    });

    const url = `${this.url}${ query }`;

    return this.http.post(url, data, { headers });
  }

  putQuery(query:string, data:any = null) {

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("token")}`
    });
    
    const url = `${this.url}${ query }`;

    return this.http.put(url, data, { headers });
  }

  deleteQuery(query:string) {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("token")}`
    });

    const url = `${this.url}${ query }`;

    return this.http.delete(url, { headers });
  }

  login(data:any) {
    return this.postQuery("access/login", data);
  }

  getAdquisiciones(anio: number, order: string) {
    return this.getQuery(`expedients/adquisiciones/cards/${anio}?sortCard=${order}`)
            .pipe( map( data => {
              return data['ListaHeadersSanAndresCholula'];
            }));
  }

  getObras(anio: number, order: string) {
    return this.getQuery(`expedients/obrapublica/cards/${anio}?sortCard=${order}`)
            .pipe( map( data => {
              return data['headerObraPublicaV1s'];
            }));
  }

  getDocumentsAdjudicacion(id:number) {
    return this.getQuery(`expedients/adquisiciones/documents?id=${id}`);
  }

  getDocumentsObras(id:string) {
    return this.getQuery(`expedients/obrapublica/documents?id=${id}`);
  }

  setObservacionExpediente(data:any) {
    return this.putQuery("expedients/document/observacion", data);
  }

  getObservacionesExpediente(tipoExpediente: string, idExpediente: number, clave: string) {
    return this.getQuery(`expedients/${tipoExpediente}/observaciones/${idExpediente}/${clave}`)
      .pipe( map(data => {
        return data['ListadoObservaciones'];
      }));
  }

  setObservacionesExpediente(tipoExpediente: string, idExpediente: number, clave: number, data: any) {
    return this.postQuery(`expedients/${tipoExpediente}/observaciones/${idExpediente}/${clave}`, data);
  }

  updateObservacionesExpediente(tipoExpediente: string, idExpediente: number, clave: number, data: any) {
    return this.putQuery(`expedients/${tipoExpediente}/observaciones/${idExpediente}/${clave}`, data);
  }

  setObservacionDocumentoObra(data:any) {
    return this.putQuery("expedients/obrapublica/document/observacion", data);
  }

  getInfoUser() {
    return this.getQuery("information/userpbr");
  }

  setEvidenciaFotografica(tipoExpediente: string, data: FormData) {
    return this.postQuery(`expedients/${tipoExpediente}/evidenciafotografica`, data);
  }

  getMosaicoExpediente(idExpediente: string) {
    return this.getQuery(`expedients/evidenciafotografica/mosaico?idExpediente=${idExpediente}`)
      .pipe( map(data => {
        return data['listResponseEvidenciaFotografica'];
      }));
  }

  setDocumentoExpedienteObra(data: FormData) {
    return this.postQuery(`expedients/obrapublica/document`, data);
  }

  setDocumentoExpedienteAdquisicion(data: FormData) {
    return this.postQuery(`expedients/adquisiciones/document`, data);
  }

  deleteDocumentoExpedienteObra(id:number, clave: string) {
    return this.deleteQuery(`expedients/obrapublica/document?id=${id}&clave=${clave}`);
  }

  deleteDocumentoExpedienteAdquisiciones(id:number, clave: string) {
    return this.deleteQuery(`expedients/adquisiciones/document?id=${id}&clave=${clave}`);
  }

  setCoordenadasObras(id: number, body: any) {
    return this.postQuery(`expedients/location/obrapublica/${id}`, body);
  }

  getCoordenadasObras(ejercicio:number) {
    return this.getQuery(`expedients/complementaria/coordenadas/obrapublica/${ejercicio}`)
      .pipe( map(data => {
        return data['listaCoordenadasObraOublicas'];
      }));
  }

  setObservacionGeneral(idExpediente:number, tipoExpediente: string, body: any) {
    return this.putQuery(`expedients/${tipoExpediente}/observaciongeneral/${idExpediente}`, body);
  }

  getDashboardData(ejercicio: number) {
    return this.getQuery(`dashboard/principal/${ejercicio}`)
      .pipe( map(data => {
        return data['Principal'];
      }));
  }

  getAuditores() {
    return this.getQuery("auditores")
      .pipe( map(data => {
        return data['Auditores'];
      }));
  }

  getDashboardExpedientes(tipoExpediente: string, ejercicio: number, idAuditor: string) {
    return this.getQuery(`dashboard/${tipoExpediente}/${ejercicio}/${idAuditor}`)
      .pipe( map(data => {
        return data['dashboardExpedientes'];
      }));
  }

  setEstatusExpediente(tipoExpediente:string, id: number, data: any) {
    return this.postQuery(`expedients/estatus/${tipoExpediente}/${id}`, data);
  }

  getVisitasObra(id: string) {
    return this.getQuery(`expedients/obrapublica/visitaobra?id=${id}`);
  }

  setNuevaVisitaObra(id: string, data: any) {
    return this.postQuery(`expedients/obrapublica/visitaobra/${id}`, data);    
  }

  setUpdateVisitaObra(id: string, data: any) {
    return this.putQuery(`expedients/obrapublica/visitaobra/${id}`, data);
  }

  setFotografiasVisitaObra(data: FormData) {
    return this.postQuery("expedients/obrapublica/visitaobra/imagenes/evidenciafotografica", data);
  }

  getFotografiasVisitaObra(idVisita: string) {
    return this.getQuery(`expedients/obrapublica/visitaobra/evidenciafotografica/mosaico?idVisita=${idVisita}`)
      .pipe( map(data => {
        return data['listResponseEvidenciaFotografica'];
      }));
  }

  deleteFotografia(idFotografia: string) {
    return this.deleteQuery(`expedients/evidenciafotografica/${idFotografia}`);
  }

  setNuevoExpedienteAdquisiciones(body: any) {
    return this.postQuery("expedients/adquisiciones/agrega", body);
  }

  setUpdateExpedienteAdquisiciones(id: number, body: any) {
    return this.putQuery(`expedients/adquisiciones/modifica/${id}`, body);
  }

  setNuevoExpedienteObra(body: any) {
    return this.postQuery("expedients/obrapublica/agrega", body);
  }

  setUpdateExpedienteObra(id: string, body: any) {
    return this.putQuery(`expedients/obrapublica/modifica?id=${id}`, body);
  }

  getPartidasPrincipales(id: string) {
    return this.getQuery(`expedients/obrapublica/partidasprincipales/${id}`)
      .pipe( map(data => {
        return data['PartidasPrincipalesObraPublica'];
      }));
  }

  setNuevaPartidaPrincipal(body: any) {
    return this.postQuery("expedients/obrapublica/partidasprincipales", body);
  } 

  setUpdateEstatus(tipoExpediente: string, idExpediente: number, clave: string, data: any) {
    return this.postQuery(`expedients/estatus/${tipoExpediente}/${idExpediente}/${clave}`, data);
  }

  getCargarAnexos(idExpediente: string) {
    return this.getQuery(`expedients/anexo/${idExpediente}`)
      .pipe( map(data => {
        return data['listResponseEvidenciaFotografica'];
      }));
  }

  setCargarAnexo(tipoExpediente: string, data: any) {
    return this.postQuery(`expedients/${tipoExpediente}/anexo`, data);
  }

  deleteAnexo(id: string) {
    return this.deleteQuery(`expedients/anexo/${id}`);
  }

  getChartsDashboardMain() {
    return this.getQuery("dashboard/global");
  }

  getChartsDashboard(tipoExpediente: string, ejercicio: number, idAuditor: string) {
    return this.getQuery(`dashboard/procedimiento/${tipoExpediente}/${ejercicio}/${idAuditor}`)
      .pipe( map(data => {
        return data['graficas'];
      }));
  }

  setActualizarContrasena(data: any) {
    return this.postQuery('password/change', data);
  }

  getListGeneralUsers() {
    return this.getQuery("configuration/list/users")
      .pipe( map(data => {
        return data['ListUsers'];
      }));
  }

  setUpdateGeneralUserPassword(data: any) {
    return this.putQuery("configuration/password/change/password", data);
  }

  setUpdateGeneralUserStatus(data: any) {
    return this.putQuery("configuration/change/status", data);
  }

  setAsignarAuditorBitacora(tipo: string, expediente: string) {
    return this.putQuery(`bitacora/asignacion/auditor/${tipo}/${expediente}`);
  }

  getDB(municipio: string) {
    return this.getQuery(`report/obra/general/adquisiciones/obra/zip/${municipio}`);
  }

}

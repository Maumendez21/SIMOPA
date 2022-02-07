import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { DashboardMainComponent } from './components/dashboard-main/dashboard-main.component';

import { ListadoObrasComponent } from './components/obras/listado-obras/listado-obras.component';
import { DetalleObrasComponent } from './components/obras/detalle-obras/detalle-obras.component';
import { MapaObrasComponent } from './components/obras/mapa-obras/mapa-obras.component';
import { EvidenciaFotograficaComponent } from './components/obras/evidencia-fotografica/evidencia-fotografica.component';
import { VisitaObraComponent } from './components/obras/visita-obra/visita-obra.component';
import { DashboardObrasComponent } from './components/obras/dashboard-obras/dashboard-obras.component';
import { EvidenciaFotograficaGeneralComponent } from './components/obras/evidencia-fotografica-general/evidencia-fotografica-general.component';
import { NuevoExpedienteObrasComponent } from './components/obras/nuevo-expediente-obras/nuevo-expediente-obras.component';
import { EditarExpedienteObrasComponent } from './components/obras/editar-expediente-obras/editar-expediente-obras.component';

import { DetalleAdquisicionesComponent } from './components/adquisiciones/detalle-adquisiciones/detalle-adquisiciones.component';
import { DocumentosAdquisicionesComponent } from './components/adquisiciones/documentos-adquisiciones/documentos-adquisiciones.component';
import { EvidenciaFotograficaAdquisicionComponent } from './components/adquisiciones/evidencia-fotografica-adquisicion/evidencia-fotografica-adquisicion.component';
import { DashboardComponent } from './components/adquisiciones/dashboard/dashboard.component';
import { NuevoExpedienteAdquisicionesComponent } from './components/adquisiciones/nuevo-expediente-adquisiciones/nuevo-expediente-adquisiciones.component';
import { EditarExpedienteAdquisicionesComponent } from './components/adquisiciones/editar-expediente-adquisiciones/editar-expediente-adquisiciones.component';
import { SumariaComponent } from './components/adquisiciones/sumaria/sumaria.component';

import { CambiarContrasenaComponent } from './components/configuracion/cambiar-contrasena/cambiar-contrasena.component';
import { UsuariosGeneralComponent } from './components/configuracion/usuarios-general/usuarios-general.component';

export const ROUTES: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'home', component: HomeComponent },
    { path: 'dashboard', component: DashboardMainComponent },
    //Obras
    { path: 'obras/detalle', component: ListadoObrasComponent },
    { path: 'obras/detalle/:estatusExpediente', component: ListadoObrasComponent },
    { path: 'obras/expediente/:id', component: DetalleObrasComponent },
    { path: 'obras/mapa', component: MapaObrasComponent },
    { path: 'obra/dashboard', component: DashboardObrasComponent },
    { path: 'obra/evidencia-fotografica/:id', component: EvidenciaFotograficaGeneralComponent },
    { path: 'obra/evidencia-fotografica/:idVisita/:idExpediente', component: EvidenciaFotograficaComponent },
    { path: 'obra/visita-obra/:id', component: VisitaObraComponent },
    { path: 'obra/nuevo-expediente', component: NuevoExpedienteObrasComponent },
    { path: 'obra/actualizar-expediente/:id', component: EditarExpedienteObrasComponent },
    //Adquisiciones
    { path: 'adquisiciones/detalle', component: DetalleAdquisicionesComponent },
    { path: 'adquisiciones/detalle/:estatusExpediente', component: DetalleAdquisicionesComponent },
    { path: 'adquisiciones/expediente/:id', component: DocumentosAdquisicionesComponent },
    { path: 'adquisicion/evidencia-fotografica/:id', component: EvidenciaFotograficaAdquisicionComponent },
    { path: 'adquisicion/dashboard', component: DashboardComponent },
    { path: 'adquisicion/nuevo-expediente', component: NuevoExpedienteAdquisicionesComponent },
    { path: 'adquisicion/actualizar-expediente/:id', component: EditarExpedienteAdquisicionesComponent },
    { path: 'adquisicion/sumaria', component: SumariaComponent },
    //Configuracion
    { path: 'configuracion/cambiar-contrasena', component: CambiarContrasenaComponent },
    { path: 'configuracion/usuarios-general', component: UsuariosGeneralComponent },
    //Restrictions
    { path: '', pathMatch: 'full', redirectTo: 'login' },
    { path: '**', pathMatch: 'full', redirectTo: 'login' }
];
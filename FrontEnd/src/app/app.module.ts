import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';
import { DataTablesModule } from 'angular-datatables';
import { registerLocaleData } from '@angular/common';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import localeEs from '@angular/common/locales/es';
registerLocaleData(localeEs);

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';

/*--- Router ---*/

import { ROUTES } from './app.routes';

/*--- Components ---*/

//Navs
import { NavbarComponent } from './components/shared/navbar/navbar.component';
import { SidebarComponent } from './components/shared/sidebar/sidebar.component';
import { FooterComponent } from './components/shared/footer/footer.component';

//Login
import { LoginComponent } from './components/login/login.component';

//General
import { HomeComponent } from './components/home/home.component';

//Obras
import { DashboardObrasComponent } from './components/obras/dashboard-obras/dashboard-obras.component';
import { ListadoObrasComponent } from './components/obras/listado-obras/listado-obras.component';
import { DetalleObrasComponent } from './components/obras/detalle-obras/detalle-obras.component';
import { MapaObrasComponent } from './components/obras/mapa-obras/mapa-obras.component';
import { EvidenciaFotograficaComponent } from './components/obras/evidencia-fotografica/evidencia-fotografica.component';
import { VisitaObraComponent } from './components/obras/visita-obra/visita-obra.component';
import { NuevoExpedienteObrasComponent } from './components/obras/nuevo-expediente-obras/nuevo-expediente-obras.component';

//Adquisiciones
import { DashboardComponent } from './components/adquisiciones/dashboard/dashboard.component';
import { DetalleAdquisicionesComponent } from './components/adquisiciones/detalle-adquisiciones/detalle-adquisiciones.component';
import { DocumentosAdquisicionesComponent } from './components/adquisiciones/documentos-adquisiciones/documentos-adquisiciones.component';
import { EvidenciaFotograficaAdquisicionComponent } from './components/adquisiciones/evidencia-fotografica-adquisicion/evidencia-fotografica-adquisicion.component';
import { NuevoExpedienteAdquisicionesComponent } from './components/adquisiciones/nuevo-expediente-adquisiciones/nuevo-expediente-adquisiciones.component';
import { SumariaComponent } from './components/adquisiciones/sumaria/sumaria.component';

//Shared
import { CardObraComponent } from './components/shared/cards/card-obra/card-obra.component';

//Shared - Dashboard Main
import { CardPresupuestoComprometidoDmComponent } from './components/shared/dashboard-main/card-presupuesto-comprometido-dm/card-presupuesto-comprometido-dm.component';
import { CardPresupuestoGraficaDmComponent } from './components/shared/dashboard-main/card-presupuesto-grafica-dm/card-presupuesto-grafica-dm.component';
import { CardTopDmComponent } from './components/shared/dashboard-main/card-top-dm/card-top-dm.component';

//Shared - Dashboard

import { CardDashboardComponent } from './components/shared/dashboard/card-dashboard/card-dashboard.component';
import { ChartDashboardComponent } from './components/shared/dashboard/chart-dashboard/chart-dashboard.component';
import { TableDocumentsComponent } from './components/shared/dashboard/table-documents/table-documents.component';

/*--- Service ---*/

import { HttpClientModule } from '@angular/common/http';
import { PbrService } from './services/pbr.service';

/*--- Pipes ---*/

import { NotextPipe } from './pipes/notext.pipe';
import { BreadcrumbComponent } from './components/shared/breadcrumb/breadcrumb/breadcrumb.component';
import { SharedService } from './services/shared.service';
import { CardAdjudicacionComponent } from './components/shared/cards/card-adjudicacion/card-adjudicacion.component';
import { FiltersListaObrasPipe } from './pipes/filters-lista-obras.pipe';
import { FiltersListaAdjudicacionesPipe } from './pipes/filters-lista-adjudicaciones.pipe';

/*--- External ---*/

import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { ToastrModule } from 'ngx-toastr';
import { ChartsModule } from 'ng2-charts';
import { DashboardMainComponent } from './components/dashboard-main/dashboard-main.component';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { GaugeChartModule } from 'angular-gauge-chart';
import { EvidenciaFotograficaGeneralComponent } from './components/obras/evidencia-fotografica-general/evidencia-fotografica-general.component';
import { EditarExpedienteAdquisicionesComponent } from './components/adquisiciones/editar-expediente-adquisiciones/editar-expediente-adquisiciones.component';
import { EditarExpedienteObrasComponent } from './components/obras/editar-expediente-obras/editar-expediente-obras.component';
import { DatatableAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/datatable-adq/datatable-adq.component';
import { CardHeaderAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/card-header-adq/card-header-adq.component';
import { CargaDocumentoModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/carga-documento-modal-adq/carga-documento-modal-adq.component';
import { RegistrarObservacionModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/registrar-observacion-modal-adq/registrar-observacion-modal-adq.component';
import { VerDocumentoModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/ver-documento-modal-adq/ver-documento-modal-adq.component';
import { CambiarEstatusModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/cambiar-estatus-modal-adq/cambiar-estatus-modal-adq.component';
import { ObservacionesRecomendacionesModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/observaciones-recomendaciones-modal-adq/observaciones-recomendaciones-modal-adq.component';
import { AnexosModalAdqComponent } from './components/shared/adquisiciones/documentos-adquisiciones/modals/anexos-modal-adq/anexos-modal-adq.component';
import { ToastUploadComponent } from './components/shared/toast-upload/toast-upload.component';
import { NavFiltersAdqComponent } from './components/shared/adquisiciones/detalle-adquisiciones/nav-filters-adq/nav-filters-adq.component';
import { CardGraphicPieComponent } from './components/shared/dashboard-main/card-graphic-pie/card-graphic-pie.component';
import { CardChartLineComponent } from './components/shared/dashboard-main/card-chart-line/card-chart-line.component';
import { TableInfoGlobalDmComponent } from './components/shared/dashboard-main/table-info-global-dm/table-info-global-dm.component';
import { FormInputComponent } from './components/shared/forms/form-input/form-input.component';
import { FormSelectComponent } from './components/shared/forms/form-select/form-select.component';
import { ChartDoughnutDashboardComponent } from './components/shared/dashboard/chart-doughnut-dashboard/chart-doughnut-dashboard.component';
import { ChartBarDashboardComponent } from './components/shared/dashboard/chart-bar-dashboard/chart-bar-dashboard.component';
import { CambiarContrasenaComponent } from './components/configuracion/cambiar-contrasena/cambiar-contrasena.component';
import { ConfiguracionLgComponent } from './components/shared/list-group/configuracion-lg/configuracion-lg.component';
import { UsuariosGeneralComponent } from './components/configuracion/usuarios-general/usuarios-general.component';
import { TableInfoGlobalEjercidoComponent } from './components/shared/dashboard-main/table-info-global-ejercido/table-info-global-ejercido.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    ListadoObrasComponent,
    DetalleObrasComponent,
    MapaObrasComponent,
    DetalleAdquisicionesComponent,
    CardObraComponent,
    DocumentosAdquisicionesComponent,
    NotextPipe,
    BreadcrumbComponent,
    CardAdjudicacionComponent,
    FiltersListaObrasPipe,
    FiltersListaAdjudicacionesPipe,
    EvidenciaFotograficaComponent,
    EvidenciaFotograficaAdquisicionComponent,
    DashboardComponent,
    DashboardObrasComponent,
    DashboardMainComponent,
    VisitaObraComponent,
    NuevoExpedienteAdquisicionesComponent,
    NuevoExpedienteObrasComponent,
    EvidenciaFotograficaGeneralComponent,
    EditarExpedienteAdquisicionesComponent,
    EditarExpedienteObrasComponent,
    CardPresupuestoComprometidoDmComponent,
    CardPresupuestoGraficaDmComponent,
    CardTopDmComponent,
    CardDashboardComponent,
    TableDocumentsComponent,
    ChartDashboardComponent,
    DatatableAdqComponent,
    CardHeaderAdqComponent,
    CargaDocumentoModalAdqComponent,
    RegistrarObservacionModalAdqComponent,
    VerDocumentoModalAdqComponent,
    CambiarEstatusModalAdqComponent,
    ObservacionesRecomendacionesModalAdqComponent,
    AnexosModalAdqComponent,
    ToastUploadComponent,
    NavFiltersAdqComponent,
    CardGraphicPieComponent,
    CardChartLineComponent,
    TableInfoGlobalDmComponent,
    SumariaComponent,
    FormInputComponent,
    FormSelectComponent,
    ChartDoughnutDashboardComponent,
    ChartBarDashboardComponent,
    CambiarContrasenaComponent,
    ConfiguracionLgComponent,
    UsuariosGeneralComponent,
    TableInfoGlobalEjercidoComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    DataTablesModule,
    FormsModule,
    BrowserAnimationsModule,
    ChartsModule,
    GaugeChartModule,
    NgxDocViewerModule,
    ToastrModule.forRoot(),
    AgmCoreModule.forRoot({
      apiKey: "AIzaSyDUxZIf-C1Tg8Uy0Bm4f7oDQZvHCeOZxZQ"
    }),
    RouterModule.forRoot( ROUTES, { useHash: true } ),
  ],
  providers: [
    PbrService,
    SharedService,
    CurrencyPipe,
    DatePipe,
    {
      provide: LOCALE_ID,
      useValue: 'es'
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

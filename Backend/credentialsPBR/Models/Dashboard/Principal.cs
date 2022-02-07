using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Dashboard
{
    public class Principal
    {
        //-----------------------------------------------------------------------------------
        //---------------------Header Main
        public double PresupuestoAutorizado { get; set; }
        public string PresupuestoAutorizado_Texto { get; set; }
        public double PresupuestoComprometido { get; set; }
        public string PresupuestoComprometido_Texto { get; set; }
        public int PorcentajeContratadoPresupuestoAutorizado { get; set; }
        //-----------------------------------------------------------------------------------
        //---------------------Header Adquisiciones
        public double PresupuestoAutorizadoAdquisiciones { get; set; }
        public string PresupuestoAutorizadoAdquisiciones_Texto { get; set; }
        public int PorcentajePresupuestoAutorizadoAdquisiciones { get; set; }
        //-----------------------------------------------------------------------------------
        //---------------------Header ObraPublica
        public double PresupuestoAutorizadoObraPublica { get; set; }
        public string PresupuestoAutorizadoObraPublica_Texto { get; set; }
        public int PorcentajePresupuestoAutorizadoObraPublica { get; set; }
        //-----------------------------------------------------------------------------------
        //---------------------Grafica Adquisiciones   
        public double MontoAdquisicionesContratadas { get; set; }
        public double MontoAdquisicionesContratadasComplemento { get; set; }
        public double MontoAdquisicionesPagadas { get; set; }
        public double MontoAdquisicionesPagadasComplemento { get; set; }
        //-----------------------------------------------------------------------------------
        //---------------------Grafica ObraPublica
        public double MontoObrasContratadas { get; set; }
        public double MontoObrasContratadasComplemento { get; set; }
        public double MontoObrasPagadas { get; set; }
        public double MontoObrasPagadasComplemento { get; set; }

        public double PorcentajeAdj { get; set; }
        public double PorcentajeObra { get; set; }

        public string montoEjercidoCabezara { get; set; }
        public string montoContratodoCabezera { get; set; }

        public string montoContratoAdquisiciones { get; set; }
        public string montoContratoObraPublica { get; set; }
        public string montoEjercido { get; set; }
        public string montoAsignado { get; set; }

        public double PorcentajeAdquisiciones { get; set; }
        public double PorcentajeObraPublica { get; set; }
    }
}


//public int AdquisicionesPresupuestadas { get; set; }
//public int AdquisicionesContratadas { get; set; }
//public int AdquisicionesConProblemas { get; set; }

//public int ObrasPresupuestadas { get; set; }
//public int ObrasContratadas { get; set; }
//public int ObrasConProblemas { get; set; }
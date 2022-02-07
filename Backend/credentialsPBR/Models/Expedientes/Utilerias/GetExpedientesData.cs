using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class GetExpedientesData
    {
        public async System.Threading.Tasks.Task<List<ExpedienteData>> GetInformationFromExpedientsAsync(string tipo, string municipio, string auditor, string ejercicio)
        {
            List<ExpedienteData> Expedientes = new List<ExpedienteData>();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);

            if (tipo == "adquisiciones")
            {
                try
                {

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");


                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio) & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                    if (auditor != "auditorestodosdq37iq84burwwjdbsd3")
                    {
                        filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio) 
                            & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio) 
                            & Builders<AdquisicionesV1>.Filter.Eq(x => x.auditor, auditor);
                    }
                    

                    var result = await collection.Find(filter).ToListAsync();

                    if (result != null)
                    {

                        foreach (var r in result)
                        {
                            ExpedienteData expediente = new ExpedienteData();

                            expediente.estatus = r.estatusExpediente;
                            expediente.idExpediente = r.Id;
                            expediente.NombreExpediente = r.objetoContrato;
                            try {
                                expediente.MontoContratado = Convert.ToDouble(r.montoAdjudicado);
                            }
                            catch (Exception ex)
                            {
                                expediente.MontoContratado = 0;
                            }

                            if (expediente.estatus == "EN REVISION")
                            {
                                int si = r.documentos.Where(x => x.estatus == "SI").Count();
                                int no = r.documentos.Where(x => x.estatus == "NO").Count();
                                expediente.AvanceDocumental = (si * 100) / (si + no);
                            }
                            else if (expediente.estatus == "REVISADO CON OBSERVACIONES")
                            {
                                int si = r.documentos.Where(x => x.estatus == "SI").Count();
                                int no = r.documentos.Where(x => x.estatus == "NO").Count();
                                expediente.AvanceDocumental = (si * 100) / (si + no);
                            }

                            //Calculo de Porcentaje
                            int valSi = r.documentos.Where(x => x.estatus == "SI").Count();
                            int valNo = r.documentos.Where(x => x.estatus == "NO").Count();
                            //int valNull = r.documentos.Where(x => x.estatus == null).Count();

                            int total = valNo + valSi; //+valNull

                            if (total == 0)
                            {
                                expediente.Porcentaje= 0;
                            }
                            else
                            {
                                expediente.Porcentaje = (valSi * 100) / total;
                            }

                            Expedientes.Add(expediente);

                        }
                    }

                    return Expedientes;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                try
                {

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    
                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio) & Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                    if (auditor != "auditorestodosdq37iq84burwwjdbsd3")
                    {
                        filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio)
                            & Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                            & Builders<ObraPublicaV1>.Filter.Eq(x => x.auditor, auditor);
                    }


                    var result = await collection.Find(filter).ToListAsync();

                    if (result != null)
                    {
                        foreach (var r in result)
                        {
                            ExpedienteData expediente = new ExpedienteData();

                            expediente.estatus = r.estatusExpediente;
                            expediente.idExpediente = r.Id;
                            expediente.NombreExpediente = r.nombreObra;
                            try
                            {
                                expediente.MontoContratado = Convert.ToDouble(r.montoContrato);
                                expediente.MontoPagado = Convert.ToDouble(r.montoEjercido);
                                expediente.MontoAsignado = Convert.ToDouble(r.montoAsignado);

                            }
                            catch
                            {
                                expediente.MontoContratado = 0;
                                expediente.MontoPagado = 0;
                                expediente.MontoAsignado = 0;
                            }
                            

                            if (expediente.estatus == "EN REVISION")
                            {
                                int si = r.documentos.Where(x => x.integracion == "SI").Count();
                                int no = r.documentos.Where(x => x.integracion == "NO").Count();
                                expediente.AvanceDocumental = (si * 100) / (si + no);
                            }
                            else if (expediente.estatus == "REVISADO CON OBSERVACIONES")
                            {
                                int si = r.documentos.Where(x => x.integracion == "SI").Count();
                                int no = r.documentos.Where(x => x.integracion == "NO").Count();
                                expediente.AvanceDocumental = (si * 100) / (si + no);
                            }

                            //Calculo de Porcentaje
                            int valSi = r.documentos.Where(x => x.integracion == "SI").Count();
                            int valNo = r.documentos.Where(x => x.integracion == "NO").Count();
                            //int valNull = r.documentos.Where(x => x.integracion == null).Count();

                            int total = valNo + valSi; //valNull + 

                            if (total == 0)
                            {
                                expediente.Porcentaje = 0;
                            }
                            else
                            {
                                expediente.Porcentaje = (valSi * 100) / total;
                            }


                            Expedientes.Add(expediente);
                        }
                    }

                    return Expedientes;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public async System.Threading.Tasks.Task<Graficas> GetInformationFromProcedimientoAsync(string tipo, string municipio, string auditor, string ejercicio)
        {
            Graficas Graficas = new Graficas();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);

            List<string> monto = new List<string>();
            List<string> label = new List<string>();
            List<string> cantidad = new List<string>();
            List<string> listPorc = new List<string>();
            List<string> porcCantidad = new List<string>();
            List<string> porcentajes = new List<string>();

            if (tipo == "adquisiciones")
            {
                try
                {
                    int adjudicion = 0;
                    int revision = 0;
                    int invitacion = 0;
                    int concurso = 0;
                    int licitacion = 0;

                    decimal montoAdjudicacion = 0;
                    decimal montoRevision = 0;
                    decimal montoInvitacion = 0;
                    decimal montoConcurso = 0;
                    decimal montoLicitacion = 0;

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio) & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                    if (auditor != "auditorestodosdq37iq84burwwjdbsd3")
                    {
                        filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                            & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                            & Builders<AdquisicionesV1>.Filter.Eq(x => x.auditor, auditor);
                    }

                    var result = await collection.Find(filter).ToListAsync();

                    if (result != null)
                    {
                        foreach (var data in result)
                        {
                            if(data.tipoAdjudicacion.Equals("ADJUDICACIÓN DIRECTA"))
                            {
                                adjudicion++;
                                montoAdjudicacion = montoAdjudicacion + Convert.ToDecimal(data.montoAdjudicado);
                            }
                            else if (data.tipoAdjudicacion.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES - 33 MIL"))
                            {
                                revision++;
                                montoRevision = montoRevision + Convert.ToDecimal(data.montoAdjudicado);
                            }
                            else if (data.tipoAdjudicacion.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                            {
                                invitacion++;
                                montoInvitacion = montoInvitacion + Convert.ToDecimal(data.montoAdjudicado);
                            }
                            else if (data.tipoAdjudicacion.Equals("CONCURSO POR INVITACIÓN"))
                            {
                                concurso++;
                                montoConcurso = montoConcurso + Convert.ToDecimal(data.montoAdjudicado);
                            }
                            else if (data.tipoAdjudicacion.Equals("LICITACIÓN PÚBLICA"))
                            {
                                licitacion++;
                                montoLicitacion = montoLicitacion + Convert.ToDecimal(data.montoAdjudicado);
                            }

                            //Calculo de Porcentaje
                            int valSi = data.documentos.Where(x => x.estatus == "SI").Count();
                            int valNo = data.documentos.Where(x => x.estatus == "NO").Count();
                            int valNull = data.documentos.Where(x => x.estatus == null).Count();
                            double porcentaje = 0;
                            int total = valNull+ valNo + valSi;

                            if (total == 0)
                            {
                                porcentaje = 0;
                            }
                            else
                            {
                                porcentaje = (valSi * 100) / total;
                            }
                            
                            listPorc.Add(porcentaje.ToString());

                        }

                        cantidad.Add(adjudicion.ToString());
                        cantidad.Add(revision.ToString());
                        cantidad.Add(invitacion.ToString());
                        cantidad.Add(concurso.ToString());
                        cantidad.Add(licitacion.ToString());

                        label.Add("ADJ.DIR.");// ADJUDICACIÓN DIRECTA");
                        label.Add("COMPRAS - 33 MIL");// REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES - 33 MIL");
                        label.Add("INV. 3 PER.");// INVITACIÓN A CUANDO MENOS TRES PERSONAS");
                        label.Add("CON.POR INV.");// CONCURSO POR INVITACIÓN");
                        label.Add("L.P");// LICITACIÓN PÚBLICA");

                        //ADJ.DIR.
                        //COMPRAS - 33 MIL
                        //INV. 3 PER.
                        //CON.POR INV.
                        //L.P

                        monto.Add(Math.Round(montoAdjudicacion, 2).ToString("###0"));
                        monto.Add(Math.Round(montoRevision, 2).ToString("###0"));
                        monto.Add(Math.Round(montoInvitacion, 2).ToString("###0"));
                        monto.Add(Math.Round(montoConcurso, 2).ToString("###0"));
                        monto.Add(Math.Round(montoLicitacion, 2).ToString("###0"));

                        Graficas.GraficaBarras = new DataGrafica();
                        Graficas.GraficaBarras.data = new List<string>();
                        Graficas.GraficaBarras.label = new List<string>();

                        Graficas.GraficaBarras.data = monto;
                        Graficas.GraficaBarras.label = label;

                        Graficas.GraficaCake = new DataGrafica();
                        Graficas.GraficaCake.data = new List<string>(); 
                        Graficas.GraficaCake.label = new List<string>();

                        label = new List<string>();

                        label.Add("ADJUDICACIÓN DIRECTA");
                        label.Add("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES - 33 MIL");
                        label.Add("INVITACIÓN A CUANDO MENOS TRES PERSONAS");
                        label.Add("CONCURSO POR INVITACIÓN");
                        label.Add("LICITACIÓN PÚBLICA");

                        Graficas.GraficaCake.data = cantidad;
                        Graficas.GraficaCake.label = label;

                        listPorc.Sort();
                        int uno = 0;
                        int dos = 0;
                        int tres = 0;
                        int cuatro = 0;
                        int cinco = 0;
                        int seis = 0;
                        int siete = 0;
                        int ocho = 0;
                        int nueve = 0;
                        int diez = 0;

                        foreach (string var in listPorc)
                        {
                            double val = Convert.ToDouble(var);
                            if(val >= 0 & val <=10)
                            {
                                uno++;
                            }
                            else if (val >= 11 & val <= 20)
                            {
                                dos++;
                            }
                            else if (val >= 21 & val <= 30)
                            {
                                tres++;
                            }
                            else if (val >= 31 & val <= 40)
                            {
                                cuatro++;
                            }
                            else if (val >= 41 & val <= 50)
                            {
                                cinco++;
                            }
                            else if (val >= 51 & val <= 60)
                            {
                                seis++;
                            }
                            else if (val >= 61 & val <= 70)
                            {
                                siete++;
                            }
                            else if (val >= 71 & val <= 80)
                            {
                                ocho++;
                            }
                            else if (val >= 81 & val <= 90)
                            {
                                nueve++;
                            }
                            else if (val >= 91 & val <= 100)
                            {
                                diez++;
                            }
                        }

                        porcCantidad.Add(uno.ToString());
                        porcCantidad.Add(dos.ToString());
                        porcCantidad.Add(tres.ToString());
                        porcCantidad.Add(cuatro.ToString());
                        porcCantidad.Add(cinco.ToString());
                        porcCantidad.Add(seis.ToString());
                        porcCantidad.Add(siete.ToString());
                        porcCantidad.Add(ocho.ToString());
                        porcCantidad.Add(nueve.ToString());
                        porcCantidad.Add(diez.ToString());

                        porcentajes.Add("0% - 10%");
                        porcentajes.Add("11% - 20%");
                        porcentajes.Add("21% - 30%");
                        porcentajes.Add("31% - 40%");
                        porcentajes.Add("41% - 50%");
                        porcentajes.Add("51% - 60%");
                        porcentajes.Add("61% - 70%");
                        porcentajes.Add("71% - 80%");
                        porcentajes.Add("81% - 90%");
                        porcentajes.Add("91% - 100%");

                        Graficas.GraficaIntegreacion = new DataGrafica();
                        Graficas.GraficaIntegreacion.data = porcCantidad;
                        Graficas.GraficaIntegreacion.label = porcentajes;

                    }

                    return Graficas;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    int adjudicionObra = 0;
                    int invCinco = 0;
                    int invTres = 0;
                    int licitacionObra = 0;

                    decimal montoAdjObra = 0;
                    decimal montoInvCinco = 0;
                    decimal montoInvTres = 0;
                    decimal montoLicObra = 0;

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio) & Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                    if (auditor != "auditorestodosdq37iq84burwwjdbsd3")
                    {
                        filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio)
                            & Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                            & Builders<ObraPublicaV1>.Filter.Eq(x => x.auditor, auditor);
                    }

                    var result = await collection.Find(filter).ToListAsync();

                    if (result != null)
                    {
                        foreach (var data in result)
                        {
                            if (data.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 5 PROVEEDORES"))
                            {
                                invCinco++;
                                montoInvCinco = montoInvCinco + Convert.ToDecimal(data.montoContrato);
                            }
                            else if (data.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 3 PROVEEDORES"))
                            {
                                invTres++;
                                montoInvTres = montoInvTres + Convert.ToDecimal(data.montoContrato);
                            }
                            else if (data.tipoAdjudicacion.Equals("LICITACIÓN PÚBLICA"))
                            {
                                licitacionObra++;
                                montoLicObra = montoLicObra + Convert.ToDecimal(data.montoContrato);
                            }
                            else if (data.tipoAdjudicacion.Equals("ADJUDICACIÓN DIRECTA"))
                            {
                                adjudicionObra++;
                                montoAdjObra = montoAdjObra + Convert.ToDecimal(data.montoContrato);
                            }

                            //Calculo de Porcentaje
                            int valSi = data.documentos.Where(x => x.integracion == "SI").Count();
                            int valNo = data.documentos.Where(x => x.integracion == "NO").Count();
                            int valNull = data.documentos.Where(x => x.integracion == null).Count();
                            double porcentaje = 0;
                            int total = valNull + valNo + valSi;

                            if (total == 0)
                            {
                                porcentaje = 0;
                            }
                            else
                            {
                                porcentaje = (valSi * 100) / total;
                            }

                            listPorc.Add(porcentaje.ToString());

                        }

                        cantidad.Add(invCinco.ToString());
                        cantidad.Add(invTres.ToString());
                        cantidad.Add(licitacionObra.ToString());
                        cantidad.Add(adjudicionObra.ToString());

                        //label.Add("INVITACIÓN RESTRINGIDA A 5 PROVEEDORES");
                        //label.Add("INVITACIÓN RESTRINGIDA A 3 PROVEEDORES");
                        //label.Add("LICITACIÓN PÚBLICA");
                        //label.Add("ADJUDICACIÓN DIRECTA");
                        
                        label.Add("INV. REST.5 PROV.");
                        label.Add("INV. REST.3 PROV.");
                        label.Add("L.P");
                        label.Add("ADJ.DIR.");

                        monto.Add(Math.Round(montoInvCinco, 2).ToString("###0"));
                        monto.Add(Math.Round(montoInvTres, 2).ToString("###0"));
                        monto.Add(Math.Round(montoLicObra, 2).ToString("###0"));
                        monto.Add(Math.Round(montoAdjObra, 2).ToString("###0"));

                        Graficas.GraficaBarras = new DataGrafica();
                        Graficas.GraficaBarras.data = new List<string>();
                        Graficas.GraficaBarras.label = new List<string>();

                        Graficas.GraficaBarras.data = monto;
                        Graficas.GraficaBarras.label = label;

                        Graficas.GraficaCake = new DataGrafica();
                        Graficas.GraficaCake.data = new List<string>();
                        Graficas.GraficaCake.label = new List<string>();

                        Graficas.GraficaCake.data = cantidad;
                        Graficas.GraficaCake.label = label;

                        listPorc.Sort();
                        int uno = 0;
                        int dos = 0;
                        int tres = 0;
                        int cuatro = 0;
                        int cinco = 0;
                        int seis = 0;
                        int siete = 0;
                        int ocho = 0;
                        int nueve = 0;
                        int diez = 0;

                        foreach (string var in listPorc)
                        {
                            double val = Convert.ToDouble(var);
                            if (val >= 0 & val <= 10)
                            {
                                uno++;
                            }
                            else if (val >= 11 & val <= 20)
                            {
                                dos++;
                            }
                            else if (val >= 21 & val <= 30)
                            {
                                tres++;
                            }
                            else if (val >= 31 & val <= 40)
                            {
                                cuatro++;
                            }
                            else if (val >= 41 & val <= 50)
                            {
                                cinco++;
                            }
                            else if (val >= 51 & val <= 60)
                            {
                                seis++;
                            }
                            else if (val >= 61 & val <= 70)
                            {
                                siete++;
                            }
                            else if (val >= 71 & val <= 80)
                            {
                                ocho++;
                            }
                            else if (val >= 81 & val <= 90)
                            {
                                nueve++;
                            }
                            else if (val >= 91 & val <= 100)
                            {
                                diez++;
                            }
                        }

                        porcCantidad.Add(uno.ToString());
                        porcCantidad.Add(dos.ToString());
                        porcCantidad.Add(tres.ToString());
                        porcCantidad.Add(cuatro.ToString());
                        porcCantidad.Add(cinco.ToString());
                        porcCantidad.Add(seis.ToString());
                        porcCantidad.Add(siete.ToString());
                        porcCantidad.Add(ocho.ToString());
                        porcCantidad.Add(nueve.ToString());
                        porcCantidad.Add(diez.ToString());

                        porcentajes.Add("0% - 10%");
                        porcentajes.Add("11% - 20%");
                        porcentajes.Add("21% - 30%");
                        porcentajes.Add("31% - 40%");
                        porcentajes.Add("41% - 50%");
                        porcentajes.Add("51% - 60%");
                        porcentajes.Add("61% - 70%");
                        porcentajes.Add("71% - 80%");
                        porcentajes.Add("81% - 90%");
                        porcentajes.Add("91% - 100%");

                        Graficas.GraficaIntegreacion = new DataGrafica();
                        Graficas.GraficaIntegreacion.data = porcCantidad;
                        Graficas.GraficaIntegreacion.label = porcentajes;
                    }

                    return Graficas;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

    }
}
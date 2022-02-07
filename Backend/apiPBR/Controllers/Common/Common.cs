using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Utilerias;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace apiPBR.Controllers.Common
{
    public class Common
    {

        public const string ADJUDICACION = "ADJUDICACIÓN DIRECTA";
        public const string INVITACION = "INVITACIÓN A CUANDO MENOS TRES PERSONAS";
        public const string CONCURSO = "CONCURSO POR INVITACIÓN";
        public const string LICITACION = "LICITACIÓN PÚBLICA";
        public const string REVISION = "REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL";
        public const string OBRA = "OBRA PÚBLICA";
        public const string NO = "NO";
        public const string COMENTARIO = "0 / 0";
        public const string DOCADMIN = "DOCUMENTACIÓN ADMINISTRATIVA";
        public const string ENTREGABLES = "ENTREGABLES";


        public List<DocumentoAdquisicionesV1> ListDocumentosAdquisiciones(string tipo)
        {
            List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();
            DocumentoAdquisicionesV1 documento = new DocumentoAdquisicionesV1();

            if(tipo.Equals(ADJUDICACION))
            {
                #region ADJUDICACION
                #endregion
            }
            else if (tipo.Equals(INVITACION))
            {
                #region INVITACION
                #endregion
            }
            else if (tipo.Equals(CONCURSO))
            {
                #region CONCURSO
                #endregion
            }
            else if (tipo.Equals(LICITACION))
            {
                #region LICITACION
                #endregion
            }
            else if (tipo.Equals(REVISION))
            {
                #region REVISION
                documento = new DocumentoAdquisicionesV1();
                documento.clave = 1;
                documento.grupo = DOCADMIN;
                documento.documento = "Oficio de solicitud de suficiencia presupuestal";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 2;
                documento.grupo = DOCADMIN;
                documento.documento = "Oficio de autorización de suficiencia presupuestal";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 3;
                documento.grupo = DOCADMIN;
                documento.documento = "Requisición por escrito del bien y/ó servicio";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 4;
                documento.grupo = ENTREGABLES;
                documento.documento = "Documento en el que conste la recepción del Bien y/ó Servicio";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 5;
                documento.grupo = ENTREGABLES;
                documento.documento = "Reporte fotografíco (para el caso que aplique)";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 6;
                documento.grupo = ENTREGABLES;
                documento.documento = "Orden de Pago";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 7;
                documento.grupo = ENTREGABLES;
                documento.documento = "Facturas";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);

                documento = new DocumentoAdquisicionesV1();
                documento.clave = 8;
                documento.grupo = ENTREGABLES;
                documento.documento = "Validación del SAT";
                documento.estatus = NO;
                documento.referencia = string.Empty;
                documento.comentario = COMENTARIO;
                documento.recomendacion = string.Empty;
                listDoc.Add(documento);
                #endregion
            }
            else if (tipo.Equals(OBRA))
            {
                #region OBRA
                #endregion
            }

            return listDoc;
        }

        public string FundamentosAdquisiciones(string municipio, string tipo, string documento)
        {
            string fundamento = string.Empty;
                
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);

            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<Fundamentos>("Fundamentos");
            var filter = Builders<Fundamentos>.Filter.Eq(x => x.Municipio, municipio)
                & Builders<Fundamentos>.Filter.Eq(x => x.Procedimiento, tipo)
                & Builders<Fundamentos>.Filter.Eq(x => x.Documento, documento);

            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
            {
                fundamento = result.Fundamento;
            }

            return fundamento;
        }


        /// <summary>
        /// Fundamentos la Adquisiciones
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="clave"></param>
        /// <returns></returns>
        public string Fundamentos(string tipo, string clave)
        {
            string fundamento = string.Empty;

            if (tipo.Equals(ADJUDICACION))
            {
                switch (clave)
                {
                    case "Oficio de solicitud de suficiencia presupuestal": fundamento = "Art. 58 LAASSPEM"; break;
                    case "Solicitud de Contratación de Servicios (Consultoría, asesría, estudios e investigaciones)": fundamento = "Art. 54 LAASSPEM"; break;
                    case "Oficio de autorización de suficiencia presupuestal": fundamento = "Art. 53 fracc. II, 58 LAASSPEM"; break;
                    case "Requisición por escrito del bien y/ó servicio": fundamento = "Art. 45 fracc. X  LAASSPEM"; break;
                    case "Constancia de no existir trabajos relacionados (consultoría, asesoría, estudios e investigación)": fundamento = "Art. 54 LAASSPEM"; break;
                    case "Cotizaciones": fundamento = "Art. 6 fracc. XVIII, 96 fracc. II LAASSPEM"; break;
                    case "Cuadro comparativo de al menos dos cotizaciones": fundamento = "Art. 96 fracc. II LAASSPEM"; break;
                    case "Dictamen de Adjudicación Directa": fundamento = "Art. 22 LAASSPEM"; break;
                    case "Dictamen de Justificación de excepción a la Licitación Pública": fundamento = "Art. 22 LAASSPEM"; break;
                    case "Contrato": fundamento = "Art. 107 LAASSPEM"; break;
                    case "Contrato Abierto (para el caso que aplique)": fundamento = "Art. 108 LAASSPEM"; break;
                    case "Convenio Modificatorio(para el caso que aplique)": fundamento = "Art. 112 LAASSPEM"; break;
                    case "Escritura constitutiva en el caso de persona moral": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Acta de nacimiento o carta de naturalización en el caso de persona fisica": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Poder General del Representante Legal": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Objeto": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Comprobante de Domicilio": fundamento = "Art. 60, 62, 177 CCELSP"; break;
                    case "Identificación Oficial vigente": fundamento = string.Empty; break;
                    case "Declaración escrita bajo protesta de decir verdad, de no encontrarse en alguno delos supuestos establecidos por el articulo 77 de la LAASSPEM": fundamento = "Art. 77 LAASSPEM"; break;
                    case "Inscripción al Regristro Federal de Contribuyentes": fundamento = "Art. 26 fracc. II LAASSPEM"; break;
                    case "Curriculum Empresarial": fundamento = "Art. 72 fracc. I LAASSPEM"; break;
                    case "Inscripción en el Padrón de Proveedores Municipal": fundamento = "Art. 25 LAASSPEM"; break;
                    case "Declaración de pago de impuestos federales correspondientes a ejercicios anuales, semestrales, trimestrales o mensuales y/ó informes de instituciones financieras": fundamento = "Art. 71 fracc. II y IV LAASSPEM"; break;
                    case "Oficio por el que se remiten al Comité Municipal, copias de los pedidos formalizados de los contratos que se celebren, de las modificaciones que a estos se realicen, asi como de la fianza o garantía que se otorgue": fundamento = "Art. 45 fracc. IX LAASSPEM"; break;
                    case "Oficio mediante el se comunica al Comité Municipal la recepción de bienes y/ó servicios": fundamento = "Art. 45 fracc. XII LAASSPEM"; break;
                    case "Notificación a la Contraloría": fundamento = "Art. 19 LAASSPEM"; break;
                    case "Acta de Fallo": fundamento = "Art. 88 LAASSPEM"; break;
                    case "Notificación del Fallo": fundamento = "Art. 88 LAASSPEM"; break;
                    case "Garantía de anticipo (para el caso que aplique)": fundamento = "Art. 126 fracc.II LAASSPEM"; break;
                    case "Garantía de cumplimiento y posibles vicios ocultos": fundamento = "Art. 126 fracc. III LAASSPEM"; break;
                    case "Documento en el que conste la recepción del Bien y/ó Servicio": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Entregables": fundamento = string.Empty; break;
                    case "Reporte fotografíco (para el caso que aplique)": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Orden compromiso": fundamento = "Art. 7 fracc.X LAASSPEM"; break;
                    case "Orden de Pago": fundamento = "Art. 7 fracc.X LAASSPEM"; break;
                    case "Facturas": fundamento = "Art. 115 LAASSPEM"; break;
                    case "Validación del SAT": fundamento = "Art. 29 y 29 A CFF"; break;
                    case "Retención del 5 al millar": fundamento = "Art. 43 LIMSACH (2018 ó 2019)"; break;

                    default: fundamento = "N/A"; break;

                }
            }
            else if (tipo.Equals(INVITACION))
            {
                switch (clave)
                {
                    case "Programa anual de adquisiciones": fundamento = "Art. 53,55,56 LAASSPEM"; break;
                    case "Oficio de solicitud de suficiencia presupuestal": fundamento = "Art. 58 LAASSPEM"; break;
                    case "Oficio de autorización de suficiencia presupuestal": fundamento = "Art. 53 fracc. II, 58 LAASSPEM"; break;
                    case "Requisición por escrito del bien y/ó servicio": fundamento = "Art. 45 fracc. X y 100 fracc. I LAASSPEM"; break;
                    case "Constancia de no existir trabajos relacionados (consultoría, asesoría, estudios e investigación)" : fundamento = "Art. 54 LAASSPEM"; break;
                    case "Dictamen de Justificación de excepción a la Licitación Pública": fundamento = "Art. 22 LAASSPEM"; break;
                    case "Invitación del concurso donde se indicarán como mínimo, la cantidad y descripción de los bienes y/ó servicios requeridos, plazo y lugar de entrega, asi como condiciones de pago": fundamento = "Art. 100 fracc. II, III y IV LAASSPEM"; break;
                    case "Confirmación de recepción y de participación por parte de la persona moral o fisica que fue invitada": fundamento = "Art. 100 Fracc. II LAASSPEM"; break;
                    case "Acta(s) de Junta(s) de Aclaraciones (cuando aplique)": fundamento = "Art. 79 Fracc.III LAASSPEM"; break;
                    case "Cotizaciones": fundamento = "Art. 96 Fracc. II LAASSPEM"; break;
                    case "Propuesta Técnica": fundamento = "Art. 100 Fracc. II y IV LAASSPEM"; break;
                    case "Cuadro comparativo de al menos dos cotizaciones": fundamento = "Art. 100 fracc. VI LAASSPEM"; break;
                    case "Propuesta Económica": fundamento = "Art. 100 Fracc. II y IV LAASSPEM"; break;
                    case "Declaración de pago de impuestos federales correspondientes a ejercicios anuales, semestrales, trimestrales o mensuales y/ó informes de instituciones financieras": fundamento = "Art. 71 fracc. II y IV LAASSPEM"; break;
                    case "Acta de Presentación y Apertura de Propuestas": fundamento = "Art. 85 Fracc. IV y 86 LAASPEM"; break;
                    case "Acta de Dictamen Legal, Técnico y Economico": fundamento = "Art. 85 Fracc. V LAASPEM"; break;
                    case "Acta de Fallo": fundamento = "Art. 88 LAASPEM"; break;
                    case "Notificación de Fallo": fundamento = "Art. 88 LAASSPEM"; break;
                    case "Contrato": fundamento = "Art. 107 LAASSPEM"; break;
                    case "Contrato Abierto (para el caso que aplique)": fundamento = "Art. 108 LAASSPEM"; break;
                    case "Convenio Modificatorio (para el caso que aplique)": fundamento = "Art. 112 LAASSPEM"; break;
                    case "Escritura constitutiva en el caso de persona moral": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Objeto": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Acta de nacimiento o carta de naturalización en el caso de persona fisica": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Poder General del Representante Legal": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Comprobante de Domicilio": fundamento = "Art. 60, 62, 177 CCELSP"; break;
                    case "Identificación Oficial vigente": fundamento = "Art. 26 fracción I, 80 fracción II LAASSPEM"; break;
                    case "Declaración escrita bajo protesta de decir verdad, de no encontrarse en alguno delos supuestos establecidos por el articulo 77 de la LAASSPEM": fundamento = "Art. 77 LAASSPEM"; break;
                    case "Inscripción al Regristro Federal de Contribuyentes": fundamento = "Art. 26 fracc. II LAASSPEM"; break;
                    case "Carta de Exclusividad": fundamento = "Art. 20 fracción VII LAASSPEM"; break;
                    case "Curriculum Empresarial": fundamento = "Art. 72 fracc. I LAASSPEM"; break;
                    case "Inscripción en el Padrón de Proveedores Municipal": fundamento = "Art. 25 LAASSPEM"; break;
                    case "Oficio por el que se remiten al Comité Municipal, copias de los pedidos formalizados de los contratos que se celebren, de las modificaciones que a estos se realicen, asi como de la fianza o garantía que se otorgue": fundamento = "Art. 45 fracc. IX LAASSPEM"; break;
                    case "Oficio mediante el se comunica al Comité Municipal la recepción de bienes y/ó servicios": fundamento = "Art. 45 fracc. XII LAASSPEM"; break;
                    case "Garantía de Seriedad de la propuesta sin impuestos": fundamento = "Art. 126 fracc. I LAASSPEM"; break;
                    case "Garantía de anticipo (para el caso que aplique)": fundamento = "Art. 126 fracc.II LAASSPEM"; break;
                    case "Garantía de cumplimiento y posibles vicios ocultos": fundamento = "Art. 126 fracc. III LAASSPEM"; break;
                    case "Documento en el que conste la recepción del Bien y/ó Servicio": fundamento = "Art. 45 fracc VI y 72 fracción V LAASSPEM"; break;
                    case "Entregables": fundamento = "Art. 29 fracción II, 69 y 77 fracción V LAASSPEM"; break;
                    case "Reporte fotografíco (para el caso que aplique)": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Orden compromiso": fundamento = "Art. 7 fracc.X LAASSPEM"; break;
                    case "Orden de Pago": fundamento = "Art. 7 fracc.X LAASSPEM"; break;
                    case "Facturas": fundamento = "Art. 115 LAASSPEM"; break;
                    case "Validación del SAT": fundamento = "Art. 29 y 29 A CFF"; break;
                    case "Retención del 5 al millar": fundamento = "Art. 43 LIMT (2018 ó 2019)"; break;

                    default: fundamento = "N/A"; break;

                }
            }
            else if (tipo.Equals(CONCURSO))
            {
                switch (clave)
                {
                    case "Programa anual de adquisiciones": fundamento = "Art. 53,55,56 LAASSPEM"; break;
                    case "Oficio de solicitud de suficiencia presupuestal": fundamento = "Art. 58 LAASSPEM"; break;
                    case "Oficio de autorización de suficiencia presupuestal": fundamento = "Art. 53 fracc. II, 58 LAASSPEM"; break;
                    case "Requisición por escrito del bien y/ó servicio": fundamento = "Art. 45 fracc. X LAASSPEM"; break;
                    case "Constancia de no existir trabajos relacionados (consultoría, asesoría, estudios e investigación)": fundamento = "Art. 54 LAASSPEM"; break;
                    case "Dictamen de Justificación de excepción a la Licitación Pública": fundamento = "Art. 22 LAASSPEM"; break;
                    case "Invitación del concurso donde se indicarán como mínimo, la cantidad y descripción de los bienes y/ó servicios requeridos, plazo y lugar de entrega, asi como condiciones de pago": fundamento = "Art. 99 Fracc. III LAASSPEM"; break;
                    case "Confirmación de recepción y de participación por parte de la persona moral o fisica que fue invitada": fundamento = "Art. 69 LAASSPEM"; break;
                    case "Acta(s) de Junta(s) de Aclaraciones (cuando aplique)": fundamento = "Art. 79 Fracc.III LAASSPEM"; break;
                    case "Propuesta Técnica": fundamento = "Art. 72 y 99 Fracc. II LAASSPEM"; break;
                    case "Propuesta Económica": fundamento = "Art. 71 y 99 Fracc. II LAASSPEM"; break;
                    case "Acta de entrega de proposiciones legales y apertura de proposiciones técnicas": fundamento = "Art. 99 LAASSPEM"; break;
                    case "Acta de Dictamen Legal, Técnico y Economico": fundamento = "Art. 99 LAASSPEM"; break;
                    case "Cuadro Comparativo": fundamento = "Art. 96 Fracc. II LAASSPEM"; break;
                    case "Escritura constitutiva en el caso de persona moral": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Acta de nacimiento o carta de naturalización en el caso de persona fisica": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Poder General del Representante Legal": fundamento = "Art. 70 LAASSPEM"; break;
                    case "Identificación Oficial vigente": fundamento = string.Empty; break;
                    case "Declaración escrita bajo protesta de decir verdad, de no encontrarse en alguno delos supuestos establecidos por el articulo 77 de la LAASSPEM": fundamento = "Art. 77 LAASSPEM"; break;
                    case "Inscripción al Regristro Federal de Contribuyentes": fundamento = "Art. 26 fracc. II LAASSPEM"; break;
                    case "Curriculum Empresarial": fundamento = "Art. 72 fracc. I LAASSPEM"; break;
                    case "Declaración de pago de impuestos federales correspondientes a ejercicios anuales, semestrales, trimestrales o mensuales y/ó informes de instituciones financieras": fundamento = "Art. 71 fracc. II y IV LAASSPEM"; break;
                    case "Inscripción en el Padrón de Proveedores Municipal": fundamento = "Art. 25 LAASSPEM"; break;
                    case "Acta de apertura de proposiciones económicas": fundamento = "Art. 74 y 99 LAASSPEM"; break;
                    case "Acta de Fallo": fundamento = "Art. 88 LAASSPEM"; break;
                    case "Notificación de Fallo": fundamento = "Art. 88 LAASSPEM"; break;
                    case "Contrato": fundamento = "Art. 107 LAASSPEM"; break;
                    case "Contrato Abierto (para el caso que aplique)": fundamento = "Art. 108 LAASSPEM"; break;
                    case "Convenio Modificatorio (para el caso que aplique)": fundamento = "Art. 112 LAASSPEM"; break;
                    case "Oficio por el que se remiten al Comité Municipal, copias de los pedidos formalizados de los contratos que se celebren, de las modificaciones que a estos se realicen, asi como de la fianza o garantía que se otorgue": fundamento = "Art. 45 fracc. IX LAASSPEM"; break;
                    case "Oficio mediante el se comunica al Comité Municipal la recepción de bienes y/ó servicios": fundamento = "Art. 45 fracc. XII LAASSPEM"; break;
                    case "Comprobante de Domicilio": fundamento = "Art. 60, 62, 177 CCELSP"; break;
                    case "Garantía de Seriedad de la propuesta sin impuestos": fundamento = "Art. 126 fracc. II LAASSPEM"; break;
                    case "Garantía de anticipo (para el caso que aplique)": fundamento = "Art. 126 fracc.III LAASSPEM"; break;
                    case "Garantía de cumplimiento y posibles vicios ocultos": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Documento en el que conste la recepción del Bien y/ó Servicio": fundamento = string.Empty; break;
                    case "Entregables": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Reporte fotografíco (para el caso que aplique)": fundamento = string.Empty; break;
                    case "Orden compromiso": fundamento = string.Empty; break;
                    case "Orden de Pago": fundamento = string.Empty; break;
                    case "Facturas": fundamento = string.Empty; break;
                    case "Validación del SAT": fundamento = "Art. 43 LIMT (2018 ó 2019) "; break;
                    case "Retención del 5 al millar": fundamento = "Art. 43 LIMSACH (2018 ó 2019) "; break;

                    default: fundamento = "N/A"; break;
                }
            }
            else if (tipo.Equals(LICITACION))
            {
                switch (clave)
                {
                    case "Disponibilidad presupuestaria": fundamento = "Art. 25 LAASSP Art. 18 RLAASSP"; break;
                    case "Requisición por escrito del Bien y/ó Servicio": fundamento = "Art. 46 LAASSP Art. 27 RLAASSP"; break;
                    case "Autorización escrita por el titular de la Dependencia o Entidad (consultoría, asesoría, estudios e investigaciones)": fundamento = "Art. 19 LAASSP cuarto párrafo"; break;
                    case "Dictamen de que no cuenta se cuenta con personal capacitado o disponible parasu realizacion, (consultoría, asesoría, estudios e investigación)": fundamento = "Art. 19 LAASSP Art. 15 RLAASSP"; break;
                    case "Investigación de mercado": fundamento = "Art. 26 sexto párrafo LAASSP Art. 28 y 29 RLAASSP"; break;
                    case "Dictamen del Procedimiento de Adjudicación mediante Licitación Pública Nacional": fundamento = string.Empty; break;
                    case "Publicación de la Convocatoria": fundamento = "Art. 29 y 30 LAASSP Art. 39 RLAASSP"; break;
                    case "Bases de la Licitación Pública": fundamento = "Art. 29 y 30 LAASSP Art. 39 RLAASSP"; break;
                    case "Acta(s) de Junta(s) de Aclaraciones (cuando aplique)": fundamento = "Art. 29 Fracc.III, 33 y 33 Bis LAASSP Art. 45 y 46 RLAASSP"; break;
                    case "Acta de presentación y apertura de proposiciones": fundamento = "Art. 35 Fracc. III LAASSP Art. 47 y 48 RLAASSP"; break;
                    case "Cuadro Comparativo": fundamento = string.Empty; break;
                    case "Escrito bajo protesta de decir verdad, que es de nacionalidad mexicana y en el caso de adquisición de bienes, manifestar que los bienes que oferta y entregará serán producidos en México y contarán con el porcentaje de contenido nacional correspondiente. (cuando aplique)": fundamento = "Art. 35 RLAASSP"; break;
                    case "Escritura constitutiva en el caso de persona moral": fundamento = "Art. 29 fracc. VII LAASSP Art. 35 tercer párrafo, fracc. I RLAASSP"; break;
                    case "Acta de nacimiento o carta de naturalización en el caso de persona fisica": fundamento = "Art. 29 fracc VII LAASSP Art. 35 tercer párrafo fracc. II RLAASSP"; break;
                    case "Poder General del Representante Legal": fundamento = "Art. 29 fracc. VII LAASSP"; break;
                    case "Comprobante de Domicilio": fundamento = "Art. 60, 62, 177 CCELSP"; break;
                    case "Identificación Oficial vigente": fundamento = "Art. 29 fracc. VII LAASSP"; break;
                    case "Declaración escrita bajo protesta de decir verdad, de no encontrarse en alguno delos supuestos establecidos por el articulo 50 de la LAASSP": fundamento = "Art. 29 fracc. VIII LAASSP"; break;
                    case "Inscripción al Regristro Federal de Contribuyentes": fundamento = "Art. 29 fracc. VII LAASSP"; break;
                    case "Inscripción en el Padrón de Proveedores": fundamento = "Art. 56 Bis LAASSP Art. 105 RLAASSP"; break;
                    case "Proposiciones": fundamento = "Art. 34 primer párrafo LAASSP Art. 39, 50 y 104 RLAASSP"; break;
                    case "Proposiciones Conjuntas (para el caso que aplique)": fundamento = "Art. 34 tercer y cuarto párrafo LAASSP Art. 44 RLAASSP"; break;
                    case "Evaluación de las proposiciones, requisitos legales, técnicos y económicos": fundamento = "Art. 36 Bis LAASSP Art. 51,52,53 y 54 RLAASSP"; break;
                    case "Acta de Fallo": fundamento = "Art. 37 LAASSP"; break;
                    case "Notificación de Fallo": fundamento = "Art. 37 y 46 LAASSP"; break;
                    case "Contrato": fundamento = "Art. 45 LAASSP Art. 81 RLAASSP"; break;
                    case "Contrato Abierto (para el caso que aplique)": fundamento = "Art. 47 LAASSP Art. 85 RLAASSP"; break;
                    case "Convenio Modificatorio (para el caso que aplique)": fundamento = "Art. 47 último párrafo y 52 LAASSP Art. 92 RLAASSP"; break;
                    case "Notificación de Entrega de bienes o servicios": fundamento = string.Empty; break;
                    case "Garantía de anticipo (para el caso que aplique)": fundamento = "Art. 48 fracc. I, 49 LAASSP"; break;
                    case "Garantía de cumplimiento": fundamento = "Art. 48 y 49 LAASSP"; break;
                    case "Documento en el que conste la recepción del Bien y/ó Servicio": fundamento = string.Empty; break;
                    case "Entregables": fundamento = "Art. 45 fracc XII LAASSP"; break;
                    case "Reporte fotografíco (para el caso que aplique)": fundamento = string.Empty; break;
                    case "Facturas": fundamento = "Art. 80 y 90 RLAASSP"; break;
                    case "Validación del SAT": fundamento = "Art. 29 antepenúltimo párrafo CFF"; break;

                    default: fundamento = "N/A"; break;
                }
            }
            else if (tipo.Equals(REVISION))
            {
                switch (clave)
                {
                    case "Oficio de solicitud de suficiencia presupuestal": fundamento = "Art. 58 LAASSPEM"; break;
                    case "Oficio de autorización de suficiencia presupuestal": fundamento = "Art. 53 fracc. II, 58 LAASSPEM"; break;
                    case "Requisición por escrito del bien y/ó servicio": fundamento = "Art. 45 fracc. X  LAASSPEM"; break;
                    case "Documento en el que conste la recepción del Bien y/ó Servicio": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Reporte fotografíco (para el caso que aplique)": fundamento = "Art. 45 fracc VI LAASSPEM"; break;
                    case "Orden de Pago": fundamento = "Art. 7 fracc.X LAASSPEM"; break;
                    case "Facturas": fundamento = "Art. 115 LAASSPEM"; break;
                    case "Validación del SAT": fundamento = "Art. 29 y 29 A CFF"; break;
                    default: fundamento = "N/A"; break;
                }
            }

            else if (tipo.Equals(OBRA))
            {
                switch(clave)
                {

                    case "Carátula": fundamento = string.Empty; break;
                    case "Cedula de información básica.": fundamento = string.Empty; break;
                    case "Oficio de solicitud de autorización.": fundamento = "Art. 62 párr. 1, LE 2017"; break;
                    case "Oficio de Autorización de recursos y/o Aprobación de Obra.": fundamento = "Art. 19 y 22          "; break;
                    case "Documento de Autorización de Cabildo para la Ejecución de la Obra (Cabildo o Consejo de Administración, etc.).": fundamento = "    Art. 133, 134, 135 y 136 LCHEP"; break;
                    case "Acta constitutiva del comité de beneficiarios de la obra con lista de asistencia.": fundamento = string.Empty; break;
                    case "Programa anual de inversión pública y servicios relacionados (Obra se encuentre incluida en Programa aprobado por el Titular)": fundamento = "Art. 22 párr. l y ll Art.16,17  Art 17 Art 11 y 13 "; break;
                    case "Dictamen de Justificación para Contratación de Servicios Relacionados.": fundamento = string.Empty; break;
                    case "Documento que especifique si la obra es financiada en su totalidad con recurso del Gobierno del Estado.": fundamento = string.Empty; break;
                    case "Validación de Dependencias Normativas": fundamento = "   Art 22 Fracc. V  "; break;
                    case "Estudios de preinversión y / o Costo Benefecio": fundamento = "Art. 21, fracc. I Art. 24 últ. Párr.  Art 17 fracc. I  "; break;
                    case "Análisis y Dictamen de Factibilidad Técnica, Económica, Ambiental y sobre el Proyecto Ejecutivo de Obra Pública.": fundamento = string.Empty; break;
                    case "Informe Preventivo de Impacto Ambiental.": fundamento = string.Empty; break;
                    case "Resolución del Estudio de Impacto Ambiental o escrito de la no aplicación": fundamento = "Art. 20 y 21 fracc. IV   Art 17 fracc. IV  "; break;
                    case "Licencia de Destino de Uso de Suelo / Factibilidad para Introducción de Servicios.": fundamento = string.Empty; break;
                    case "Licencias  de Construcción.": fundamento = "Art. 19 y 21 fracc. XI, XIV   Art.  22 fracc. II  "; break;
                    case "Documento que acredite la Propiedad del Predio o Inmueble.": fundamento = string.Empty; break;
                    case "Banco de Tiro.": fundamento = string.Empty; break;
                    case "Estudios complementarios y/o de apoyo a proyectos (Mecánica de suelos, geotecnia, etc..)": fundamento = "Art. 18 párr.. V y 24, párr.. lll Art. 23 fracc. II  Art 17 fracc. IX  "; break;
                    case "Validación, Aprobación o Autorización de Proyecto (Dictamen Técnico en obras complejas) ( Responsiva Técnica)": fundamento = "Art. 24 párr. IV y V Art. 23    "; break;
                    case "Dictamén de Vialidad o Alineamiento y Número Oficial.": fundamento = string.Empty; break;
                    case "Plano de Macro Localización del Proyecto, indicando calle, Colonia,  calles así como la Ubicación Geográfica.": fundamento = string.Empty; break;
                    case "Plano de Micro Localización del Proyecto, indicando calle, Colonia,  calles así como la Ubicación Geográfica.": fundamento = string.Empty; break;
                    case "Reporte Fotográfico del Estado Actual con referencias.": fundamento = string.Empty; break;
                    case "Proyecto Ejecutivo o Básico (Planos Arquitectónicos, Ingeniería, Estructurales, Instalaciones, etc.) Incluye:": fundamento = "Art. 24 párr. lll Art. 23 y 24  Art 17 párr. IX y 22 párr. IV y V Art 15 párr. I y II, y 16 párr. I "; break;
                    case "Mem. de Cál.: CROQUISES": fundamento = "Art. 24 párr. lll Art. 23 y 24  Art 17 párr. IX y 22 párr. IV y V Art 15 párr. I y II, y 16 párr. I "; break;
                    case "Calendario o Programa de Ejecución de obra elaborado por la Dependencia Ejecutora": fundamento = "Art. 21 Fracc. VIII Art. 23 últ párr.  Art 17 párr. Vll Art. 15 párr. II "; break;
                    case "Catálogo de Conceptos.": fundamento = string.Empty; break;
                    case "Números Generadores del Proyecto elaborado por la Dependencia Ejecutora": fundamento = "Art. 4 fracc. I y II   Art 17 fracc. XI  Art. 15  y 16 "; break;
                    case "Presupuesto por Partida.": fundamento = string.Empty; break;
                    case "Presupuesto Base de la Dependencia Ejecutora": fundamento = "Art.   21 fracc. 12 Art.   23 fracc. I  Art 17 fracc. XI  Art 15 párr. lid, y 16 párr. I "; break;
                    case "Ficha Técnica.": fundamento = string.Empty; break;
                    case "Publicación en COMPRANET, según sea el caso.": fundamento = "Art. ll párr. ll y Art. 32 Art. 31    "; break;
                    case "Convocatoria Pública o Invitación a Contratistas, según sea el caso.": fundamento = "Art. 32 Art. 31 párr. ll  Art                                   27 y 29  "; break;
                    case "Bases del Concurso.": fundamento = "Art. 31, 38 Art. 34  Art. 27 Y 30 Art.   20     "; break;
                    case "Acta de Visita al Lugar de la Obra, con Manifestación escrita de conocer el sitio de realización de los trabajos y sus condiciones ambientales": fundamento = "Art. 31 Fracc. IX Art. 38 y 44 frac. I  Art  30, fracc XV Art 24 "; break;
                    case "Acta de la Junta de Aclaraciones": fundamento = "Art. 34 fracc. ll, y 35 Art. 39  Art 30 fracc. IV Art    25 "; break;
                    case "Invitación al Órgano de Control para el acto de presentación y apertura de proposiciones.": fundamento = "Art. 38 fracc VI   Art 24 y 27 bis   "; break;
                    case "Técnica": fundamento = "Art. 37 Art. 41 y 62  Art 33, 34 y 35 Art. 33 y 34 "; break;
                    case "Análisis del total de los precios unitarios de los conceptos de trabajo": fundamento = " 45 A   fracc. I  Art. 36 fracc. I Art 31 Inciso A "; break;
                    case "Programa de ejecución general de los trabajos conforme al catálogo de conceptos con sus erogaciones": fundamento = " 45 A, fracc. X   Art 31 Inciso J "; break;
                    case "Dictamen de Fallo.": fundamento = "   Art. 36 fracc. Ill párr. lll Art. 44 "; break;
                    case "Acta de fallo.": fundamento = "Art. 39 Art. 68  Art. 46 Fracc. V Art 45 "; break;
                    case "Revisar que los casos de excepción a la licitación pública exista un dictamen y una Justificación que estén debidamente fundados y motivados en criterios de economía, eficacia, eficiencia, imparcialidad, honradez y transparencia, que validen dicha determinación.": fundamento = "Art. 41 Art. 73  Art. 43 y 44 Art. 50"; break;
                    case "Acta o acuerdo para la constitución del comité de obra pública y servicios relacionados con las mismas.": fundamento = "Art. 25 párr. l y ll Art. 26    "; break;
                    case "Documentación que demuestre el capital contable mínimo requerido.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                    case "Acta constitutiva de la empresa ganadora y en su caso modificaciones a la misma o registro de la persona física ante el Servicio de Administración Tributaria.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                    case "Documentación que compruebe la capacidad técnica y experiencia en el tipo de obra o servicio solicitado.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                    case "Relación de contratos contraídos a la fecha de presentación de su propuesta.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                    case "Declaración de no estar en los supuestos de los artículos 51 y 55 de la Ley de Obras Públicas y Servicios Relacionados con las Mismas vigentes.": fundamento = "LASEP"; break;
                    case "Identificación oficial vigente con fotografía del representante legal o en su caso de la persona física de la propuesta ganadora.": fundamento = "LASEP"; break;
                    case "Registro Federal de Contribuyentes vigente.": fundamento = "LASEP"; break;
                    case "Contrato con sus anexos debidamente requisitados": fundamento = "Art. 46 y 47 Art. 79  Art 48 y 49 Art 52 y 53       "; break;
                    case "Catalogo de conceptos con montos y/o presupuesto de la empresa adjudicada (debidamente firmado).": fundamento = "Art. 46, fracc. V Art. 79  Art 49 fracc. l Art. 177 fracc. VII "; break;
                    case "Programas de ejecución de los trabajos pactados.": fundamento = "Art. 46, Vll Art. 79  Art 49 fracc. lV Art. 177 fracc. IV "; break;
                    case "Garantía por la correcta y oportuna inversión del anticipo.": fundamento = "Art. 48, fracc.I Art, 89, 90, (55 de LFPRH)  Art 52 fracc.II Art 65       "; break;
                    case "Garantía de cumplimiento del Contrato.":fundamento = "Art. 48, fracc.II Art. 89,90, 91(Art. 55 LFPRH)  Art 52 fracc.III Art. 62    "; break;
                    case "Oficio de asignación del Residente responsable de la obra por parte de la dependencia ejecutora.": fundamento = "Art. 53  Art. 111, 112  Art. 85      "; break;
                    case "Escrito de asignación del Superintendente responsable de la obra por parte de la empresa contratista.": fundamento = "Art. 53 últ. Párr. Art. 111    "; break;
                    case "Escrito emitido por la Dependencia Ejecutora y dirigido al Contratista de la disposición del o los inmuebles.": fundamento = "Art. 52   Art 63  "; break;
                    case "Oficio de inicio de los trabajos.": fundamento = "   Art. 49 frac. lV y 63  "; break;
                    case "Reprogramación por la entrega tardía del anticipo (Diferimiento).": fundamento = "Art. 50 fracc. I Art. 140  Art. 54 fracc. I Art. 100 "; break;
                    case "Estimación, Factura, cálculo e integración de los importes correspondientes a cada estimación considerando las deducciones que correspondan.": fundamento = "Art. 54 Art. 127 y 128  Art. 54 Art 89 al 93 "; break;
                    case "Números generadores de obra debidamente referenciados, con sus claves según catálogo, resumen de generadores, croquis y resumen de la estimación por concepto y partida.": fundamento = "Art. 54 Art. 132 fracc. I  Art. 54 Art 93 Fracc.  I "; break;
                    case "Croquis de ubicación de los trabajos que amparan la estimación, debidamente requisitado por los que intervienen en la obra.": fundamento = "Art. 54 Art. 132 fracc. III  Art. 54 Art 93 Fracc.  III "; break;
                    case "Programa de avance de obra.": fundamento = string.Empty; break;
                    case "Notas de folio": fundamento = "Art. 54 Art. 132 fracc. II  Art. 54 Art 93 Fracc.  II "; break;
                    case "Controles de calidad y pruebas de laboratorio.": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                    case "Minutas actas acuerdos comunicados que hayan sido generados  respecto de la obra.": fundamento = " Art. 115 fracc. IV inciso D  Art 64 Art 80 Fracc.  II inciso D "; break;
                    case "Reporte fotográfico.": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                    case "Coordenadas de ubicación": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                    case "Cedula de verificación de los trabajos ejecutados.": fundamento = " Art. 113 fracc. XII   Art. 77 Fracc.  Xl "; break;
                    case "Solicitud de ajustes de costos (contratista)": fundamento = "Art. 56 y 57 Art. 175 y 176  Art. 66 y 67 Art 95 "; break;
                    case "Estudios y documentación soporte": fundamento = "Art. 56 párr. II Art. 178  Art. 67  "; break;
                    case "Determinación de ajustes de costos por la dependencia o Entidad": fundamento = "Art. 56 párr. IV Art. 178-184  Art. 68 Art. 95  "; break;
                    case "Oficio de Autorización de Modificaciones y Anexos.": fundamento = "Art. 59   Art. 69  "; break;
                    case "Convenios con la documentación soporte (catalogo de conceptos de convenio, programas de ejecución modificados, proyecto ejecutivo modificado)": fundamento = "Art. 59 Art. 99, 109  Art 69 Art. 67 "; break;
                    case "Anotación en bitácora de la modificación de los trabajo y documentación justificatoria (Residencia)": fundamento = " Art. 2 párr. Vlll, 103, 105 y 125  Art. 63 Art. 87 párr. lV "; break;
                    case "Dictamen técnico que funde y motive las causas (Residente Dependencia)": fundamento = " Art. 99   Art 67 y/74 fracc. II "; break;
                    case "Ampliación de garantía de cumplimiento de contrato (Contratista)": fundamento = " Art. 91 y 92   Art. 74 fracc. VII, c) "; break;
                    case "Autorización por escrito de prórrogas": fundamento = " Art. 99  Art. 69 Art. 71 "; break;
                    case "Notificación por escrito de la necesidad de ejecutar cantidades adicionales o conceptos no previstos en el catálogo original (Contratista) con los precios unitarios correspondientes y documentación soporte.": fundamento = string.Empty; break;
                    case "Autorización por escrito o en bitácora de ejecutar cantidades adicionales o conceptos no previstos en el catálogo original (Residente).": fundamento = "Art. 59 Art. 105 y 107  Art. 69 Art 71 "; break;
                    case "Informe al Órgano Interno de Control de la Dependencia Ejecutora de la autorización del Convenio (si es superior al 25 % del Contrato)": fundamento = "Art. 59 fracc. lll   Art 69  "; break;
                    case "Aviso a la Contratista de la suspensión o terminación anticipada.": fundamento = "Art. 61 fracc. I  Art. 144  Art 71, párr. l  "; break;
                    case "Acta circunstanciada de suspensión de obra.": fundamento = "Art. 62 párr. l Art. 144 fracc l, y 147  Art. 72 párr. Vl Art 118 "; break;
                    case "Acta circunstanciada de terminación anticipada de obra.": fundamento = " Art. 151  Art. 72 párr. Vl Art 106 y 125 "; break;
                    case "Resolución de rescisión del Contrato y notificación a la Contratista.": fundamento = "Art. 61, fracc. II   Art. 71, III  "; break;
                    case "Acta circunstanciada de rescisión del contrato.": fundamento = "Art. 61, fracc. II Art. 159  Art 72 fracc. VI Art 118 "; break;
                    case "Informe al Órgano Interno de Control de la suspensión, terminación anticipada o rescisión administrativa.": fundamento = "Art. 63   Art 73 91"; break;
                    //91        fundamento = "Art. 46 últ. Párr.. Art. 122   Art. 63 párr. ll y lll Art. 84 al 88 y 194 "; break;
                    case "Aviso de terminación de la obra emitido por el Contratista": fundamento = "Art. 64 Art. 164  Art 74 Art  122,123 "; break;
                    case "Acta Entrega- Recepción física de los trabajos": fundamento = "Art. 64 Art. 166  Art 74 Art 124 "; break;
                    case "Finiquito de obra debidamente requisitado conforme a la normatividad aplicable.": fundamento = "Art. 64, párr. II Art. 168, 170  Art 74, párr. ll Art  127, 128 "; break;
                    case "Fianza de Vicios Ocultos": fundamento = "Art. 66, párr. II Art. 95,97,166 últ. párr.  Art  77        últ. Párr.  "; break;
                    case "Acta de Extinción de Derechos": fundamento = "Art. 64, párr. último Art. 172  Art 75 Art 130 "; break;
                    case "Reporte de la aplicación de los recursos en el Sistema de Formato Único, (trimestral).": fundamento = "Art. 85 fracc. ll, ley LFPRH"; break;
                    case "Autorización para ejecución de obras del FISM por parte de SEDESOL, y por parte de la MIDS.": fundamento = "Art. 33, ley  LCF"; break;


                    default: fundamento = "N/A"; break;
                }
            }


           


            return fundamento;
        }

        /// <summary>
        /// Fundamentos de Obra Publica
        /// </summary>
        /// <param name="clave"></param>
        /// <returns></returns>
        public string FundamentosObra(string clave)
        {
            string fundamento = string.Empty;

            switch (clave)
            {
                case "Carátula": fundamento = string.Empty; break;
                case "Cedula de información básica.": fundamento = string.Empty; break;
                case "Oficio de solicitud de autorización.": fundamento = "Art. 62 párr. 1, LE 2017"; break;
                case "Oficio de Autorización de recursos y/o Aprobación de Obra.": fundamento = "Art. 19 y 22          "; break;
                case "Documento de Autorización de Cabildo para la Ejecución de la Obra (Cabildo o Consejo de Administración, etc.).": fundamento = "    Art. 133, 134, 135 y 136 LCHEP"; break;
                case "Acta constitutiva del comité de beneficiarios de la obra con lista de asistencia.": fundamento = string.Empty; break;
                case "Programa anual de inversión pública y servicios relacionados (Obra se encuentre incluida en Programa aprobado por el Titular)": fundamento = "Art. 22 párr. l y ll Art.16,17  Art 17 Art 11 y 13 "; break;
                case "Dictamen de Justificación para Contratación de Servicios Relacionados.": fundamento = string.Empty; break;
                case "Documento que especifique si la obra es financiada en su totalidad con recurso del Gobierno del Estado.": fundamento = string.Empty; break;
                case "Validación de Dependencias Normativas": fundamento = "   Art 22 Fracc. V  "; break;
                case "Estudios de preinversión y / o Costo Benefecio.": fundamento = "LEY Y REGLAM. FEDERAL // LOPSRM Art. 21, fracc. I / REGLAMENTO LOPSRM Art. 24 últ. Párr., LEY Y REGLAM. DEL ESTADO DE PUEBLA // LOPSRMEP Art 17 fracc. I"; break;
                case "Análisis y Dictamen de Factibilidad Técnica, Económica, Ambiental y sobre el Proyecto Ejecutivo de Obra Pública.": fundamento = string.Empty; break;
                case "Informe Preventivo de Impacto Ambiental.": fundamento = string.Empty; break;
                case "Resolución del Estudio de Impacto Ambiental o escrito de la no aplicación": fundamento = "Art. 20 y 21 fracc. IV   Art 17 fracc. IV  "; break;
                case "Licencia de Destino de Uso de Suelo / Factibilidad para Introducción de Servicios.": fundamento = string.Empty; break;
                case "Licencias  de Construcción.": fundamento = "Art. 19 y 21 fracc. XI, XIV   Art.  22 fracc. II  "; break;
                case "Documento que acredite la Propiedad del Predio o Inmueble.": fundamento = string.Empty; break;
                case "Banco de Tiro.": fundamento = string.Empty; break;
                case "Estudios complementarios y/o de apoyo a proyectos (Mecánica de suelos, geotecnia, etc..)": fundamento = "Art. 18 párr.. V y 24, párr.. lll Art. 23 fracc. II  Art 17 fracc. IX  "; break;
                case "Validación, Aprobación o Autorización de Proyecto (Dictamen Técnico en obras complejas) ( Responsiva Técnica)": fundamento = "Art. 24 párr. IV y V Art. 23    "; break;
                case "Dictamén de Vialidad o Alineamiento y Número Oficial.": fundamento = string.Empty; break;
                case "Plano de Macro Localización del Proyecto, indicando calle, Colonia,  calles así como la Ubicación Geográfica.": fundamento = string.Empty; break;
                case "Plano de Micro Localización del Proyecto, indicando calle, Colonia,  calles así como la Ubicación Geográfica.": fundamento = string.Empty; break;
                case "Reporte Fotográfico del Estado Actual con referencias.": fundamento = string.Empty; break;
                case "Proyecto Ejecutivo o Básico (Planos Arquitectónicos, Ingeniería, Estructurales, Instalaciones, etc.) Incluye:": fundamento = "Art. 24 párr. lll Art. 23 y 24  Art 17 párr. IX y 22 párr. IV y V Art 15 párr. I y II, y 16 párr. I "; break;
                case "Mem. de Cál.: CROQUISES": fundamento = "Art. 24 párr. lll Art. 23 y 24  Art 17 párr. IX y 22 párr. IV y V Art 15 párr. I y II, y 16 párr. I "; break;

                case "Mem. Descrip.:": fundamento = "LEY Y REGLAM. FEDERAL // LOPSRM Art. 24 párr. lll / REGLAMENTO LOPSRM Art. 23 y 24, LEY Y REGLAM. DEL ESTADO DE PUEBLA // LOPSRMEP Art 17 párr. IX y 22 párr. IV y V /  REGLAMENTO LOPSRMEP Art 15 párr. I y II, y 16 párr. I"; break;
                case "Acta de Apertura:": fundamento = "LEY Y REGLAM. FEDERAL // LOPSRM Art. 37 /  REGLAMENTO LOPSRM Art. 41 y 62, LEY Y REGLAM. DEL ESTADO DE PUEBLA // LOPSRMEP Art 33, 34 y 35 /  REGLAMENTO LOPSRMEP Art. 33 y 34"; break;
                case "Notas de bitácora del periodo de la estimación.": fundamento = "LEY Y REGLAM. FEDERAL // LOPSRM Art. 54 /  REGLAMENTO LOPSRM Art. 132 fracc. II, LEY Y REGLAM. DEL ESTADO DE PUEBLA // LOPSRMEP Art. 54 /  REGLAMENTO LOPSRMEP Art 93 Fracc. II"; break;
                case "Bitácora de obra.": fundamento = "LEY Y REGLAM. FEDERAL // LOPSRM Art. 46 últ. Párr.. /  REGLAMENTO LOPSRM Art. 122, LEY Y REGLAM. DEL ESTADO DE PUEBLA // LOPSRMEP Art. 63 párr. ll y lll /  REGLAMENTO LOPSRMEP Art. 84 al 88 y 194"; break;

                case "Calendario o Programa de Ejecución de obra elaborado por la Dependencia Ejecutora": fundamento = "Art. 21 Fracc. VIII Art. 23 últ párr.  Art 17 párr. Vll Art. 15 párr. II "; break;
                case "Catálogo de Conceptos.": fundamento = string.Empty; break;
                case "Números Generadores del Proyecto elaborado por la Dependencia Ejecutora": fundamento = "Art. 4 fracc. I y II   Art 17 fracc. XI  Art. 15  y 16 "; break;
                case "Presupuesto por Partida.": fundamento = string.Empty; break;
                case "Presupuesto Base de la Dependencia Ejecutora": fundamento = "Art.   21 fracc. 12 Art.   23 fracc. I  Art 17 fracc. XI  Art 15 párr. lid, y 16 párr. I "; break;
                case "Ficha Técnica.": fundamento = string.Empty; break;
                case "Publicación en COMPRANET, según sea el caso.": fundamento = "Art. ll párr. ll y Art. 32 Art. 31    "; break;
                case "Convocatoria Pública o Invitación a Contratistas, según sea el caso.": fundamento = "Art. 32 Art. 31 párr. ll  Art                                   27 y 29  "; break;
                case "Bases del Concurso.": fundamento = "Art. 31, 38 Art. 34  Art. 27 Y 30 Art.   20     "; break;
                case "Acta de Visita al Lugar de la Obra, con Manifestación escrita de conocer el sitio de realización de los trabajos y sus condiciones ambientales": fundamento = "Art. 31 Fracc. IX Art. 38 y 44 frac. I  Art  30, fracc XV Art 24 "; break;
                case "Acta de la Junta de Aclaraciones": fundamento = "Art. 34 fracc. ll, y 35 Art. 39  Art 30 fracc. IV Art    25 "; break;
                case "Invitación al Órgano de Control para el acto de presentación y apertura de proposiciones.": fundamento = "Art. 38 fracc VI   Art 24 y 27 bis   "; break;
                case "Técnica": fundamento = "Art. 37 Art. 41 y 62  Art 33, 34 y 35 Art. 33 y 34 "; break;
                case "Análisis del total de los precios unitarios de los conceptos de trabajo": fundamento = " 45 A   fracc. I  Art. 36 fracc. I Art 31 Inciso A "; break;
                case "Programa de ejecución general de los trabajos conforme al catálogo de conceptos con sus erogaciones": fundamento = " 45 A, fracc. X   Art 31 Inciso J "; break;
                case "Dictamen de Fallo.": fundamento = "   Art. 36 fracc. Ill párr. lll Art. 44 "; break;
                case "Acta de fallo.": fundamento = "Art. 39 Art. 68  Art. 46 Fracc. V Art 45 "; break;
                case "Revisar que los casos de excepción a la licitación pública exista un dictamen y una Justificación que estén debidamente fundados y motivados en criterios de economía, eficacia, eficiencia, imparcialidad, honradez y transparencia, que validen dicha determinación.": fundamento = "Art. 41 Art. 73  Art. 43 y 44 Art. 50"; break;
                case "Acta o acuerdo para la constitución del comité de obra pública y servicios relacionados con las mismas.": fundamento = "Art. 25 párr. l y ll Art. 26    "; break;
                case "Documentación que demuestre el capital contable mínimo requerido.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                case "Acta constitutiva de la empresa ganadora y en su caso modificaciones a la misma o registro de la persona física ante el Servicio de Administración Tributaria.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                case "Documentación que compruebe la capacidad técnica y experiencia en el tipo de obra o servicio solicitado.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                case "Relación de contratos contraídos a la fecha de presentación de su propuesta.": fundamento = "Art. 46 Art. 79  Art 48 y 49 Art 52 y 53 "; break;
                case "Declaración de no estar en los supuestos de los artículos 51 y 55 de la Ley de Obras Públicas y Servicios Relacionados con las Mismas vigentes.": fundamento = "LASEP"; break;
                case "Identificación oficial vigente con fotografía del representante legal o en su caso de la persona física de la propuesta ganadora.": fundamento = "LASEP"; break;
                case "Registro Federal de Contribuyentes vigente.": fundamento = "LASEP"; break;
                case "Contrato con sus anexos debidamente requisitados": fundamento = "Art. 46 y 47 Art. 79  Art 48 y 49 Art 52 y 53       "; break;
                case "Catalogo de conceptos con montos y/o presupuesto de la empresa adjudicada (debidamente firmado).": fundamento = "Art. 46, fracc. V Art. 79  Art 49 fracc. l Art. 177 fracc. VII "; break;
                case "Programas de ejecución de los trabajos pactados.": fundamento = "Art. 46, Vll Art. 79  Art 49 fracc. lV Art. 177 fracc. IV "; break;
                case "Garantía por la correcta y oportuna inversión del anticipo.": fundamento = "Art. 48, fracc.I Art, 89, 90, (55 de LFPRH)  Art 52 fracc.II Art 65       "; break;
                case "Garantía de cumplimiento del Contrato.": fundamento = "Art. 48, fracc.II Art. 89,90, 91(Art. 55 LFPRH)  Art 52 fracc.III Art. 62    "; break;
                case "Oficio de asignación del Residente responsable de la obra por parte de la dependencia ejecutora.": fundamento = "Art. 53  Art. 111, 112  Art. 85      "; break;
                case "Escrito de asignación del Superintendente responsable de la obra por parte de la empresa contratista.": fundamento = "Art. 53 últ. Párr. Art. 111    "; break;
                case "Escrito emitido por la Dependencia Ejecutora y dirigido al Contratista de la disposición del o los inmuebles.": fundamento = "Art. 52   Art 63  "; break;
                case "Oficio de inicio de los trabajos.": fundamento = "   Art. 49 frac. lV y 63  "; break;
                case "Reprogramación por la entrega tardía del anticipo (Diferimiento).": fundamento = "Art. 50 fracc. I Art. 140  Art. 54 fracc. I Art. 100 "; break;
                case "Estimación, Factura, cálculo e integración de los importes correspondientes a cada estimación considerando las deducciones que correspondan.": fundamento = "Art. 54 Art. 127 y 128  Art. 54 Art 89 al 93 "; break;
                case "Números generadores de obra debidamente referenciados, con sus claves según catálogo, resumen de generadores, croquis y resumen de la estimación por concepto y partida.": fundamento = "Art. 54 Art. 132 fracc. I  Art. 54 Art 93 Fracc.  I "; break;
                case "Croquis de ubicación de los trabajos que amparan la estimación, debidamente requisitado por los que intervienen en la obra.": fundamento = "Art. 54 Art. 132 fracc. III  Art. 54 Art 93 Fracc.  III "; break;
                case "Programa de avance de obra.": fundamento = string.Empty; break;
                case "Notas de folio": fundamento = "Art. 54 Art. 132 fracc. II  Art. 54 Art 93 Fracc.  II "; break;
                case "Controles de calidad y pruebas de laboratorio.": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                case "Minutas actas acuerdos comunicados que hayan sido generados  respecto de la obra.": fundamento = " Art. 115 fracc. IV inciso D  Art 64 Art 80 Fracc.  II inciso D "; break;
                case "Reporte fotográfico.": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                case "Coordenadas de ubicación": fundamento = "Art. 54 Art. 132 fracc. IV  Art. 54 Art 93 Fracc.  IV "; break;
                case "Cedula de verificación de los trabajos ejecutados.": fundamento = " Art. 113 fracc. XII   Art. 77 Fracc.  Xl "; break;
                case "Solicitud de ajustes de costos (contratista)": fundamento = "Art. 56 y 57 Art. 175 y 176  Art. 66 y 67 Art 95 "; break;
                case "Estudios y documentación soporte": fundamento = "Art. 56 párr. II Art. 178  Art. 67  "; break;
                case "Determinación de ajustes de costos por la dependencia o Entidad": fundamento = "Art. 56 párr. IV Art. 178-184  Art. 68 Art. 95  "; break;
                case "Oficio de Autorización de Modificaciones y Anexos.": fundamento = "Art. 59   Art. 69  "; break;
                case "Convenios con la documentación soporte (catalogo de conceptos de convenio, programas de ejecución modificados, proyecto ejecutivo modificado)": fundamento = "Art. 59 Art. 99, 109  Art 69 Art. 67 "; break;
                case "Anotación en bitácora de la modificación de los trabajo y documentación justificatoria (Residencia)": fundamento = " Art. 2 párr. Vlll, 103, 105 y 125  Art. 63 Art. 87 párr. lV "; break;
                case "Dictamen técnico que funde y motive las causas (Residente Dependencia)": fundamento = " Art. 99   Art 67 y/74 fracc. II "; break;
                case "Ampliación de garantía de cumplimiento de contrato (Contratista)": fundamento = " Art. 91 y 92   Art. 74 fracc. VII, c) "; break;
                case "Autorización por escrito de prórrogas": fundamento = " Art. 99  Art. 69 Art. 71 "; break;
                case "Notificación por escrito de la necesidad de ejecutar cantidades adicionales o conceptos no previstos en el catálogo original (Contratista) con los precios unitarios correspondientes y documentación soporte.": fundamento = string.Empty; break;
                case "Autorización por escrito o en bitácora de ejecutar cantidades adicionales o conceptos no previstos en el catálogo original (Residente).": fundamento = "Art. 59 Art. 105 y 107  Art. 69 Art 71 "; break;
                case "Informe al Órgano Interno de Control de la Dependencia Ejecutora de la autorización del Convenio (si es superior al 25 % del Contrato)": fundamento = "Art. 59 fracc. lll   Art 69  "; break;
                case "Aviso a la Contratista de la suspensión o terminación anticipada.": fundamento = "Art. 61 fracc. I  Art. 144  Art 71, párr. l  "; break;
                case "Acta circunstanciada de suspensión de obra.": fundamento = "Art. 62 párr. l Art. 144 fracc l, y 147  Art. 72 párr. Vl Art 118 "; break;
                case "Acta circunstanciada de terminación anticipada de obra.": fundamento = " Art. 151  Art. 72 párr. Vl Art 106 y 125 "; break;
                case "Resolución de rescisión del Contrato y notificación a la Contratista.": fundamento = "Art. 61, fracc. II   Art. 71, III  "; break;
                case "Acta circunstanciada de rescisión del contrato.": fundamento = "Art. 61, fracc. II Art. 159  Art 72 fracc. VI Art 118 "; break;
                case "Informe al Órgano Interno de Control de la suspensión, terminación anticipada o rescisión administrativa.": fundamento = "Art. 63   Art 73 91"; break;
                //91        fundamento = "Art. 46 últ. Párr.. Art. 122   Art. 63 párr. ll y lll Art. 84 al 88 y 194 "; break;
                case "Aviso de terminación de la obra emitido por el Contratista": fundamento = "Art. 64 Art. 164  Art 74 Art  122,123 "; break;
                case "Acta Entrega- Recepción física de los trabajos": fundamento = "Art. 64 Art. 166  Art 74 Art 124 "; break;
                case "Finiquito de obra debidamente requisitado conforme a la normatividad aplicable.": fundamento = "Art. 64, párr. II Art. 168, 170  Art 74, párr. ll Art  127, 128 "; break;
                case "Fianza de Vicios Ocultos": fundamento = "Art. 66, párr. II Art. 95,97,166 últ. párr.  Art  77        últ. Párr.  "; break;
                case "Acta de Extinción de Derechos": fundamento = "Art. 64, párr. último Art. 172  Art 75 Art 130 "; break;
                case "Reporte de la aplicación de los recursos en el Sistema de Formato Único, (trimestral).": fundamento = "Art. 85 fracc. ll, ley LFPRH"; break;
                case "Autorización para ejecución de obras del FISM por parte de SEDESOL, y por parte de la MIDS.": fundamento = "Art. 33, ley  LCF"; break;
                default: fundamento = "N/A"; break;
            }
            return fundamento;
        }

        /// <summary>
        /// Metodo que valida la longitud
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        public bool IsValidLength(string valor, int longitud)
        {
            if (valor.Length > longitud)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Metodo que verifica si la cadena contiene caracteres especiales
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool hasSpecialChar(string input)
        {
            string specialChar = @"@\|!#$%&/()=?»«@£§€{}.+*^~[]¡¿¬°;'<>_,";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }

        public bool IsValidCaracterEspecial(string strNumber)
        {
            //    (                   # Start of group
            //        (?=.*\d)        #   must contain at least one digit
            //        (?=.*[A - Z])   #   must contain at least one uppercase character
            //        (?=.*\W)        #   must contain at least one special symbol
            //        .               #   match anything with previous condition checking
            //        { 8,8}          #   length is exactly 8 characters
            //)                       # End of group
            //((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,8}) original

            Regex regex = new Regex(@"(?=.*\d)(?=.*[A - Z])");
            Match match = regex.Match(strNumber);

            if (match.Success)
                return true;
            else
                return false;
        }

        public bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{9,9}");
            var hasLowerChar = new Regex(@"[a-z]+");
            //var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "El Password deberia contener al menos una letra minuscula";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "El Password deberia tener por lo menos una letra Mayuscula";
                return false;
            }
            else if (hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "El Password deberia tener 8 caracteres";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "El Password deberia tener por lo menos un numero";
                return false;
            }

            //else if (!hasSymbols.IsMatch(input))
            //{
            //    ErrorMessage = "Password should contain At least one special case characters";
            //    return false;
            //}
            else
            {
                return true;
            }
        }


    }
}
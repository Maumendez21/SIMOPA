using credentialsPBR.Models.Expedientes.ObraPublica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes.ObraPublica
{
    public class ResponsePartidaPrincipalObra:GenericClass
    {
        public List<PartidasPrincipalesObra> PartidasPrincipalesObraPublica { get; set; }
    }
}
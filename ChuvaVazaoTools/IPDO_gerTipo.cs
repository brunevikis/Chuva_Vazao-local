//------------------------------------------------------------------------------
// <auto-generated>
//    O código foi gerado a partir de um modelo.
//
//    Alterações manuais neste arquivo podem provocar comportamento inesperado no aplicativo.
//    Alterações manuais neste arquivo serão substituídas se o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChuvaVazaoTools
{
    using System;
    using System.Collections.Generic;
    
    public partial class IPDO_gerTipo
    {
        public IPDO_gerTipo()
        {
            this.IPDO_gerTerm = new HashSet<IPDO_gerTerm>();
        }
    
        public int id { get; set; }
        public string nome { get; set; }
    
        public virtual ICollection<IPDO_gerTerm> IPDO_gerTerm { get; set; }
    }
}

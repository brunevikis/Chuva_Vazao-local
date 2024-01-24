using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary.Dadvaz
{
    public class DiaBlock : BaseBlock<DiaLine>
    {

    }

    public class DiaLine : BaseLine
    {

        //public OperLine() : base() { this[0] = "GL"; }

        public int diainicial { get { return (int)this[0]; } set { this[0] = value; } }
        public int indiceSemana { get { return (int)this[1]; } set { this[1] = value; } }
        public int NumSemana { get { return (int)this[2]; } set { this[2] = value; } }
        public int FlagSimul { get { return (int)this[3]; } set { this[3] = value; } }

        public override BaseField[] Campos { get { return DiaCampos; } }

        static readonly BaseField[] DiaCampos = new BaseField[] {
               new BaseField(1  , 1 ,"I1"    , "diainicial"),
               new BaseField(3  , 3 ,"I1"    , "Semana custo"),
               new BaseField(5  , 5 ,"I1"    , "Num semanas"),
               new BaseField(7  , 7 ,"I1"    , "Flag simul"),
            };
    }
}

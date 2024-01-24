using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary.Dadvaz
{
    public class DataBlock : BaseBlock<DataLine>
    {

    }

    public class DataLine : BaseLine
    {

        //public OperLine() : base() { this[0] = "GL"; }

        public int Hora { get { return (int)this[0]; } set { this[0] = value; } }
        public int Dia { get { return (int)this[1]; } set { this[1] = value; } }
        public int Mes { get { return (int)this[2]; } set { this[2] = value; } }
        public int Ano { get { return (int)this[3]; } set { this[3] = value; } }

        public override BaseField[] Campos { get { return DataCampos; } }

        static readonly BaseField[] DataCampos = new BaseField[] {
               new BaseField(1  , 2 ,"I2"    , "hora"),
               new BaseField(5  , 6 ,"I2"    , "dia"),
               new BaseField(9  , 10 ,"I2"    , "mes"),
               new BaseField(13  , 16 ,"I4"    , "ano"),
            };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary.Dadvaz
{
    public class VazoesBlock : BaseBlock<VazoesLine>
    {

    }

    public class VazoesLine : BaseLine
    {

        //public OperLine() : base() { this[0] = "GL"; }

        public int Usina { get { return (int)this[0]; } set { this[0] = value; } }
        public string Nome { get { return this[1].ToString(); } set { this[1] = value; } }

        public int TipoVaz { get { return (int)this[2]; } set { this[2] = value; } }
        public string DiaInic { get { return this[3].ToString(); } set { this[3] = value; } }
        public int HoraDiaInic { get { return (int)this[4]; } set { this[4] = value; } }
        public int MeiaHoraDiaInic { get { return (int)this[5]; } set { this[5] = value; } }
        public string DiaFinal { get { return this[6].ToString(); } set { this[6] = value; } }
        public int HoraDiaIFinal { get { return (int)this[7]; } set { this[7] = value; } }

        public int MeiaHoraDiaFinal { get { return (int)this[8]; } set { this[8] = value; } }

        public float Vazao { get { return (float)this[9]; } set { this[9] = value; } }

        public override BaseField[] Campos { get { return VazoesCampos; } }

        static readonly BaseField[]VazoesCampos = new BaseField[] {
                new BaseField(1  , 3 ,"I3"    , "NumUsina"),//
                new BaseField(5  , 16 ,"A12"    , "Nome"),//
                new BaseField(20  , 20 ,"I1"    , "FlagTipoVazao"),//pode ter letra
                new BaseField(25  , 26 ,"A2"    , "diainicial"),//
                new BaseField(28  , 29 ,"I2"    , "HoraDiaInic"),//
                new BaseField(31  , 31 ,"I1"    , "FlagMeiahoraInici"),//podes ser letra
                new BaseField(33  , 34 ,"A2"    , "DiaFinal"),//
                new BaseField(36  , 37 ,"I2"    , "HoraDiaFinal"),//
                new BaseField(39  , 39 ,"I1"    , "FlagMeiahorafinal"),
                new BaseField(45  , 53 ,"F9.0"    , "vazao"),


            };
    }
}

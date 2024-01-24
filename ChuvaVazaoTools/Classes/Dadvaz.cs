using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary.Dadvaz
{
    public class Dadvaz : BaseDocument
    {
        Dictionary<string, IBlock<BaseLine>> blocos = new Dictionary<string, IBlock<BaseLine>>() {
                    {"HR", new DataBlock()},
                    {"DIA", new DiaBlock()},
                    {"VAZOES", new VazoesBlock()},

                };

        public override Dictionary<string, IBlock<BaseLine>> Blocos
        {
            get { return blocos; }
        }

        public DataBlock BlocoData { get { return (DataBlock)Blocos["HR"]; } set { Blocos["HR"] = value; } }
        public DiaBlock BlocoDia { get { return (DiaBlock)Blocos["DIA"]; } set { Blocos["DIA"] = value; } }
        public VazoesBlock BlocoVazoes { get { return (VazoesBlock)Blocos["VAZOES"]; } set { Blocos["VAZOES"] = value; } }

        public override void Load(string fileContent)
        {


            var lines = fileContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string comments = null;

            var currentBlock = "";
            for(int i = 0;i< lines.Count();i++)
            {
                if (i == 9)
                {
                    currentBlock = "HR";
                    var newL = Blocos[currentBlock].CreateLine(lines[i]);
                    newL.Comment = comments;
                    comments = null;
                    Blocos[currentBlock].Add(newL);
                    currentBlock = "";
                }
                else if (i == 12)
                {
                    currentBlock = "DIA";
                    var newL = Blocos[currentBlock].CreateLine(lines[i]);
                    newL.Comment = comments;
                    comments = null;
                    Blocos[currentBlock].Add(newL);
                    currentBlock = "";
                }
                else if(i >= 16 && i < (lines.Count() - 1) && !lines[i].Contains("FIM"))
                {
                    currentBlock = "VAZOES";
                    var newL = Blocos[currentBlock].CreateLine(lines[i]);
                    newL.Comment = comments;
                    comments = null;
                    Blocos[currentBlock].Add(newL);
                    currentBlock = "";
                }
                else
                {
                    comments = comments == null ? lines[i] : comments + Environment.NewLine + lines[i];
                }
                //if (!Blocos.ContainsKey(currentBlock))
                //{
                //    continue;
                //}
               

            }
            if (comments != null)
            {
                BottonComments = comments;
            }
        }


        public override bool IsComment(string line)
        {
            return line.StartsWith("&");
        }
    }
}

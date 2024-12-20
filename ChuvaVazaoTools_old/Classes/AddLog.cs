﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuvaVazaoTools.Classes
{
    public class AddLog
    {
        public AddLog(string texto)
        {
            try
            {
                var path = "H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log";
                var file = "Chuva_vazao" + DateTime.Today.ToString("yyyyMMdd");
                if (File.Exists(Path.Combine(path, file)))
                {
                    texto += File.ReadAllText(Path.Combine(path, file));
                    File.WriteAllText(Path.Combine(path, file), texto);
                }
                else
                {
                    File.Create(path + file);
                    new AddLog(texto);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

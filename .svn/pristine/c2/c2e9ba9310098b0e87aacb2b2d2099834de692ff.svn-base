using GradsHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChuvaVazaoTools
{
    public static class Program
    {
        public static Tuple<string, string> GetPrevivazExPath(string path)
        {
            string anchorKeyD = @"SOFTWARE\Classes\*\shell\decompToolsShellX";
            string ctxMenuD = @"SOFTWARE\Classes\*\ContextMenus\decompToolsShellX";

            try
            {
                var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(anchorKeyD);

                var k2 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ctxMenuD);
                k2 = k2.OpenSubKey("shell");

                var k2_1 = k2.OpenSubKey("cmd3");
                var p = k2_1.OpenSubKey("command").GetValue("");

                var fcmd = p.ToString().Replace("%1", path);

                var tm = fcmd.Split(new string[] { " previvaz " }, StringSplitOptions.None);

                var ret = new Tuple<string, string>(tm[0], fcmd.Substring(tm[0].Length));

                return ret;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Config.Read();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {

                try
                {
                    if (args[0].Equals("auto", StringComparison.OrdinalIgnoreCase))
                    {
                        RunAutoRoutines("all");
                    }
                    else
                    {
                        RunAutoRoutines(args[0].ToLower());
                    }
                }
                finally { }
            }
            else
            {
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }

        }

        private static void RunAutoRoutines(string tipo)
        {

            var logFile = Config.CaminhoLogAutoRun;

            var logF = new LogFile(logFile);
            logF.WriteLine("Iniciando AutoRoutine");

            var data = DateTime.Today;

            switch (tipo)
            {
                case "download":
                    logF.WriteLine("Iniciando AutoDownload");
                    AutoDownload(data, logF);
                    break;
                case "run":
                    logF.WriteLine("Iniciando AutoRun");
                    AutoRun(data, logF);
                    break;
                case "report":
                    logF.WriteLine("Iniciando AutoReport");
                    AutoReport(data, logF);
                    break;
                case "all": //roda tudo
                    logF.WriteLine("Iniciando Tudo");
                    AutoDownload(data, logF);
                    AutoRun(data, logF);
                    AutoReport(data, logF);
                    break;
                default:
                    break;
            }

            logF.WriteLine("Encerrando AutoRoutine");
        }


        internal static void AutoDownload(DateTime date, System.IO.TextWriter logF)
        {
            var config = Config.ConfigConjunto;

            var searchPath = System.IO.Path.Combine(Config.CaminhoPrevisao, date.ToString("yyyyMM"), date.ToString("dd"));

            if (!System.IO.Directory.Exists(searchPath)) System.IO.Directory.CreateDirectory(searchPath);
            string funEuro = string.Empty;
            //FUNCEME
            try
            {

                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + date.ToString("yyyy_MM_dd") + @"\OBSERVADO";

                List<string> h;

                if (!File.Exists(Path.Combine(directoryToSaveGif, "statusFunceme.txt")))
                    File.Create(Path.Combine(directoryToSaveGif, "statusFunceme.txt"));

                h = Tools.Tools.readHistory(Path.Combine(directoryToSaveGif, "statusFunceme.txt")).ToList();

                //if(!h.Any(x=>x.Contains("EURO"))

                //if (((!System.IO.Directory.Exists(directoryToSaveGif) || !System.IO.File.Exists(System.IO.Path.Combine(directoryToSaveGif, "funceme.gif"))) 
                if (DateTime.Now.Hour >= 7 && (h.Count() == 0) || h.Any(y => y.Contains("EURO")))
                {
                    logF.WriteLine("FUNCEME");

                    var pr = cptec.DownloadFunceme();
                    string[] filesInside;
                    string path = string.Empty;

                    #region Euro Funceme 

                    //caso os dados de precipitação estejam vazios, para o dia atual, são coletados os dados antigos(quanto mais recente, melhor) de EURO

                    if (pr == null && pr.Prec.Any(x => x.Value == 0) && h.Count() == 0)
                    {


                        pr = null;

                        for (int x = 1; true; x++)
                            if (System.IO.Directory.Exists(Path.Combine("H:\\Middle - Preço\\Acompanhamento de Precipitação\\Previsao_Numerica", date.AddDays(-x).ToString("yyyyMM"), date.AddDays(-x).ToString("dd"), "ECMWF00")))
                            {
                                path = Path.Combine("H:\\Middle - Preço\\Acompanhamento de Precipitação\\Previsao_Numerica", date.AddDays(-x).ToString("yyyyMM"), date.AddDays(-x).ToString("dd"), "ECMWF00");

                                filesInside = Directory.GetFiles(path);
                                break;
                            }


                        foreach (var file in filesInside.Where(y => y.Contains("ctl")))
                        {
                            pr = PrecipitacaoFactory.BuildFromMergeFile(file);

                            if (pr.Data == date && pr.Prec.Count != 0)
                            {
                                funEuro = " do Euro";
                                Tools.Tools.addHistory(directoryToSaveGif, "Funceme do EURO");
                                var retornoEmail = Tools.Tools.SendMail("", "Dados coletados em: http://mobile.funceme.br/tempo/inmet.php?acao=4&sensor=22&intervalo=24 <br> Será utilizado os dados anteriormente fornecidos pelo modelo do Euro.", "Alerta de Funceme Vazio [AUTO]", "preco");
                                break;
                            }
                            pr = null;
                        }
                    }



                    #endregion

                    if (pr != null && (h.Count() == 0 || h.Any(y => y.Contains("EURO"))))
                    {
                        var localPath = System.IO.Path.GetTempPath() + "FUNCEME\\";
                        if (!System.IO.Directory.Exists(localPath)) System.IO.Directory.CreateDirectory(localPath);

                        var fanem = System.IO.Path.Combine(localPath, "funceme_" + date.ToString("yyyyMMdd"));
                        pr.SalvarModeloBin(fanem);
                        Grads.ConvertCtlToImg(fanem, "FUNCEME" + funEuro, "Precipacao observada entre " + date.AddDays(-1).ToString("dd/MM") + " e " + date.ToString("dd/MM"), "funceme.gif", System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs")); cptec.CopyGifs(localPath, directoryToSaveGif);
                        cptec.CopyBin(localPath, System.IO.Path.Combine(Config.CaminhoFunceme, date.Year.ToString("0000"), date.Month.ToString("00")));

                        var remo = new PrecipitacaoConjunto(config);
                        var dic = new Dictionary<DateTime, Precipitacao>();
                        dic[pr.Data] = pr;
                        var chuvaMEDIA = remo.ConjuntoLivre(dic, null);

                        fanem = System.IO.Path.Combine(localPath, "funcememed_" + date.ToString("yyyyMMdd"));
                        chuvaMEDIA[date].SalvarModeloBin(fanem);
                        Grads.ConvertCtlToImg(fanem, "FUNCEME Medio", "Precipacao observada entre " + date.AddDays(-1).ToString("dd/MM") + " e " + date.ToString("dd/MM"), "funcememed.gif", System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
                        cptec.CopyGifs(localPath, directoryToSaveGif);

                        System.IO.Directory.Delete(localPath, true);

                        foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ"))
                        {
                            for (int i = 0; i < chuvaMEDIA.Keys.Count; i++)
                            {
                                PrecipitacaoRepository.SaveAverage(chuvaMEDIA.Keys.ToArray()[i], pCo.Agrupamento.Nome, pCo.Nome, pCo.precMedia[i], "FUNCEME");
                            }

                        }

                        remo = new PrecipitacaoConjunto(config);
                        var chuvaMediaBacia = remo.MediaBacias(dic);
                        foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
                        {
                            for (int i = 0; i < chuvaMediaBacia.Keys.Count; i++)
                            {
                                PrecipitacaoRepository.SaveAverage(chuvaMediaBacia.Keys.ToArray()[i], pCo.Key.Nome, "", pCo.First().precMedia[i], "FUNCEME");
                            }
                        }

                        if (funEuro == "")
                        {
                            File.WriteAllText(Path.Combine(directoryToSaveGif, "statusFunceme.txt"), "Funceme Oficial");
                        }
                    }

                    else
                        logF.WriteLine("FUNCEME OK");
                }


            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

            //MERGE
            try
            {
                logF.WriteLine("MERGE");
                var resp = cptec.ListNewMerge(logF);

                if (resp.Contains("baixando Merge do dia"))
                {
                    var retornoEmail = Tools.Tools.SendMail("", "Precipitação obvservada (MERGE) mais recente disponível", "Precipitação Obvservada [AUTO]", "preco");
                    //sendNotification(, "bruno.araujo@cpas.com.br,natalia.biondo@cpas.com.br,diana.lima@cpas.com.br");
                }

                //create image
                if (!resp.Equals("Nada novo", StringComparison.OrdinalIgnoreCase))
                {

                    //var data = dtAtual.Value.Date;
                    var chuvasMerge = new Dictionary<DateTime, Precipitacao>();

                    var localPath = System.IO.Path.GetTempPath() + "MERGE\\";
                    if (!System.IO.Directory.Exists(localPath)) System.IO.Directory.CreateDirectory(localPath);

                    for (DateTime dt = date.AddDays(-9); dt <= date.Date; dt = dt.AddDays(1))
                    {
                        var mergeCtlFile = System.IO.Directory.GetFiles(Path.Combine(Config.CaminhoMerge, dt.ToString("yyyy")), "prec_" + dt.ToString("yyyyMMdd") + ".ctl", System.IO.SearchOption.AllDirectories);

                        if (mergeCtlFile.Length == 1)
                        {
                            var prec = PrecipitacaoFactory.BuildFromMergeFile(mergeCtlFile[0]);
                            prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeCtlFile[0]);
                            prec.Data = dt;

                            chuvasMerge[dt] = prec;

                            var fanem = System.IO.Path.Combine(localPath, "merge_" + dt.ToString("yyyyMMdd"));
                            prec.SalvarModeloBin(fanem);

                            Grads.ConvertCtlToImg(fanem, "MERGE", "Precipacao observada entre " + dt.AddDays(-1).ToString("dd/MM") + " e " + dt.ToString("dd/MM"), "merge.gif", System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
                            cptec.CopyGifs(localPath, @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\OBSERVADO");
                        }
                    }

                    var remo = new PrecipitacaoConjunto(config);
                    var chuvaMEDIA = remo.ConjuntoLivre(chuvasMerge, null);

                    foreach (var c in chuvaMEDIA)
                    {
                        var fanem = System.IO.Path.Combine(localPath, "mergemed_" + c.Key.ToString("yyyyMMdd"));
                        c.Value.SalvarModeloBin(fanem);
                        Grads.ConvertCtlToImg(fanem, "MERGE Medio", "Precipacao observada entre " + c.Key.AddDays(-1).ToString("dd/MM") + " e " + c.Key.ToString("dd/MM"), "mergemed.gif", System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
                        cptec.CopyGifs(localPath, @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + c.Key.ToString("yyyy_MM_dd") + @"\OBSERVADO");
                    }

                    foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ"))
                    {
                        for (int i = 0; i < chuvaMEDIA.Keys.Count; i++)
                        {
                            PrecipitacaoRepository.SaveAverage(chuvaMEDIA.Keys.ToArray()[i], pCo.Agrupamento.Nome, pCo.Nome, pCo.precMedia[i], "MERGE");
                        }

                    }

                    remo = new PrecipitacaoConjunto(config);
                    var chuvaMediaBacia = remo.MediaBacias(chuvasMerge);
                    foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
                    {
                        for (int i = 0; i < chuvaMediaBacia.Keys.Count; i++)
                        {
                            PrecipitacaoRepository.SaveAverage(chuvaMediaBacia.Keys.ToArray()[i], pCo.Key.Nome, "", pCo.First().precMedia[i], "MERGE");
                        }
                    }

                    System.IO.Directory.Delete(localPath, true);
                }

            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

            //ETA
            try
            {
                IPrecipitacaoForm frm = null;


                var funcLogs = new Action<string>(hora =>
                {

                    WaitForm2.TipoEta eta;
                    WaitForm2.TipoGefs gefs = WaitForm2.TipoGefs._00h;

                    switch (hora)
                    {
                        case "00":
                            eta = WaitForm2.TipoEta._00h;
                            break;
                        case "12":
                            eta = WaitForm2.TipoEta._12h;
                            break;
                        default:
                            return;
                    }
                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = WaitForm.TipoConjunto.Eta40;
                    frm.RemoveLimiteETA = false;
                    frm.RemoveViesETA = false;
                    frm.SalvarDados = true;

                    frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, "eta" + hora + ".log"));
                });

                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "eta00.log")) || !System.IO.Directory.Exists(System.IO.Path.Combine(searchPath, "ETA00")))
                {
                    logF.WriteLine("ETA 00");
                    cptec.DownloadETA40Gambiarra(date, logF, "00");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemEta00)
                    {
                        funcLogs("00");
                    }
                }
                else logF.WriteLine("ETA 00 OK");

            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

            //GEFS
            try
            {

                IPrecipitacaoForm frm = null;

                var funcLogs = new Action<string>(hora =>
                {

                    var eta = WaitForm2.TipoEta._00h;
                    WaitForm2.TipoGefs gefs;

                    switch (hora)
                    {
                        case "00":
                            gefs = WaitForm2.TipoGefs._00h;
                            break;
                        case "06":
                            gefs = WaitForm2.TipoGefs._06h;
                            break;
                        case "12":
                            gefs = WaitForm2.TipoGefs._12h;
                            break;
                        default:
                            return;
                    }

                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = WaitForm.TipoConjunto.Gefs;
                    frm.RemoveLimiteGEFS = false;
                    frm.RemoveViesGEFS = false;
                    frm.SalvarDados = true;

                    frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, "gefs" + hora + ".log"));
                });


                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gefs00.log")))
                {

                    logF.WriteLine("GEFS 00");
                    cptec.DownloadNoaaImgs(date, logF, "GEFS", "00");
                    //gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GEFS", "00");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGefs00)
                    {
                        funcLogs("00");
                        var retornoEmail = Tools.Tools.SendMail("", "GEFS 00h Disponivel", "GEFS 00h [AUTO]", "preco");
                        //sendNotification("GEFS 00h Disponivel", "bruno.araujo@cpas.com.br,marcos.albarracin@cpas.com.br;diana.lima@cpas.com.br");
                    }
                }
                else logF.WriteLine("GEFS 00 OK");
                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gefs06.log")))
                {
                    logF.WriteLine("GEFS 06");
                    cptec.DownloadNoaaImgs(date, logF, "GEFS", "06");
                    // gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GEFS", "06");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGefs06)
                    {
                        funcLogs("06");
                    }
                }
                else logF.WriteLine("GEFS 06 OK");
                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gefs12.log")))
                {

                    logF.WriteLine("GEFS 12");
                    cptec.DownloadNoaaImgs(date, logF, "GEFS", "12");
                    //gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GEFS", "12");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGefs12)
                    {
                        funcLogs("12");
                        var retornoEmail = Tools.Tools.SendMail("", "GEFS 12h Disponivel", "GEFS 12h [AUTO]", "preco");
                        //sendNotification("GEFS 12h Disponivel", "bruno.araujo@cpas.com.br;diana.lima@cpas.com.br");
                    }
                }
                else logF.WriteLine("GEFS 12 OK");
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

            //CONJUNTO
            try
            {
                IPrecipitacaoForm frm = null;


                var funcLogs = new Action<string>(hora =>
                {

                    var eta = hora == "00" ? WaitForm2.TipoEta._00h : WaitForm2.TipoEta._12h;
                    var gefs = hora == "00" ? WaitForm2.TipoGefs._00h : WaitForm2.TipoGefs._12h;

                    frm.LimparCache();
                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = WaitForm.TipoConjunto.Conjunto;
                    frm.SalvarDados = true;

                    var chuvasConjunto = frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, "conjunto" + hora + ".log"));

                    var conjPath = System.IO.Path.Combine(searchPath, "CONJUNTO" + hora);

                    if (!System.IO.Directory.Exists(conjPath)) System.IO.Directory.CreateDirectory(conjPath);

                    foreach (var prec in chuvasConjunto)
                    {
                        PrecipitacaoFactory.SalvarModeloBin(prec.Value,
                            System.IO.Path.Combine(conjPath,
                            "pp" + date.ToString("yyyyMMdd") + "_" + ((prec.Key - date).TotalHours + (hora == "00" ? 12 : 0)).ToString("0000")
                            )
                        );
                    }
                    //criar imagens conjunto                        
                    cptec.ProcessaConjunto(date, hora);

                });

                var funcLogsExtended = new Action(() =>
                {

                    var eta = WaitForm2.TipoEta._00h;
                    var gefs = WaitForm2.TipoGefs._00h;

                    frm.LimparCache();
                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = frm.TemEta00 ? WaitForm.TipoConjunto.Conjunto : WaitForm.TipoConjunto.Gefs;
                    frm.Previsoes2Semanas = true;
                    frm.SalvarDados = true;

                    var chuvasConjunto = frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, frm.TemEta00 ? "conjunto00_EXT.log" : "gefsvies00_EXT.log"));

                    var conjPath = System.IO.Path.Combine(searchPath, frm.TemEta00 ? "CONJUNTO2W00" : "GEFSVIES2W00");

                    if (!System.IO.Directory.Exists(conjPath)) System.IO.Directory.CreateDirectory(conjPath);

                    foreach (var prec in chuvasConjunto)
                    {
                        PrecipitacaoFactory.SalvarModeloBin(prec.Value,
                            System.IO.Path.Combine(conjPath,
                            "pp" + date.ToString("yyyyMMdd") + "_" + ((prec.Key - date).TotalHours + 12).ToString("0000")
                            )
                        );
                    }
                });

                var funcLogsExtendedVies = new Action(() =>
                {

                    var eta = WaitForm2.TipoEta._00h;
                    var gefs = WaitForm2.TipoGefs._00h;

                    frm.LimparCache();
                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = WaitForm.TipoConjunto.Conjunto;
                    frm.Previsoes2Semanas = true;
                    frm.SalvarDados = true;
                    frm.RemoveLimiteETA = frm.RemoveLimiteGEFS = frm.RemoveViesETA = frm.RemoveViesGEFS = false;

                    var chuvasConjunto = frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, "conjunto00_EXT_COMVIES.log"));

                    var conjPath = System.IO.Path.Combine(searchPath, "CONJUNTO2W_COMVIES_00");

                    if (!System.IO.Directory.Exists(conjPath)) System.IO.Directory.CreateDirectory(conjPath);

                    foreach (var prec in chuvasConjunto)
                    {
                        PrecipitacaoFactory.SalvarModeloBin(prec.Value,
                            System.IO.Path.Combine(conjPath,
                            "pp" + date.ToString("yyyyMMdd") + "_" + ((prec.Key - date).TotalHours + 12).ToString("0000")
                            )
                        );
                    }
                });



                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "conjunto00.log")))
                {
                    frm = WaitForm2.CreateInstance(date);
                    if (frm.TemEta00 && frm.TemGefs00)
                    {
                        logF.WriteLine("CONJUNTO 00");
                        funcLogs("00");


                        funcLogsExtended();

                    }
                    else if (frm.TemGefs00 && !System.IO.Directory.Exists(System.IO.Path.Combine(searchPath, "GEFSVIES2W00")))
                    {
                        funcLogsExtended();
                    }

                }
                else logF.WriteLine("CONJUNTO 00 OK");

                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "conjunto00_EXT.log")))
                {
                    frm = WaitForm2.CreateInstance(date);
                    if (frm.TemEta00 && frm.TemGefs00)
                    {
                        funcLogsExtended();
                    }
                }


                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "conjunto00_EXT_COMVIES.log")))
                {
                    frm = WaitForm2.CreateInstance(date);
                    if (frm.TemEta00 && frm.TemGefs00)
                    {
                        funcLogsExtendedVies();
                    }
                }

            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.Message);
            }

            ////GFS
            try
            {
                IPrecipitacaoForm frm = null;


                var funcLogs = new Action<string>(hora =>
                {

                    var eta = hora == "00" ? WaitForm2.TipoEta._00h : WaitForm2.TipoEta._12h;
                    WaitForm2.TipoGefs gefs;

                    switch (hora)
                    {
                        case "00":
                            gefs = WaitForm2.TipoGefs._ctl_00h;
                            break;
                        case "06":
                            gefs = WaitForm2.TipoGefs._ctl_06h;
                            break;
                        case "12":
                            gefs = WaitForm2.TipoGefs._ctl_12h;
                            break;
                        default:
                            return;
                    }
                    frm.Eta = eta;
                    frm.Gefs = gefs;
                    frm.Tipo = WaitForm.TipoConjunto.Gefs;
                    frm.RemoveLimiteGEFS = false;
                    frm.RemoveViesGEFS = false;
                    frm.SalvarDados = true;

                    frm.ProcessarConjunto(saveLogFile: System.IO.Path.Combine(searchPath, "gfs" + hora + ".log"));
                });


                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gfs00.log")))
                {
                    logF.WriteLine("GFS 00");
                    //gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GFS", "00");
                    //cptec.DownloadGFSNoaa(data, logF, "GFS", "00");

                    cptec.DownloadNoaaImgs(date, logF, "GFS", "00");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGfs00)
                    {
                        funcLogs("00");
                    }
                }
                else logF.WriteLine("GFS 00 OK");
                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gfs06.log")))
                {
                    logF.WriteLine("GFS 06");
                    //gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GFS", "06");
                    //cptec.DownloadGFSNoaa(data, logF, "GFS", "06");

                    cptec.DownloadNoaaImgs(date, logF, "GFS", "06");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGfs06)
                    {
                        funcLogs("06");
                    }
                }
                else logF.WriteLine("GFS 06 OK");
                if (!System.IO.File.Exists(System.IO.Path.Combine(searchPath, "gfs12.log")))
                {

                    logF.WriteLine("GFS 12");
                    //gefsOK = cptec.DownloadGEFSNoaa(data, logF, "GFS", "12");
                    //cptec.DownloadGFSNoaa(data, logF, "GFS", "12");
                    cptec.DownloadNoaaImgs(date, logF, "GFS", "12");

                    frm = WaitForm2.CreateInstance(date);

                    if (frm.TemGfs12)
                    {
                        funcLogs("12");
                    }
                }
                else logF.WriteLine("GFS 12 OK");

            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

            //MODELO EURO
            try
            {
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(searchPath, "ECMWF00")))
                {
                    logF.WriteLine("EURO 00");
                    logF.WriteLine(cptec.DownloadMeteologixImgs(date, logF, out _));
                }
                else logF.WriteLine("EURO 00 OK");

                if (!System.IO.Directory.Exists(System.IO.Path.Combine(searchPath, "ECMWF12")))
                {
                    logF.WriteLine("EURO 12");
                    logF.WriteLine(cptec.DownloadMeteologixImgs(date, logF, out _, "12"));
                    var retornoEmail = Tools.Tools.SendMail("", "Euro 12h Disponivel", "Euro 12h [AUTO]", "preco");
                    //sendNotification("EURO 12h Disponivel", "bruno.araujo@cpas.com.br;diana.lima@cpas.com.br");
                }
                else logF.WriteLine("EURO 12 OK");

            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }


            // Converte Bin para Dat arquivos do GFS00 e do ECMWF00
            try
            {
                Convert_BinDat("ECMWF");
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }

        }

        internal static async void AutoExec(DateTime date, System.IO.TextWriter logF)
        {
            ///Rodada automática
            /// 
            var frmMain = new FrmMain(true);

            //frmMain.Run(logF, out _);
            //return;

            try
            {
                if (date.DayOfWeek != DayOfWeek.Thursday)
                {
                    //rodada com offset na remoção de viés.
                    frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoUmaSemanaEuro);
                }
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            /*var ac = new Action(() => { });
            await System.Threading.Tasks.Task.Run(ac);*/
            try
            {

                frmMain.RunExecProcess(logF, out var runIdT);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                if (date.DayOfWeek != DayOfWeek.Thursday)
                {
                    //rodada com offset na remoção de viés.
                    frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoUmaSemana);
                }
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGEFS);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasEuro);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGFS);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.RunExecProcess(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGFS2x);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
        }

        internal static void AutoRun(DateTime date, System.IO.TextWriter logF)
        {
            ///Rodada automática
            /// 
            var frmMain = new FrmMain(true);

            //frmMain.Run(logF, out _);
            //return;

            try
            {
                if (date.DayOfWeek != DayOfWeek.Thursday)
                {
                    //rodada com offset na remoção de viés.
                    frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoUmaSemanaEuro);
                }
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {

                frmMain.Run(logF, out var runIdT);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                if (date.DayOfWeek != DayOfWeek.Thursday)
                {
                    //rodada com offset na remoção de viés.
                    frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoUmaSemana);
                }
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGEFS);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasEuro);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGFS);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
            try
            {
                frmMain.Run(logF, out _, FrmMain.EnumRemo.RemocaoDuasSemanasGFS2x);
            }
            catch (Exception ex)
            {
                logF.WriteLine(ex.ToString());
            }
        }

        internal static void AutoReport(DateTime date, System.IO.TextWriter logF)
        {
            //condicões para relatorios: Downloads OK / (Rodadas CV do dia ok || 8:40 da manhã)
            var nextRev = Tools.Tools.GetNextRev(date);
            var previsoesPath0 = Path.Combine(@"H:\Middle - Preço\16_Chuva_Vazao", nextRev.revDate.ToString("yyyy_MM"), $"RV{nextRev.rev}", date.ToString("yy-MM-dd"));

            var dirA = (Path.Combine(previsoesPath0, "CV_ACOMPH_FUNC"));
            var dirB = (Path.Combine(previsoesPath0, "CV_ACOMPH_FUNC_d-1"));

            try
            {
                //caminho = @"P:\Marcos.Albarracin\Relatorio Final\" + "Relatorio_Compass_" + data.ToString("dd_MM_yyyy") + "_(" + hora.ToString() + " hrs).pdf";
                if ((Directory.Exists(dirB) && File.Exists(Path.Combine(dirB, "enadiaria.log"))) || $"{DateTime.Now.Hour:00}{DateTime.Now.Minute:00}".CompareTo("0800") > 0)
                {
                    var caminhoRel = @"P:\Marcos.Albarracin\Relatorio Final\Relatorios\" + "Relatorio_Compass_" + date.ToString("dd_MM_yyyy") + "_(" + 0.ToString() + " hrs)_Preliminar.pdf";

                    if (!File.Exists(caminhoRel))
                    {
                        Tools.Tools.addHistory("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log\\report.txt", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "- tentativa de gerar relatório via pârametro [preliminar == true]");
                        logF.WriteLine("Gerando relatorio:");
                        logF.WriteLine(caminhoRel);
                        Report.Program.CriarRelatorio2(date, caminhoRel, true);

                        var retornoEmail = Tools.Tools.SendMail(caminhoRel, "Relatório de acompanhamento disponível", "Relatório de acompanhamento [AUTO]", "preco");
                        //sendNotification("Relatório de acompanhamento disponível", "bruno.araujo@cpas.com.br,natalia.biondo@cpas.com.br,diana.lima@cpas.com.br,pedro.modesto@cpas.com.br", caminhoRel);
                        retornoEmail.Wait();
                    }
                    else
                    {
                        logF.WriteLine("Relatório já existente:");
                        logF.WriteLine(caminhoRel);
                    }
                    // relatorio 00 h preliminar
                }

                if ((Directory.Exists(dirA) && File.Exists(Path.Combine(dirA, "enadiaria.log"))))
                {
                    var caminhoRel = @"P:\Marcos.Albarracin\Relatorio Final\Relatorios\" + "Relatorio_Compass_" + date.ToString("dd_MM_yyyy") + "_(" + 0.ToString() + " hrs).pdf";

                    if (!File.Exists(caminhoRel))
                    {
                        Tools.Tools.addHistory("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log\\report.txt", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "- tentativa de gerar relatório via pârametro [preliminar == false]");
                        logF.WriteLine("Gerando relatorio:");
                        logF.WriteLine(caminhoRel);
                        Report.Program.CriarRelatorio2(date, caminhoRel, false);

                        var retornoEmail = Tools.Tools.SendMail(caminhoRel, "Relatório de acompanhamento disponível", "Relatório de acompanhamento [AUTO]", "preco");

                        retornoEmail.Wait();
                        //sendNotification("Relatório de acompanhamento disponível", "bruno.araujo@cpas.com.br,natalia.biondo@cpas.com.br,diana.lima@cpas.com.br,pedro.modesto@cpas.com.br", caminhoRel);
                    }
                    else
                    {
                        logF.WriteLine("Relatório já existente:");
                        logF.WriteLine(caminhoRel);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.Tools.addHistory("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log\\report.txt", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "- catch acionado na tentativa de gerar relatório via pârametro: " + ex.Message);
                logF.WriteLine(ex.ToString());
            }
        }


        internal static void Convert_BinDat(string metodo)
        {
            DateTime data_inicial = DateTime.Today.AddDays(-5);
            DateTime data_final = DateTime.Today;
            var frmMain = new FrmMain();

            
            var localPath = Config.CaminhoPrevisao;

            var localModelo = Config.CaminhoModelo;
            

            var p1 = System.IO.Path.Combine(localPath, data_final.ToString("yyyyMM"), data_final.ToString("dd"));

            var p2 = System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"));

            if (metodo == "funceme")
            {
                localPath = Config.CaminhoFunceme;

                p1 = System.IO.Path.Combine(localPath, data_inicial.ToString("yyyy"), data_inicial.ToString("MM"));

            }


            while (System.IO.Directory.Exists(p1) && (data_inicial <= data_final))
            {
               

                p2 = System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"));
                if (metodo == "funceme")
                {
                    p2 = System.IO.Path.Combine(localModelo, metodo, data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"));
                }
                if (!System.IO.Directory.Exists(p2))
                {


                    var localfilePath = System.IO.Path.Combine(localPath, data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"), metodo + "_00h");
                    if (!System.IO.Directory.Exists(localfilePath))
                    {
                        localfilePath = System.IO.Path.Combine(localPath, data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"), metodo + "00");
                    }
                    if (metodo == "funceme")
                    {
                        localfilePath = System.IO.Path.Combine(localPath, data_inicial.ToString("yyyy"), data_inicial.ToString("MM"));
                    }

                    if (System.IO.Directory.Exists(localfilePath))
                    {
                        string[] arquivos = Directory.GetFiles(localfilePath, "*.ctl");
                        if (arquivos != null)
                        {
                            foreach (var file in arquivos)
                            {
                                if (System.IO.Path.GetExtension(file).ToUpperInvariant() == ".CTL")
                                {
                                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"pp(\d{4})(\d{2})(\d{2})_(\d+)");
                                    if (metodo == "funceme")
                                    {
                                         r = new System.Text.RegularExpressions.Regex(@"(\d{4})(\d{2})(\d{2})");
                                    }
                                    
                                    var fMatch = r.Match(file);
                                    if (fMatch.Success)
                                    {
                                        var data = new DateTime(
                                            int.Parse(fMatch.Groups[1].Value),
                                            int.Parse(fMatch.Groups[2].Value),
                                            int.Parse(fMatch.Groups[3].Value))
                                            ;
                                        var dataPrev = data;
                                        if (metodo != "funceme")
                                        {
                                            var horas = int.Parse(fMatch.Groups[4].Value);

                                            dataPrev = data.AddHours(horas).Date;
                                        }

                                        frmMain.chuvas[dataPrev] = PrecipitacaoFactory.BuildFromMergeFile(file);
                                        frmMain.chuvas[dataPrev].Descricao = System.IO.Path
                                            .GetFileName(file);

                                        frmMain.chuvas[dataPrev].Data = dataPrev;

                                    }
                                }
                            }
                            var CaminhoArq = System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyy"));
                            if (metodo == "funceme")
                            {
                                if (!System.IO.Directory.Exists(System.IO.Path.Combine(localModelo, metodo, data_inicial.ToString("yyyyMM"))))
                                {
                                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(localModelo, metodo, data_inicial.ToString("yyyyMM")));
                                }
                                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(localModelo, metodo, data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd")));
                                CaminhoArq = System.IO.Path.Combine(localModelo, metodo, data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"));
                            }
                            else
                            {
                                if (!System.IO.Directory.Exists(System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM"))))
                                {
                                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM")));
                                }
                                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd")));
                                CaminhoArq = System.IO.Path.Combine(localModelo, metodo + "00", data_inicial.ToString("yyyyMM"), data_inicial.ToString("dd"));

                            }
                            if (metodo == "funceme")
                            {
                                foreach (var prec in frmMain.chuvas.Where(x => x.Key == data_inicial))
                                {
                                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"(\d{4})(\d{2})(\d{2})");
                                    string ajuste_data = prec.Value.Descricao.ToString().Split('.').First();
                                    var fMatch = r.Match(ajuste_data);
                                    if (fMatch.Success)
                                    {
                                        var data = new DateTime(
                                            int.Parse(fMatch.Groups[1].Value),
                                            int.Parse(fMatch.Groups[2].Value),
                                            int.Parse(fMatch.Groups[3].Value))
                                            ;
                                        var raiznome = metodo + "_p" + data.ToString("ddMMyy") + "a" + prec.Value.Data.ToString("ddMMyy") + ".dat";
                                        prec.Value.SalvarModeloEta(System.IO.Path.Combine(CaminhoArq, raiznome));
                                    }
                                   // var raiznome = prec.Value.Descricao.ToString().Split('.').First() + ".dat";
                                   // raiznome = metodo + "_p" + data.ToString("ddMMyy") + "a" + prec.Value.Data.ToString("ddMMyy") + ".dat";
                                   // prec.Value.SalvarModeloEta(System.IO.Path.Combine(CaminhoArq, raiznome));
                                }
                            }
                            else
                            {
                                foreach (var prec in frmMain.chuvas.Where(x => x.Key >= data_inicial))
                                {

                                    if (metodo == "GEFS" || metodo == "GFS")
                                    {
                                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"pp(\d{4})(\d{2})(\d{2})_(\d+)");
                                        string ajuste_data = prec.Value.Descricao.ToString().Split('.').First();
                                        var fMatch = r.Match(ajuste_data);
                                        if (fMatch.Success)
                                        {
                                            var data = new DateTime(
                                                int.Parse(fMatch.Groups[1].Value),
                                                int.Parse(fMatch.Groups[2].Value),
                                                int.Parse(fMatch.Groups[3].Value))
                                                ;

                                            var raiznome = metodo + "_p" + data.ToString("ddMMyy") + "a" + prec.Value.Data.ToString("ddMMyy") + ".dat";
                                            prec.Value.SalvarModeloDAT(System.IO.Path.Combine(CaminhoArq, raiznome), metodo);
                                        }
                                    }
                                    else
                                    {
                                        var raiznome = prec.Value.Descricao.ToString().Split('.').First() + ".dat";
                                        prec.Value.SalvarModeloEta(System.IO.Path.Combine(CaminhoArq, raiznome));

                                    }
                                }
                            }
                        }


                    }
                    data_inicial = data_inicial.AddDays(1);

                }
                else
                {
                    data_inicial = data_inicial.AddDays(1);
                }
            }
            if (metodo == "ECMWF")
            {
                Convert_BinDat("GFS");
            }
            if (metodo == "GFS")
            {
                Convert_BinDat("GEFS");
            }
            if(metodo == "GEFS")
            {
                Convert_BinDat("funceme");
            }




        }


       // public static void sendNotification(string message, string tos, string attachment = null)
        //{

        //    /*if (!tos.Contains("@"))
        //        tos = ChuvaVazaoTools.Config.ReceiversGroup;*/

        //    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

        //    msg.Body = " Acompanhamento de Precipitação \r\n\r\n";

        //    msg.Body = msg.Body + message + "\r\n";

        //    msg.Subject = "Acompanhamento de Precipitação";

        //    msg.Sender = msg.From = new System.Net.Mail.MailAddress("cpas.robot@gmail.com");


        //    msg.ReplyToList.Add(new System.Net.Mail.MailAddress("bruno.araujo@cpas.com.br"));


        //    foreach (var to in tos.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
        //    {
        //        msg.To.Add(to);
        //    }

        //    if (!string.IsNullOrWhiteSpace(attachment) && System.IO.File.Exists(attachment))
        //    {
        //        msg.Attachments.Add(
        //            new System.Net.Mail.Attachment(attachment)
        //            );
        //    }

        //    System.Net.Mail.SmtpClient cli = new System.Net.Mail.SmtpClient();

        //    cli.Host = "smtp.gmail.com";
        //    cli.Port = 587;
        //    cli.Credentials = new System.Net.NetworkCredential("cpas.robot@gmail.com", "cp@s9876");

        //    cli.EnableSsl = true;

        //    cli.Send(msg);  //.SendMailAsync(msg);
        //}
    }

    public class LogFile : System.IO.TextWriter
    {
        string file = "";

        string computerName = "";




        public LogFile(string filePath)
        {

            computerName = System.Environment.MachineName;
            file = filePath;
        }

        public override void WriteLine(string value)
        {
            try
            {
                System.IO.File.AppendAllText(file, $"{DateTime.Now.ToString()} - {computerName} - {value}{System.Environment.NewLine}");
                base.WriteLine(value);
            }
            finally { }
        }

        public override Encoding Encoding { get { return System.Text.Encoding.UTF8; } }
    }

    public static class Helper
    {
        public static Microsoft.Office.Interop.Excel.Application StartExcel()
        {
            Microsoft.Office.Interop.Excel.Application instance = null;
            try
            {
                instance = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                instance = new Microsoft.Office.Interop.Excel.Application();
            }
            instance.Visible = true;

            return instance;
        }

        public static void Release(object o)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
        }
    }

}

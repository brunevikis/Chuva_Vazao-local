using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ChuvaVazaoTools
{
    class Gerar_Mapas_R
    {
        public static void Gerar_R(string path_Conj, System.IO.TextWriter logF, bool shadow = false, bool merge = false, bool cfs = false)
        {

            DateTime data_Atual = DateTime.Today;
            var path_H = @"H:\Middle - Preço\Acompanhamento de Precipitação\";
            var path_Previsao = Path.Combine(path_H, "Previsao_Numerica");
            var path_CSV = Path.Combine(path_Conj, "Trabalho\\Uruguai\\Passo Sao Joao");
            var path_Acomph = @"H:\Middle - Preço\Acompanhamento de vazões\ACOMPH\1_historico\";
            var path_ModeloR = Path.Combine(path_Previsao, "Modelo_R");

            //var oneDrivePath_ori = Environment.GetEnvironmentVariable("OneDriveCommercial");//C:\Enercore\Energy Core Trading
            var oneDrivePath_ori = @"C:\Enercore\Energy Core Trading";//C:\Enercore\Energy Core Trading
            //C:\Compass\MinhaTI\Alex Freires Marques - Compass\Trading
            //var oneDrive = Path.Combine(oneDrivePath_ori, @"Compass\Pedro\NOAA\");
            //if (!Directory.Exists(oneDrive))
            //{
            //    oneDrive = Path.Combine(oneDrivePath_ori.Replace(oneDrivePath_ori.Split('\\').Last(), @"MinhaTI\Alex Freires Marques - Compass\Pedro\NOAA\"));
            //}

            var oneDrive_preco = Path.Combine(oneDrivePath_ori, @"Energy Core Pricing - Documents\Acompanhamento_de_Precipitacao\Previsao\");
            //B:\Enercore\Energy Core Trading\Energy Core Pricing - Documents\Acompanhamento_de_Precipitacao
            if (!Directory.Exists(oneDrive_preco))
            {
                oneDrive_preco = oneDrive_preco.Replace("Energy Core Pricing - Documents", "Energy Core Pricing - Documentos");
            }
            // Date of VE
            int dias_ve = -1;
            int ve_antecipada = 0;
            DateTime[] feriados_ve = ChuvaVazaoTools.Tools.Tools.feriados;

            #region ve padrão quinta-feira
            var runRev_Curr = ChuvaVazaoTools.Tools.Tools.GetCurrRev(data_Atual);

            ve_antecipada = feriados_ve.Contains(runRev_Curr.revDate) ? -2 : feriados_ve.Contains(runRev_Curr.revDate.AddDays(-1)) ? -3 : -1;
            dias_ve = ve_antecipada;
            var cv1 = runRev_Curr.revDate.AddDays(dias_ve);

            //if (cv1.Date == new DateTime (2025,12,23).Date || cv1.Date == new DateTime(2025,12,30).Date)//todo: retirar esse if no dia 01 de janeiro apos radadas acomph
            //{
            //    cv1 = cv1.AddDays(-1);
            //}

            var cvx = cv1.AddDays(-1);

            logF.WriteLine("VE_CV1 = " + cv1.ToString("dd/MM/yyyy"));
            logF.WriteLine("VE_CVX = " + cvx.ToString("dd/MM/yyyy"));


            var runRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual);
            ve_antecipada = feriados_ve.Contains(runRev.revDate) ? -2 : feriados_ve.Contains(runRev.revDate.AddDays(-1)) ? -3 : -1;
            var cv2 = runRev.revDate.AddDays(ve_antecipada);

            //if (cv2.Date == new DateTime(2025, 12, 30).Date)//todo: retirar esse if no dia 01 de janeiro apos radadas acomph
            //{
            //    cv2 = cv2.AddDays(-1);
            //}

            logF.WriteLine("VE_CV2 = " + cv2.ToString("dd/MM/yyyy"));

            var runRev3 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 2);
            ve_antecipada = feriados_ve.Contains(runRev3.revDate) ? -2 : feriados_ve.Contains(runRev3.revDate.AddDays(-1)) ? -3 : -1;
            var cv3 = runRev3.revDate.AddDays(ve_antecipada);
            logF.WriteLine("VE_CV3 = " + cv3.ToString("dd/MM/yyyy"));

            var runRev4 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 3);
            ve_antecipada = feriados_ve.Contains(runRev4.revDate) ? -2 : feriados_ve.Contains(runRev4.revDate.AddDays(-1)) ? -3 : -1;
            var cv4 = runRev4.revDate.AddDays(ve_antecipada);
            logF.WriteLine("VE_CV4 = " + cv4.ToString("dd/MM/yyyy"));

            var runRev5 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 4);
            ve_antecipada = feriados_ve.Contains(runRev5.revDate) ? -2 : feriados_ve.Contains(runRev5.revDate.AddDays(-1)) ? -3 : -1;
            var cv5 = runRev5.revDate.AddDays(ve_antecipada);
            #endregion

            #region VE padrão quarta-feira
            //verificar a regra de feriado para VE (por enquanto usaremos a regra: caso feriado tanto quinta quanto quarta pasar a VE para terça)
            //dias_ve = -2;
            //var runRev_Curr = ChuvaVazaoTools.Tools.Tools.GetCurrRev(data_Atual);

            //ve_antecipada = feriados_ve.Contains(runRev_Curr.revDate.AddDays(-1)) ? -3 : feriados_ve.Contains(runRev_Curr.revDate.AddDays(-2)) ? -3 : -2;
            //dias_ve = ve_antecipada;
            //var cv1 = runRev_Curr.revDate.AddDays(dias_ve);
            //logF.WriteLine("VE_CV1 = " + cv1.ToString("dd/MM/yyyy"));


            //var runRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual);
            //ve_antecipada = feriados_ve.Contains(runRev.revDate.AddDays(-1)) ? -3 : feriados_ve.Contains(runRev.revDate.AddDays(-2)) ? -3 : -2;
            //var cv2 = runRev.revDate.AddDays(ve_antecipada);
            //logF.WriteLine("VE_CV2 = " + cv2.ToString("dd/MM/yyyy"));

            //var runRev3 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 2);
            //ve_antecipada = feriados_ve.Contains(runRev3.revDate.AddDays(-1)) ? -3 : feriados_ve.Contains(runRev3.revDate.AddDays(-2)) ? -3 : -2;
            //var cv3 = runRev3.revDate.AddDays(ve_antecipada);
            //logF.WriteLine("VE_CV3 = " + cv3.ToString("dd/MM/yyyy"));

            //var runRev4 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 3);
            //ve_antecipada = feriados_ve.Contains(runRev4.revDate.AddDays(-1)) ? -3 : feriados_ve.Contains(runRev4.revDate.AddDays(-2)) ? -3 : -2;
            //var cv4 = runRev4.revDate.AddDays(ve_antecipada);
            //logF.WriteLine("VE_CV4 = " + cv4.ToString("dd/MM/yyyy"));

            //var runRev5 = ChuvaVazaoTools.Tools.Tools.GetNextRev(data_Atual, 4);
            //ve_antecipada = feriados_ve.Contains(runRev5.revDate.AddDays(-1)) ? -3 : feriados_ve.Contains(runRev5.revDate.AddDays(-2)) ? -3 : -2;
            //var cv5 = runRev5.revDate.AddDays(ve_antecipada);

            #endregion


            if (File.Exists(Path.Combine(path_Conj, "error.log")))
            {
                File.Delete(Path.Combine(path_Conj, "error.log"));
            }

            string funcemePsatPre = "funceme";

            try
            {
                // Roda PSAT
                //  executar_R(path_Conj, "ons.R convert_psat_remvies_V2.R");

                //Last day of Acomph

                var dt_acomph = data_Atual;

                logF.WriteLine("Verificando Acomph");
                while (!File.Exists(Path.Combine(path_Acomph, dt_acomph.ToString("yyyy"), dt_acomph.ToString("MM_yyyy"), "ACOMPH_" + dt_acomph.ToString("dd-MM-yyyy") + ".xls")))
                {
                    dt_acomph = dt_acomph.AddDays(-1);
                }
                // dt_acomph = dt_acomph.AddDays(-1);
                //Check if exist funceme of Today
                //psat preliminar
                logF.WriteLine("Verificando PsatPreliminar data atual");
                bool temPsat = false;
                string psatPrelFolder = Path.Combine(oneDrive_preco.Replace("Previsao", "Observado"), data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "IMERG+GEFS");

                string compIMERG_MERGE = merge == true ? "MERGE+GEFS" : "IMERG+GEFS";
                string psatPrelFolderK = Path.Combine("K:\\cv_temp", data_Atual.ToString("yyyyMMdd"), compIMERG_MERGE);

                string psatpre = "";

                if (Directory.Exists(psatPrelFolderK))
                {
                    psatpre = Directory.GetFiles(psatPrelFolderK).Where(x => x.EndsWith(".dat", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (psatpre.Count() > 0)
                    {
                        temPsat = true;
                        funcemePsatPre = "PsatPreliminar";
                        logF.WriteLine("PsatPreliminar Encontrado via HEADNODE!");
                    }
                }
                else if (Directory.Exists(psatPrelFolder))
                {
                    psatpre = Directory.GetFiles(Path.Combine(oneDrive_preco.Replace("Previsao", "Observado"), data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "IMERG+GEFS")).Where(x => x.EndsWith(".dat", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (psatpre.Count() > 0)
                    {
                        temPsat = true;
                        funcemePsatPre = "PsatPreliminar";
                        logF.WriteLine("PsatPreliminar Encontrado!");
                    }
                }



                //fim psat preliminar
                var funcemeFolder = Path.Combine(@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\Modelo_R\funceme\", data_Atual.ToString("yyyyMM"), data_Atual.ToString("dd"));
                if (!Directory.Exists(funcemeFolder))
                {
                    Directory.CreateDirectory(funcemeFolder);
                }

                var funceme = Directory.GetFiles(Path.Combine(@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\Modelo_R\funceme\", data_Atual.ToString("yyyyMM"), data_Atual.ToString("dd")));

                logF.WriteLine("Verificando Funceme data atual");
                if (funceme.Length != 0 || temPsat == true) //(funceme.Length != 0)
                {
                    if (funceme.Length != 0)
                    {
                        logF.WriteLine("Funceme Encontrado!");
                    }
                    //ultimo dia de atualização da previsão



                    logF.WriteLine("Tranferindo arquivos GEFS para Entrada");
                    //Verifca o GEFS ONS, caso existir copia para os arquivos de entrada 

                    var path_Dia = Path.Combine(path_Previsao, data_Atual.ToString("yyyyMM"), data_Atual.ToString("dd"));

                    var GEFS_NOAA = Path.Combine(path_Previsao, "Modelo_R\\GEFS00");
                    var GEFS_NOAA_05 = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "GEFS_0.5_00");
                    var GEFS_05_K = Path.Combine("K:\\cv_temp", data_Atual.ToString("yyyyMMdd"), "GEFS_0.5_00");

                    if (Directory.Exists(GEFS_05_K))
                    {
                        var GEFS_05_KDats = Directory.GetFiles(GEFS_05_K, "GEFS_*").Where(x => x.EndsWith(".dat"));
                        if (GEFS_05_KDats.Count() > 14)
                        {
                            GEFS_NOAA_05 = GEFS_05_K;
                            logF.WriteLine("GEFS Encontrado via HEADNODE!");
                        }
                        else
                        {
                            logF.WriteLine("Buscando GEFS via Repositorio ONEDRIVE!");
                        }
                    }

                    //var path_ArqPrev = Path.Combine(path_Conj, "Arq_Entrada\\Previsao");
                    var path_ArqPrev = Path.Combine(path_Conj, "grid");
                    if (!Directory.Exists(path_ArqPrev)) Directory.CreateDirectory(path_ArqPrev);

                    var GEFS_ONS = Directory.GetFiles(path_Dia, "GEFS_*").Where(x => x.EndsWith(".dat"));
                    var GEFS_05 = Directory.GetFiles(GEFS_NOAA_05, "GEFS_*").Where(x => x.EndsWith(".dat"));

                    if (GEFS_ONS != null)
                    {
                        if (!Directory.Exists(Path.Combine(path_ArqPrev, "GEFS"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "GEFS"));
                        //14 dias do GEFS ONS
                        ///foreach (var GEFS in GEFS_ONS)
                        foreach (var GEFS in GEFS_ONS)
                        {

                            if (GEFS.EndsWith(".dat"))
                            {
                                var num_carecteres = GEFS.Split('\\').Last().Length;
                                if (num_carecteres == 23)
                                {
                                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                                    var data_mapa = r.Match(GEFS);
                                    if (data_mapa.Success)
                                    {
                                        File.Copy(GEFS, Path.Combine(path_ArqPrev, "GEFS", GEFS.Split('\\').Last()), true);
                                    }
                                }
                                //else if (GEFS.Contains("GEFS_m"))
                                // {
                                //    File.Copy(GEFS, Path.Combine(path_Conj, "Arq_Entrada", "GEFS", GEFS.Split('\\').Last()), true);
                                // }
                            }
                        }
                    }

                    if (GEFS_05 != null)
                    {
                        //Todos os dias do GEFS NOAA
                        var Ult_GEFS = Directory.GetFiles(GEFS_NOAA_05).Where(File => !GEFS_ONS.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

                        if (Ult_GEFS != null)
                        {
                            foreach (var Ult in Ult_GEFS)
                            {
                                if (Ult.EndsWith(".dat"))
                                {

                                    var num_carecteres = Ult.Split('\\').Last().Length;
                                    if (num_carecteres == 23)
                                    {
                                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                                        var data_mapa = r.Match(Ult);
                                        if (data_mapa.Success)
                                        {
                                            File.Copy(Ult, Path.Combine(path_ArqPrev, "GEFS", Ult.Split('\\').Last()), true);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    GEFS_Ext(cv2, Path.Combine(path_ArqPrev, "GEFS"));

                    //ETA 10 ONS dias 


                    var ETA_ONS = Directory.GetFiles(path_Dia, "ETA40_*").Where(x => x.EndsWith(".dat"));

                    if (ETA_ONS != null)
                    {
                        if (!Directory.Exists(Path.Combine(path_ArqPrev, "ETA40"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ETA40"));
                        foreach (var ETA in ETA_ONS)
                        {
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                            var data_mapa = r.Match(ETA);
                            if (data_mapa.Success)
                            {
                                File.Copy(ETA, Path.Combine(path_ArqPrev, "ETA40", ETA.Split('\\').Last()), true);
                            }

                        }
                    }

                    //Euro 14 ONS dias 

                    logF.WriteLine("Verificando arquivos ECMWF Ensemble ...");

                    var Euro_ONS = Directory.GetFiles(path_Dia, "ECMWF_*").Where(x => x.EndsWith(".dat"));

                    if (Euro_ONS != null && Euro_ONS.Count() >= 14)
                    {
                        logF.WriteLine("Tranferindo arquivos ECWMF ONS para Entrada");

                        if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWF"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWF"));
                        foreach (var Euro in Euro_ONS)
                        {
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                            var data_mapa = r.Match(Euro);
                            if (data_mapa.Success)
                            {
                                File.Copy(Euro, Path.Combine(path_ArqPrev, "ECMWF", Euro.Split('\\').Last()), true);
                            }

                        }
                    }
                    else
                    {
                        var ECMWFensemTempo = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "ECMWF");

                        if (Directory.Exists(ECMWFensemTempo))
                        {
                            var ecmwfEnsemTempArqs = Directory.GetFiles(ECMWFensemTempo, "ECMWF_*").Where(x => x.EndsWith(".dat"));
                            if (ecmwfEnsemTempArqs != null && ecmwfEnsemTempArqs.Count() >= 14)
                            {
                                logF.WriteLine("Tranferindo arquivos ECWMF Ensemble TempoOK para Entrada");

                                if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWF"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWF"));
                                foreach (var EcmwfTempo in ecmwfEnsemTempArqs)
                                {
                                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                                    var data_mapa = r.Match(EcmwfTempo);
                                    if (data_mapa.Success)
                                    {
                                        File.Copy(EcmwfTempo, Path.Combine(path_ArqPrev, "ECMWF", EcmwfTempo.Split('\\').Last()), true);
                                    }

                                }
                            }
                        }
                        else
                        {
                            var ECMWFensem_DataStoreFolder = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "ECMWF-ENS00", "txt");
                            var ECMWFensem_DataStoreK = Path.Combine("K:\\cv_temp", data_Atual.ToString("yyyyMMdd"), "ECMWF-ENS00");

                            if (Directory.Exists(ECMWFensem_DataStoreK))
                            {
                                ECMWFensem_DataStoreFolder = ECMWFensem_DataStoreK;
                                logF.WriteLine("ECWMF Ensemble DATA STORE Encontrado via HEADNODE!");

                            }
                            if (Directory.Exists(ECMWFensem_DataStoreFolder))
                            {
                                var ecmwf_DataStoreArqs = Directory.GetFiles(ECMWFensem_DataStoreFolder, "ECMWF-ENS_*").Where(x => x.EndsWith(".dat"));
                                if (ecmwf_DataStoreArqs != null && ecmwf_DataStoreArqs.Count() >= 14)
                                {
                                    logF.WriteLine("Tranferindo arquivos ECWMF Ensemble DATA STORE para Entrada");

                                    if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWF"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWF"));
                                    foreach (var EcmwfData in ecmwf_DataStoreArqs)
                                    {
                                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                                        var data_mapa = r.Match(EcmwfData);
                                        if (data_mapa.Success)
                                        {
                                            File.Copy(EcmwfData, Path.Combine(path_ArqPrev, "ECMWF", EcmwfData.Split('\\').Last().Replace("ECMWF-ENS", "ECMWF")), true);
                                        }

                                    }
                                }
                            }

                        }

                    }
                    if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWF"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWF"));

                    int ECMWFGridArqs = Directory.GetFiles(Path.Combine(path_ArqPrev, "ECMWF")).Count();
                    //if (ECMWFGridArqs < 14)
                    //{
                    logF.WriteLine("Transferidos " + ECMWFGridArqs.ToString() + " arquivos, os demais serão tranferidos do ECMWF extendido mais recente");

                    //}

                    logF.WriteLine("Transferindo arquivos ECWMF extendido...");

                    var data_ecmwf_ext = ECMWF_Ext(cv2, Path.Combine(path_ArqPrev, "ECMWF"), -dias_ve + 13);

                    int ECMWFGridArqs2 = Directory.GetFiles(Path.Combine(path_ArqPrev, "ECMWF")).Count();

                    int difer = ECMWFGridArqs2 - ECMWFGridArqs;

                    logF.WriteLine("Transferidos " + difer.ToString() + " arquivos, Total: " + ECMWFGridArqs2.ToString() + " arquivos !");


                    // ECWMF OP
                    logF.WriteLine("Verificando arquivos ECWMF OP ...");

                    var EcmwfTempoOK = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "ECMWFop", "txt");
                    var EcmwfOP_DataStoreFolder = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "ECMWF-HRES00", "txt");///mudar

                    var EcmwfOP_DataStoreK = Path.Combine("K:\\cv_temp", data_Atual.ToString("yyyyMMdd"), "ECMWF-HRES00");
                    string complementoOP = "ECWMF op DATA STORE Encontrado!";

                    if (Directory.Exists(EcmwfOP_DataStoreK))
                    {
                        EcmwfOP_DataStoreFolder = EcmwfOP_DataStoreK;
                        complementoOP = "ECWMF op DATA STORE Encontrado via HEADNODE!";
                    }

                    if (!Directory.Exists(EcmwfTempoOK))
                    {
                        Directory.CreateDirectory(EcmwfTempoOK);//para evitar de dar problemas na criação dos mapas caso o ecmwftempoOk não tenha baixado,a pasta é criada para não interromper o fluxo de verificação e é passado pra verificar o ecmwf meteologix
                    }
                    var ecmwfTempArqs = Directory.GetFiles(EcmwfTempoOK, "ECMWFop_*").Where(x => x.EndsWith(".dat"));


                    if (ecmwfTempArqs != null && ecmwfTempArqs.Count() >= 9)
                    {
                        if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWFop"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWFop"));
                        logF.WriteLine("Transferindo arquivos ECMWF OP Tempo OK");

                        foreach (var arqs in ecmwfTempArqs)
                        {
                            string nameFile = Path.GetFileName(arqs);

                            File.Copy(arqs, Path.Combine(path_ArqPrev, "ECMWFop", nameFile), true);
                        }
                    }
                    else if (Directory.Exists(EcmwfOP_DataStoreFolder))
                    {
                        var ecmwfOP_DataStoreArqs = Directory.GetFiles(EcmwfOP_DataStoreFolder, "ECMWF-HRES_*").Where(x => x.EndsWith(".dat"));

                        if (ecmwfOP_DataStoreArqs != null && ecmwfOP_DataStoreArqs.Count() >= 9)
                        {
                            logF.WriteLine(complementoOP);

                            if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWFop"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWFop"));
                            logF.WriteLine("Transferindo arquivos ECMWF OP DATA STORE");

                            foreach (var EcmwfOpData in ecmwfOP_DataStoreArqs)
                            {
                                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");
                                var data_mapa = r.Match(EcmwfOpData);
                                if (data_mapa.Success)
                                {
                                    File.Copy(EcmwfOpData, Path.Combine(path_ArqPrev, "ECMWFop", EcmwfOpData.Split('\\').Last().Replace("ECMWF-HRES", "ECMWFop")), true);
                                }

                            }
                        }

                    }
                    else
                    {
                        var ECMWFs = Directory.GetFiles(Path.Combine(path_ModeloR, "ECMWF00", data_Atual.ToString("yyyyMM"), data_Atual.ToString("dd"))).Where(x => x.EndsWith(".dat"));

                        if (ECMWFs != null)
                        {
                            if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWFop"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWFop"));
                            logF.WriteLine("Transferindo arquivos ECMWF OP  meteologix");

                            foreach (var ECMWF in ECMWFs)
                            {

                                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"pp(\d{4})(\d{2})(\d{2})_(\d+)");

                                var fMatch = r.Match(ECMWF);
                                if (fMatch.Success)
                                {
                                    var data = new DateTime(
                                        int.Parse(fMatch.Groups[1].Value),
                                        int.Parse(fMatch.Groups[2].Value),
                                        int.Parse(fMatch.Groups[3].Value))
                                        ;

                                    var horas = int.Parse(fMatch.Groups[4].Value);

                                    var dataPrev = data.AddHours(horas).Date;

                                    File.Copy(ECMWF, Path.Combine(path_ArqPrev, "ECMWFop", "ECMWFop_p" + data.ToString("ddMMyy") + "a" + dataPrev.ToString("ddMMyy") + ".dat"), true);

                                }
                            }
                        }
                    }

                    if (!Directory.Exists(Path.Combine(path_ArqPrev, "ECMWFop"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "ECMWFop"));

                    int ECMWFOpGridArqs = Directory.GetFiles(Path.Combine(path_ArqPrev, "ECMWFop")).Count();

                    logF.WriteLine("Transferidos " + ECMWFOpGridArqs.ToString() + " arquivos !");

                    //var data_ecmwf_ext = ECMWF_Ext(cv2, Path.Combine(path_ArqPrev, "ECMWF"), -dias_ve + 13);
                    //gfs
                    logF.WriteLine("Tranferindo arquivos GFS para Entrada");

                    if (!Directory.Exists(Path.Combine(path_ArqPrev, "GFS"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "GFS"));

                    var GFS_NOAA_K = Path.Combine("K:\\cv_temp", data_Atual.ToString("yyyyMMdd"), "GFS00");

                    var GFS_FOLDER = Path.Combine(oneDrive_preco, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "GFS00", "txt");

                    if (Directory.Exists(GFS_NOAA_K))
                    {
                        var GFS_NOAA_Karqs = Directory.GetFiles(GFS_NOAA_K).Where(x => x.EndsWith(".dat"));
                        if (GFS_NOAA_Karqs.Count() > 14)
                        {
                            GFS_FOLDER = GFS_NOAA_K;
                            logF.WriteLine("Transferindo arquivos GFS via HEADNODE!");

                        }
                    }

                    if (Directory.Exists(GFS_FOLDER))
                    {
                        var GFS_NOAA = Directory.GetFiles(GFS_FOLDER).Where(x => x.EndsWith(".dat"));

                        if (GFS_NOAA.Count() > 14)// verifica de novo para o caso de não existir a pasta no HEADNODE e por tanto esta tentando usar do onedrive
                        {
                            foreach (var arq in GFS_NOAA)
                            {
                                File.Copy(arq, Path.Combine(path_ArqPrev, "GFS", arq.Split('\\').Last()), true);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("Arquivos GFS não encontrados");
                        }
                    }



                    //Descompactar o Zip Com dat


                    // System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(oneDrive, data_Atual.ToString("yyyy"), data_Atual.ToString("MM"), data_Atual.ToString("dd"), "GFS00", "txt.zip"), Path.Combine(path_ArqPrev, "GFS"));

                    //System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(path_Previsao, data_Atual.ToString("yyyyMM"), data_Atual.ToString("dd"), "GFSNOAA00", "txt.zip"), Path.Combine(path_ArqPrev, "GFS"));

                    var GFSs = Directory.GetFiles(Path.Combine(path_ArqPrev, "GFS"), "gfs_mp*");

                    foreach (var GFS in GFSs)
                    {
                        try
                        {
                            File.Delete(GFS);
                        }
                        catch
                        {

                        }
                    }

                    // Verifica Merge, caso não tenha usa o funceme
                    logF.WriteLine("Verifica Funceme/Merge/PsatPreliminar");
                    DateTime dt_func = data_Atual;
                    // while (dt_func != dt_acomph)
                    //{
                    var Merge = Directory.GetFiles(Path.Combine(path_ModeloR, "merge", dt_func.ToString("yyyyMM"), dt_func.ToString("dd"))).Where(x => x.EndsWith(".dat"));
                    //if (!Directory.Exists(Path.Combine(path_ArqPrev, "funceme"))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, "funceme"));
                    if (!Directory.Exists(Path.Combine(path_ArqPrev, funcemePsatPre))) Directory.CreateDirectory(Path.Combine(path_ArqPrev, funcemePsatPre));

                    if (temPsat)
                    {
                        if (merge == true)
                        {
                            logF.WriteLine("Transferindo MERGE+GEFS");
                        }
                        else
                        {
                            logF.WriteLine("Transferindo PsatPreliminar");
                        }

                        string psatArq = $"imerg+GEFS_p{data_Atual:ddMMyy}a{data_Atual:ddMMyy}.dat";
                        File.Copy(psatpre, Path.Combine(path_ArqPrev, funcemePsatPre, psatArq), true);
                    }
                    else if (Merge.Count() > 0)
                    {
                        logF.WriteLine("Transferindo Merge");

                        foreach (var arq in Merge)
                        {
                            File.Copy(arq, Path.Combine(path_ArqPrev, "Funceme", arq.Split('\\').Last().Replace("merge", "funceme")), true);
                        }
                    }
                    else
                    {
                        logF.WriteLine("Transferindo Funceme");

                        var Func = Directory.GetFiles(Path.Combine(path_ModeloR, "funceme", dt_func.ToString("yyyyMM"), dt_func.ToString("dd"))).Where(x => x.EndsWith(".dat"));
                        string funcArq = "";
                        if (Func == null || Func.Count() == 0)
                        {
                            logF.WriteLine("Arquivos psatPreliminar e funceme não encontrados");
                            throw new NotImplementedException("Arquivos psatPreliminar e funceme não encontrados");
                        }

                        if (Func.Any(x => x.Contains("LATE_funceme")))
                        {
                            funcArq = Func.Where(x => x.Contains("LATE_funceme")).First();
                            File.Copy(funcArq, Path.Combine(path_ArqPrev, "funceme", "funceme_" + funcArq.Split('\\').Last().Split('_').Last()), true);
                        }
                        else if (Func.Any(x => Path.GetFileName(x).StartsWith("funceme_")))
                        {
                            funcArq = Func.Where(x => Path.GetFileName(x).StartsWith("funceme_")).First();
                            File.Copy(funcArq, Path.Combine(path_ArqPrev, "funceme", funcArq.Split('\\').Last()), true);
                        }
                        else if (Func.Any(x => x.Contains("LATE_Inmet")))
                        {
                            funcArq = Func.Where(x => x.Contains("LATE_Inmet")).First();
                            File.Copy(funcArq, Path.Combine(path_ArqPrev, "funceme", "funceme_" + funcArq.Split('\\').Last().Split('_').Last()), true);
                        }
                        else //if(Func.Any(x => x.Contains("Inmet_funceme")))
                        {
                            funcArq = Func.Where(x => x.Contains("Inmet_funceme")).First();
                            File.Copy(funcArq, Path.Combine(path_ArqPrev, "funceme", "funceme_" + funcArq.Split('\\').Last().Split('_').Last()), true);
                        }
                        //foreach (var arq in Func)
                        //{
                        //    //File.Copy(arq, Path.Combine(path_ArqPrev, "funceme", arq.Split('\\').Last().Replace('p' + dt_func.ToString("ddMMyy"), dt_func.ToString("ddMMyy"))), true);

                        //    if (Func.Count() > 1 && arq.Contains("LATE"))
                        //    {
                        //        File.Copy(arq, Path.Combine(path_ArqPrev, "funceme", "funceme_" + arq.Split('\\').Last().Split('_').Last()), true);
                        //    }
                        //    else
                        //    {
                        //        File.Copy(arq, Path.Combine(path_ArqPrev, "funceme", arq.Split('\\').Last()), true);
                        //    }

                        //}
                    }
                    //   dt_func = dt_func.AddDays(-1);
                    //}



                    //Completa Historico Arq Entrada

                    // Hist_Entrada("ECMWF", path_Conj, path_Previsao, data_Atual);
                    // Hist_Entrada("ETA40", path_Conj, path_Previsao, data_Atual);

                    //transferECMWFmembrosShadow(path_Conj, "ECENS45m", "CVSMAP_ECENS45m");

                    try
                    {
                        //Tools.Tools.ManageOneDrive("stop");// para execução do onedrive para otimizar o uso da maquina nos processos de geração de mapas e rodadas 
                    }
                    catch { }


                    logF.WriteLine("Executando Script");

                    if (shadow == true)
                    {
                        executar_R(path_Conj, "formato_novo_shadow.r");
                    }
                    else
                    {
                        executar_R(path_Conj, "formato_novo.r");
                    }

                    executar_R(path_Conj, "ons.R Roda_Conjunto_V3.2.R");
                    // executar_R(path_Conj, "vies_ve_woutGEFS.R " + cv1.ToString("dd/MM/yy") + " " + cv2.ToString("dd/MM/yy"));
                    logF.WriteLine("Vies VE" + cv1.ToString("dd/MM/yy") + "   " + cv2.ToString("dd/MM/yy"));
                    if (runRev.rev == 0)
                    {
                        executar_R(path_Conj, "vies_ve.R " + cv1.ToString("dd/MM/yy") + " " + cv2.ToString("dd/MM/yy") + " " + cv3.ToString("dd/MM/yy") + " " + cv4.ToString("dd/MM/yy") + " " + cvx.ToString("dd/MM/yy"));
                    }
                    else
                    {
                        executar_R(path_Conj, "vies_ve.R " + cv1.ToString("dd/MM/yy") + " " + cv2.ToString("dd/MM/yy") + " " + cv3.ToString("dd/MM/yy") + " " + cv4.ToString("dd/MM/yy"));
                    }
                    executar_R(path_Conj, "madeira.r");

                    logF.WriteLine("Copiando arquivos ENS_Est_rv Clusters e probabilidades");
                    bool temECEN45 = transferECMWFmembros(path_Conj, "ECENS45m", "CVSMAP_ECENS45m",true);

                    if (temECEN45 == false)
                    {
                        logF.WriteLine("Arquivos ENS_Est_rv\\Clusters não encontrados");
                        throw new NotImplementedException("Arquivos ENS_Est_rv\\Clusters não encontrados");
                    }

                    //Organização das Rodada para rvx+1


                    logF.WriteLine("Criando Pastas RVX+1");

                    //  var path_ArqSaida = Path.Combine(path_Conj, "Arq_Saida");
                    var path_ArqSaida = Path.Combine(path_Conj, "madeira");


                    var vies_cv1 = Directory.GetFiles(Path.Combine(path_ArqSaida, "vies_" + cv1.ToString("dd-MM")));


                    Directory.CreateDirectory(Path.Combine(path_ArqSaida, "vies_" + cv2.ToString("dd-MM")));
                    var vies_cv2 = Directory.GetFiles(Path.Combine(path_ArqSaida, "vies_" + cv2.ToString("dd-MM")));

                    var vies_cv3 = Directory.GetFiles(Path.Combine(path_ArqSaida, "vies_" + cv3.ToString("dd-MM")));
                    var vies_cv4 = Directory.GetFiles(Path.Combine(path_ArqSaida, "vies_" + cv4.ToString("dd-MM")));



                    rvx1(path_Conj, "GEFS", "CV_VIES_VE", vies_cv1, vies_cv2);

                    logF.WriteLine("CV_VIES_VE Criada!");

                    rvx1(path_Conj, "GFS", "CV_GFS", vies_cv1, vies_cv2);

                    logF.WriteLine("CV_GFS Criada!");

                    rvx1(path_Conj, "ECMWF", "CV_EURO", vies_cv1, vies_cv2);

                    logF.WriteLine("CV_EURO Criada!");

                    rvx1(path_Conj, "ECMWFop", "CV_EUROop", vies_cv1, vies_cv2);

                    logF.WriteLine("CV_EURO_op Criada!");

                    if (runRev.rev == 0)
                    {
                        var vies_cvx = Directory.GetFiles(Path.Combine(path_ArqSaida, "vies_" + cvx.ToString("dd-MM")));

                        rvx1(path_Conj, "GEFS", "CV0_VIES_VE", vies_cvx, vies_cv2);

                        logF.WriteLine("CV0_VIES_VE Criada!");

                        rvx1(path_Conj, "ECMWF", "CV0_EURO", vies_cvx, vies_cv2);

                        logF.WriteLine("CV0_EURO Criada!");
                    }

                    //testes mapasa sem remoção EURO E GEFS
                    rvxPura(path_Conj, "GEFS", "CVPURO_PUROGEFS");
                    rvxPura(path_Conj, "ECMWF", "CVPURO_PUROECMWF");

                    //fim teste





                    //Organização das Rodada para rvx+2

                    logF.WriteLine("Criando Pastas RVX+2");

                    rvx2(path_Conj, "ECMWF", "CV2_EURO", vies_cv2);
                    //  MCP(cv2, Path.Combine(path_Conj, "CV2_EURO"), path_ModeloR);
                    logF.WriteLine("CV2_EURO Criada!");

                    rvx2(path_Conj, "ECMWFop", "CV2_EUROop", vies_cv2);
                    //  MCP(cv2, Path.Combine(path_Conj, "CV2_EUROop"), path_ModeloR);
                    logF.WriteLine("CV2_EURO_op Criada!");


                    rvx2(path_Conj, "GEFS", "CV2_GEFS", vies_cv2);
                    //   MCP(cv2, Path.Combine(path_Conj, "CV2_GEFS"), path_ModeloR);
                    logF.WriteLine("CV2_GEFS Criada!");

                    rvx2(path_Conj, "GFS", "CV2_GFS", vies_cv2);
                    //   MCP(cv2, Path.Combine(path_Conj, "CV2_GFS"), path_ModeloR);
                    logF.WriteLine("CV2_GFS Criada!");


                    rvxX(path_Conj, "GEFS", "CV3_GEFS", vies_cv3);
                    logF.WriteLine("CV3_GEFS Criada!");

                    rvxX(path_Conj, "ECMWF", "CV3_EURO", vies_cv3);
                    logF.WriteLine("CV3_ECMWF Criada!");

                    rvxX(path_Conj, "GEFS", "CV4_GEFS", vies_cv4);
                    logF.WriteLine("CV4_GEFS Criada!");

                    rvxX(path_Conj, "ECMWF", "CV4_EURO", vies_cv4);
                    logF.WriteLine("CV4_ECMWF Criada!");

                    //CV_FUNC 
                    //Remoção de vies a partir do dia atual, completando com MCP se necessário
                    logF.WriteLine("Criando Pasta CV_FUNC");

                    var arqs_PMEDIA = Directory.GetFiles(path_ArqSaida, "PM*.dat");
                    var cv_func = Path.Combine(path_Conj, "CV_FUNC");

                    Directory.CreateDirectory(cv_func);

                    foreach (var arq in arqs_PMEDIA)
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");

                        var data_mapa = r.Match(arq);

                        string mapa = data_mapa.ToString() + ".dat";


                        File.Copy(arq, Path.Combine(cv_func, mapa), true);
                    }
                    MCP_FUNC(cv1.AddDays(-(dias_ve + 1)), Path.Combine(path_Conj, "CV_FUNC"), path_ModeloR);
                    logF.WriteLine("CV_FUNC Criada!");


                    // tetes euro45m smapext
                    if (temECEN45 == true)
                    {

                        logF.WriteLine("Tranferindo arquivos ECENS45_Membros para Entrada");

                        //rvxSmapEXT(path_Conj, "ECENS45m", "CVSMAP_ECENS45m");
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_FUNC", "CV_FUNC", "ECENS45m", cv1);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_GEFS", "CV_VIES_VE", "ECENS45m", cv1);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_GFS", "CV_GFS", "ECENS45m", cv1);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_EURO", "CV_EURO", "ECENS45m", cv1);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_EUROop", "CV_EUROop", "ECENS45m", cv1);

                        if (runRev.rev == 0)
                        {
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP0_GEFS", "CV0_VIES_VE", "ECENS45m", cvx);
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP0_EURO", "CV0_EURO", "ECENS45m", cvx);
                        }


                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP2_EURO", "CV2_EURO", "ECENS45m", cv2);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP2_EUROop", "CV2_EUROop", "ECENS45m", cv2);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP2_GEFS", "CV2_GEFS", "ECENS45m", cv2);
                        rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP2_GFS", "CV2_GFS", "ECENS45m", cv2);

                       // if (runRev4.rev == 0)
                       // {
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP3_GEFS", "CV3_GEFS", "ECENS45m", cv3, true);
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP3_EURO", "CV3_EURO", "ECENS45m", cv3, true);
                       // }
                        //else
                        //{
                        //    rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP3_GEFS", "CV3_GEFS", "ECENS45m", cv3);
                        //    rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP3_EURO", "CV3_EURO", "ECENS45m", cv3);
                        //}

                       // if (runRev5.rev == 0 || runRev5.rev == 1)
                       // {
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP4_GEFS", "CV4_GEFS", "ECENS45m", cv4, true);
                            rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP4_EURO", "CV4_EURO", "ECENS45m", cv4, true);
                       // }
                        //else
                        //{
                        //    rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP4_GEFS", "CV4_GEFS", "ECENS45m", cv4);
                        //    rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP4_EURO", "CV4_EURO", "ECENS45m", cv4);
                        //}
                        

                    }

                    // fim teste


                    //Completa com Funceme ou psatpreliminar se não tiver acomph referente a data

                    if (data_Atual != dt_acomph)
                    {
                        logF.WriteLine("Acomph desatualizado, renoamendo arquivos");
                        var dirs = Directory.GetDirectories(path_Conj).Where(x => x.Split('\\').Last().StartsWith("CV"));
                        //var arq_funceme = Directory.GetFiles(Path.Combine(path_ArqSaida, "funceme"));
                        var arq_funceme = Directory.GetFiles(Path.Combine(path_ArqSaida, funcemePsatPre));

                        foreach (var arq in arq_funceme)//ajusta as datas dos dats para as rodadas pré acomph
                        {
                            foreach (var dir in dirs)
                            {

                                File.Copy(arq, Path.Combine(dir, arq.Split('\\').Last()), true);
                                Atualiza_DT(dir, dt_acomph);
                            }
                        }

                    }

                    if (data_Atual.DayOfWeek == DayOfWeek.Friday)
                    {
                        var count_mapas = Directory.GetFiles(Path.Combine(path_Conj, "CV_EURO")).Count();
                        if (count_mapas < 15) MCP_rv1(dt_acomph, Path.Combine(path_Conj, "CV_VIES_VE"), path_ModeloR, true);
                        if (count_mapas < 15) MCP_rv1(dt_acomph, Path.Combine(path_Conj, "CV_GFS"), path_ModeloR, true);
                        MCP_rv1(dt_acomph, Path.Combine(path_Conj, "CV_EURO"), path_ModeloR);
                        MCP_rv1(dt_acomph, Path.Combine(path_Conj, "CV_EUROop"), path_ModeloR);
                        MCP_rv1(dt_acomph, Path.Combine(path_Conj, "CV_FUNC"), path_ModeloR);
                    }

                    var dirs_cvs = Directory.GetDirectories(path_Conj).Where(x => x.Split('\\').Last().StartsWith("CV"));

                    foreach (var dir in dirs_cvs)
                    {
                        var name_cv = dir.Split('\\').Last().Split('_').First();

                        if (!Directory.Exists(Path.Combine(path_Conj, name_cv)))
                        {
                            Directory.CreateDirectory(Path.Combine(path_Conj, name_cv));
                        }

                        DirectoryCopy(dir, Path.Combine(path_Conj, name_cv, dir.Split('\\').Last()), true);
                        Directory.Delete(dir, true);


                    }
                    var email = Tools.Tools.SendMail("", $"Mapas Gerados com Sucesso!{DateTime.Now: dd/MM/yyyy HH:mm:ss}", "SUCESSO AO GERAR MAPAS", "desenv");
                    email.Wait(30000);
                    logF.WriteLine("Mapas Gerados com Sucesso!");




                }



            }
            catch (Exception a)
            {
                var log_C = Path.Combine(path_Conj, "error.log");


                File.WriteAllText(log_C, a.ToString());
                var email = Tools.Tools.SendMail("", a.ToString(), "ERRO AO GERAR MAPAS", "desenv");
                email.Wait(30000);
                logF.WriteLine("Erro ao Gerar Mapas");
            }


        }

        internal static void Hist_Entrada(string modelo, string path_Conj, string path_Previsao, DateTime data_Atual)
        {
            //Completa Historico Arq Entrada
            var arq_Ent_EURO = Directory.GetFiles(Path.Combine(path_Conj, "Arq_Entrada", modelo));

            var data_hist = data_Atual;
            var File_modelo = modelo + "_m_" + data_hist.ToString("ddMMyy") + ".dat";
            var Path_modelo = Path.Combine(path_Conj, "Arq_Entrada", modelo, File_modelo);

            while (!File.Exists(Path_modelo))
            {
                var arq_Dia_euro = Path.Combine(path_Previsao, data_hist.ToString("yyyyMM"), data_hist.ToString("dd"), File_modelo);
                if (File.Exists(arq_Dia_euro)) File.Copy(arq_Dia_euro, Path_modelo);

                data_hist = data_hist.AddDays(-1);
                File_modelo = modelo + "_m_" + data_hist.ToString("ddMMyy") + ".dat";
                Path_modelo = Path.Combine(path_Conj, "Arq_Entrada", modelo, File_modelo);
            }

        }

        internal static void Atualiza_DT(string dir, DateTime dt_acomph)
        {


            var arqs = Directory.GetFiles(dir);

            foreach (var arq in arqs)
            {
                var nome = arq.Split('\\').Last();

                var fim_nome = nome.Split('.').First().Split('a').Last();

                var nome_Final = "p" + dt_acomph.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                if (File.Exists(Path.Combine(dir, nome_Final)))
                {
                    File.Delete(Path.Combine(dir, nome_Final));
                }

                File.Move(arq, Path.Combine(dir, nome_Final));

            }
        }

        internal static void rvxPura(string path_Conj, string modelo, string nome_path)
        {

            var path_cv = Path.Combine(path_Conj, nome_path);
            //    var path_ArqSaida = Path.Combine(path_Conj, "Arq_Saida");
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");
            Directory.CreateDirectory(path_cv);
            var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo));
            //var Modelo1 = out_Modelo.Where(File => !vies_cv1.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));
            //var Modelo2 = Modelo1.Where(File => !vies_cv2.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

            //DateTime data_final = DateTime.Today.AddDays(-1);
            //foreach (var arq_CV in vies_cv1)
            //{
            //    File.Copy(arq_CV, Path.Combine(path_cv, arq_CV.Split('\\').Last()), true);
            //    var data_arq = DateTime.ParseExact(arq_CV.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
            //    if (data_arq >= data_final)
            //    {
            //        data_final = data_arq;
            //    }
            //}

            foreach (var arq in out_Modelo)
            {
                //var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                //if (data_arq <= data_final)
                //{
                File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);
                //}
            }


        }

        internal static bool transferECMWFmembros(string path_Conj, string modelo, string nome_path, bool usarCFS = false)
        {
            DateTime data = DateTime.Today;
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");

            string sitemasFolder = $@"C:\Sistemas\ChuvaVazao";
            string ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Clusters";
            string ecenProbDat = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Arq_Saida\ECMWF\Clust\prob.dat";
            int contagem = 0;

            DateTime dataCFS = DateTime.Today;
            string cfsFolderK = Path.Combine("K:\\cv_temp", dataCFS.ToString("yyyyMMdd"), "cfs");
            int contCfs = 0;
            List<string> out_CFS = new List<string>();

            while (!Directory.Exists(cfsFolderK) && contCfs < 4)//procura diretorio até 4 dias atras
            {
                dataCFS = dataCFS.AddDays(-1);
                cfsFolderK = Path.Combine("K:\\cv_temp", dataCFS.ToString("yyyyMMdd"), "cfs");

                contCfs++;
            }

            if (Directory.Exists(cfsFolderK))
            {
                out_CFS = Directory.GetFiles(cfsFolderK).OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture)).ToList();
            }

            try
            {
                while (!Directory.Exists(ecenFolder) && !File.Exists(ecenProbDat) && contagem < 16)//procura diretorio até 16 dias atras
                {
                    data = data.AddDays(-1);
                    ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Clusters";
                    ecenProbDat = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Arq_Saida\ECMWF\Clust\prob.dat";

                    contagem++;
                }

                if (contagem >= 16)
                {
                    return false;
                }
                //var path_ArqSaida = @"C:\Files";//

                //var out_ModeloFolder = Path.Combine(path_ArqSaida, modelo);

                if (Directory.Exists(ecenFolder) && File.Exists(ecenProbDat))
                {
                    File.WriteAllText(Path.Combine(path_Conj, "data.txt"), "ECMWF_CLUSTER:" + data.ToString("dd/MM/yyyy"));
                    for (int i = 0; i <= 9; i++)
                    {
                        //nome_path = nome_path + 1.ToString("00");
                        string search = modelo + i.ToString("00");
                        var path_cv = Path.Combine(path_Conj, "madeira", search);
                        string clustFolder = Path.Combine(ecenFolder, "c" + i.ToString("00"));
                        Directory.CreateDirectory(path_cv);

                        //var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo), search + "*").OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));
                        var out_Modelo = Directory.GetFiles(clustFolder).OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));

                        DateTime data_final = DateTime.Today;

                        string lastfile = "";
                        foreach (var arq in out_Modelo)
                        {
                            var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                            if (data_arq > data_final)
                            {
                                var nome = arq.Split('\\').Last();

                                var fim_nome = nome.Split('.').First().Split('a').Last();

                                var nome_Final = "p" + data_final.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                                File.Copy(arq, Path.Combine(path_cv, nome_Final), true);
                                lastfile = Path.Combine(path_cv, nome_Final);
                            }
                        }
                        int cont = Directory.GetFiles(path_cv).Count();

                        if (Directory.Exists(cfsFolderK) && out_CFS.Count() > 45 && usarCFS == true)
                        {

                            foreach (var cfs in out_CFS)
                            {
                                var dtCfs = DateTime.ParseExact(cfs.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);

                                if (dtCfs > data_final)
                                {
                                    var nome = cfs.Split('\\').Last();
                                    var fim_nome = nome.Split('.').First().Split('a').Last();
                                    var nome_Final = "p" + data_final.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                                    string arquivoFinal = Path.Combine(path_cv, nome_Final);

                                    if (!File.Exists(arquivoFinal))//copia apenas os arquivos que ainda faltam para completar os 55 dias 
                                    {
                                        File.Copy(cfs, arquivoFinal, true);
                                    }

                                    cont = Directory.GetFiles(path_cv).Count();
                                    if (cont == 55)
                                    {
                                        break;
                                    }
                                }


                            }

                        }
                        else
                        {
                            while (cont < 55 && lastfile != "")
                            {
                                string MediaMERGE = @"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\Modelo_R\merge\avg";

                                var data_arq = DateTime.ParseExact(lastfile.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);

                                string MergeFile = Path.Combine(MediaMERGE, "merge_mean_" + data_arq.Month.ToString("00") + ".dat");

                                var nome = lastfile.Split('\\').Last();

                                var fim_nome = nome.Split('.').First().Split('a').Last();

                                var nome_Final = "p" + data_final.ToString("ddMMyy") + "a" + data_arq.AddDays(1).ToString("ddMMyy") + ".dat";
                                File.Copy(MergeFile, Path.Combine(path_cv, nome_Final), true);
                                lastfile = Path.Combine(path_cv, nome_Final);

                                cont = Directory.GetFiles(path_cv).Count();
                            }
                        }

                    }

                    File.Copy(ecenProbDat, Path.Combine(path_Conj, "prob.dat"), true);
                    File.Copy(ecenProbDat, Path.Combine(sitemasFolder, "prob.dat"), true);

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {

                return false;
            }


        }

        internal static bool transferECMWFmembrosShadow(string path_Conj, string modelo, string nome_path)
        {
            DateTime data = DateTime.Today;
            var path_ArqSaida = Path.Combine(path_Conj, "grid");

            string sitemasFolder = $@"C:\Sistemas\ChuvaVazao";
            string ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Clusters";
            string ecenProbDat = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Arq_Saida\ECMWF\Clust\prob.dat";
            int contagem = 0;
            try
            {
                while (!Directory.Exists(ecenFolder) && !File.Exists(ecenProbDat) && contagem < 16)//procura diretorio até 16 dias atras
                {
                    data = data.AddDays(-1);
                    ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Clusters";
                    ecenProbDat = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ENS_Est_rv\Arq_Saida\ECMWF\Clust\prob.dat";

                    contagem++;
                }

                if (contagem >= 16)
                {
                    return false;
                }
                //var path_ArqSaida = @"C:\Files";//

                //var out_ModeloFolder = Path.Combine(path_ArqSaida, modelo);

                if (Directory.Exists(ecenFolder) && File.Exists(ecenProbDat))
                {
                    File.WriteAllText(Path.Combine(path_Conj, "data.txt"), "ECMWF_CLUSTER:" + data.ToString("dd/MM/yyyy"));
                    for (int i = 0; i <= 9; i++)
                    {
                        //nome_path = nome_path + 1.ToString("00");
                        string search = modelo + i.ToString("00");
                        var path_cv = Path.Combine(path_Conj, "grid", search);
                        string clustFolder = Path.Combine(ecenFolder, "c" + i.ToString("00"));
                        Directory.CreateDirectory(path_cv);

                        //var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo), search + "*").OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));
                        var out_Modelo = Directory.GetFiles(clustFolder).OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));

                        DateTime data_final = DateTime.Today;

                        string lastfile = "";
                        foreach (var arq in out_Modelo)
                        {
                            var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                            if (data_arq > data_final)
                            {
                                var nome = arq.Split('\\').Last();

                                var fim_nome = nome.Split('.').First().Split('a').Last();

                                var nome_Final = search + "_p" + data_final.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                                File.Copy(arq, Path.Combine(path_cv, nome_Final), true);
                                lastfile = Path.Combine(path_cv, nome_Final);
                            }
                        }
                        int cont = Directory.GetFiles(path_cv).Count();
                        while (cont < 55 && lastfile != "")
                        {
                            string MediaMERGE = @"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\Modelo_R\merge\avg";

                            var data_arq = DateTime.ParseExact(lastfile.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);

                            string MergeFile = Path.Combine(MediaMERGE, "merge_mean_" + data_arq.Month.ToString("00") + ".dat");

                            var nome = lastfile.Split('\\').Last();

                            var fim_nome = nome.Split('.').First().Split('a').Last();

                            var nome_Final = search + "_p" + data_final.ToString("ddMMyy") + "a" + data_arq.AddDays(1).ToString("ddMMyy") + ".dat";
                            File.Copy(MergeFile, Path.Combine(path_cv, nome_Final), true);
                            lastfile = Path.Combine(path_cv, nome_Final);

                            cont = Directory.GetFiles(path_cv).Count();
                        }
                    }

                    File.Copy(ecenProbDat, Path.Combine(path_Conj, "prob.dat"), true);
                    File.Copy(ecenProbDat, Path.Combine(sitemasFolder, "prob.dat"), true);

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {

                return false;
            }


        }

        internal static bool transferECMWFmembrosAntigo(string path_Conj, string modelo, string nome_path)
        {
            DateTime data = DateTime.Today;
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");

            string ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ECENS45m_precip_{data:yyyyMMdd}T00";
            int contagem = 0;
            try
            {
                while (!Directory.Exists(ecenFolder) && contagem < 5)//procura diretorio até 5 dias atras
                {
                    data = data.AddDays(-1);
                    ecenFolder = $@"H:\Middle - Preço\Acompanhamento de Precipitação\Previsao_Numerica\{data:yyyyMM}\{data:dd}\ECENS45m_precip_{data:yyyyMMdd}T00";
                    contagem++;
                }

                if (contagem >= 5)
                {
                    return false;
                }
                //var path_ArqSaida = @"C:\Files";//

                //var out_ModeloFolder = Path.Combine(path_ArqSaida, modelo);

                if (Directory.Exists(ecenFolder))
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        //nome_path = nome_path + 1.ToString("00");
                        string search = modelo + i.ToString("00");
                        var path_cv = Path.Combine(path_Conj, "grid", search);

                        Directory.CreateDirectory(path_cv);

                        //var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo), search + "*").OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));
                        var out_Modelo = Directory.GetFiles(ecenFolder, search + "*").OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture));

                        DateTime data_final = DateTime.Today;

                        string lastfile = "";
                        foreach (var arq in out_Modelo)
                        {
                            var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                            if (data_arq > data_final)
                            {
                                var nome = arq.Split('\\').Last();

                                var fim_nome = nome.Split('.').First().Split('a').Last();

                                var nome_Final = search + "_p" + data_final.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                                File.Copy(arq, Path.Combine(path_cv, nome_Final), true);
                                lastfile = Path.Combine(path_cv, nome_Final);
                            }
                        }
                        int cont = Directory.GetFiles(path_cv).Count();
                        while (cont < 45 && lastfile != "")
                        {
                            var data_arq = DateTime.ParseExact(lastfile.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);

                            var nome = lastfile.Split('\\').Last();

                            var fim_nome = nome.Split('.').First().Split('a').Last();

                            var nome_Final = search + "_p" + data_final.ToString("ddMMyy") + "a" + data_arq.AddDays(1).ToString("ddMMyy") + ".dat";
                            File.Copy(lastfile, Path.Combine(path_cv, nome_Final), true);
                            lastfile = Path.Combine(path_cv, nome_Final);

                            cont = Directory.GetFiles(path_cv).Count();
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {

                return false;
            }


        }

        internal static void rvxSmapEXT(string path_Conj, string modelo, string nome_path)
        {

            var path_ArqSaida = Path.Combine(path_Conj, "madeira");

            for (int i = 0; i <= 9; i++)
            {
                string search = modelo + i.ToString("00");

                var out_ModeloFolder = Path.Combine(path_ArqSaida, search);
                //Directory.CreateDirectory(path_cv);
                if (Directory.Exists(out_ModeloFolder))
                {
                    string nome_pathAlt = nome_path + i.ToString("00");
                    var path_cv = Path.Combine(path_Conj, nome_pathAlt);

                    Directory.CreateDirectory(path_cv);

                    var out_Modelo = Directory.GetFiles(out_ModeloFolder);

                    foreach (var arq in out_Modelo)
                    {

                        File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);

                    }


                }
            }
        }

        internal static void rvxSmapExtByModel(string path_Conj, string modelo, string nome_path, string modeloBase, string clusterName, DateTime iniVE, bool usarCFS = false)
        {                                           //raiz       // ECENS45m      //CVSMAP1_FUNC        //CV_FUNC    //FUNC_ECENS45m       
            //rvxSmapEXT(path_Conj, "ECENS45m", "CVSMAP_ECENS45m");
            //rvxSmapExtByModel(path_Conj, "ECENS45m", "CVSMAP1_FUNC","CV_FUNC","FUNC_ENCENS45m");
            var path_cvBase = Path.Combine(path_Conj, modeloBase);
            var outModeloBase = Directory.GetFiles(path_cvBase);
            //string smapFolder = Path.Combine(path_Conj, nome_path);
            DateTime fimVE = iniVE.AddDays(14);

            var path_ArqSaida = Path.Combine(path_Conj, "madeira");
            //Directory.CreateDirectory(smapFolder);

            //string previvazClust = Path.Combine(smapFolder, clusterName + "PREVIVAZ");
            string previvazClust = Path.Combine(path_Conj, nome_path + clusterName + "10");
            Directory.CreateDirectory(previvazClust);

            foreach (var arq in outModeloBase)// 
            {
                var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);


                File.Copy(arq, Path.Combine(previvazClust, arq.Split('\\').Last()), true);



            }

            for (int i = 0; i <= 9; i++)
            {
                string search = modelo + i.ToString("00");

                var out_ModeloFolder = Path.Combine(path_ArqSaida, search);
                //Directory.CreateDirectory(path_cv);
                if (Directory.Exists(out_ModeloFolder))
                {
                    //string nome_pathAlt = clusterName + i.ToString("00");
                    string nome_pathAlt = nome_path + clusterName + i.ToString("00");
                    // var path_cv = Path.Combine(smapFolder, nome_pathAlt);
                    var path_cv = Path.Combine(path_Conj, nome_pathAlt);

                    Directory.CreateDirectory(path_cv);

                    var out_Modelo = Directory.GetFiles(out_ModeloFolder);

                    foreach (var arq in out_Modelo)
                    {

                        File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);

                    }

                    foreach (var arq in outModeloBase)// substitui os dias referentes aos modelos e remoção de vies
                    {
                        var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (data_arq <= fimVE)
                        {
                            File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);

                        }

                    }

                    //todo: copiar arquivos CFS somente dos dias restantes para completar a partir do ultimo dia dos clusters , criar varival para rastrear e saber de qual dia deve começar a copiar
                    //essa função sera usada somente em casos que a cv3 e cv4 forem rv0 e rv1

                    if (usarCFS == true)
                    {
                        DateTime dataCFS = DateTime.Today;
                        DateTime data_final = DateTime.Today;

                        string cfsFolderK = Path.Combine("K:\\cv_temp", dataCFS.ToString("yyyyMMdd"), "cfs");
                        int contCfs = 0;
                        List<string> out_CFS = new List<string>();

                        while (!Directory.Exists(cfsFolderK) && contCfs < 4)//procura diretorio até 4 dias atras
                        {
                            dataCFS = dataCFS.AddDays(-1);
                            cfsFolderK = Path.Combine("K:\\cv_temp", dataCFS.ToString("yyyyMMdd"), "cfs");

                            contCfs++;
                        }

                        if (Directory.Exists(cfsFolderK))
                        {
                            out_CFS = Directory.GetFiles(cfsFolderK).OrderBy(x => DateTime.ParseExact(x.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture)).ToList();
                        }

                        if (Directory.Exists(cfsFolderK) && out_CFS.Count() > 45)
                        {

                            foreach (var cfs in out_CFS)
                            {
                                var dtCfs = DateTime.ParseExact(cfs.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);

                                if (dtCfs > data_final)
                                {
                                    var nome = cfs.Split('\\').Last();
                                    var fim_nome = nome.Split('.').First().Split('a').Last();
                                    var nome_Final = "p" + data_final.ToString("ddMMyy") + "a" + fim_nome + ".dat";
                                    string arquivoFinal = Path.Combine(path_cv, nome_Final);

                                    if (!File.Exists(arquivoFinal))//copia apenas os arquivos que ainda faltam para completar todos os dias disponiveis 
                                    {
                                        File.Copy(cfs, arquivoFinal, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void rvx1(string path_Conj, string modelo, string nome_path, string[] vies_cv1, string[] vies_cv2)
        {

            var path_cv = Path.Combine(path_Conj, nome_path);
            //    var path_ArqSaida = Path.Combine(path_Conj, "Arq_Saida");
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");
            Directory.CreateDirectory(path_cv);
            var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo));
            var Modelo1 = out_Modelo.Where(File => !vies_cv1.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));
            var Modelo2 = Modelo1.Where(File => !vies_cv2.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

            DateTime data_final = DateTime.Today.AddDays(-1);
            foreach (var arq_CV in vies_cv1)
            {
                File.Copy(arq_CV, Path.Combine(path_cv, arq_CV.Split('\\').Last()), true);
                var data_arq = DateTime.ParseExact(arq_CV.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq >= data_final)
                {
                    data_final = data_arq;
                }
            }

            foreach (var arq in Modelo2)
            {
                var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq <= data_final)
                {
                    File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);
                }
            }


        }

        internal static void rvx2(string path_Conj, string modelo, string nome_path, string[] vies_cv2)
        {

            var path_cv = Path.Combine(path_Conj, nome_path);
            //  var path_ArqSaida = Path.Combine(path_Conj, "Arq_Saida");
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");
            Directory.CreateDirectory(path_cv);



            var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo));

            var Modelo2 = out_Modelo.Where(File => !vies_cv2.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

            DateTime data_final = DateTime.Today.AddDays(-1);

            foreach (var arq_CV in vies_cv2)
            {
                File.Copy(arq_CV, Path.Combine(path_cv, arq_CV.Split('\\').Last()), true);
                var data_arq = DateTime.ParseExact(arq_CV.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq >= data_final)
                {
                    data_final = data_arq;
                }
            }


            foreach (var arq in Modelo2)
            {
                var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq <= data_final)
                {
                    File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);
                }
            }

            if (modelo == "ECMWFop")
            {
                var arqs_ONS = Directory.GetFiles(Path.Combine(path_ArqSaida, "ECMWF"));

                var Modelo3 = arqs_ONS.Where(File => !vies_cv2.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));
                var arqs_GEFS_EURO = Modelo3.Where(File => !Modelo2.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

                foreach (var arq_Euro in arqs_GEFS_EURO)
                {
                    var data_arq = DateTime.ParseExact(arq_Euro.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (data_arq <= data_final)
                    {
                        File.Copy(arq_Euro, Path.Combine(path_cv, arq_Euro.Split('\\').Last()), true);
                    }
                }
            }



        }

        internal static void rvxX(string path_Conj, string modelo, string nome_path, string[] vies_cv)
        {

            var path_cv = Path.Combine(path_Conj, nome_path);
            //  var path_ArqSaida = Path.Combine(path_Conj, "Arq_Saida");
            var path_ArqSaida = Path.Combine(path_Conj, "madeira");
            Directory.CreateDirectory(path_cv);



            var out_Modelo = Directory.GetFiles(Path.Combine(path_ArqSaida, modelo));

            var Modelo2 = out_Modelo.Where(File => !vies_cv.Any(x => File.EndsWith(x.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)));

            DateTime data_final = DateTime.Today.AddDays(-1);
            foreach (var arq_CV in vies_cv)
            {
                File.Copy(arq_CV, Path.Combine(path_cv, arq_CV.Split('\\').Last()), true);

                var data_arq = DateTime.ParseExact(arq_CV.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq >= data_final)
                {
                    data_final = data_arq;
                }
            }

            foreach (var arq in Modelo2)
            {
                var data_arq = DateTime.ParseExact(arq.Split('\\').Last().Split('.').First().Split('a').Last(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                if (data_arq <= data_final)
                {
                    File.Copy(arq, Path.Combine(path_cv, arq.Split('\\').Last()), true);
                }
            }




        }

        internal static void GEFS_Ext(DateTime cv, string path)
        {
            var dt = DateTime.Today.AddDays(-1);
            // var oneDrivePath_ori = Environment.GetEnvironmentVariable("OneDriveCommercial");
            var oneDrivePath_ori = @"C:\Enercore\Energy Core Trading";
            //B:\Compass\MinhaTI\Alex Freires Marques - Compass\Trading
            var oneDrive = Path.Combine(oneDrivePath_ori, @"Energy Core Pricing - Documents\Acompanhamento_de_Precipitacao\Previsao\");
            if (!Directory.Exists(oneDrive))
            {
                oneDrive = oneDrive.Replace("Energy Core Pricing - Documents", "Energy Core Pricing - Documentos");
            }

            var oneDrive_gefs = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "GEFS_0.5_00");
            while (!Directory.Exists(oneDrive_gefs))
            {
                dt = dt.AddDays(-1);
                oneDrive_gefs = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "GEFS_0.5_00");
            }
            if (Directory.Exists(oneDrive_gefs))
            {
                var files_gefs = Directory.GetFiles(oneDrive_gefs);
                while (files_gefs.Count() < 30)
                {
                    dt = dt.AddDays(-1);
                    oneDrive_gefs = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "GEFS_0.5_00");
                    files_gefs = Directory.GetFiles(oneDrive_gefs);
                }

                var arqs = Directory.GetFiles(path);
                //for (int i = 0; i <= dias; i++)
                for (int i = 0; i <= files_gefs.Count(); i++)
                {
                    var data = DateTime.Today.AddDays(i + 1);
                    if (!File.Exists(Path.Combine(path, "GEFS_p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")))
                    {
                        var file_gefs = files_gefs.Where(x => x.Contains(data.ToString("ddMMyy") + ".dat")).FirstOrDefault();
                        try
                        {
                            File.Copy(file_gefs, Path.Combine(path, "GEFS_p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"));
                        }
                        catch { }
                        //File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                    }
                }
            }
        }
        internal static void MCP(DateTime cv, string path, string Modelo_R)
        {
            var arqs = Directory.GetFiles(path);
            for (int i = 1; i <= 12; i++)
            {
                var data = cv.AddDays(i);
                if (!File.Exists(Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")))
                {
                    File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                }
            }
        }

        internal static void MCP_rv1(DateTime dt, string path, string Modelo_R, Boolean Gfs = false)
        {
            var arqs = Directory.GetFiles(path);
            var dias = arqs.Count();
            if (dias < 15)
            {
                for (int i = 1; i <= 15; i++)
                {
                    var data = dt.AddDays(i);
                    if (!File.Exists(Path.Combine(path, "p" + dt.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")))
                    {
                        File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + dt.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                    }
                }
            }
            else if (Gfs)
            {

                var data = DateTime.Today.AddDays(dias - 1);

                var last_file = Path.Combine(path, "p" + dt.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat");
                if (File.Exists(last_file))
                {
                    File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + dt.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                }

            }
        }

        internal static void MCP_FUNC(DateTime cv, string path, string Modelo_R)
        {
            var arqs = Directory.GetFiles(path);
            for (int i = 1; i <= 18; i++)
            {
                var data = cv.AddDays(i);
                if (!File.Exists(Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")) && !File.Exists(Path.Combine(path, "PMEDIA_p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")))
                {
                    File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                }
            }
        }

        internal static DateTime ECMWF_Ext(DateTime cv, string path, int dias = 14)
        {
            var dt = DateTime.Today;
            var data_final = DateTime.Today;
            //var oneDrivePath_ori = Environment.GetEnvironmentVariable("OneDriveCommercial");
            var oneDrivePath_ori = @"C:\Enercore\Energy Core Trading";
            //B:\Compass\MinhaTI\Alex Freires Marques - Compass\Trading
            var oneDrive = Path.Combine(oneDrivePath_ori, @"Energy Core Pricing - Documents\Acompanhamento_de_Precipitacao\Previsao\");
            if (!Directory.Exists(oneDrive))
            {
                oneDrive = oneDrive.Replace("Energy Core Pricing - Documents", "Energy Core Pricing - Documentos");
            }
            var oneDrive_ecmwf = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "ECMWF45");
            while (!Directory.Exists(oneDrive_ecmwf))
            {
                dt = dt.AddDays(-1);
                oneDrive_ecmwf = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "ECMWF45");
            }
            if (Directory.Exists(oneDrive_ecmwf))
            {
                var files_ecmwf = Directory.GetFiles(oneDrive_ecmwf);
                while (files_ecmwf.Count() < 30)
                {
                    dt = dt.AddDays(-1);
                    oneDrive_ecmwf = Path.Combine(oneDrive, dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), "ECMWF45");
                    files_ecmwf = Directory.GetFiles(oneDrive_ecmwf);
                }

                var arqs = Directory.GetFiles(path);
                //for (int i = 0; i <= dias; i++)
                for (int i = 0; i <= files_ecmwf.Count(); i++)
                {



                    var data = DateTime.Today.AddDays(i + 1);
                    if (!File.Exists(Path.Combine(path, "ECMWF_p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat")))
                    {
                        var file_gefs = files_ecmwf.Where(x => x.Contains(data.ToString("ddMMyy") + ".dat")).FirstOrDefault();
                        try
                        {
                            if (data >= data_final)
                            {
                                data_final = data;
                            }
                            File.Copy(file_gefs, Path.Combine(path, "ECMWF_p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"));
                        }
                        catch { }
                        //File.Copy(Path.Combine(Modelo_R, "MCP", "prec_mct1318_" + data.Month.ToString().PadLeft(2, '0') + ".dat"), Path.Combine(path, "p" + DateTime.Today.ToString("ddMMyy") + "a" + data.ToString("ddMMyy") + ".dat"), true);

                    }
                }
            }
            return data_final;
        }



        static void executar_R(string path, string Comando)
        {

            string argumentos = "";
            var coms = Comando.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (coms.Count() > 1)
            {
                foreach (var cm in coms)
                {
                    if (cm == coms[0])
                    {
                        continue;
                    }
                    argumentos += " " + cm;
                }
            }
            //string executar = @"/C " + "H: & cd " + txtCaminho.Text + "& bat.bat";


            //string executar = @"/c " + "N: & cd Middle - Preço\\16_Chuva_Vazao\\Conjunto-PastasEArquivos/ & bat.bat";


            var letra_Dir = path.Split('\\').First();
            var path_Scripts = @"H:\TI - Sistemas\UAT\ChuvaVazao\remocao_R\scripts\";
            //var path_Scripts = @"H:\TI - Sistemas\UAT\ChuvaVazao\remocao_R\scripts\testeBruno\";
            //string executar = @"/C " + letra_Dir + " & cd " + path + @" & Rscript.exe " + path_Scripts + Comando;
            string executar = @"/C " + @"Rscript.exe " + "\"" + path_Scripts + coms[0] + "\"" + argumentos;
            System.Diagnostics.Process pr = new System.Diagnostics.Process();

            var prInfo = new System.Diagnostics.ProcessStartInfo();
            prInfo.FileName = @"C:\Windows\System32\cmd.exe";
            prInfo.UseShellExecute = true;
            prInfo.WorkingDirectory = path;
            prInfo.Arguments = executar;
            pr.StartInfo = prInfo;
            pr.Start();
            pr.WaitForExit();

            // System.Diagnostics.Process.Start("cmd.exe", executar).WaitForExit(1200000);



        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }





    }


}




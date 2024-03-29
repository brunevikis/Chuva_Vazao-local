﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GradsHelper;

namespace ChuvaVazaoTools
{
    public class cptec
    {
        public static async Task<string> DownloadETA40Async(DateTime dt, System.IO.TextWriter log = null)
        {
            return await Task.Factory.StartNew(() => DownloadETA40Gambiarra(dt, log));
        }

        public static Precipitacao DownloadFunceme()
        {
            try
            {
                System.Net.WebClient c = new System.Net.WebClient();
                var dataStr = c.DownloadString("http://mobile.funceme.br/tempo/inmet.php?acao=4&sensor=22&intervalo=24");
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(dataStr) as Newtonsoft.Json.Linq.JArray;

                Dictionary<Tuple<decimal, decimal>, float> data = new Dictionary<Tuple<decimal, decimal>, float>();

                foreach (Newtonsoft.Json.Linq.JToken item in obj)
                {

                    if (item.Value<float?>("valor").HasValue)
                    {
                        data.Add(
                        new Tuple<decimal, decimal>(Convert.ToDecimal(item["lat"]), Convert.ToDecimal(item["lon"])),
                        Convert.ToSingle(item["valor"]));
                    }

                }

                return PrecipitacaoFactory.BuildFromJsonData(data, DateTime.Today);

            }
            catch { return null; }



        }

        public static string DownloadGFSNoaa2(DateTime dt, System.IO.TextWriter log = null, string modelo = "GFSB", string horasToDownload = "00")
        {

            var message = new StringBuilder();

            var localPath = System.IO.Path.GetTempPath() + modelo + "\\";

            var baseUrl = "ftp://ftp.ncep.noaa.gov/pub/data/nccf/com/gfs/prod";///"gfs." + dt.ToString("yyyyMMdd");

            ///pub/data/nccf/com/gfs/prod/gfs.2018101906/gfs.t06z.pgrb2.1p00.f108
            var folderDia = "gfs." + dt.ToString("yyyyMMdd");

            var horasDisponiveis = new List<string>();

            try
            {
                var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(baseUrl);
                ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                ftpRq.Proxy = null;

                using (var resp = ftpRq.GetResponse())
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var f = reader.ReadLine();

                        if (f.ToLowerInvariant().Contains(folderDia.ToLowerInvariant()))
                        {
                            horasDisponiveis.Add(f.Substring(f.Length - 2, 2));
                        }
                    }
                }
            }
            catch
            {
                //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
            }

            foreach (var hora in horasDisponiveis)
            {
                if (!horasToDownload.Contains(hora)) continue;


                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\" + modelo + hora;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                //if (System.IO.Directory.Exists(directoryToSaveGif) && System.IO.Directory.Exists(directoryToSaveBin)) continue;

                //if (System.IO.Directory.Exists(directoryToSave) && System.IO.Directory.GetFiles(directoryToSave, "prev" + "*.gif").Length != 0) continue;

                if (log != null)
                {
                    log.WriteLine("baixando " + modelo + hora);
                }

                //gfs.t06z.pgrb2.1p00.f108
                //gfs.t06z.pgrb2b.1p00.f000

                var urlHora = baseUrl + "/" + folderDia + hora.ToString() + "/";
                var fileRadical = "gfs.t" + hora + "z.pgrb2.1p00.f";

                var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                var filesToDownload = new List<string>();

                try
                {
                    var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(urlHora);
                    ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                    ftpRq.Proxy = null;

                    using (var resp = ftpRq.GetResponse())
                    using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            var f = reader.ReadLine();
                            if (f.StartsWith(fileRadical))
                                filesToDownload.Add(f);
                        }
                    }
                }
                catch
                {
                    //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
                }


                if (filesToDownload.Count < 180) continue;



                if (!System.IO.Directory.Exists(dest) || System.IO.Directory.GetFiles(dest).Length < filesToDownload.Count)
                {
                    if (log != null)
                    {
                        log.WriteLine("Falha ao baixar arquivos.");
                    }

                    return "CANCELADO";
                }

                message.AppendLine("baixando: " + dt.ToString("yyyyMM") + "\\" + dt.ToString("dd") + "\\" + modelo + hora);

                // create Ctls
                var sysfiles = System.IO.Directory.GetFiles(Config.CaminhoAuxiliar);

                foreach (var sysFile in sysfiles)
                {
                    System.IO.File.Copy(sysFile,
                        System.IO.Path.Combine(dest, System.IO.Path.GetFileName(sysFile)),
                        true
                        );
                }

                if (log != null)
                {
                    log.WriteLine("Criando CTLs");
                }


                if (log != null)
                {
                    log.WriteLine("Criando mapas " + modelo);
                }


                Grads.ConvertNoaaGFSToImg(dt, hora, dest, modelo, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));

                CopyGifs(dest, directoryToSaveGif);
                CopyBin(dest, directoryToSaveBin);


                if (System.IO.Directory.Exists(dest))
                {
                    if (log != null)
                    {
                        log.WriteLine("limpando pasta temporaria");
                    }

                    System.IO.Directory.Delete(dest, true);
                }
            }

            return message.ToString(); ;
        }

        public static string DownloadGFSNoaa(DateTime dt, System.IO.TextWriter log = null, string modelo = "GFSB", string horasToDownload = "00")
        {

            var message = new StringBuilder();

            var localPath = System.IO.Path.GetTempPath() + modelo + "\\";

            var baseUrl = "ftp://ftp.ncep.noaa.gov/pub/data/nccf/com/gfs/prod";///"gfs." + dt.ToString("yyyyMMdd");

            ///pub/data/nccf/com/gfs/prod/gfs.2018101906/gfs.t06z.pgrb2.1p00.f108
            var folderDia = "gfs." + dt.ToString("yyyyMMdd");

            var horasDisponiveis = new List<string>();

            try
            {
                var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(baseUrl);
                ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                ftpRq.Proxy = null;

                using (var resp = ftpRq.GetResponse())
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var f = reader.ReadLine();

                        if (f.ToLowerInvariant().Contains(folderDia.ToLowerInvariant()))
                        {
                            horasDisponiveis.Add(f.Substring(f.Length - 2, 2));
                        }
                    }
                }
            }
            catch
            {
                //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
            }

            foreach (var hora in horasDisponiveis)
            {
                if (!horasToDownload.Contains(hora)) continue;


                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\" + modelo + hora;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                if (System.IO.Directory.Exists(directoryToSaveGif) && System.IO.Directory.Exists(directoryToSaveBin)) continue;

                //if (System.IO.Directory.Exists(directoryToSave) && System.IO.Directory.GetFiles(directoryToSave, "prev" + "*.gif").Length != 0) continue;

                if (log != null)
                {
                    log.WriteLine("baixando " + modelo + hora);
                }

                //gfs.t06z.pgrb2.1p00.f108
                //gfs.t06z.pgrb2b.1p00.f000

                var urlHora = baseUrl + "/" + folderDia + hora.ToString() + "/";
                var fileRadical = "gfs.t" + hora + "z.pgrb2.1p00.f";

                var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                var filesToDownload = new List<string>();

                try
                {
                    var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(urlHora);
                    ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                    ftpRq.Proxy = null;

                    using (var resp = ftpRq.GetResponse())
                    using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            var f = reader.ReadLine();
                            if (f.StartsWith(fileRadical))
                                filesToDownload.Add(f);
                        }
                    }
                }
                catch
                {
                    //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
                }


                if (filesToDownload.Count < 180) continue;

                int i = 0;

                foreach (var f in filesToDownload)
                {
                    var destFile = System.IO.Path.Combine(dest, f);

                    if (!System.IO.File.Exists(destFile))
                    {
                        if (download(urlHora + f, destFile))
                        {
                            if (log != null)
                            {
                                log.WriteLine("download " + (++i).ToString() + " de " + filesToDownload.Count);
                            }
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.WriteLine("Falhou " + (++i).ToString() + " de " + filesToDownload.Count);
                            }
                        }
                    }
                    else
                    {
                        if (log != null)
                        {
                            log.WriteLine("já existente " + (++i).ToString() + " de " + filesToDownload.Count);
                        }
                    }
                }

                if (System.IO.Directory.GetFiles(dest).Length < filesToDownload.Count)
                {
                    if (log != null)
                    {
                        log.WriteLine("Falha ao baixar arquivos.");
                    }

                    return "";
                }

                message.AppendLine("baixando: " + dt.ToString("yyyyMM") + "\\" + dt.ToString("dd") + "\\" + modelo + hora);

                // create Ctls
                var sysfiles = System.IO.Directory.GetFiles(Config.CaminhoAuxiliar);

                foreach (var sysFile in sysfiles)
                {
                    System.IO.File.Copy(sysFile,
                        System.IO.Path.Combine(dest, System.IO.Path.GetFileName(sysFile)),
                        true
                        );
                }

                if (log != null)
                {
                    log.WriteLine("Criando CTLs");
                }

                foreach (var fileToConvert in System.IO.Directory.GetFiles(dest)
                    .Where(f => !System.IO.Path.GetExtension(f).Equals(".idx"))
                    .Where(f => System.IO.Path.GetFileName(f).Contains(".pgrb2.")))
                {
                    //if (System.IO.Path.GetExtension(fileToConvert).StartsWith(".pgrb2af"))
                    //{

                    var pr = new System.Diagnostics.Process();
                    var prInfo = new System.Diagnostics.ProcessStartInfo();
                    //prInfo.UseShellExecute = false;
                    prInfo.CreateNoWindow = true;
                    prInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    prInfo.FileName = @"toctl.bat";
                    prInfo.WorkingDirectory = dest;
                    prInfo.Arguments = System.IO.Path.GetFileName(fileToConvert);
                    pr.StartInfo = prInfo;
                    pr.Start();
                    pr.WaitForExit();
                    //}
                }


                if (log != null)
                {
                    log.WriteLine("Criando mapas " + modelo);
                }

                Grads.ConvertNoaaGFSToImg(dt, hora, dest, modelo, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));



                CopyGifs(dest, directoryToSaveGif);
                CopyBin(dest, directoryToSaveBin);


                if (System.IO.Directory.Exists(dest))
                {
                    if (log != null)
                    {
                        log.WriteLine("limpando pasta temporaria");
                    }

                    System.IO.Directory.Delete(dest, true);
                }
            }

            return message.ToString(); ;
        }

        public static string DownloadGEFSNoaa(DateTime dt, System.IO.TextWriter log = null, string modelo = "GEFS", string horasToDownload = "00;06;12")
        {

            var message = new StringBuilder();

            var localPath = System.IO.Path.GetTempPath() + modelo + "\\";

            var baseUrl = "ftp://ftp.ncep.noaa.gov/pub/data/nccf/com/gens/prod/gefs." + dt.ToString("yyyyMMdd");

            ///pub/data/nccf/com/gfs/prod/gfs.2018101906

            var horasDisponiveis = new List<string>();

            try
            {
                var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(baseUrl);
                ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                ftpRq.Proxy = null;

                using (var resp = ftpRq.GetResponse())
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var f = reader.ReadLine();

                        horasDisponiveis.Add(f.Split('/')[1]);
                    }
                }
            }
            catch
            {
                //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
            }

            foreach (var hora in horasDisponiveis)
            {
                if (!horasToDownload.Contains(hora)) continue;


                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\" + modelo + hora;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                if (System.IO.Directory.Exists(directoryToSaveGif) && System.IO.Directory.Exists(directoryToSaveBin)) continue;

                //if (System.IO.Directory.Exists(directoryToSave) && System.IO.Directory.GetFiles(directoryToSave, "prev" + "*.gif").Length != 0) continue;

                if (log != null)
                {
                    log.WriteLine("baixando " + modelo + hora);
                }

                var urlHora = baseUrl + "/" + hora.ToString() + "/pgrb2a/";
                var fileRadical = modelo == "GEFS" ? "geavg.t" : "gec00.t";

                var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                var filesToDownload = new List<string>();

                try
                {
                    var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(urlHora);
                    ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                    ftpRq.Proxy = null;

                    using (var resp = ftpRq.GetResponse())
                    using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            var f = reader.ReadLine();
                            if (f.StartsWith(fileRadical))
                                filesToDownload.Add(f);
                        }
                    }
                }
                catch
                {
                    //  message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
                }


                if (filesToDownload.Count < 130) continue;

                int i = 0;

                foreach (var f in filesToDownload)
                {
                    var destFile = System.IO.Path.Combine(dest, f);

                    if (!System.IO.File.Exists(destFile))
                    {
                        if (download(urlHora + f, destFile))
                        {
                            if (log != null)
                            {
                                log.WriteLine("download " + (++i).ToString() + " de " + filesToDownload.Count);
                            }
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.WriteLine("Falhou " + (++i).ToString() + " de " + filesToDownload.Count);
                            }
                        }
                    }
                    else
                    {
                        if (log != null)
                        {
                            log.WriteLine("já existente " + (++i).ToString() + " de " + filesToDownload.Count);
                        }
                    }
                }

                if (System.IO.Directory.GetFiles(dest).Length < filesToDownload.Count)
                {
                    if (log != null)
                    {
                        log.WriteLine("Falha ao baixar arquivos.");
                    }

                    return "";
                }

                message.AppendLine("baixando: " + dt.ToString("yyyyMM") + "\\" + dt.ToString("dd") + "\\" + modelo + hora);

                // create Ctls
                var sysfiles = System.IO.Directory.GetFiles(Config.CaminhoAuxiliar);

                foreach (var sysFile in sysfiles)
                {
                    System.IO.File.Copy(sysFile,
                        System.IO.Path.Combine(dest, System.IO.Path.GetFileName(sysFile)),
                        true
                        );
                }

                if (log != null)
                {
                    log.WriteLine("Criando CTLs");
                }

                foreach (var fileToConvert in System.IO.Directory.GetFiles(dest)
                    .Where(f => System.IO.Path.GetExtension(f).StartsWith(".pgrb2af")))
                {
                    //if (System.IO.Path.GetExtension(fileToConvert).StartsWith(".pgrb2af"))
                    //{

                    var pr = new System.Diagnostics.Process();
                    var prInfo = new System.Diagnostics.ProcessStartInfo();
                    //prInfo.UseShellExecute = false;
                    prInfo.CreateNoWindow = true;
                    prInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    prInfo.FileName = @"toctl.bat";
                    prInfo.WorkingDirectory = dest;
                    prInfo.Arguments = System.IO.Path.GetFileName(fileToConvert);
                    pr.StartInfo = prInfo;
                    pr.Start();
                    pr.WaitForExit();
                    //}
                }


                if (log != null)
                {
                    log.WriteLine("Criando mapas " + modelo);
                }

                Grads.ConvertNoaaGEFSToImg(dt, hora, dest, modelo, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));



                CopyGifs(dest, directoryToSaveGif);
                CopyBin(dest, directoryToSaveBin);


                if (System.IO.Directory.Exists(dest))
                {
                    if (log != null)
                    {
                        log.WriteLine("limpando pasta temporaria");
                    }

                    System.IO.Directory.Delete(dest, true);
                }
            }

            return message.ToString(); ;
        }

        public static string DownloadNoaaImgs(DateTime dt, System.IO.TextWriter log = null, string modelo = "GEFS", string horasToDownload = "12")
        {
            //https://www.tropicaltidbits.com/analysis/models/gfs-ens/2018110500/ //gfs-ens_apcpn24_samer_3.png
            //https://www.tropicaltidbits.com/analysis/models/gfs/2018110500/ //gfs_apcpn24_samer_19.png

            var fileRadical = modelo == "GEFS" ? "gfs-ens" : "gfs";

            var localPath = System.IO.Path.GetTempPath() + modelo + "\\";

            foreach (var hora in horasToDownload.Split(';'))
            {
                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\" + modelo + hora;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);
                var directoryToSaveBin40 = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + "40_" + hora);
                var url = @"https://www.tropicaltidbits.com/analysis/models/" + fileRadical + "/" + dt.ToString("yyyyMMdd") + hora;
                int horaI = int.Parse(hora);
                System.Net.WebClient c = new System.Net.WebClient();
                var filesToDownload = new Dictionary<int, string>();
                try
                {
                    if (log != null) log.WriteLine(url);
                    var fileList = c.DownloadString(url);

                    var r = new Regex(@"href=\""(" + fileRadical + @"_apcpn24_samer_(\d+)\.png)");

                    var matches = r.Matches(fileList);


                    if (matches.Count >= 49)
                    {
                        var fNsGEFS12 = "1 5 9 13 17 21 25 29 33 37 41 45 49 53 57 61".Split(' ').ToList();
                        var fNsGEFS06 = "2 6 10 14 18 22 26 30 34 38 42 46 50 54 58".Split(' ').ToList();
                        var fNsGEFS00 = "3 7 11 15 19 23 27 31 35 39 43 47 51 55 59".Split(' ').ToList();

                        var fNsGFS12 = "1 5 9 13 17 21 25 29 33 37 41 45 49 53 57 61".Split(' ').ToList();
                        var fNsGFS06 = "2 6 10 14 18 22 26 30 34 38 42 46 50 54 58".Split(' ').ToList();
                        var fNsGFS00 = "3 7 11 15 19 23 27 31 35 39 43 47 51 55 59".Split(' ').ToList();

                        foreach (Match mat in matches)
                        {
                            if (modelo == "GEFS")
                            {
                                if (hora == "00" && fNsGEFS00.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGEFS00.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                                if (hora == "06" && fNsGEFS06.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGEFS06.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                                if (hora == "12" && fNsGEFS12.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGEFS12.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                            }
                            else if (modelo == "GFS")
                            {
                                if (hora == "00" && fNsGFS00.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGFS00.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                                if (hora == "06" && fNsGFS06.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGFS06.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                                if (hora == "12" && fNsGFS12.Contains(mat.Groups[2].Value)) filesToDownload.Add(fNsGFS12.IndexOf(mat.Groups[2].Value), mat.Groups[1].Value);
                            }
                        }
                    }
                }
                catch
                {
                    if (log != null) log.WriteLine("Nao encontrado");
                    return "Nao encontrado";
                }
                if (filesToDownload.Count >= 15)
                {

                    var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

                    foreach (var f in filesToDownload)
                    {
                        if (!System.IO.Directory.Exists(dest)) System.IO.Directory.CreateDirectory(dest);

                        var destFile = System.IO.Path.Combine(dest, f.Value);

                        if (!System.IO.File.Exists(destFile))
                        {
                            c.DownloadFile(url + "/" + f.Value, destFile);
                            if (log != null)
                            {
                                log.WriteLine("download " + f.Value);
                            }
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.WriteLine("já existente " + f.Value);
                            }
                        }

                        var precO = PrecipitacaoFactory.BuildFromImage2(destFile, dt.AddDays(f.Key + 1));
                        var fname = "pp" + dt.ToString("yyyyMMdd") + "_" + ((36 - horaI) + f.Key * 24).ToString("0000");
                        precO.SalvarModeloBin(Path.Combine(dest, fname));
                    }
                    if (log != null) log.WriteLine("Lendo Imagens");
                    Grads.ConvertNoaaTropsToImg(dt, hora, dest, modelo, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
                    if (log != null) log.WriteLine("Copiando imagens e binários");
                    cptec.CopyGifs(dest, directoryToSaveGif);
                    cptec.CopyBin(dest, directoryToSaveBin);

                    foreach (var f in filesToDownload)
                    {
                        var destFile = System.IO.Path.Combine(dest, f.Value);

                        var precO40 = PrecipitacaoFactory.BuildFromImage3(destFile, dt.AddDays(f.Key + 1));
                        var fname = "pp" + dt.ToString("yyyyMMdd") + "_" + ((36 - horaI) + f.Key * 24).ToString("0000");
                        precO40.SalvarModeloBin(Path.Combine(dest, fname));
                    }
                    cptec.CopyBin(dest, directoryToSaveBin40);

                }


                if (log != null) log.WriteLine("OK");

            }
            return "OK";
        }

        public static string DownloadMeteologixImgs(DateTime dt, System.IO.TextWriter log, out List<Precipitacao> precs, string hora = "00")
        {

            var cc = new System.Net.CookieContainer();

            var httpHndlr = new System.Net.Http.HttpClientHandler();

            httpHndlr.CookieContainer = cc;
            httpHndlr.Proxy = null;
            httpHndlr.UseCookies = true;

            var httpCli = new System.Net.Http.HttpClient(httpHndlr);



            precs = new List<Precipitacao>();
            //https://img2.meteologix.com/images/data/cache/model/model_modez_2019020500_48_1444_63.png
            var modelo = "ECMWF";

            var localPath = System.IO.Path.GetTempPath() + modelo + "\\";

            var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\" + modelo + hora;
            var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);
            var url = @"https://img2.meteologix.com/images/data/cache/model";
            int horaI = int.Parse(hora);
            //System.Net.WebClient c = new System.Net.WebClient();
            //c.Headers.Add(System.Net.HttpRequestHeader.Host, "meteologix.com");
            //c.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "AutoTool");

            var filesToDownload = new Dictionary<int, string>();
            try
            {
                if (log != null) log.WriteLine(url);
                for (int i = 0; i < 9; i++)
                {
                    filesToDownload.Add(i, "model_modez_" + dt.ToString("yyyyMMdd") + hora + "_" + (i * 24 + 36 - horaI).ToString() + "_1444_63.png");
                }
                filesToDownload.Add(9, "model_modez_" + dt.ToString("yyyyMMdd") + hora + "_" + (240).ToString() + "_1444_63.png");

            }
            catch
            {
                if (log != null) log.WriteLine("Nao encontrado");

                return "Nao encontrado";
            }

            var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), modelo + hora);

            foreach (var f in filesToDownload)
            {
                if (!System.IO.Directory.Exists(dest)) System.IO.Directory.CreateDirectory(dest);

                var destFile = System.IO.Path.Combine(dest, f.Value);

                if (!System.IO.File.Exists(destFile))
                {
                    var r = httpCli.GetAsync($"https://meteologix.com/br/model-charts/euro/{dt.ToString("yyyyMMdd")}{hora}/brazil/precipitation-total/{dt.AddDays(f.Key + 2).ToString("yyyyMMdd")}-0000z.html");
                    r.Wait(6000);
                    r.Result.EnsureSuccessStatusCode();

                    if (log != null)
                    {
                        log.WriteLine("download " + f.Value);
                    }


                    r = httpCli.GetAsync(url + "/" + f.Value);
                    r.Wait();
                    r.Result.EnsureSuccessStatusCode();

                    var str = r.Result.Content.ReadAsStreamAsync();

                    str.Wait();
                    using (var fstr = System.IO.File.Create(destFile))
                    {
                        str.Result.CopyTo(fstr);
                    }
                }
                else
                {
                    if (log != null)
                    {
                        log.WriteLine("já existente " + f.Value);
                    }
                }

                var precO = PrecipitacaoFactory.BuildFromImage4(destFile, dt.AddDays(f.Key + 1));
                precs.Add(precO);

                var fname = "pp" + dt.ToString("yyyyMMdd") + "_" + ((36 - horaI) + f.Key * 24).ToString("0000");
                precO.SalvarModeloBin(Path.Combine(dest, fname));
            }
            if (log != null) log.WriteLine("Lendo Imagens");
            Grads.ConvertNoaaTropsToImg(dt, hora, dest, modelo, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
            if (log != null) log.WriteLine("Copiando imagens e binários");
            cptec.CopyGifs(dest, directoryToSaveGif);
            cptec.CopyBin(dest, directoryToSaveBin);

            if (log != null) log.WriteLine("OK");
            return "OK";
        }

        public static string DownloadETA40CPTEC(DateTime dt, System.IO.TextWriter log = null, string horasToDownload = "00;12")
        {

            var message = new StringBuilder();

            //var localPath = Config.CaminhoPrevisao;
            var localPath = System.IO.Path.GetTempPath() + "ETA\\";

            ///etamdl/Products/Eta_tempo/Eta40km/prec24/Etaens_P1D0_CPTM00/2019020100
            //var baseUrl = "ftp://ftp1.cptec.inpe.br/modelos/io/tempo/regional/Eta40km_ENS/prec24/";


            //ftp://ftp1.cptec.inpe.br/etamdl/Products/Eta_tempo/Eta40km/prec24/Etaens_P1D0_CFSM01

            var baseUrl = "ftp://ftp1.cptec.inpe.br/etamdl/Products/Eta_tempo/Eta40km/prec24/Etaens_P1D0_CPTM00/";


            var downloadAction = new Action<string>(s =>
            {
                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\ETA" + s;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), "ETA" + s);

                if (System.IO.Directory.Exists(directoryToSaveGif) && System.IO.Directory.Exists(directoryToSaveBin)) return;

                //ETA40 00h
                Uri cptecUri = new Uri(baseUrl + dt.ToString("yyyyMMdd") + "00" + "/");

                var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(cptecUri);
                ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                ftpRq.Proxy = null;

                try
                {
                    var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), "ETA" + s);
                    var novidade = false;

                    var filesToDownload = new List<Tuple<string, string>>();

                    using (var resp = ftpRq.GetResponse())
                    using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            var f = reader.ReadLine();

                            var fileDest = System.IO.Path.Combine(dest, f);
                            if (!System.IO.File.Exists(fileDest))
                            {
                                filesToDownload.Add(new Tuple<string, string>(cptecUri + f, fileDest));
                                //download(cptecUri + f, dest);
                                novidade = true;
                            }
                        }
                    }

                    foreach (var f in filesToDownload)
                    {
                        if (log != null)
                        {
                            log.WriteLine("baixando: " + f.Item1);
                        }

                        message.AppendLine("baixando: " + dt.ToString("yyyyMMdd") + s + "/");
                        download(f.Item1, f.Item2);
                    }

                    if (novidade)
                    {
                        if (log != null)
                        {
                            log.WriteLine("convertendo imagens (grads)");
                        }



                        Grads.ConvertEta12ToImg(dt, s, dest, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));

                        CopyGifs(dest, directoryToSaveGif);
                        CopyBin(dest, directoryToSaveBin);

                        System.IO.Directory.Delete(dest, true);
                    }

                }
                catch
                {
                    message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
                }
            });

            foreach (var hora in horasToDownload.Split(';'))
            {
                downloadAction(hora);
            }


            return message.ToString();
        }

        public static string DownloadETA40Gambiarra(DateTime dt, System.IO.TextWriter log = null, string horasToDownload = "00;12")
        {

            var message = new StringBuilder();

            //var localPath = Config.CaminhoPrevisao;
            var localPath = System.IO.Path.GetTempPath() + "ETA\\";

            var downloadAction = new Action<string>(s =>
            {
                var searchPath = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"));
                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\ETA" + s;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), "ETA" + s);

                if (System.IO.Directory.Exists(directoryToSaveGif) && System.IO.Directory.Exists(directoryToSaveBin)) return;

                //Prioriza utilizar eta 40 do ons, caso não encontrado, buscar no ftp CPTEC
                if (!File.Exists(System.IO.Path.Combine(searchPath, "ETA40_p" + dt.ToString("ddMMyy") + "a" + dt.AddDays(1).ToString("ddMMyy") + ".dat")) && File.Exists(System.IO.Path.Combine(searchPath, "Eta40_precipitacao10d.zip")))
                {
                    using (var zfile = System.IO.Compression.ZipFile.Open(System.IO.Path.Combine(searchPath, "Eta40_precipitacao10d.zip"), System.IO.Compression.ZipArchiveMode.Read))
                    {
                        //ETA40_p050719a060719.dat
                        if (zfile.Entries.Any(x => x.Name == "ETA40_p" + dt.ToString("ddMMyy") + "a" + dt.AddDays(1).ToString("ddMMyy") + ".dat"))
                            System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.Combine(searchPath, "Eta40_precipitacao10d.zip"), searchPath);
                    }
                }

                if (System.IO.Directory.GetFiles(searchPath, "ETA40_p" + dt.ToString("ddMMyy") + "a*.*", System.IO.SearchOption.TopDirectoryOnly).Length < 10)
                {
                    DownloadETA40CPTEC(dt, log, s);
                    return;
                }



                try
                {
                    var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), "ETA" + s);


                    System.IO.Directory.CreateDirectory(dest);
                    System.IO.Directory.CreateDirectory(directoryToSaveBin);

                    var offset = s == "00" ? 12 : 0;

                    for (int i = 1; i <= 10; i++)
                    {
                        var dataPrev = dt.AddDays(i);
                        var raiznome = "ETA40_p" + dt.ToString("ddMMyy") + "a" + dataPrev.ToString("ddMMyy") + ".dat";
                        var precEta = PrecipitacaoFactory.BuildFromEtaFile(System.IO.Path.Combine(searchPath, raiznome));


                        PrecipitacaoFactory.SalvarModeloBin(precEta,
                            System.IO.Path.Combine(dest,
                            "pp" + dt.ToString("yyyyMMdd") + "_" + ((dataPrev - dt).TotalHours + offset).ToString("0000")
                            )
                        );
                    }


                    if (log != null)
                    {
                        log.WriteLine("convertendo imagens (grads)");
                    }


                    Grads.ConvertEta12ToImg(dt, s, dest, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));

                    CopyGifs(dest, directoryToSaveGif);
                    CopyBin(dest, directoryToSaveBin);

                    System.IO.Directory.Delete(dest, true);

                }
                catch
                {
                    message.AppendLine("não encontrado: " + dt.ToString("yyyyMMdd") + s + "/");
                }
            });

            foreach (var hora in horasToDownload.Split(';'))
            {
                downloadAction(hora);
            }


            return message.ToString();
        }

        public static void ProcessaConjunto(DateTime dt, string hora, System.IO.TextWriter log = null)
        {
            var message = new StringBuilder();

            //var localPath = Config.CaminhoPrevisao;
            var localPath = System.IO.Path.GetTempPath() + "CONJ\\";

            try
            {
                var dest = System.IO.Path.Combine(localPath, dt.ToString("yyyyMM"), dt.ToString("dd"), "CONJUNTO" + hora);
                var directoryToSaveGif = @"P:\Trading\Acompanhamento Metereologico Semanal\spiderman\" + dt.ToString("yyyy_MM_dd") + @"\CONJUNTO" + hora;
                var directoryToSaveBin = System.IO.Path.Combine(Config.CaminhoPrevisao, dt.ToString("yyyyMM"), dt.ToString("dd"), "CONJUNTO" + hora);

                if (!System.IO.Directory.Exists(dest)) System.IO.Directory.CreateDirectory(dest);

                foreach (var item in System.IO.Directory.GetFiles(directoryToSaveBin))
                {
                    System.IO.File.Copy(item,
                        System.IO.Path.Combine(dest, System.IO.Path.GetFileName(item))
                        );
                }


                if (log != null) log.WriteLine("Criando imagens");
                Grads.ConvertConjToImg(dt, hora, dest, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"));
                CopyGifs(dest, directoryToSaveGif);

                System.IO.Directory.Delete(dest, true);
            }
            catch (Exception ex)
            {
                if (log != null) log.WriteLine(ex.Message);
            }
            //            return message.ToString();

        }

        public static void CreateCustomImages(DateTime dt, string binFilesPath, string header)
        {
            var localPath = System.IO.Path.GetTempPath() + "GradsCustomMaps\\";

            try
            {

                if (!System.IO.Directory.Exists(localPath)) System.IO.Directory.CreateDirectory(localPath);

                foreach (var item in System.IO.Directory.GetFiles(binFilesPath))
                {
                    System.IO.File.Copy(item,
                        System.IO.Path.Combine(localPath, System.IO.Path.GetFileName(item))
                        );
                }

                //                if (log != null) log.WriteLine("Criando imagens");
                Grads.ConvertConjToImg(dt, "12", localPath, System.IO.Path.Combine(Config.CaminhoAuxiliar, "CREATE_GIF.gs"), header);
                CopyGifs(localPath, binFilesPath);

                System.IO.Directory.Delete(localPath, true);
            }
            catch (Exception)
            {
                //if (log != null) log.WriteLine(ex.Message);
            }
        }

        public static void CopyGifs(string tempPath, string directoryToSave)
        {
            if (!System.IO.Directory.Exists(directoryToSave)) System.IO.Directory.CreateDirectory(directoryToSave);



            foreach (var models in System.IO.Directory.GetFiles(tempPath, "*.gif")
                //.Where(x => System.Text.RegularExpressions.Regex.Match(System.IO.Path.GetFileNameWithoutExtension(x), "\\d{2}(\\d+)$").Success)
                .Select(x =>
                {
                    var fn = System.IO.Path.GetFileNameWithoutExtension(x);
                    string model = null;
                    int i = -1;
                    if (System.Text.RegularExpressions.Regex.Match(fn, "^[a-zA-Z]+\\d{3,}").Success)
                    {
                        model = System.Text.RegularExpressions.Regex.Match(fn, "^[a-zA-Z]+\\d{2}").Value;
                        i = int.Parse(System.Text.RegularExpressions.Regex.Match(fn, "\\d{2}(\\d+)$").Groups[1].Value);
                    }
                    else if (System.Text.RegularExpressions.Regex.Match(fn, "^[^\\d]+\\d+").Success && System.Text.RegularExpressions.Regex.Match(fn, "(\\d+)$").Success)
                    {
                        model = System.Text.RegularExpressions.Regex.Match(fn, "^[^\\d]+").Value;
                        i = int.Parse(System.Text.RegularExpressions.Regex.Match(fn, "(\\d+)$").Groups[1].Value);
                    }

                    return new { model, i, fn, gifFile = x };

                }).Where(x => x.model != null).GroupBy(x => x.model))
            {
                if (models.Count() >= 10)
                {
                    using (var c = new AnimatedGif.AnimatedGifCreator(System.IO.Path.Combine(tempPath, models.Key + "_animado.gif"), 500)) //2fps
                    {
                        foreach (var file in models.OrderBy(x => x.i))
                        {
                            var gifFile = file.gifFile;
                            c.AddFrame(gifFile);
                        }
                    }
                }
            }

            foreach (var gifFile in System.IO.Directory.GetFiles(tempPath, "*.gif"))
            {
                var fileToSave = System.IO.Path.Combine(directoryToSave, System.IO.Path.GetFileName(gifFile));
                //if (System.IO.File.Exists(fileToSave)) System.IO.File.Delete(fileToSave);
                MoveAndReplaceFile(gifFile, fileToSave);
            }
        }

        public static void CopyBin(string tempPath, string directoryToSave)
        {

            if (!System.IO.Directory.Exists(directoryToSave))
            {
                System.IO.Directory.CreateDirectory(directoryToSave);
            }



            foreach (var binFile in System.IO.Directory.GetFiles(tempPath, "*.bin"))
            {
                //System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"prev" + hora + @"(\d+).bin");

                //var fMatch = r.Match(binFile);
                //if (fMatch.Success)
                //                {

                //var n = int.Parse(fMatch.Groups[1].Value);

                //var horas = (12 - int.Parse(hora)) + ((n + 1) * 24);

                //var finalname = "pp" + dt.ToString("yyyyMMdd") + "_" + horas.ToString("000");
                var finalname = System.IO.Path.GetFileNameWithoutExtension(binFile);

                MoveAndReplaceFile(binFile,
                    System.IO.Path.Combine(directoryToSave, finalname + ".bin")
                );

                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"pp(\d{4}\d{2}\d{2})_(\d+)");

                if (!System.IO.File.Exists(System.IO.Path.Combine(tempPath, finalname + ".ctl")))
                {
                    var fMatch = r.Match(finalname);
                    if (fMatch.Success)
                    {
                        var dt = DateTime.ParseExact(fMatch.Groups[1].Value, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        var hrs = int.Parse(fMatch.Groups[2].Value);

                        var ctlContent =
    @"DSET ^" + finalname + ".bin" + @"
UNDEF -9999.
TITLE Previsao
XDEF  42  LINEAR  -75.00   1.00
YDEF  41  LINEAR  -35.00   1.00
ZDEF   1 LEVELS 1000
TDEF   1 LINEAR 12Z" + dt.AddHours(hrs).ToString("ddMMMyyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo) + @" 24hr
VARS  1
PREC    0  99     Total  24h Precip.        (m)
ENDVARS
";
                        System.IO.File.WriteAllText(System.IO.Path.Combine(tempPath, finalname + ".ctl"), ctlContent);
                    }
                    else
                    {
                        var ctlContent =
    @"DSET ^" + finalname + ".bin" + @"
UNDEF -9999.
TITLE Previsao com alguma coisa estranha
XDEF  42  LINEAR  -75.00   1.00
YDEF  41  LINEAR  -35.00   1.00
ZDEF   1 LEVELS 1000
TDEF   1 LINEAR 12Z" + DateTime.Today.ToString("ddMMMyyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo) + @" 24hr
VARS  1
PREC    0  99     Total  24h Precip.        (m)
ENDVARS
"; System.IO.File.WriteAllText(System.IO.Path.Combine(tempPath, finalname + ".ctl"), ctlContent);
                    }
                }


                MoveAndReplaceFile(System.IO.Path.Combine(tempPath, finalname + ".ctl"),
                        System.IO.Path.Combine(directoryToSave, finalname + ".ctl")
                    );
            }
        }

        static void MoveAndReplaceFile(string ori, string dest)
        {
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(dest)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dest));
            }
            else if (System.IO.File.Exists(dest))
            {
                System.IO.File.Delete(dest);
            }

            System.IO.File.Move(ori, dest);
        }

        public static string ListNewMerge(System.IO.TextWriter log = null)
        {
            var message = new StringBuilder();

            var localPath = Config.CaminhoMerge;

            var dtB = DateTime.Today;
            var dtA = dtB.AddDays(-7);

            var baseUrl = "ftp://ftp1.cptec.inpe.br/modelos/io/produtos/MERGE/";

            var nameLimiteInf = dtA.ToString("yyyy") + "/prec_" + dtA.ToString("yyyyMMdd");

            Uri cptecUri = new Uri(baseUrl + dtB.ToString("yyyy"));

            var ftpRq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(cptecUri);
            ftpRq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
            ftpRq.Proxy = null;

            var fileList = new List<Tuple<string, DateTime>>();

            using (var resp = ftpRq.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var f = reader.ReadLine();

                        if (f.CompareTo(nameLimiteInf) > 0)
                        {

                            var fileTSreq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(baseUrl + f);
                            fileTSreq.Method = System.Net.WebRequestMethods.Ftp.GetDateTimestamp;
                            fileTSreq.Proxy = null;

                            using (var respTS = (System.Net.FtpWebResponse)fileTSreq.GetResponse())
                                fileList.Add(new Tuple<string, DateTime>(f, respTS.LastModified));
                        }
                    }
                }
            }

            foreach (var file in fileList)
            {

                var fileName = file.Item1.Split('/')[1];
                DateTime fileDate;
                if (DateTime.TryParseExact(fileName.Substring(5, 8)
                    , "yyyyMMdd"
                    , System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.AssumeLocal
                    , out fileDate))
                {

                    var localfilePath = System.IO.Path.Combine(localPath
                    , fileDate.Year.ToString()
                    , fileDate.Month.ToString("00"),
                    fileName);

                    if (System.IO.File.Exists(localfilePath))
                    {

                        var fileInfo = new System.IO.FileInfo(localfilePath);

                        if (fileInfo.LastWriteTime < file.Item2)
                        {
                            if (fileInfo.LastWriteTime.Date < file.Item2.Date && fileDate.Date == DateTime.Today)
                            {
                                if (log != null) log.WriteLine("baixando Merge do dia: " + fileName);
                                message.AppendLine("baixando Merge do dia: " + fileName);
                            }
                            else
                            {
                                if (log != null) log.WriteLine("atualizando: " + fileName);
                                message.AppendLine("atualizando: " + fileName);
                            }

                            download(baseUrl + file.Item1, localfilePath);
                        }
                    }
                    else
                    {
                        if (log != null) log.WriteLine("baixando: " + fileName);
                        message.AppendLine("baixando: " + fileName);
                        download(baseUrl + file.Item1, localfilePath);
                    }
                }
            }


            if (log != null && message.ToString().Length == 0) log.WriteLine("Nada novo");

            return message.ToString().Length == 0 ? "Nada novo" : message.ToString();

        }//method end

        static bool download(string url, string destination, int retrial = 0)
        {

            bool tryagain = false;

            try
            {

                var fileTSreq = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(url);
                fileTSreq.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                fileTSreq.Proxy = null;

                using (var respTS = (System.Net.FtpWebResponse)fileTSreq.GetResponse())
                {


                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(destination)))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination));
                    }

                    using (var fs = System.IO.File.Create(destination))
                        //fs.Write()
                        respTS.GetResponseStream().CopyTo(fs);
                }


            }
            catch
            {
                if (System.IO.File.Exists(destination)) System.IO.File.Delete(destination);
                System.Threading.Thread.Sleep(700);
                tryagain = true;

            }

            if (tryagain && retrial < 3)
            {
                return download(url, destination, ++retrial);
            }
            else if (tryagain)
            {
                return false;
            }
            else
                return true;

        }

    }
}

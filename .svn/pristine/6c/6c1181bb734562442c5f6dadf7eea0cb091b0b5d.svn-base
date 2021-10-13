using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ChuvaVazaoTools.Classes;
using System.Runtime.Serialization.Json;

namespace ChuvaVazaoTools
{
    public partial class FrmMain : Form
    {
        private bool Busy
        {
            set
            {
                panel3.Enabled = panel1.Enabled = panel2.Enabled = btnConsultarVazObserv.Enabled = panel4.Enabled = panel5.Enabled = panel6.Enabled = !value;


                this.Cursor = value ? Cursors.WaitCursor : Cursors.Default;
            }
        }

        public string ArquivoPrevsBase { get { return this.txtPrevs.Text; } set { this.txtPrevs.Text = value; } }

        public string ArquivosDeEntradaPrevivaz { get { return this.txtPrevivaz.Text; } set { this.txtPrevivaz.Text = value; } }

        public string ArquivosDeEntradaModelo { get { return this.txtEntrada.Text; } set { this.txtEntrada.Text = value; } }

        public string ArquivosDeSaida { get { return this.txtCaminho.Text; } set { this.txtCaminho.Text = value; } }

        public DateTime? DataSemanaPrevsBase { get; private set; }

        public List<ModeloChuvaVazao> modelosChVz = new List<ModeloChuvaVazao>();
        public Dictionary<DateTime, Precipitacao> chuvas = new Dictionary<DateTime, Precipitacao>();

        bool runAuto = false;
        TextBoxLogger textLogger = null;


        #region Public Methods

        public FrmMain(bool run) : this()
        {
            runAuto = run;
        }

        public FrmMain()
        {
            InitializeComponent();

            this.Text += " - " + GetRunningVersion().ToString();

            dtAtual.Value = DateTime.Today.Date;
        }

        public void Ler()
        {
            try
            {
                var path = txtCaminho.Text;

                if (!System.IO.Directory.Exists(path))
                {
                    MessageBox.Show("Caminho não existente");
                    return;
                }

                modelosChVz.Clear();

                var modelos = System.IO.Directory.GetDirectories(path);

                foreach (var modelo in modelos)
                {

                    var nomeModelo = modelo.Replace(System.IO.Path.GetDirectoryName(modelo), "").Remove(0, 1);

                    if (nomeModelo.StartsWith("SMAP", StringComparison.OrdinalIgnoreCase))
                    {
                        var bacias = System.IO.Directory.GetDirectories(modelo);
                        foreach (var bacia in bacias)
                        {
                            modelosChVz.Add(new ChuvaVazaoTools.SMAP.ModeloSmap(bacia));
                        }
                    }

                    AddLog("\t" + modelo);
                }


                modelosChVz.ForEach(x => x.ColetarSaida());

                listView1.Items.Clear();

                listView1.Items.AddRange(modelosChVz.Select(x => new ModeloItemView(x)).ToArray());

                dtModelo.Value = modelosChVz.Min(x => x.DataPrevisao);

                AddLog("- Modelos Carregados");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                AddLog("\t" + "Erro no método FrmMain/Ler: " + e.Message);
            }
        }

        private void btnLer_Click(object sender, EventArgs e)
        {
            Ler();
        }

        public class ModeloItemView : ListViewItem
        {

            public ModeloItemView(ModeloChuvaVazao x)
            {
                this.Text = x.GetType().Name + " - " + System.IO.Path.GetDirectoryName(x.Caminho);
            }
        }

        public class PrecipitacaoItemView : ListViewItem
        {

            Precipitacao prec = null;

            public Precipitacao Prec { get { return prec; } }

            public String Descricao { get; set; }
            public DateTime DataChuva { get; set; }


            public PrecipitacaoItemView(Precipitacao prec)
                : base(new string[] { prec.Data.ToString("dd/MM/yyyy"), prec.Descricao })
            {
                this.prec = prec;
            }
        }

        public void CopiarResultados()
        {
            modelosChVz.ForEach(x => x.ColetarSaida());

            var vaz = modelosChVz.SelectMany(x => x.Vazoes).ToList();
            var minData = vaz.Min(x => x.Vazoes.Keys.Min());
            var maxData = vaz.Max(x => x.Vazoes.Keys.Max());

            int rows = (int)(maxData - minData).TotalDays + 1;
            int cols = vaz.Count();

            object[,] results = new object[rows + 1, cols + 1];

            for (int i = 0; i < cols; i++)
            {
                results[0, i + 1] = vaz[i].CaminhoArquivo;
            }

            for (int d = 0; d < rows; d++)
            {
                var dt = minData.AddDays(d);
                results[d + 1, 0] = dt;
                for (int i = 0; i < cols; i++)
                {
                    if (vaz[i].Vazoes.ContainsKey(dt))
                    {
                        results[d + 1, i + 1] = vaz[i].Vazoes[dt];
                    }
                }
            }
            string s = "";

            for (int c = 0; c < results.GetLength(1); c++)
            {
                for (int r = 0; r < results.GetLength(0); r++)
                {
                    if (results[r, c] != null) s += results[r, c].ToString();
                    s += "\t";
                }
                s += Environment.NewLine;
            }

            System.Windows.Forms.Clipboard.SetText(s, TextDataFormat.Text);
            MessageBox.Show("Salvo na área de transferência");
        }

        public enum EnumRemo
        {
            RemocaoAtual,
            RemocaoUmaSemana,
            RemocaoUmaSemanaEuro,
            RemocaoDuasSemanasEuro,
            RemocaoDuasSemanasGEFS,
            RemocaoDuasSemanasGFS,
            RemocaoDuasSemanasGFS2x,
        };

        public void EuroSem(IPrecipitacaoForm frm)
        {
            #region EUROPEU SEM
            frm.LimparCache();
            frm.Eta = WaitForm2.TipoEta._euro_00h;
            //frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Eta40;
            frm.RemoveViesETA = false;
            frm.RemoveLimiteETA = false;
            frm.SalvarDados = false;

            var chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            #endregion

            #region GEFS COM
            frm.LimparCache();
            frm.Eta = WaitForm2.TipoEta._00h;
            frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Conjunto;

            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.RemoveViesETA = true;
            frm.RemoveLimiteETA = true;

            frm.sobrescreverCB = false;
            frm.SalvarDados = false;

            var dtRemocao = dtAtual.Value;

            if (dtRemocao.DayOfWeek == DayOfWeek.Thursday)
                dtRemocao = dtRemocao.AddDays(+7);
            else
            {
                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday)
                    dtRemocao = dtRemocao.AddDays(+1);
            }

            frm.DateRemocao = dtRemocao;

            chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();

            #endregion
        }

        public void EuroSemGefsCom(IPrecipitacaoForm frm)
        {
            #region EUROPEU SEM
            frm.LimparCache();
            frm.Eta = WaitForm2.TipoEta._euro_00h;
            //frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Eta40;
            frm.RemoveViesETA = false;
            frm.RemoveLimiteETA = false;
            frm.SalvarDados = false;
            frm.Previsoes2Semanas = true;

            var chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            #endregion

            #region GEFS COM
            frm.LimparCache();
            //frm.Eta = WaitForm2.TipoEta._euro_00h;
            frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.sobrescreverCB = false;
            frm.SalvarDados = false;

            var dtRemocao = dtAtual.Value;

            if (dtRemocao.DayOfWeek == DayOfWeek.Thursday)
                dtRemocao = dtRemocao.AddDays(+7);
            else
            {
                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday)
                    dtRemocao = dtRemocao.AddDays(+1);

                dtRemocao = dtRemocao.AddDays(+7);
            }


            frm.DateRemocao = dtRemocao;


            chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();

            #endregion
        }

        public void GfsComGfsCom(IPrecipitacaoForm frm)
        {
            #region GFS COM
            frm.LimparCache();
            frm.Gefs = WaitForm2.TipoGefs._ctl_00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.SalvarDados = false;
            frm.DateRemocao = dtAtual.Value;
            frm.Previsoes2Semanas = true;

            var chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }
            #endregion

            #region GFS COM sem sobrescrever

            frm.LimparCache();
            frm.Gefs = WaitForm2.TipoGefs._ctl_00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.sobrescreverCB = false;
            frm.SalvarDados = false;

            var dtRemocao = dtAtual.Value;

            if (dtRemocao.DayOfWeek == DayOfWeek.Thursday)
                dtRemocao = dtRemocao.AddDays(+7);
            else
            {
                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday)
                    dtRemocao = dtRemocao.AddDays(+1);

                dtRemocao = dtRemocao.AddDays(+7);
            }

            frm.DateRemocao = dtRemocao;
            chuvasConjunto = frm.ProcessarConjunto();

            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();

            #endregion
        }

        public void GfsSemGefsCom(IPrecipitacaoForm frm)
        {
            #region GFS COM
            frm.LimparCache();
            //frm.Eta = WaitForm2.TipoEta._euro_00h;
            frm.Gefs = WaitForm2.TipoGefs._ctl_00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = false;
            frm.RemoveLimiteGEFS = false;
            frm.SalvarDados = false;
            frm.DateRemocao = dtAtual.Value;
            frm.Previsoes2Semanas = true;

            var chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }
            #endregion

            #region GEFS COM
            frm.LimparCache();
            //frm.Eta = WaitForm2.TipoEta._euro_00h;
            frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.sobrescreverCB = false;
            frm.SalvarDados = false;

            var dtRemocao = dtAtual.Value;

            if (dtRemocao.DayOfWeek == DayOfWeek.Thursday)
                dtRemocao = dtRemocao.AddDays(+7);
            else
            {
                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday)
                    dtRemocao = dtRemocao.AddDays(+1);

                dtRemocao = dtRemocao.AddDays(+7);
            }


            frm.DateRemocao = dtRemocao;


            chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();

            #endregion

        }
        public void GefsSemGefsCom(IPrecipitacaoForm frm)
        {
            #region GeFS COM
            frm.LimparCache();
            //frm.Eta = WaitForm2.TipoEta._euro_00h;
            frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = false;
            frm.RemoveLimiteGEFS = false;
            frm.SalvarDados = false;
            frm.DateRemocao = dtAtual.Value;
            frm.Previsoes2Semanas = true;

            var chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }
            #endregion

            #region GEFS COM
            frm.LimparCache();
            //frm.Eta = WaitForm2.TipoEta._euro_00h;
            frm.Gefs = WaitForm2.TipoGefs._00h;
            frm.Tipo = WaitForm.TipoConjunto.Gefs;
            frm.RemoveViesGEFS = true;
            frm.RemoveLimiteGEFS = true;
            frm.sobrescreverCB = false;
            frm.SalvarDados = false;

            var dtRemocao = dtAtual.Value;

            if (dtRemocao.DayOfWeek == DayOfWeek.Thursday)
                dtRemocao = dtRemocao.AddDays(+7);
            else
            {
                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday)
                    dtRemocao = dtRemocao.AddDays(+1);

                dtRemocao = dtRemocao.AddDays(+7);
            }

            frm.DateRemocao = dtRemocao;

            chuvasConjunto = frm.ProcessarConjunto();
            foreach (var c in chuvasConjunto)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();

            #endregion

        }

        //runStatusFile : { caso copiado, pronto para execucao, executado, resultado coletado, previvaz pronto, finalizado } int[6]
        class RunStatus
        {
            internal enum statuscode : int
            {
                nonInitialized = 0,
                initialialized = 1,
                completed = 2,
                error = 3,
            }

            string filePath = "";

            internal RunStatus(string folder)
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                filePath = Path.Combine(folder, "status.log");

                if (File.Exists(filePath))
                {
                    statuses = File.ReadAllText(filePath).Split(' ').Select(x => int.Parse(x)).ToArray();
                }
                else
                {
                    statuses = new int[] { 0, 0, 0, 0, 0, 0 };
                    Save();
                }
            }
            void Save()
            {
                File.WriteAllText(filePath, string.Join(" ", statuses));
            }

            int[] statuses;

            internal statuscode Creation { get { return (statuscode)statuses[0]; } set { statuses[0] = (int)value; Save(); } }
            internal statuscode Preparation { get { return (statuscode)statuses[1]; } set { statuses[1] = (int)value; Save(); } }
            internal statuscode Execution { get { return (statuscode)statuses[2]; } set { statuses[2] = (int)value; Save(); } }
            internal statuscode Collect { get { return (statuscode)statuses[3]; } set { statuses[3] = (int)value; Save(); } }
            internal statuscode Previvaz { get { return (statuscode)statuses[4]; } set { statuses[4] = (int)value; Save(); } }
            internal statuscode PostProcessing { get { return (statuscode)statuses[5]; } set { statuses[5] = (int)value; Save(); } }
        }

        public void RunExecProcess(System.IO.TextWriter logF, out string runId, EnumRemo offset = EnumRemo.RemocaoAtual)
        {
            dtAtual.Value = DateTime.Today.Date;
            runId = null;

            DateTime datModel;
            string horaPrev = "";
            var name = "CPM_CV"; //Computational Processing Model
            var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(dtAtual.Value);


            var runRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(dtAtual.Value,
                (offset == EnumRemo.RemocaoDuasSemanasEuro || offset == EnumRemo.RemocaoDuasSemanasGEFS || offset == EnumRemo.RemocaoDuasSemanasGFS || offset == EnumRemo.RemocaoDuasSemanasGFS2x) ? 2 : 1
                );


            if (logF != null) logF.WriteLine("INICIANDO RODADA AUTOMÁTICA");

            IPrecipitacaoForm frm = null;
            frm = WaitForm2.CreateInstance(dtAtual.Value);

            try
            {
                if (frm.TemEta00 && frm.TemGefs00)
                {
                    AddLog("CONJUNTO 00");
                }
                else if (frm.TemGefs00)
                {
                    AddLog("GEFS 00");
                    horaPrev = "_GEFS";
                }
                else
                {
                    throw new FileLoadException("Previões para o dia não encontradas - ENCERRANDO");
                }
            }
            catch
            {
                if (logF != null) logF.WriteLine("Previões para o dia não encontradas - ENCERRANDO");
                AddLog("Previões para o dia não encontradas");
                return;
            }

            name = name + horaPrev;

            try
            {
                PreencherVazObservada(out DateTime dataModelo, out string fonteVaz);

                CarregarPrecRealMedia(dtAtual.Value.Date, out string modeloPrecReal);

                //modelosChVz

                if (modeloPrecReal.EndsWith("-1"))
                {
                    AddLog("Chuva realizada do dia ainda não está disponível");
                    return;
                }

                name = name + "_" + modeloPrecReal;

                if (dataModelo < dtAtual.Value.Date.AddDays(-1))
                {
                    name = name + "_d-1";
                }
                switch (offset)
                {
                    case EnumRemo.RemocaoUmaSemana:
                        name = name + "_VIES_VE";
                        break;
                    case EnumRemo.RemocaoUmaSemanaEuro:
                        name = name + "_EURO";
                        break;
                    case EnumRemo.RemocaoDuasSemanasEuro:
                        name = name + "_EURO";
                        name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
                        break;
                    case EnumRemo.RemocaoDuasSemanasGEFS:
                        name = name + "_GEFS";
                        name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
                        break;
                    case EnumRemo.RemocaoDuasSemanasGFS:
                        name = name + "_GFS";
                        name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
                        break;
                    case EnumRemo.RemocaoDuasSemanasGFS2x:
                        name = name + "_GFS2x";
                        name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
                        break;
                }
                datModel = dataModelo;
            }
            catch
            {
                logF.WriteLine("Possivel falha no PreencherVazObservada e CarregarPrecRealMedia");
                return;
            }

            ArquivosDeSaida = @"H:\Middle - Preço\16_Chuva_Vazao\" + runRev.revDate.ToString("yyyy_MM") + @"\RV" + runRev.rev.ToString() + @"\" + DateTime.Now.ToString("yy-MM-dd") + @"\" + name;
            var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

            var statusF = new RunStatus(ArquivosDeSaida);

            if (System.IO.Directory.Exists(ArquivosDeSaida) && System.IO.File.Exists(Path.Combine(ArquivosDeSaida, "resumoENA.gif")))
            {
                AddLog("Caso já executado para essa data: " + name);
                if (logF != null) logF.WriteLine("Caso já executado para essa data: " + name);
                runId = "OK - " + name;

                return;
            }

            if (statusF.Creation == RunStatus.statuscode.initialialized
                || statusF.Previvaz == RunStatus.statuscode.initialialized
                || statusF.PostProcessing == RunStatus.statuscode.initialialized
                || statusF.Preparation == RunStatus.statuscode.initialialized
                || statusF.Execution == RunStatus.statuscode.initialialized
                )
            {
                AddLog("Caso em execução: " + name);
                if (logF != null) logF.WriteLine("Caso em execução: " + name);
                return;
            }

            runId = name;

            if (logF != null) logF.WriteLine("INICIANDO RODADA: " + name);

            if (!Directory.Exists(pastaBase) || !(Directory.Exists(System.IO.Path.Combine(pastaBase, "Modelos_Chuva_Vazao")) && Directory.Exists(System.IO.Path.Combine(pastaBase, "Previvaz", "Arq_Entrada")) && System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories).Length > 0))
            {
                if (logF != null) logF.WriteLine("Arquivos de entrada nao disponiveis");
                return;
            }

            this.ArquivosDeEntradaModelo = System.IO.Path.Combine(pastaBase, "Modelos_Chuva_Vazao");
            this.ArquivosDeEntradaPrevivaz = System.IO.Path.Combine(pastaBase, "Previvaz", "Arq_Entrada");
            this.ArquivoPrevsBase = System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories)[0];
            this.DataSemanaPrevsBase = currRev.revDate;

            if (!System.IO.Directory.Exists(ArquivosDeSaida) || statusF.Creation != RunStatus.statuscode.completed)
            {
                try
                {
                    CriarCaso(statusF);
                }
                catch
                {
                    logF.WriteLine("Falha na movimentação de pastas do Smap");
                    return;
                }
            }
            if (!System.IO.File.Exists(Path.Combine(ArquivosDeSaida, "chuvamedia.log")) || statusF.Preparation != RunStatus.statuscode.completed)
            {
                statusF.Preparation = RunStatus.statuscode.initialialized;

                Ler();

                CarregarPrecObserv();
                PreencherPrecObserv();

                //btnConsultarVazObserv_Click(sender, e);
                PreencherVazObservada(out _, out _);
                {

                    if (offset == EnumRemo.RemocaoUmaSemanaEuro) EuroSem(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasEuro) EuroSemGefsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGFS2x) GfsComGfsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGFS) GfsSemGefsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGEFS) GefsSemGefsCom(frm);
                    else
                    {
                        var funcLogs = new Action<string>(hora =>
                        {
                            //var eta = hora.Contains("00") ? WaitForm2.TipoEta._00h : WaitForm2.TipoEta._12h;
                            //var gefs = hora.Contains("00") ? WaitForm2.TipoGefs._00h : WaitForm2.TipoGefs._12h;

                            frm.LimparCache();
                            frm.Eta = WaitForm2.TipoEta._00h;
                            frm.Gefs = WaitForm2.TipoGefs._00h;
                            frm.Tipo = hora.Contains("GEFS") ? WaitForm.TipoConjunto.Gefs : WaitForm.TipoConjunto.Conjunto;
                            frm.SalvarDados = false;

                            if (offset == EnumRemo.RemocaoUmaSemana)
                            {
                                var dtRemocao = dtAtual.Value;
                                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday) dtRemocao = dtRemocao.AddDays(1);

                                frm.DateRemocao = dtRemocao;
                                frm.sobrescreverCB = true;
                            }
                            else if (dtAtual.Value.DayOfWeek == DayOfWeek.Friday ||
                            dtAtual.Value.DayOfWeek == DayOfWeek.Saturday ||
                            dtAtual.Value.DayOfWeek == DayOfWeek.Sunday)
                            {
                                frm.TodasAsPrevisoes = true;
                            }

                            var chuvasConjunto = frm.ProcessarConjunto();
                            foreach (var c in chuvasConjunto)
                            {
                                chuvas[c.Key] = c.Value;
                            }

                            RefreshPrecipList();
                        });
                        funcLogs("00");
                    }
                }

                dtAtual.Value = datModel.AddDays(1);
                dtModelo.Value = dtAtual.Value.Date;
                Reiniciar(dtModelo.Value);

                AddLog(" --- ");
                AddLog(" --- Executar Parte B quando pronto --- ");

                PreencherPrecObserv();
                btnSalvarPrecObserv_Click(null, null);
                SalvarVazObserv();
                SalvarPrecPrev();

                if (!File.Exists(Path.Combine(ArquivosDeSaida, "chuvamedia.log")))
                    statusF.Preparation = RunStatus.statuscode.error;
                else
                    statusF.Preparation = RunStatus.statuscode.completed;
            }

            if (statusF.Preparation == RunStatus.statuscode.completed && statusF.Creation == RunStatus.statuscode.completed)
            {
                statusF.Execution = RunStatus.statuscode.initialialized; //TODO: criar um status para o metodo automatico do executingProcess

                if (modelosChVz.Count == 0)
                    Ler();
            }

            #region Propagacoes sem Excell
            try
            {
                List<Propagacao> propagacoes = null;
                if (statusF.Preparation == RunStatus.statuscode.completed && statusF.Creation == RunStatus.statuscode.completed)
                {
                    propagacoes = new ExecutingProcess().ProcessResultsPart1(modelosChVz);
                    if (propagacoes.Count != 0 || propagacoes != null)
                        statusF.Execution = RunStatus.statuscode.completed;

                }
                if (propagacoes.Count != 0 || propagacoes != null)
                {
                    MemoryStream stream1 = new MemoryStream();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Propagacao>));

                    ser.WriteObject(stream1, propagacoes);
                    stream1.Position = 0;


                    if (statusF.Execution == RunStatus.statuscode.completed)
                    {
                        File.WriteAllText(Path.Combine(ArquivosDeSaida, "Propagacoes_Automaticas.txt"), new StreamReader(stream1).ReadToEnd());

                        statusF.Previvaz = RunStatus.statuscode.initialialized;

                        var p = Program.GetPrevivazExPath(Path.Combine(ArquivosDeSaida, "Propagacoes_Automaticas.txt"));

                        if (p != null)
                        {

                            AddLog("EXECUCAO PREVIVAZ");
                            if (logF != null) logF.WriteLine("EXECUCAO PREVIVAZ");
                            var pr = System.Diagnostics.Process.Start(p.Item1, p.Item2);
                            pr.WaitForExit();

                            try
                            {
                                var procId = pr.BasePriority;
                                if (statusF != null) statusF.Previvaz = RunStatus.statuscode.completed;
                            }
                            catch (Exception)
                            {
                                statusF.Previvaz = RunStatus.statuscode.error;
                            }

                        }
                        else
                        {
                            if (statusF != null && System.IO.Directory.Exists(Path.Combine(ArquivosDeSaida, "Propagacoes_Automaticas.txt"))) statusF.Previvaz = RunStatus.statuscode.error;
                            return;
                        }
                    }
                }
                else
                {
                    statusF.Execution = RunStatus.statuscode.error;
                    throw new Exception("As propagações foram enviadas ao método e retornaram vazias ou com erro");
                }

                if (statusF.Creation == RunStatus.statuscode.completed &&
                    statusF.Execution == RunStatus.statuscode.completed &&
                    statusF.Preparation == RunStatus.statuscode.completed &&
                    statusF.Previvaz == RunStatus.statuscode.completed
                    )
                {
                    var email = Tools.Tools.SendMail(Path.Combine(ArquivosDeSaida, "Propagacoes_Automaticas.txt"), "Sucesso ao executar as propagações automáticas!", "Propagações sem Excell [AUTO]", "desenv");
                    email.Wait();
                }
            }
            catch (Exception exce)
            {
                statusF.Execution = RunStatus.statuscode.error;
                var email = Tools.Tools.SendMail("", "ERRO: " + exce.Message, "Erro nas propagações sem Excell [AUTO]", "desenv");
                email.Wait();
            }
            #endregion

        }

        public void Run(System.IO.TextWriter logF, out string runId, EnumRemo offset = EnumRemo.RemocaoAtual)
        {
            dtAtual.Value = DateTime.Today.Date;

            runId = null;
            if (logF != null) logF.WriteLine("INICIANDO RODADA AUTOMÁTICA");

            var name = "CV";


            var runRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(dtAtual.Value,
                (offset == EnumRemo.RemocaoDuasSemanasEuro || offset == EnumRemo.RemocaoDuasSemanasGEFS || offset == EnumRemo.RemocaoDuasSemanasGFS || offset == EnumRemo.RemocaoDuasSemanasGFS2x) ? 2 : 1
                );

            var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(dtAtual.Value);


            IPrecipitacaoForm frm = null;
            frm = WaitForm2.CreateInstance(dtAtual.Value);
            string horaPrev = "";

            try
            {
                if (frm.TemEta00 && frm.TemGefs00)
                {
                    AddLog("CONJUNTO 00");
                }
                else if (frm.TemGefs00)
                {
                    AddLog("GEFS 00");
                    horaPrev = "_GEFS";
                }
                else
                {
                    if (logF != null) logF.WriteLine("Previões para o dia não encontradas - ENCERRANDO");
                    AddLog("Previões para o dia não encontradas");
                    return;
                }
            }
            catch { }

            name = name + horaPrev;

            PreencherVazObservada(out DateTime dataModelo, out string fonteVaz);

            name = name + "_" + fonteVaz.ToUpper();



            //CarregarPrecReal(dtAtual.Value.Date, out string modeloPrecReal);
            CarregarPrecRealMedia(dtAtual.Value.Date, out string modeloPrecReal);

            if (modeloPrecReal.EndsWith("-1"))
            {
                AddLog("Chuva realizada do dia ainda não está disponível");
                return;
            }

            name = name + "_" + modeloPrecReal;

            if (dataModelo < dtAtual.Value.Date.AddDays(-1))
            {
                name = name + "_d-1";
            }

            if (offset == EnumRemo.RemocaoUmaSemana)
            {
                name = name + "_VIES_VE";
            }
            else if (offset == EnumRemo.RemocaoUmaSemanaEuro)
            {
                name = name + "_EURO";
            }
            else if (offset == EnumRemo.RemocaoDuasSemanasEuro)
            {
                name = name + "_EURO";
                name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
            }
            else if (offset == EnumRemo.RemocaoDuasSemanasGEFS)
            {
                name = name + "_GEFS";
                name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
            }
            else if (offset == EnumRemo.RemocaoDuasSemanasGFS)
            {
                name = name + "_GFS";
                name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
            }
            else if (offset == EnumRemo.RemocaoDuasSemanasGFS2x)
            {
                name = name + "_GFS2x";
                name = name.Replace("_" + modeloPrecReal, "").Replace("CV_", "CV2_");
            }

            var pastaSaida = @"H:\Middle - Preço\16_Chuva_Vazao\" + runRev.revDate.ToString("yyyy_MM") + @"\RV" + runRev.rev.ToString() + @"\" + DateTime.Now.ToString("yy-MM-dd") + @"\" + name;
            //var pastaSaida = @"C:\temp\" + runRev.revDate.ToString("yyyy_MM") + @"\RV" + runRev.rev.ToString() + @"\" + DateTime.Now.ToString("yy-MM-dd") + @"\" + name;

            var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

            var statusF = new RunStatus(pastaSaida);
            if (statusF.Creation == RunStatus.statuscode.initialialized
                || statusF.Previvaz == RunStatus.statuscode.initialialized
                || statusF.PostProcessing == RunStatus.statuscode.initialialized
                || statusF.Preparation == RunStatus.statuscode.initialialized
                || statusF.Execution == RunStatus.statuscode.initialialized
                )
            {
                AddLog("Caso em execução: " + name);
                if (logF != null) logF.WriteLine("Caso em execução: " + name);
                return;
            }

            if ((System.IO.Directory.Exists(pastaSaida) && System.IO.File.Exists(Path.Combine(pastaSaida, "resumoENA.gif"))) &&
                statusF.PostProcessing == RunStatus.statuscode.completed)
            {
                AddLog("Caso já executado para essa data: " + name);
                if (logF != null) logF.WriteLine("Caso já executado para essa data: " + name);

                runId = "OK - " + name;
                return;
            }



            runId = name;

            if (logF != null) logF.WriteLine("INICIANDO RODADA: " + name);

            //var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\12_2018\Dados_de_Entrada_e_Saida_201812_RV0";


            if (!Directory.Exists(pastaBase) || !(Directory.Exists(System.IO.Path.Combine(pastaBase, "Modelos_Chuva_Vazao")) && Directory.Exists(System.IO.Path.Combine(pastaBase, "Previvaz", "Arq_Entrada")) && System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories).Length > 0))
            {
                if (logF != null) logF.WriteLine("Arquivos de entrada nao disponiveis");
                return;
            }

            this.ArquivosDeEntradaModelo = System.IO.Path.Combine(pastaBase, "Modelos_Chuva_Vazao");
            this.ArquivosDeEntradaPrevivaz = System.IO.Path.Combine(pastaBase, "Previvaz", "Arq_Entrada");
            this.ArquivoPrevsBase = System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories)[0];
            this.DataSemanaPrevsBase = currRev.revDate;

            ArquivosDeSaida = pastaSaida;


            if (!System.IO.Directory.Exists(pastaSaida) || statusF.Creation != RunStatus.statuscode.completed)
            {
                CriarCaso(statusF);
            }

            if (!System.IO.File.Exists(Path.Combine(pastaSaida, "chuvamedia.log")) || statusF.Preparation != RunStatus.statuscode.completed)
            {
                statusF.Preparation = RunStatus.statuscode.initialialized;

                Ler();

                CarregarPrecObserv();
                PreencherPrecObserv();

                //btnConsultarVazObserv_Click(sender, e);
                PreencherVazObservada(out _, out _);
                {

                    if (offset == EnumRemo.RemocaoUmaSemanaEuro) EuroSem(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasEuro) EuroSemGefsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGFS2x) GfsComGfsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGFS) GfsSemGefsCom(frm);
                    else if (offset == EnumRemo.RemocaoDuasSemanasGEFS) GefsSemGefsCom(frm);
                    else
                    {
                        var funcLogs = new Action<string>(hora =>
                        {
                            //var eta = hora.Contains("00") ? WaitForm2.TipoEta._00h : WaitForm2.TipoEta._12h;
                            //var gefs = hora.Contains("00") ? WaitForm2.TipoGefs._00h : WaitForm2.TipoGefs._12h;

                            frm.LimparCache();
                            frm.Eta = WaitForm2.TipoEta._00h;
                            frm.Gefs = WaitForm2.TipoGefs._00h;
                            frm.Tipo = hora.Contains("GEFS") ? WaitForm.TipoConjunto.Gefs : WaitForm.TipoConjunto.Conjunto;
                            frm.SalvarDados = false;

                            if (offset == EnumRemo.RemocaoUmaSemana)
                            {
                                var dtRemocao = dtAtual.Value;
                                while (dtRemocao.DayOfWeek != DayOfWeek.Thursday) dtRemocao = dtRemocao.AddDays(1);

                                frm.DateRemocao = dtRemocao;
                                frm.sobrescreverCB = true;
                            }
                            else if (dtAtual.Value.DayOfWeek == DayOfWeek.Friday ||
                            dtAtual.Value.DayOfWeek == DayOfWeek.Saturday ||
                            dtAtual.Value.DayOfWeek == DayOfWeek.Sunday)
                            {
                                frm.TodasAsPrevisoes = true;
                            }

                            var chuvasConjunto = frm.ProcessarConjunto();
                            foreach (var c in chuvasConjunto)
                            {
                                chuvas[c.Key] = c.Value;
                            }

                            RefreshPrecipList();
                        });
                        funcLogs("00");
                    }
                }

                dtAtual.Value = dataModelo.AddDays(1);
                dtModelo.Value = dtAtual.Value.Date;
                Reiniciar(dtModelo.Value);

                AddLog(" --- ");
                AddLog(" --- Executar Parte B quando pronto --- ");

                PreencherPrecObserv();
                btnSalvarPrecObserv_Click(null, null);
                SalvarVazObserv();
                SalvarPrecPrev();

                if (!File.Exists(Path.Combine(pastaSaida, "chuvamedia.log")))
                    statusF.Preparation = RunStatus.statuscode.error;
                else
                    statusF.Preparation = RunStatus.statuscode.completed;
            }

            if (statusF.Preparation != RunStatus.statuscode.completed) return;

            if (statusF.Execution != RunStatus.statuscode.completed)
            {
                ExecutarTudo(statusF);
            }

            if (statusF.Execution == RunStatus.statuscode.completed)
            {
                AddLog(" --- ");
                AddLog(" --- Parte B Concluída --- ");

                if (logF != null) logF.WriteLine("EXECUCAO OK - PRECESSANDO RESULTADOS");

                ProcessarResultados(pastaSaida, logF, runRev.rev, statusF);

                if (logF != null) logF.WriteLine("FINALIZADO");

                runId = "OK - " + name;
            }
            else
            {
                if (logF != null) logF.WriteLine("SMAPS NAO EXECUTADOS");
            }
        }

        public void Reiniciar(DateTime dataPrevisao)
        {
            foreach (var modelo in modelosChVz)
            {
                if (modelo.DataPrevisao != dataPrevisao)
                {
                    modelo.DataPrevisao = dataPrevisao;
                    if (modelo is SMAP.ModeloSmap)
                    {
                        ((SMAP.ModeloSmap)modelo).SubBacias.ForEach(x => x.ReiniciarParametros());
                    }
                    modelo.SalvarParametros();
                }
            }
            AddLog("- Modelos Iniciados para o dia: " + dtAtual.Value.Date.ToShortDateString());
        }


        private void btnReinicar_Click(object sender, EventArgs e)
        {
            dtModelo.Value = dtAtual.Value.Date;
            Reiniciar(dtModelo.Value);
        }

        public void CarregarPrecObserv()
        {

            for (DateTime data = dtModelo.Value.Date; data <= dtAtual.Value.Date; data = data.AddDays(1))
            {
                //CarregarPrecReal(data, out _);
                chuvas[data] = CarregarPrecRealMedia(data, out _);

            }

            RefreshPrecipList();

            AddLog("- Precipitação MERGE Carregada");
        }

        public void CarregarPrecReal(DateTime data, out string modelo)
        {
            var mergeCtlFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".ctl", System.IO.SearchOption.AllDirectories);
            var mergeDatFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".dat", System.IO.SearchOption.AllDirectories);
            if (mergeCtlFile.Length > 0)
            {
                AddLog(mergeCtlFile[0]);

                var prec = PrecipitacaoFactory.BuildFromMergeFile(mergeCtlFile[0]);
                prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeCtlFile[0]);
                prec.Data = data;


                chuvas[data] = prec;

                modelo = "MERGE";

            }
            else if (mergeDatFile.Length > 0)
            {
                AddLog(mergeDatFile[0]);

                var prec = PrecipitacaoFactory.BuildFromEtaFile(mergeDatFile[0]);
                prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeDatFile[0]);
                prec.Data = data;

                chuvas[data] = prec;

                modelo = "MERGE";
            }
            else
            {
                AddLog("\tmerge para a data " + data.ToShortDateString() + " não encontrado");

                var funcfile = System.IO.Path.Combine(Config.CaminhoFunceme, data.Year.ToString("0000"), data.Month.ToString("00"), "funceme_" + data.ToString("yyyyMMdd") + ".ctl");

                if (
                    System.IO.File.Exists(funcfile) && (runAuto ||
                    MessageBox.Show("Merge para a data " + data.ToShortDateString() + " não encontrado.\r\nUsar funceme?", "Precip Observada - Chuva Vazão", MessageBoxButtons.YesNo)
                    == DialogResult.Yes))
                {
                    var prec = PrecipitacaoFactory.BuildFromMergeFile(funcfile);
                    prec.Descricao = System.IO.Path.GetFileNameWithoutExtension(funcfile);
                    prec.Data = data;
                    chuvas[data] = prec;
                    chuvas[data].Data = data;

                    modelo = "FUNCEME";
                }
                else if (runAuto || MessageBox.Show("Merge para a data " + data.ToShortDateString() + " não encontrado.\r\nUsar merge anterior?", "Precip Observada - Chuva Vazão", MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                    chuvas[data] = chuvas[data.AddDays(-1)].Duplicar();
                    chuvas[data].Data = data;

                    modelo = "MER-1";
                }
                else modelo = "NULO";
            }
        }

        public void SalvarVazObserv()
        {
            modelosChVz.ForEach(x => x.SalvarVazaoObservada());

            AddLog("- Arquivos de Vazao Observada Salva");
        }

        public Precipitacao CarregarPrecRealMedia(DateTime data, out string modelo)
        {
            Precipitacao merge = null;
            Precipitacao funceme = null;
            Precipitacao ons = null;

            //MERGE
            {

                var mergeCtlFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".ctl", System.IO.SearchOption.AllDirectories);
                var mergeDatFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".dat", System.IO.SearchOption.AllDirectories);
                if (mergeCtlFile.Length > 0)
                {
                    AddLog(mergeCtlFile[0]);

                    var prec = PrecipitacaoFactory.BuildFromMergeFile(mergeCtlFile[0]);
                    prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeCtlFile[0]);
                    prec.Data = data;


                    merge = prec;


                }
                else if (mergeDatFile.Length > 0)
                {
                    AddLog(mergeDatFile[0]);

                    var prec = PrecipitacaoFactory.BuildFromEtaFile(mergeDatFile[0]);
                    prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeDatFile[0]);
                    prec.Data = data;

                    merge = prec;

                }

            }

            //FUNCEME
            {
                var funcfile = System.IO.Path.Combine(Config.CaminhoFunceme, data.Year.ToString("0000"), data.Month.ToString("00"), "funceme_" + data.ToString("yyyyMMdd") + ".ctl");

                if (System.IO.File.Exists(funcfile))
                {
                    var prec = PrecipitacaoFactory.BuildFromMergeFile(funcfile);
                    prec.Descricao = System.IO.Path.GetFileNameWithoutExtension(funcfile);
                    prec.Data = data;
                    funceme = prec;
                    funceme.Data = data;

                }
            }

            //ONS
            {
                try
                {
                    var onsFile = $"P:\\Trading\\Acompanhamento Metereologico Semanal\\spiderman\\{data.ToString("yyyy_MM_dd")}\\ETA\\observado.gif";

                    if (merge != null && File.Exists(onsFile))
                    {
                        ons = PrecipitacaoFactory.BuildFromImage(onsFile);
                    }
                }
                finally { }
            }

            if (funceme == null && merge == null)
            {
                var ret = CarregarPrecRealMedia(data.AddDays(-1), out modelo);
                modelo += "-1";
                return ret;
            }
            else if (funceme == null)
            {
                if (ons != null)
                {
                    Precipitacao media = merge.Duplicar();

                    foreach (var v in merge.Prec.Keys)
                    {
                        var vf = ons[v];

                        if (vf > 0)
                        {
                            media[v] = (media[v] + vf) / 2;
                        }
                    }

                    media.Descricao = "Media Ons e Merge";
                    modelo = "ONSeMERG";
                    return media;
                }

                modelo = "MERG";
                return merge;
            }
            else if (merge == null)
            {
                modelo = "FUNC";
                return funceme;
            }
            else if (ons == null)
            {
                Precipitacao media = merge.Duplicar();

                foreach (var v in merge.Prec.Keys)
                {
                    var vf = funceme[v];

                    if (vf > 0)
                    {
                        media[v] = (media[v] + vf) / 2;
                    }
                }

                media.Descricao = "Media Func e Merge";
                modelo = "FUNCeMERG";
                return media;
            }
            else
            {
                Precipitacao media = merge.Duplicar();

                foreach (var v in merge.Prec.Keys)
                {
                    var vf = funceme[v];
                    var vo = ons[v];

                    media[v] = (media[v] + vf + vo) / 3;

                }

                media.Descricao = "Media Func, Merge e ONS";
                modelo = "FUNCeMERGeONS";
                return media;
            }
        }

        public void SelecionarSaida()
        {
            Ookii.Dialogs.VistaFolderBrowserDialog ofd = new Ookii.Dialogs.VistaFolderBrowserDialog();

            var currRevDate = dtAtual.Value;

            var nextRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(currRevDate);
            ofd.SelectedPath = @"H:\Middle - Preço\16_Chuva_Vazao\" + nextRev.revDate.ToString("yyyy_MM") + @"\RV" + nextRev.rev.ToString() + @"\" + DateTime.Now.ToString("yy-MM-dd") + @"\"; ;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ArquivosDeSaida = ofd.SelectedPath;
            }
        }

        void CriarCaso(RunStatus statusF = null)
        {

            if (statusF != null) statusF.Creation = RunStatus.statuscode.initialialized;

            var modelos = new string[] { "SMAP" };

            var dir = System.IO.Directory.GetDirectories(txtEntrada.Text);

            if (!Directory.Exists(txtCaminho.Text)) Directory.CreateDirectory(txtCaminho.Text);

            foreach (var d in dir)
            {
                var name = d.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last().ToUpperInvariant();

                if (modelos.Contains(name))
                {

                    AddLog("\tCopiando modelo: " + name);

                    if (Directory.Exists(Path.Combine(txtCaminho.Text, name))) Directory.Delete(Path.Combine(txtCaminho.Text, name), true);

                    SMAPDirectoryCopy(d, Path.Combine(txtCaminho.Text, name), true);
                }
            }

            if (statusF != null) statusF.Creation = RunStatus.statuscode.completed;
        }

        public void PreencherPrecObserv()
        {

            foreach (var prec in chuvas.Where(x => x.Key <= dtAtual.Value.Date))
            {
                foreach (var postoPlu in modelosChVz.SelectMany(x => x.PostosPlu))
                {

                    if (postoPlu.Preciptacao.Values.Any(x => x.HasValue))
                        postoPlu.Preciptacao[prec.Key] = prec.Value[postoPlu.Codigo];
                    else postoPlu.Preciptacao[prec.Key] = null;
                }
            }

            AddLog("- Preciptação Observada Carregada");
        }

        public void SalvarPrecObserv()
        {
            foreach (var modelo in modelosChVz)
            {
                modelo.SalvarPrecObservada();
            }

            foreach (var prec in chuvas.Where(x => x.Key <= dtAtual.Value.Date))
            {

                var raiznome = this.txtNomeChuvaPrev.Text + "p" + dtAtual.Value.Date.ToString("ddMMyy") + "a" + prec.Key.ToString("ddMMyy") + ".dat";
                prec.Value.SalvarModeloEta(System.IO.Path.Combine(this.ArquivosDeSaida, raiznome));
            }

            var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
            var chuvaMedia = remo.MediaBacias(chuvas.Where(x => x.Key <= dtAtual.Value.Date).ToDictionary(x => x.Key, x => x.Value));
            //chuvas = chuvaMedia;
            //RefreshPrecipList();
            var dadoslog = new StringBuilder();
            var header = "Precipitacao média";
            dadoslog.AppendLine(header);
            dadoslog.AppendLine("Bacia\t" + string.Join("\t", chuvaMedia.Keys.Select(x => x.ToString("yyyy-MM-dd"))));
            foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
            {
                dadoslog.Append(pCo.Key.Nome + "\t");
                dadoslog.AppendLine(string.Join("\t", pCo.First().precMedia.Select(x => x.ToString("0.00"))));
            }

            File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamediaObservada.log"), dadoslog.ToString());



            AddLog("Arquivos de Preciptação Observada Salvos");
        }

        public void PrecipitacaoPrevista()
        {
            var data = dtAtual.Value.Date;

            var modelo = "*";

            //carregar ou criar nova?

            Ookii.Dialogs.TaskDialog tskD1 = new Ookii.Dialogs.TaskDialog(this.components) { WindowTitle = "Precipitação Prevista" };
            var bN = new Ookii.Dialogs.TaskDialogButton { ButtonType = Ookii.Dialogs.ButtonType.Custom, Text = "Novo" };
            var bE = new Ookii.Dialogs.TaskDialogButton { ButtonType = Ookii.Dialogs.ButtonType.Custom, Text = "Existente" };
            var bR = new Ookii.Dialogs.TaskDialogButton { ButtonType = Ookii.Dialogs.ButtonType.Custom, Text = "Modelo R" };

            tskD1.Buttons.Add(bN);
            tskD1.Buttons.Add(bE);
            tskD1.Buttons.Add(bR);

            var existente = false;
            var modeloR = false;
            tskD1.ButtonClicked += (se, ev) =>
            {
                if (ev.Item == bE) existente = true;
                else if (ev.Item == bN) existente = false;
                else if (ev.Item == bR)
                {
                    PrecipitacaoPrevista_R();
                    modeloR = true;
                }
            };


            tskD1.ShowDialog();


            if (existente)
            {

                Ookii.Dialogs.VistaFolderBrowserDialog d = new Ookii.Dialogs.VistaFolderBrowserDialog();
                d.SelectedPath = System.IO.Path.Combine(Config.CaminhoPrevisao, data.ToString("yyyyMM"), data.ToString("dd"));
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    var searchPath = d.SelectedPath;
                    bool tryCtl = false;

                    for (int i = 1; i <= 30; i++)
                    {

                        var dataPrev = data.AddDays(i);
                        var raiznome = "p" + data.ToString("ddMMyy") + "a" + dataPrev.ToString("ddMMyy");

                        var prevFiles = System.IO.Directory.GetFiles(searchPath, modelo + raiznome + ".dat", SearchOption.TopDirectoryOnly);
                        string prevFile = null;
                        if (prevFiles.Length == 0 && modelo == "*")
                        {
                            //MessageBox.Show("Nenhuma previsão encontrada");
                            tryCtl = true;
                            break;
                        }
                        else if (prevFiles.Length == 0 && modelo != "*")
                        {

                            AddLog("   Precipitação Prevista não encontrada: " + modelo + raiznome + ".dat");
                            break;

                        }
                        else if (prevFiles.Length > 1 && modelo == "*")
                        {

                            dialogModeloPrev.RadioButtons.Clear();
                            prevFiles.ToList().ForEach(x =>
                                dialogModeloPrev.RadioButtons.Add(new Ookii.Dialogs.TaskDialogRadioButton()
                                {
                                    Text =
                                        x.Substring(x.LastIndexOf('\\') + 1, x.LastIndexOf(raiznome) - x.LastIndexOf('\\') - 1)
                                        + "\r\n" + x
                                }));
                            if (Ookii.Dialogs.TaskDialog.OSSupportsTaskDialogs)
                                dialogModeloPrev.ShowDialog(this);

                            modelo = dialogModeloPrev.RadioButtons.First(x => x.Checked).Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0].Trim();
                            prevFile = prevFiles.First(x => x.Contains(modelo + raiznome));
                        }
                        else prevFile = prevFiles[0];

                        if (modelo == "*")
                        {
                            modelo = prevFile.Substring(
                                prevFile.LastIndexOf('\\') + 1,
                                prevFile.LastIndexOf(raiznome) - prevFile.LastIndexOf('\\') - 1
                                );
                        }



                        chuvas[dataPrev] = PrecipitacaoFactory.BuildFromEtaFile(prevFile);
                        chuvas[dataPrev].Descricao = "PREV NUM - " + modelo + raiznome;
                    }

                    if (tryCtl)
                    {
                        var prevFiles = System.IO.Directory.GetFiles(searchPath, "*.ctl", SearchOption.TopDirectoryOnly);

                        if (prevFiles.Length > 0)
                        {
                            foreach (var prevFile in prevFiles)
                            {
                                var precip = PrecipitacaoFactory.BuildFromMergeFile(prevFile);

                                chuvas[precip.Data] = precip;

                                modelo = System.IO.Path.GetDirectoryName(prevFile).Split('\\').Last();
                                var raiznome = System.IO.Path.GetFileName(prevFile);

                                chuvas[precip.Data].Descricao = "PREV NUM - " + modelo + raiznome;
                            }
                        }
                    }
                }
            }
            else
            {
                if (modeloR == false)
                    GerarPrevisaoConjunto();
                //btnPrecConjunto_Click(sender, e);
            }
            RefreshPrecipList();

            AddLog("- Precipitação Prevista Carregada");
        }


        public void PrecipitacaoPrevista_R()
        {
            var data = dtAtual.Value.Date;

            var modelo = "*";

            var existente = false;


            existente = true;

            if (existente)
            {

                Ookii.Dialogs.VistaFolderBrowserDialog d = new Ookii.Dialogs.VistaFolderBrowserDialog();

                //d.SelectedPath = System.IO.Path.Combine(Config.CaminhoPrevisao, data.ToString("yyyyMM"), data.ToString("dd"));
                d.SelectedPath = System.IO.Path.Combine("H:\\Middle - Preço\\16_Chuva_Vazao\\2019_09\\RV3\\19 - 09 - 19\\Teste");
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {


                    var searchPath = d.SelectedPath;

                    for (int i = 1; i <= 30; i++)
                    {

                        var dataPrev = data.AddDays(i);
                        var raiznome = "p" + data.ToString("ddMMyy") + "a" + dataPrev.ToString("ddMMyy");
                        var prevFiles = System.IO.Directory.GetFiles(searchPath, "*" + raiznome + ".dat", SearchOption.TopDirectoryOnly);
                        var prevFiles_R = System.IO.Directory.GetFiles(searchPath, "*.dat", SearchOption.TopDirectoryOnly);
                        string prevFile = null;

                        Mapas_R(prevFiles);

                        if (prevFiles.Length == 0 && modelo == "*")
                        {
                            //MessageBox.Show("Nenhuma previsão encontrada");

                            break;
                        }
                        else if (prevFiles.Length == 0 && modelo != "*")
                        {

                            AddLog("   Precipitação Prevista não encontrada: " + modelo + raiznome + ".dat");
                            break;

                        }

                        else
                        {

                            prevFile = prevFiles[0];

                        }

                        if (modelo == "*")
                        {
                            modelo = "ETA40";

                        }

                        chuvas[dataPrev] = PrecipitacaoFactory.BuildFromEtaFile(prevFile);
                        chuvas[dataPrev].Descricao = "PREV NUM - " + "ETA40_" + raiznome;




                    }
                    RefreshPrecipList();
                    //  Ler();

                    //foreach (var prec in chuvas.Where(x => x.Key > dtAtual.Value.Date))
                    //{
                    //    /*if (prec.Key == DateTime.Today.AddDays(4))
                    //    {

                    //    }*/

                    //    var raiznome1 = this.txtNomeChuvaPrev.Text + "p" + dtAtual.Value.Date.ToString("ddMMyy") + "a" + prec.Key.ToString("ddMMyy") + ".dat";
                    //    prec.Value.SalvarModeloEta(System.IO.Path.Combine(this.ArquivosDeSaida, raiznome1));
                    //}



                    //foreach (var modelo1 in modelosChVz)
                    //{
                    //    modelo1.DataPrevisao = dtAtual.Value.Date;
                    //    modelo1.SalvarPrecPrevista(chuvas);
                    //    modelo1.SalvarParametros();
                    //}
                }


            }
            else
            {

                GerarPrevisaoConjunto();
                //btnPrecConjunto_Click(sender, e);
            }

            MessageBox.Show("Carregados com Sucesso!");



        }

        public void GravarPrec()
        {
            if (listView_PrecPrev.SelectedItems.Count == 1)
            {

                var i = listView_PrecPrev.SelectedItems[0] as PrecipitacaoItemView;

                SaveFileDialog fd = new SaveFileDialog();
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //i.Prec.Salvar(fd.FileName);

                    i.Prec.SalvarModeloBin(fd.FileName);

                }
                // PrevViewer.ShowViewer(i.Prec, "Previsao " + i.Prec.Data.ToString("dd-MM-yyyy"));

            }
            else
                MessageBox.Show("Selecione uma chuva");
        }

        public void AtualizarAcompHBD()
        {
            PreencherVazObservada(out _, out _);
        }

        public void SalvarPrecPrev()
        {
            if (chuvas.Count == 0)
            {
                AddLog("Selecione as chuvas");
            }

            var img = false;
            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                SelecionarSaida();
                img = true;
            }

            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                return;
            }

            if (img)
            {
                this.Busy = true;

                foreach (var prec in chuvas.Where(x => x.Key > dtAtual.Value.Date))
                {
                    PrecipitacaoFactory.SalvarModeloBin(prec.Value,
                        System.IO.Path.Combine(this.ArquivosDeSaida,
                        "pp" + dtAtual.Value.ToString("yyyyMMdd") + "_" + ((prec.Key - dtAtual.Value).TotalHours).ToString("0000")
                        )
                    );
                }

                cptec.CreateCustomImages(dtAtual.Value, this.ArquivosDeSaida, this.txtNomeChuvaPrev.Text);

                var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
                //var dic = new Dictionary<DateTime, Precipitacao>();
                // dic[pr.Data] = pr;
                ///
                var chuvaMEDIA = remo.ConjuntoLivre(chuvas, null);
                /// 
                //var chuvaMedia = remo.MediaBacias(chuvas);

                var dadoslog = new StringBuilder();

                var header = "Precipitacao média";

                dadoslog.AppendLine(header);
                foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ"))
                {
                    dadoslog.Append(pCo.Agrupamento.Nome + "\t" + pCo.Nome + "\t");
                    dadoslog.AppendLine(string.Join("\t", pCo.precMedia.Select(x => x.ToString("0.00"))));
                }

                File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamedia.log"), dadoslog.ToString());

                this.ArquivosDeSaida = "";

                this.Busy = false;
            }
            else
            {

                foreach (var prec in chuvas.Where(x => x.Key > dtAtual.Value.Date))
                {
                    /*if (prec.Key == DateTime.Today.AddDays(4))
                    {

                    }*/

                    var raiznome = this.txtNomeChuvaPrev.Text + "p" + dtAtual.Value.Date.ToString("ddMMyy") + "a" + prec.Key.ToString("ddMMyy") + ".dat";
                    prec.Value.SalvarModeloEta(System.IO.Path.Combine(this.ArquivosDeSaida, raiznome));
                }



                foreach (var modelo in modelosChVz)
                {
                    modelo.DataPrevisao = dtAtual.Value.Date;
                    modelo.SalvarPrecPrevista(chuvas);
                    modelo.SalvarParametros();
                }

                var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
                var chuvaMedia = remo.MediaBacias(chuvas.Where(x => x.Key > dtAtual.Value.Date).ToDictionary(x => x.Key, x => x.Value));
                //chuvas = chuvaMedia;
                //RefreshPrecipList();
                var dadoslog = new StringBuilder();
                var header = "Precipitacao média";
                dadoslog.AppendLine(header);
                dadoslog.AppendLine("Bacia\t" + string.Join("\t", chuvaMedia.Keys.Select(x => x.ToString("yyyy-MM-dd"))));
                foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
                {
                    dadoslog.Append(pCo.Key.Nome + "\t");
                    dadoslog.AppendLine(string.Join("\t", pCo.First().precMedia.Select(x => x.ToString("0.00"))));
                }

                File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamedia.log"), dadoslog.ToString());

            }

            AddLog("- Precipitação Prevista Salva");
        }

        public async void GerarPrevisaoConjunto()
        {
            var data = dtAtual.Value.Date;

            //WaitForm2 form = await WaitForm2.ShowAsync(data);

            WaitForm2 form = await WaitForm2.ShowAsync(data);


            if (form.DialogResult != System.Windows.Forms.DialogResult.OK) return;


            Dictionary<DateTime, Precipitacao> chuvaConjunto = form.ChuvaConjunto;

            foreach (var c in chuvaConjunto)
            {
                /*foreach (var teste in c.Value.Prec)
                {
                    if (teste.Value < 0) c.Value.Prec[teste.Key] = 0;     //  teste.Value = 0;
                }*/

                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();
            await Task.Yield();
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            textLogger = new TextBoxLogger(txtLogPrecip, this);

            Ookii.Dialogs.VistaFolderBrowserDialog ofd = new Ookii.Dialogs.VistaFolderBrowserDialog();

            #region Caminho de Entrada
            var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(DateTime.Today);

            var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

            if (Directory.Exists(pastaBase))
            {
                ofd.SelectedPath = System.IO.Path.Combine(Config.CaminhoInicialEntrada, currRev.revDate.ToString("MM_yyyy"));

                this.ArquivosDeEntradaModelo = System.IO.Path.Combine(pastaBase, "Modelos_Chuva_Vazao");
                this.ArquivosDeEntradaPrevivaz = System.IO.Path.Combine(pastaBase, "Previvaz", "Arq_Entrada");
                this.ArquivoPrevsBase = System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories)[0];
            }


            #endregion

            #region Caminho de saída


            var nextRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(dtAtual.Value);
            this.ArquivosDeSaida = @"H:\Middle - Preço\16_Chuva_Vazao\" + nextRev.revDate.ToString("yyyy_MM") + @"\RV" + nextRev.rev.ToString() + @"\" + DateTime.Now.ToString("yy-MM-dd") + @"\"; ;


            #endregion
        }

        private Version GetRunningVersion()
        {

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {

                var curV = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;

                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += (object sender, System.Deployment.Application.CheckForUpdateCompletedEventArgs e) =>
                {
                    if (e.UpdateAvailable)
                        MessageBox.Show("Nova versão disponível (" + e.AvailableVersion.ToString() + "), reinicie o aplicativo para instalar.");
                };
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();

                return curV;

            }
            else
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        }

        //ler precipitacao obsr
        private void button7_Click(object sender, EventArgs e)
        {

            var diag = new Ookii.Dialogs.VistaFolderBrowserDialog();



            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var path = diag.SelectedPath;

                foreach (var postoPlu in modelosChVz.SelectMany(x => x.PostosPlu))
                {

                    var existingFiles = System.IO.Directory.GetFiles(path,
                   postoPlu.Codigo + "_c.txt",
                    System.IO.SearchOption.AllDirectories);

                    if (existingFiles.Length > 0)
                    {
                        var exFile = existingFiles[0];

                        using (var onsFile = System.IO.File.OpenText(exFile))
                        {
                            while (!onsFile.EndOfStream)
                            {

                                var l = onsFile.ReadLine();
                                DateTime dt;
                                if (l.Split(' ').Length > 1 &&
                                    DateTime.TryParseExact(l.Split(' ')[1], "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out dt)
                                    )
                                {
                                    postoPlu.Preciptacao[dt] =
                                        float.Parse(l.Split(' ')[3].Replace("-", "0"), System.Globalization.NumberFormatInfo.InvariantInfo);

                                }
                            }
                        }
                    }
                }
            }
        }

        #region Partes
        public void ParteA()
        {
            try
            {
                Busy = true;
                progressoParte.Value = 10;

                ClearLog();
                progressoParte.Value += 10;

                CriarCaso();
                progressoParte.Value += 10;

                Ler();
                progressoParte.Value += 10;

                CarregarPrecObserv();
                progressoParte.Value += 10;

                PreencherPrecObserv();
                progressoParte.Value += 10;

                PreencherVazObservada(out DateTime dtVaz, out _);
                progressoParte.Value += 10;

                PrecipitacaoPrevista();
                progressoParte.Value += 10;

                dtAtual.Value = dtVaz.AddDays(1);
                dtModelo.Value = dtAtual.Value.Date;

                Reiniciar(dtModelo.Value);
                progressoParte.Value = this.progressoParte.Maximum;

                //MessageBox.Show("Executar Parte B quando pronto");
                AddLog(" --- ");
                AddLog(" --- Executar Parte B quando pronto --- ");

                listLogs.SelectedIndex = this.listLogs.Items.Count - 1;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                progressoParte.Value = 0;
                Busy = false;
            }
        }

        public async void ParteB()
        {
            try
            {
                this.Busy = true;
                PreencherPrecObserv();
                progressoParte.Value += 20;
                SalvarPrecObserv();
                progressoParte.Value += 20;
                SalvarPrecPrev();
                progressoParte.Value += 20;
                SalvarVazObserv();
                progressoParte.Value += 20;

                await ExecutarTudoAsync();

                //btnColetarResultados_Click(sender, e);

                progressoParte.Value += 10;

                AddLog(" --- ");
                AddLog(" --- Parte B Concluída --- ");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Busy = false;
                progressoParte.Value = 0;
            }
        }

        public void ParteC()
        {
            try
            {
                var app = Helper.StartExcel();
                progressoParte.Value += 10;

                ColetaDeResultados(app, out Microsoft.Office.Interop.Excel.Workbook wb);
                progressoParte.Value += 30;

                wb.Activate();
                progressoParte.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                progressoParte.Value = 0;
            }
        }

        public async void ParteB_R()
        {
            try
            {
                Renomear_Eta40();
                this.Busy = true;
                PreencherPrecObserv();
                progressoParte.Value += 20;
                SalvarPrecObserv_R();
                progressoParte.Value += 20;
                SalvarPrecPrev_R();
                progressoParte.Value += 20;
                SalvarVazObserv();
                progressoParte.Value += 20;

                await ExecutarTudoAsync();

                //btnColetarResultados_Click(sender, e);

                progressoParte.Value += 10;

                AddLog(" --- ");
                AddLog(" --- Parte B Concluída --- ");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Busy = false;
                progressoParte.Value = 0;
            }
        }

        #endregion

        private void btnCarregarPrecObserv_Click(object sender, EventArgs e)
        {
            CarregarPrecObserv();
        }

        void ExecutarTudo(RunStatus statusF = null)
        {

            if (statusF != null) statusF.Execution = RunStatus.statuscode.initialialized;

            Parallel.ForEach(modelosChVz, x =>
            {
                AddLog("\t Executando: " + x.Caminho);
                x.Executar();

                if (x.ErroNaExecucao == true)
                {
                    AddLog(x.Caminho + " não executado");
                    File.AppendAllText(Path.Combine(txtCaminho.Text, "error.log"), x.Caminho + " não executado");
                }
                else
                {
                    x.ColetarSaida();
                    AddLog("\t Finalizado: " + x.Caminho);
                }

            });

            if (statusF != null) statusF.Execution =
                    modelosChVz.All(x => x.ErroNaExecucao == false) ? RunStatus.statuscode.completed : RunStatus.statuscode.error;

            ;

        }
        private async Task ExecutarTudoAsync()
        {

            //await Task.Delay(5000);
            var execs = modelosChVz.Select(async x =>
            {
                AddLog("\t Executando: " + x.Caminho);
                await x.ExecutarAsync();

                if (x.ErroNaExecucao == true)
                {
                    AddLog(x.Caminho + " não executado");
                    File.AppendAllText(Path.Combine(txtCaminho.Text, "error.log"), x.Caminho + " não executado");
                    MessageBox.Show(x.Caminho + " não executado", "Execução CHUVA VAZÃO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    x.ColetarSaida();
                    AddLog("\t Finalizado: " + x.Caminho);
                }
            });
            await Task.WhenAll(execs);
        }

        private async void btnExecutarTudo_Click(object sender, EventArgs e)
        {
            try
            {

                this.Busy = true;

                await ExecutarTudoAsync();
                MessageBox.Show("Execução Finalizada");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
            finally
            {
                this.Busy = false;
                AddLog("- Execução Finalizada");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {


            var iniVazao = "INICIALIZACAO_VAZAO.txt";

            var vazInicialConfigs = System.IO.File.ReadLines(iniVazao)
                .Where(x => x.Length >= 115 && x[0] != '#')
                .Select(x => new
                {
                    Arquivo = x.Substring(0, 36).Trim(),
                    ArquivoONS = x.Substring(36, 68).Trim(),
                    Posto = x.Substring(104, 3).Trim(),
                    PostoTipo = x.Substring(111, 3).Trim(),
                });




            var diag = new Ookii.Dialogs.VistaFolderBrowserDialog();

            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var path = diag.SelectedPath;

                var arqus = modelosChVz.SelectMany(x => x.Vazoes).Join(vazInicialConfigs,
                    x => System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant(), y => y.Arquivo.ToUpperInvariant(), (x, y) => new { entrada = x, arqONS = y.ArquivoONS });

                foreach (var arqEntrada in arqus)
                {

                    var existingFiles = System.IO.Directory.GetFiles(path,
                    arqEntrada.arqONS,
                    System.IO.SearchOption.AllDirectories);

                    if (existingFiles.Length > 0)
                    {
                        var exFile = existingFiles[0];
                        var arqONS = new VazoesRealizadas(exFile);

                        foreach (var vaz in arqONS.Vazoes)
                        {
                            arqEntrada.entrada.Vazoes[vaz.Key] = arqONS.Vazoes[vaz.Key];
                        }

                        arqEntrada.entrada.SalvarVazoes();
                    }
                }
            }

            MessageBox.Show("OK");
        }

        public object[,] ColetarResultado()
        {
            try
            {
                modelosChVz.ForEach(x => x.ColetarSaida());

                var vaz = modelosChVz.SelectMany(x => x.Vazoes).ToList();
                var minData = vaz.Min(x => x.Vazoes.Keys.Min());
                var maxData = vaz.Max(x => x.Vazoes.Keys.Max());

                //s += "\t" + string.Join("\t", vaz.Select(x => x.Nome)) + Environment.NewLine;

                int rows = (int)(maxData - minData).TotalDays + 1;
                int cols = vaz.Count();

                object[,] results = new object[rows + 1, cols + 1];

                for (int i = 0; i < cols; i++)
                {
                    results[0, i + 1] = vaz[i].Nome;
                }

                for (int d = 0; d < rows; d++)
                {
                    var dt = minData.AddDays(d);
                    results[d + 1, 0] = dt;
                    for (int i = 0; i < cols; i++)
                    {
                        if (vaz[i].Vazoes.ContainsKey(dt))
                        {
                            results[d + 1, i + 1] = vaz[i].Vazoes[dt];
                        }
                    }
                }
                AddLog("- Resultados coletados");

                return results;
            }
            catch (Exception e)
            {
                AddLog("\t" + "Erro no método FrmMain/ColetarResultado: " + e.Message);
                return null;
            }
        }

        private void btnCopiarResultados_Click(object sender, EventArgs e)
        {
            CopiarResultados();
        }

        private void btnSalvarVazObserv_Click(object sender, EventArgs e)
        {
            SalvarVazObserv();
        }

        private void btnAtualizarAcomphTXT_Click(object sender, EventArgs e)
        {
            var vazInicialConfigs = System.IO.File.ReadLines(Config.IniVazao)
                .Where(x => x.Length >= 115 && x[0] != '#')
                .Select(x => new
                {
                    Arquivo = x.Substring(0, 36).Trim(),
                    ArquivoONS = x.Substring(36, 68).Trim(),
                    Posto = x.Substring(104, 3).Trim(),
                    PostoTipo = x.Substring(111, 3).Trim(),
                    Vazoes = x.Substring(114).Trim()
                });

            foreach (var arqEntrada in modelosChVz.SelectMany(x => x.Vazoes).Join(vazInicialConfigs,
                x => System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant(), y => y.Arquivo.ToUpperInvariant(), (x, y) => new { entrada = x, posto = y.Posto, vazoes = y.Vazoes }))
            {
                if (!string.IsNullOrWhiteSpace(arqEntrada.posto))
                {
                    var dataIni = DateTime.ParseExact(
                        arqEntrada.vazoes.Split(' ')[0],
                        "yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                    var v = arqEntrada.vazoes.Split(' ').Skip(1).ToArray();

                    for (int i = 0; i < v.Length; i++)
                    {
                        var vazArq = float.Parse(v[i]);

                        if (vazArq > 0) arqEntrada.entrada.Vazoes[dataIni.AddDays(i)] = vazArq;
                    }
                }
            }
            AddLog("- Vazoes Passadas Carregadas de ACOMPH/RDH");
        }

        private void ClearLog()
        {
            this.listLogs.Items.Clear();
            progressoParte.Value = 0;
        }


        private static void SMAPDirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (dir.Name.Equals("Arq_Pos_Processamento", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

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
            if (dir.Name.Equals("ARQ_ENTRADA", StringComparison.OrdinalIgnoreCase))
            {
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (!file.Extension.EndsWith("dat", StringComparison.OrdinalIgnoreCase))
                    {
                        // Create the path to the new copy of the file.
                        string temppath = Path.Combine(destDirName, file.Name);

                        // Copy the file.
                        file.CopyTo(temppath, true);
                    }
                }
            }

            if (dir.Name.Equals("ARQ_SAIDA", StringComparison.OrdinalIgnoreCase))
            {
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Name.EndsWith("_AJUSTE.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        // Create the path to the new copy of the file.
                        string temppath = Path.Combine(destDirName, file.Name);

                        // Copy the file.
                        file.CopyTo(temppath, true);
                    }
                }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    SMAPDirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
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

        #region Partes de execução

        private void btnParteA_Click(object sender, EventArgs e)
        {
            ParteA();
        }

        private void btnParteB_Click(object sender, EventArgs e)
        {
            ParteB();
        }

        private void btnParteC_Click(object sender, EventArgs e)
        {
            ParteC();
        }

        #endregion

        private void btnSelecionarEntrada_Click(object sender, EventArgs e)
        {
            Ookii.Dialogs.VistaFolderBrowserDialog ofd = new Ookii.Dialogs.VistaFolderBrowserDialog();

            ofd.SelectedPath = Config.CaminhoInicialEntrada;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ArquivosDeEntradaModelo = System.IO.Path.Combine(ofd.SelectedPath, "Modelos_Chuva_Vazao");
                this.ArquivosDeEntradaPrevivaz = System.IO.Path.Combine(ofd.SelectedPath, "Previvaz", "Arq_Entrada");
                this.ArquivoPrevsBase = System.IO.Directory.GetFiles(ofd.SelectedPath, "prevs.*", SearchOption.AllDirectories)[0];
            }
        }

        private void btnSelecionarSaida_Click(object sender, EventArgs e)
        {
            SelecionarSaida();

            //Consulta("");
        }



        //private string _error;
        //private void Consulta(string message)
        //{
        //    string _return = string.Empty;


        //    try
        //    {
        //        this._error = "";

        //        //Criando o bind dos parâmentros que serão passados para o API
        //        //e convertendo em ByteArray para populado no corpo do Request 
        //        //para o Server
        //       // string _paramenterText = bindParmeters(pMethod, pParameters);
        //        //_body = Encoding.UTF8.GetBytes(_paramenterText);

        //        //Chamando função que criptografará os parâmentros a serem enviados
        //        _sign = cripParametersSign(_paramenterText);

        //        //Criando Metodo de Request para o Servidor do Mercado Bitcoin
        //        WebRequest request = null;
        //        request = WebRequest.Create(_REQUEST_HOST + _REQUEST_PATH);

        //        request.Method = "POST";
        //        request.Headers.Add("tapi-id", _MB_TAPI_ID);
        //        request.Headers.Add("tapi-mac", _sign);
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.ContentLength = _body.Length;
        //        request.Timeout = 360000;

        //        //Escrevendo parâmentros no corpo do Request para serem enviados a API
        //        Stream _req = request.GetRequestStream();
        //        _req.Write(_body, 0, _body.Length);
        //        _req.Close();

        //        //Pegando retorno do servidor
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        Stream dataStream = response.GetResponseStream();

        //        //Convertendo Stream de retorno em texto para 
        //        //Texto de retorno será um JSON 
        //        using (StreamReader reader = new StreamReader(dataStream))
        //            _return = reader.ReadToEnd();

        //        //Liberando objetos para o Coletor de Lixo
        //        dataStream.Close();
        //        dataStream.Dispose();
        //        response.Close();
        //        response.Dispose();

        //    }
        //    catch (Exception ex)
        //    {
        //        //this._error = ex.Message;
        //        _return = "";
        //    }
        //}

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }



        private void AddLog(String text)
        {
            lock (this)
            {
                listLogs.Items.Add(text);
                if (listLogs.Items.Count > 0) listLogs.SelectedIndex = listLogs.Items.Count - 1;
            }
        }

        private void btnbtnCriarCaso_Click(object sender, EventArgs e)
        {
            CriarCaso();
        }

        private void ColetaDeResultados(Excel.Application app, out Excel.Workbook wb)
        {
            if (modelosChVz.Count == 0) Ler();

            //new ExecutingProcess().ProcessResults(modelosChVz);


            //wb = null;

            //return;
            //ExecutingProcess proc = new ExecutingProcess();


            var excelFile = Config.XltmResultado;

            wb = app.Workbooks.Add(excelFile);
            while (!app.Ready)
            {
                System.Threading.Thread.Sleep(200);
            }

            var ws = wb.Worksheets["RESULTADOS"] as Microsoft.Office.Interop.Excel.Worksheet;

            ws.Select();
            ws.UsedRange.ClearContents();

            var res = ColetarResultado();
            //modelosChVz



            ws.Range[ws.Cells[1, 1], ws.Cells[res.GetLength(0), res.GetLength(1)]].Value2 = res;

            ws = wb.Worksheets["Aux"] as Microsoft.Office.Interop.Excel.Worksheet;
            (ws.Range[ws.Cells[3, 2], ws.Cells[3, 2]] as Excel.Range).Value2 = this.ArquivoPrevsBase;
            (ws.Range[ws.Cells[4, 2], ws.Cells[4, 2]] as Excel.Range).Value2 = this.ArquivosDeEntradaPrevivaz;

            (ws.Range[ws.Cells[1, 2], ws.Cells[1, 2]] as Excel.Range).Value2 = this.DataSemanaPrevsBase ?? this.dtModelo.Value;

            ws = wb.Worksheets["PREVS_SMAP"] as Microsoft.Office.Interop.Excel.Worksheet;
            (ws.Range[ws.Cells[1, 2], ws.Cells[1, 2]] as Excel.Range).Value2 = this.dtModelo.Value;

            foreach (dynamic conn in wb.Connections)
            {
                conn.Refresh();
            }
        }

        #region Precipitação

        private void btnPreencherPrecObserv_Click(object sender, EventArgs e)
        {
            PreencherPrecObserv();
        }

        private void btnSalvarPrecObserv_Click(object sender, EventArgs e)
        {
            SalvarPrecObserv();
        }

        private void PrecipitacaoObesvAlternativa()
        {
            bool sobreescrever = false;

            for (DateTime data = chuvas.Min(x => x.Key); data <= DateTime.Today; data = data.AddDays(1))
            {

                foreach (var postoPlu in modelosChVz.Where(x => x is SMAP.ModeloSmap).SelectMany(x => x.PostosPlu))
                {
                    if (sobreescrever || !postoPlu.Preciptacao.ContainsKey(data)) postoPlu.Preciptacao[data] = 2;
                }
            }

            AddLog("- Preciptação Observada ALTERNATIVA Carregada");

            foreach (var modelo in modelosChVz)
            {
                modelo.SalvarPrecObservada();
            }

            MessageBox.Show("Arquivos de Preciptação Observada Salvos");
        }

        private void btnCarregarPrecPrev_Click(object sender, EventArgs e)
        {
            PrecipitacaoPrevista();
        }

        private void RefreshPrecipList()
        {
            listView_PrecPrev.Items.Clear();
            listView_PrecPrev.Items.AddRange(
                chuvas.Select(c => new PrecipitacaoItemView(c.Value) { DataChuva = c.Key, Descricao = c.Value.Descricao })
                .OrderBy(x => x.DataChuva)
                .ToArray()
                );
        }

        private void btnSalvarPrecPrev_Click(object sender, EventArgs e)
        {
            SalvarPrecPrev();
        }

        private void btnEditarPrecip_Click(object sender, EventArgs e)
        {
            if (listView_PrecPrev.SelectedItems.Count == 1)
            {
                var i = listView_PrecPrev.SelectedItems[0] as PrecipitacaoItemView;
                PrevViewer.ShowViewer(i.Prec, "Previsao " + i.Prec.Data.ToString("dd-MM-yyyy"));
            }
        }

        private void btnGravarPrec_Click(object sender, EventArgs e)
        {
            GravarPrec();
        }

        private void btnDeletarPrec_Click_2(object sender, EventArgs e)
        {
            if (listView_PrecPrev.SelectedItems.Count == 1)
            {
                var i = listView_PrecPrev.SelectedItems[0] as PrecipitacaoItemView;
                chuvas.Remove(i.DataChuva);
                RefreshPrecipList();
            }
            else
                MessageBox.Show("Selecione uma chuva");
        }

        private void btnCopiarPrecip_Click(object sender, EventArgs e)
        {
            if (listView_PrecPrev.SelectedItems.Count == 1)
            {
                var i = listView_PrecPrev.SelectedItems[0] as PrecipitacaoItemView;
                var dataSel = i.DataChuva;
                var dataInicio = chuvas.Keys.Max().AddDays(1);

                for (int d = 0; d < (dataInicio - dataSel).TotalDays; d++)
                {

                    var np = chuvas[dataSel.AddDays(d)].Duplicar();
                    np.Data = dataInicio.AddDays(d);
                    np.Descricao = "Cópia - " + np.Descricao;
                    chuvas.Add(np.Data, np);
                }

                RefreshPrecipList();
            }
            else
                MessageBox.Show("Selecione uma chuva");
        }

        private void btnVerTudoPrecip_Click(object sender, EventArgs e)
        {
            if (listView_PrecPrev.SelectedItems.Count == 1)
            {
                var chuvasimages = chuvas
                //.Where(x => x.Key > this.dtModelo.Value)
                .OrderBy(c => c.Key)
                .Select(c => c.Value)
                .ToList();

                PrevViewer.ShowViewer(new IEnumerable<Precipitacao>[] { chuvasimages }, this);
            }
            else
                MessageBox.Show("Selecione uma chuva");
        }

        #endregion

        private void PreencherVazObservada(out DateTime ultimaDataDisponivel, out string fonte)
        {

            var iniVazao = Config.PostosFlu;


            var vazInicialConfigs = System.IO.File.ReadLines(iniVazao)
                .Select(x => x.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length >= 5)
                .Select(x =>
                {
                    var configFlu = new
                    {
                        Arquivo = x[0].Trim(),
                        TipoAtualizacao = x[1].Trim(),
                        Origem = new List<(float Fator, int Posto, string Tipo)>(),
                    };


                    for (int i = 0; i < (x.Length - 2) / 3; i++)
                    {
                        configFlu.Origem.Add((
                            float.Parse(x[2 + i * 3], System.Globalization.NumberFormatInfo.InvariantInfo),
                            int.Parse(x[3 + i * 3]),
                            x[4 + i * 3].Trim()
                            ));
                    }

                    return configFlu;
                }
                ).ToList();



            var vazInicialConfigsVazias = System.IO.File.ReadLines(iniVazao)
                .Select(x => x.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length == 1)
                .Select(x => new
                {
                    Arquivo = x[0].Trim()
                }).ToList();


            //ajuste de parametros de modelos PARCIAIS
            foreach (var vazConfig in vazInicialConfigs.ToList().Where(x => x.TipoAtualizacao == "COMPOSTA").GroupBy(x => x.Origem[0].Posto))
            {
                var arquivosEntrada = modelosChVz.SelectMany(x => x.Vazoes).Where(x => vazConfig.Select(y => y.Arquivo.ToUpperInvariant()).Contains(System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant()));
                var totalDic = arquivosEntrada.SelectMany(x => x.Vazoes).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Value));


                foreach (var arquEntrada in arquivosEntrada)
                {

                    var f = arquEntrada.Vazoes.Sum(x => x.Value) / totalDic.Sum(x => x.Value);

                    var oldConfig = vazInicialConfigs.Where(x => x.Arquivo == System.IO.Path.GetFileName(arquEntrada.CaminhoArquivo).ToUpperInvariant()).First();
                    oldConfig.Origem.Add((f, oldConfig.Origem[0].Posto, oldConfig.Origem[0].Tipo));
                    oldConfig.Origem.RemoveAt(0);
                }
            }


            var dataI = dtAtual.Value.Date.AddDays(-31);
            ultimaDataDisponivel = dataI;

            List<CONSULTA_VAZAO> dados = null;


            if (Config.FonteVazao.Trim().Equals("db", StringComparison.OrdinalIgnoreCase))
            {
                using (var ctx = new IPDOEntities1())
                {
                    dados = ctx.CONSULTA_VAZAO.Where(x => x.data >= dataI).ToList();
                }
            }
            else
            {
                dados = ReadVazoesPassadas(Config.HistoricoVazao);
            }

            //checar disponibilidade de dados:
            for (DateTime dt = dataI; dt < dtAtual.Value; dt = dt.AddDays(1))
                if (!dados.Any(x => x.data == dt))
                {
                    var txt = "  Vazoes Passadas Não encontradas para o dia " + dt.ToString("dd/MM/yyyy");
                    AddLog(txt);
                    if (!runAuto) MessageBox.Show(txt);
                }
                else
                {
                    ultimaDataDisponivel = dt;
                }

            fonte = dados.Where(x => x.posto == 1).OrderByDescending(x => x.data).First().fonte;

            foreach (var arqEntrada in modelosChVz.SelectMany(x => x.Vazoes).Join(vazInicialConfigs,
                x => System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant(),
                y => y.Arquivo.ToUpperInvariant(), (x, y) => new { posto = x, config = y }))
            {

                for (DateTime dt = dataI; dt < dtAtual.Value; dt = dt.AddDays(1))
                {
                    if (arqEntrada.config.Origem.All(y => dados.Any(x => x.data == dt && y.Posto == x.posto)))
                    {
                        if (arqEntrada.config.TipoAtualizacao == "TOTAL" || (dtAtual.Value.Date - dt).TotalDays <= 7)
                        {
                            var vazAcomph =
                            arqEntrada.config.Origem.Select(ori =>
                            {
                                var value = ori.Tipo == "NAT" ?
                                    dados.First(x => x.data == dt && x.posto == ori.Posto).qnat.Value
                                    : dados.First(x => x.data == dt && x.posto == ori.Posto).qinc.Value;
                                value = value < 0 ? 0 : value;
                                value = (int)(value * ori.Fator);

                                return value;
                            }).Sum();
                            arqEntrada.posto.Vazoes[dt] = vazAcomph;
                        }
                    }
                }

                //arqEntrada.posto.SalvarVazoes();
            }

            foreach (var arqEntrada in modelosChVz.SelectMany(x => x.Vazoes).Join(vazInicialConfigsVazias,
                x => System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant(),
                y => y.Arquivo.ToUpperInvariant(), (x, y) => new { posto = x, config = y }))
            {
                for (DateTime dt = dataI; dt < dtAtual.Value; dt = dt.AddDays(1))
                {

                    if (!arqEntrada.posto.Vazoes.ContainsKey(dt) && arqEntrada.posto.Vazoes.ContainsKey(dt.AddDays(-1)))
                    {
                        arqEntrada.posto.Vazoes[dt] = arqEntrada.posto.Vazoes[dt.AddDays(-1)];
                    }
                    else if (!arqEntrada.posto.Vazoes.ContainsKey(dt))
                    {
                        arqEntrada.posto.Vazoes[dt] = arqEntrada.posto.Vazoes.Average(x => x.Value);
                    }
                }
            }


            AddLog("- Vazoes Passadas Carregadas de ACOMPH/RDH");
        }

        private void btnAtualizarAcompHBD_Click(object sender, EventArgs e)
        {
            AtualizarAcompHBD();
        }

        private void btnAtualizarRDHBD_Click(object sender, EventArgs e)
        {
            var iniVazao = Config.PostosFlu;

            var vazInicialConfigs = System.IO.File.ReadLines(iniVazao)
                .Select(x => x.Split('\t'))
                .Where(x => x.Length >= 5)
                .Select(x => new
                {
                    Arquivo = x[0].Trim(),
                    Fator = float.Parse(x[1], System.Globalization.NumberFormatInfo.InvariantInfo),
                    Posto = int.Parse(x[2]),
                    PostoTipo = x[3].Trim(),
                    TipoAtualizacao = x[4].Trim()
                }).ToList();


            var dataI = dtAtual.Value.Date.AddDays(-31);

            List<CONSULTA_VAZAO_RDH> dados = null;

            using (var ctx = new IPDOEntities1())
            {
                dados = ctx.CONSULTA_VAZAO_RDH.Where(x => x.data >= dataI).ToList();
            }

            //checar disponibilidade de dados:
            for (DateTime dt = dataI; dt < dtAtual.Value; dt = dt.AddDays(1))
                if (!dados.Any(x => x.data == dt))
                {
                    var txt = "  Vazoes Passadas Não encontradas para o dia " + dt.ToString("dd/MM/yyyy");
                    AddLog(txt);
                    MessageBox.Show(txt);
                }

            //ajuste de parametros de modelos PARCIAIS
            foreach (var vazConfig in vazInicialConfigs.ToList().Where(x => x.TipoAtualizacao == "COMPOSTA").GroupBy(x => x.Posto))
            {
                var arquivosEntrada = modelosChVz.SelectMany(x => x.Vazoes).Where(x => vazConfig.Select(y => y.Arquivo.ToUpperInvariant()).Contains(System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant()));
                var totalDic = arquivosEntrada.SelectMany(x => x.Vazoes).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Value));


                foreach (var arquEntrada in arquivosEntrada)
                {

                    var f = arquEntrada.Vazoes.Sum(x => x.Value) / totalDic.Sum(x => x.Value);

                    var oldConfig = vazInicialConfigs.Where(x => x.Arquivo == System.IO.Path.GetFileName(arquEntrada.CaminhoArquivo).ToUpperInvariant()).First();

                    vazInicialConfigs.Remove(oldConfig);
                    vazInicialConfigs.Add(new { oldConfig.Arquivo, Fator = f, oldConfig.Posto, oldConfig.PostoTipo, oldConfig.TipoAtualizacao });
                }
            }

            foreach (var arqEntrada in modelosChVz.SelectMany(x => x.Vazoes).Join(vazInicialConfigs,
                x => System.IO.Path.GetFileName(x.CaminhoArquivo).ToUpperInvariant(),
                y => y.Arquivo.ToUpperInvariant(), (x, y) => new { posto = x, config = y }))
            {

                for (DateTime dt = dataI; dt < dtAtual.Value; dt = dt.AddDays(1))
                {
                    if (dados.Any(x => x.data == dt && x.posto == arqEntrada.config.Posto))
                    {
                        if (arqEntrada.config.TipoAtualizacao == "TOTAL" || (dtAtual.Value.Date - dt).TotalDays <= 7)
                        {
                            var value = arqEntrada.config.PostoTipo == "NAT" ?
                                    dados.First(x => x.data == dt && x.posto == arqEntrada.config.Posto).qnat.Value
                                    : dados.First(x => x.data == dt && x.posto == arqEntrada.config.Posto).qinc.Value;
                            value = value < 0 ? 0 : value;
                            arqEntrada.posto.Vazoes[dt] =
                                arqEntrada.config.Fator * value;
                        }
                    }
                }

                //arqEntrada.posto.SalvarVazoes();
            }


            AddLog("- Vazoes Passadas Carregadas de ACOMPH/RDH");
        }

        private List<CONSULTA_VAZAO> ReadVazoesPassadas(string historicoVazao)
        {

            List<CONSULTA_VAZAO> dados;


            var fileLines = System.IO.File.ReadAllLines(historicoVazao);


            dados = fileLines
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x.Split(new char[] { ';', '\t' }))
                .Where(x => x.Length >= 4)
                .Select(x =>
                {
                    int _posto;
                    DateTime _data;
                    int _q_nat;
                    int _q_inc;


                    if (!int.TryParse(x[1], out _posto) || !DateTime.TryParse(x[0], out _data))
                    {
                        return (CONSULTA_VAZAO)null;
                    }


                    int.TryParse(x[2], out _q_nat);
                    int.TryParse(x[3], out _q_inc);

                    return new CONSULTA_VAZAO() { data = _data, posto = _posto, qinc = _q_inc, qnat = _q_nat, fonte = "FILE" };
                }
                )
                .Where(x => x != null).ToList();

            return dados;
        }

        private void btnInserirPrecip_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "(*.dat)|*.dat|(*.ctl)|*.ctl|(*.gif)|*.gif|(*.png)|*.png";


            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                foreach (var file in ofd.FileNames)
                {


                    if (System.IO.Path.GetExtension(file).ToUpperInvariant() == ".DAT")
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"a(\d{2})(\d{2})(\d{2})");

                        var fMatch = r.Match(file);
                        if (fMatch.Success)
                        {


                            var data = new DateTime(
                                int.Parse(fMatch.Groups[3].Value) + 2000,
                                int.Parse(fMatch.Groups[2].Value),
                                int.Parse(fMatch.Groups[1].Value))
                                ;


                            //if (this.chuvas.ContainsKey(data))
                            //{
                            //    var precToAdd = PrecipitacaoFactory.BuildFromEtaFile(file);
                            //    foreach (var nk in precToAdd.Prec)
                            //   {
                            //        this.chuvas[data][nk.Key] = nk.Value;
                            //    }

                            //}
                            //else
                            //{
                            this.chuvas[data] = PrecipitacaoFactory.BuildFromEtaFile(file);
                            this.chuvas[data].Descricao = "PREV NUM - " + System.IO.Path
                                .GetFileName(file);
                            //}



                            AddLog("- Precipitação carregada: " + file);
                        }

                    }
                    else if (System.IO.Path.GetExtension(file).ToUpperInvariant() == ".CTL")
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"pp(\d{4})(\d{2})(\d{2})_(\d+)");

                        var fMatch = r.Match(file);
                        if (fMatch.Success)
                        {
                            var data = new DateTime(
                                int.Parse(fMatch.Groups[1].Value),
                                int.Parse(fMatch.Groups[2].Value),
                                int.Parse(fMatch.Groups[3].Value))
                                ;

                            var horas = int.Parse(fMatch.Groups[4].Value);

                            var dataPrev = data.AddHours(horas).Date;


                            this.chuvas[dataPrev] = PrecipitacaoFactory.BuildFromMergeFile(file);
                            this.chuvas[dataPrev].Descricao = "PREV NUM - " + System.IO.Path
                                .GetFileName(file);

                            this.chuvas[dataPrev].Data = dataPrev;

                            AddLog("- Precipitação carregada: " + file);

                        }
                        else
                        {
                            r = new System.Text.RegularExpressions.Regex(@"([^\\]+)_(\d{4})(\d{2})(\d{2})\.");
                            fMatch = r.Match(file);
                            if (fMatch.Success)
                            {
                                var ano = int.Parse(fMatch.Groups[2].Value);




                                var data = new DateTime(
                                    ano < DateTime.Today.Year ? DateTime.Today.Year : ano,
                                    int.Parse(fMatch.Groups[3].Value),
                                    int.Parse(fMatch.Groups[4].Value))
                                    ;


                                this.chuvas[data] = PrecipitacaoFactory.BuildFromMergeFile(file);
                                this.chuvas[data].Descricao = fMatch.Groups[1].Value + " - " + System.IO.Path
                                    .GetFileName(file);

                                this.chuvas[data].Data = data;

                                AddLog("- Precipitação carregada: " + file);

                            }
                            else
                            {
                                MessageBox.Show("Data não indentificada");
                            }
                        }



                    }
                    else if (System.IO.Path.GetFileName(file).ToUpperInvariant() == "OBSERVADO.GIF")
                    {

                        var precO = PrecipitacaoFactory.BuildFromImage(file);

                        var dataPrev = precO.Data;


                        this.chuvas[dataPrev] = precO;
                        this.chuvas[dataPrev].Descricao = "OBSEVADO ONS - " + System.IO.Path.GetFileName(file);

                        this.chuvas[dataPrev].Data = dataPrev;

                    }
                    else if (System.IO.Path.GetExtension(file).ToUpperInvariant() == ".GIF")
                    {

                        var precO = PrecipitacaoFactory.BuildFromImage0(file);

                        var dataPrev = precO.Data;


                        this.chuvas[dataPrev] = precO;
                        this.chuvas[dataPrev].Descricao = System.IO.Path.GetFileName(file);

                        this.chuvas[dataPrev].Data = dataPrev;

                    }
                    else if (System.IO.Path.GetExtension(file).ToUpperInvariant() == ".PNG" && System.IO.Path.GetFileName(file).Contains("gfs"))
                    {

                        var precO = PrecipitacaoFactory.BuildFromImage2(file, DateTime.Today);

                        var dataPrev = precO.Data;


                        this.chuvas[dataPrev] = precO;
                        this.chuvas[dataPrev].Descricao = "PREVISAO GFS - " + System.IO.Path.GetFileName(file);

                        this.chuvas[dataPrev].Data = dataPrev;

                    }

                    RefreshPrecipList();
                }
            }
        }

        private void btnTempView_Click(object sender, EventArgs e)
        {
            string caminhoBase = Config.CaminhoTemperatura;

            DateTime dt = dtAtual.Value.Date;

            var caminho = System.IO.Path.Combine(caminhoBase, dt.ToString("yyyyMM"), dt.ToString("dd"));
            var filePrevd = System.IO.Path.Combine(caminho, "dia.txt");
            var filePrevd_1 = System.IO.Path.Combine(caminho, "dia_1.txt");
            var filePrevd_2 = System.IO.Path.Combine(caminho, "dia_2.txt");
            var filePrevd_3 = System.IO.Path.Combine(caminho, "dia_3.txt");
            var filePrevd_4 = System.IO.Path.Combine(caminho, "dia_4.txt");



            Temperatura.Show(
                filePrevd
                ,
                filePrevd_1,
                filePrevd_2,
                filePrevd_3,
                filePrevd_4
                );



            //"SAO_PAULO, SP, BR"

            //graf frm = new graf(atual.Where(x => x.Cidade == "SAO_PAULO, SP, BR").First().Previsao,
            //    anterior.Where(x => x.Cidade == "SAO_PAULO, SP, BR").First().Previsao);
        }

        private void btnCompararTemp_Click(object sender, EventArgs e)
        {

            string caminhoBase = Config.CaminhoTemperatura;

            DateTime dt = dtAtual.Value.Date;
            var caminho = System.IO.Path.Combine(caminhoBase, dt.ToString("yyyyMM"), dt.ToString("dd"));
            var filePrevd = System.IO.Path.Combine(caminho, "dia.txt");

            DateTime dtAtn = dt_TempAnterior.Value.Date;
            var caminhoAnt = System.IO.Path.Combine(caminhoBase, dtAtn.ToString("yyyyMM"), dtAtn.ToString("dd"));
            var filePrevdAnt = System.IO.Path.Combine(caminhoAnt, "dia.txt");




            Temperatura.ShowCompara(
                filePrevd,
                filePrevdAnt
                );


        }

        private void btnPrecRealizadaMedia_Click(object sender, EventArgs e)
        {
            var config = Config.ConfigConjunto;
            //var data = dtAtual.Value.Date;
            var chuvasMerge = new Dictionary<DateTime, Precipitacao>();

            for (DateTime data = dtAtual.Value.Date.AddDays(-9); data <= dtAtual.Value.Date; data = data.AddDays(1))
            {
                var mergeCtlFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".ctl", System.IO.SearchOption.AllDirectories);
                //var mergeDatFile = System.IO.Directory.GetFiles(Config.CaminhoMerge, "prec_" + data.ToString("yyyyMMdd") + ".dat", System.IO.SearchOption.AllDirectories);

                if (mergeCtlFile.Length == 1)
                {


                    var prec = PrecipitacaoFactory.BuildFromMergeFile(mergeCtlFile[0]);
                    prec.Descricao = "MERGE - " + System.IO.Path.GetFileNameWithoutExtension(mergeCtlFile[0]);
                    prec.Data = data;

                    chuvasMerge[data] = prec;
                }
                else
                {

                    if (MessageBox.Show("Merge para a data " + data.ToShortDateString() + " não encontrado.\r\nUsar dia anterior?", "Precip Observada - Chuva Vazão", MessageBoxButtons.YesNo)
                        == DialogResult.Yes)
                    {
                        chuvasMerge[data] = chuvasMerge[data.AddDays(-1)].Duplicar();
                        chuvasMerge[data].Data = data;
                    }

                }
            }

            var remo = new PrecipitacaoConjunto(config);
            var chuvaMEDIA = remo.Conjunto(chuvasMerge, null, WaitForm.TipoConjunto.Eta40);

            Console.WriteLine("Realizada Media");
            foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "ETA40"))
            {
                Console.Write(pCo.Agrupamento.Nome + "\t" + pCo.Nome + "\t");
                Console.WriteLine(string.Join("\t", pCo.precMedia));
            }

            var vwr = PrevViewer.ShowViewer(new IEnumerable<Precipitacao>[] { chuvasMerge.Values, chuvaMEDIA.Values }, this, caption: "Merge", viewrSize: new Size(240, 278));


            foreach (var c in chuvaMEDIA)
            {
                chuvas[c.Key] = c.Value;
            }

            RefreshPrecipList();
        }

        private void btnDownloadMerge_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(cptec.ListNewMerge(), "Chuva-vazão : Download MERGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Chuva-vazão", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry) btnDownloadMerge_Click(sender, e);
            }
        }

        private async void btnVerDifEta_Click(object sender, EventArgs e)
        {
            Busy = true;

            try
            {

                var chuvasEta00 = new Dictionary<DateTime, Precipitacao>();
                var chuvasEta12 = new Dictionary<DateTime, Precipitacao>();
                var data = dtAtual.Value.Date;
                var searchPath = System.IO.Path.Combine(Config.CaminhoPrevisao, data.ToString("yyyyMM"), data.ToString("dd"));

                txtLogPrecip.ResetText();


                //txtLogPrecip.strea


                //Precipitacao prec;
                //ETA40_00h
                //ETA40_12h
                for (int i = 1; i <= 10; i++)
                {
                    var dataPrev = data.AddDays(i);
                    var raiznome00 = "pp" + data.ToString("yyyyMMdd") + "_" + ((i * 24) + 12).ToString("0000") + ".ctl";
                    var raiznome12 = "pp" + data.ToString("yyyyMMdd") + "_" + ((i * 24)).ToString("0000") + ".ctl";


                    var f00 = System.IO.Path.Combine(searchPath, "ETA40_00h", raiznome00);
                    var f12 = System.IO.Path.Combine(searchPath, "ETA40_12h", raiznome12);

                    if (!File.Exists(f00) || !File.Exists(f12)) MessageBox.Show(await cptec.DownloadETA40Async(data, textLogger));
                    if (!File.Exists(f00) || !File.Exists(f12)) return;

                    chuvasEta00[dataPrev] = PrecipitacaoFactory.BuildFromMergeFile(f00);
                    chuvasEta12[dataPrev] = PrecipitacaoFactory.BuildFromMergeFile(f12);


                    chuvasEta00[dataPrev].Data =
                    chuvasEta12[dataPrev].Data = dataPrev;

                    chuvasEta00[dataPrev].Descricao = "ETA40 00h - " + dataPrev.ToString("dd/MM/yy");
                    chuvasEta12[dataPrev].Descricao = "ETA40 12h - " + dataPrev.ToString("dd/MM/yy");
                }
                var vwr = PrevViewer.ShowViewer(new IEnumerable<Precipitacao>[] { chuvasEta00.Values, chuvasEta12.Values }, this, caption: "ETA_40 - 00h + 12h", viewrSize: new Size(240, 278));
            }
            finally
            {
                Busy = false;
            }
        }

        private async void btnBaixarDados_Click(object sender, EventArgs e)
        {
            Busy = true;

            try
            {
                var data = dtAtual.Value.Date;

                DownloadForm frm = new DownloadForm(data, chuvas);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtLogPrecip.ResetText();
                    await frm.Acao(textLogger);
                    //await cptec.DownloadGEFSAsync(data, new TextBoxLogger(txtLogPrecip));
                    RefreshPrecipList();
                }
            }
            catch (Exception Ex)
            {

            }
            finally
            {
                Busy = false;
            }
        }

        private void btnGerarPrevisaoConjunto_Click(object sender, EventArgs e)
        {
            GerarPrevisaoConjunto();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var p = cptec.DownloadFunceme();
            chuvas[p.Data] = p;

            RefreshPrecipList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);

            //foreach (var mes in Enumerable.Range(1, 12))
            //{
            //    var dt = new DateTime(2019, 04, mes);
            //    chuvas[dt] = remo.MLT(mes);
            //    chuvas[dt].Data = dt;
            //}

            //RefreshPrecipList();


            //return;





            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                btnSelecionarSaida_Click(this, e);

            }

            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                return;
            }

            var prec = chuvas;


            var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);

            var chuvaMedia = remo.MediaBacias(prec);
            chuvas = chuvaMedia;

            RefreshPrecipList();

            var dadoslog = new StringBuilder();

            var header = "Precipitacao média";

            dadoslog.AppendLine(header);
            dadoslog.AppendLine("Bacia\t" + string.Join("\t", chuvas.Keys.Select(x => x.ToString("yyyy-MM-dd"))));
            foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
            {
                dadoslog.Append(pCo.Key.Nome + "\t");
                dadoslog.AppendLine(string.Join("\t", pCo.First().precMedia.Select(x => x.ToString("0.00"))));

                //for (int i = 0; i < chuvas.Keys.Count; i++)
                //{
                //    PrecipitacaoRepository.SaveAverage(chuvas.Keys.ToArray()[i], pCo.Key.Nome, "", pCo.First().precMedia[i], "MERGE");
                //}


            }

            //remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
            //var chuvaMEDIA = remo.ConjuntoLivre(prec, null);

            //foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ"))
            //{
            //    for (int i = 0; i < chuvaMEDIA.Keys.Count; i++)
            //    {
            //        PrecipitacaoRepository.SaveAverage(chuvaMEDIA.Keys.ToArray()[i], pCo.Agrupamento.Nome, pCo.Nome, pCo.precMedia[i], "MERGE");
            //    }

            //}

            File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamedia.log"), dadoslog.ToString());

            this.ArquivosDeSaida = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var pastaSaida = ArquivosDeSaida;

            var rev = new Ookii.Dialogs.InputDialog();
            rev.MainInstruction = "Numero da revisão";
            rev.MaxLength = 1;
            if (rev.ShowDialog() == DialogResult.OK)
            {

                if (int.TryParse(rev.Input, out int revnum))
                {
                    ProcessarResultados(pastaSaida, revnum: revnum, statusF: new RunStatus(pastaSaida));
                }

            }
            AddLog("FINALIZADO");
        }

        private void ProcessarResultados(string pastaSaida, System.IO.TextWriter logF = null, int? revnum = null, RunStatus statusF = null)
        {
            Excel.Workbook wbCen = null;
            Excel.Workbook wb = null;

            var xlsApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                if (modelosChVz.Count == 0)
                    Ler();

                int nextRevNum = 0;

                if (!revnum.HasValue)
                {
                    var nextRev = ChuvaVazaoTools.Tools.Tools.GetNextRev(dtAtual.Value);
                    nextRevNum = nextRev.rev;
                }
                else
                    nextRevNum = revnum.Value;

                var currRev = Tools.Tools.GetCurrRev(this.DataSemanaPrevsBase.HasValue ? this.DataSemanaPrevsBase.Value.AddDays(-1) : dtAtual.Value);

                int code = pastaSaida.GetHashCode();

                while (!xlsApp.Ready)
                {
                    System.Threading.Thread.Sleep(200);
                }

                xlsApp.Visible = true;
                xlsApp.ScreenUpdating = true;
                xlsApp.DisplayAlerts = false;

                var pathResult = Path.Combine(pastaSaida, $"CHUVAVAZAO_{code}.xlsm");

                if (!File.Exists(pathResult) || statusF?.Collect != RunStatus.statuscode.completed)
                {
                    if (statusF != null) statusF.Collect = RunStatus.statuscode.initialialized;
                    ColetaDeResultados(xlsApp, out wb);
                    wb.SaveAs(
                        pathResult, wb.FileFormat
                        );
                }
                else
                {
                    wb = xlsApp.Workbooks.Open(pathResult);
                }
                if (statusF != null) statusF.Collect = RunStatus.statuscode.completed;

                var pathCen = Path.Combine(pastaSaida, $"CHUVAVAZAO_CENARIO_{code}.xlsm");

                var prevsname = "prevs.rv" + nextRevNum.ToString();


                if (!File.Exists(Path.Combine(pastaSaida, prevsname)) || !File.Exists(pathCen) || statusF?.Previvaz != RunStatus.statuscode.completed)
                {
                    try
                    {
                        if (statusF != null) statusF.Previvaz = RunStatus.statuscode.initialialized;
                        xlsApp.DisplayAlerts = false;

                        xlsApp.Run($"'CHUVAVAZAO_{code}.xlsm'!CriarCenario");

                        wbCen = xlsApp.ActiveWorkbook;

                        while (!xlsApp.Ready)
                        {
                            System.Threading.Thread.Sleep(2000);
                        }

                        try
                        {
                            foreach (Microsoft.Office.Interop.Excel.Name wbName in wbCen.Names)
                            {
                                if (wbName.Visible && wbName.Name == "_gravarPrevivaz") wbName.RefersToRange.Value = true;
                            }
                        }
                        finally { }

                        wbCen.SaveAs(
                            pathCen, wb.FileFormat
                            );

                    }
                    catch
                    {
                        AddLog("Erro criando planilha de cenarios");
                        if (logF != null) logF.WriteLine("Erro criando planilha de cenarios");
                        if (wbCen != null) wbCen.Close(SaveChanges: false);
                        wb.Close(SaveChanges: false);

                        return;
                    }
                    finally
                    {
                        if (wbCen != null) wbCen.Close(SaveChanges: false);
                        wbCen = null;

                        wb.Close(SaveChanges: false);
                        wb = null;
                    }



                    var p = Program.GetPrevivazExPath(pathCen);

                    if (p != null)
                    {
                        AddLog("EXECUCAO PREVIVAZ");
                        if (logF != null) logF.WriteLine("EXECUCAO PREVIVAZ");
                        var pr = System.Diagnostics.Process.Start(p.Item1, p.Item2);

                        pr.WaitForExit();
                    }
                    else
                    {
                        if (statusF != null) statusF.Previvaz = RunStatus.statuscode.error;
                        return;
                    }

                    if (statusF != null) statusF.Previvaz = RunStatus.statuscode.completed;
                }

                if (statusF?.Previvaz != RunStatus.statuscode.completed) return;

                if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.initialialized;

                if (!File.Exists(Path.Combine(pastaSaida, prevsname)))
                    try
                    {

                        wbCen = xlsApp.Workbooks.Open(pathCen, ReadOnly: true);


                        if (nextRevNum == 0 || (nextRevNum == 1 && currRev.rev != 0))
                        {
                            xlsApp.Run($"'CHUVAVAZAO_CENARIO_{code}.xlsm'!ExportarPrevsM1", pastaSaida);
                        }
                        else
                        {
                            xlsApp.Run($"'CHUVAVAZAO_CENARIO_{code}.xlsm'!ExportarPrevs", pastaSaida);
                        }

                        var fprevs = Path.Combine(pastaSaida, "prevs.prv");

                        if (File.Exists(fprevs))
                        {

                            if (File.Exists(Path.Combine(pastaSaida, prevsname))) File.Delete(Path.Combine(pastaSaida, prevsname));

                            if (File.Exists(Path.Combine(pastaSaida, "prevs.prv")))
                                System.IO.File.Move(Path.Combine(pastaSaida, "prevs.prv"), Path.Combine(pastaSaida, prevsname));

                            var nomeDoCaso = pastaSaida.Split('\\').Last();

                            if (nomeDoCaso.StartsWith("CV_") || nomeDoCaso.StartsWith("CV2_"))
                            {
                                var pathDestino = Path.Combine("L:\\cpas_ctl_common", "auto", DateTime.Today.ToString("yyyyMMdd") + "_" + nomeDoCaso);
                                if (!System.IO.Directory.Exists(pathDestino)) Directory.CreateDirectory(pathDestino);
                                File.Copy(Path.Combine(pastaSaida, prevsname), Path.Combine(pathDestino, prevsname));
                                if (System.IO.File.Exists(Path.Combine(pastaSaida, "resumoENA.gif")))
                                    File.Copy(Path.Combine(pastaSaida, "resumoENA.gif"), Path.Combine(pathDestino, "resumoENA.gif"));
                            }

                            AddLog(Path.Combine(pastaSaida, prevsname));
                            if (logF != null) logF.WriteLine(Path.Combine(pastaSaida, prevsname));

                            if (statusF != null) statusF.Previvaz = RunStatus.statuscode.completed;
                        }
                        else //deu ruim na exportação do Prevs. (provavelmente erro na execução do previvaz)
                        {
                            if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.error;
                            if (statusF != null) statusF.Previvaz = RunStatus.statuscode.error;
                            AddLog("Erro na execução do Previvaz");

                            if (logF != null) logF.WriteLine("Erro na execução do Previvaz");

                            if (wbCen != null)
                            {
                                wbCen.Close(SaveChanges: false);
                                if (File.Exists(pathCen)) File.Delete(pathCen);
                                wbCen = null;
                            }

                            throw new Exception();
                        }
                    }
                    catch
                    {
                        if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.error;
                        if (statusF != null) statusF.Previvaz = RunStatus.statuscode.error;
                        AddLog("Erro na execução do Previvaz");

                        if (logF != null) logF.WriteLine("Erro na execução do Previvaz");
                        if (wbCen != null) { wbCen.Close(SaveChanges: false); }
                    }

                if (statusF?.Previvaz != RunStatus.statuscode.completed) return;

                if (!File.Exists(Path.Combine(pastaSaida, "enasemanal.log")))
                    try
                    {
                        if (wbCen == null) wbCen = xlsApp.Workbooks.Open(pathCen, ReadOnly: true);

                        var valoresSemanais = wbCen.Worksheets["Cen1"].Range["B14", "N61"].Value as object[,];

                        var enaText = "";
                        for (int i = 1; i <= valoresSemanais.GetLength(0); i++)
                        {
                            for (int j = 1; j <= valoresSemanais.GetLength(1); j++)
                            {
                                enaText += valoresSemanais[i, j]?.ToString() + "\t";
                            }
                            enaText += "\r\n";
                        }

                        File.WriteAllText(Path.Combine(pastaSaida, "enasemanal.log"), enaText);
                    }
                    catch (Exception ex)
                    {

                        AddLog("Erro em exportação de imagem");
                        if (logF != null) logF.WriteLine("Erro em exportação de imagem");
                        AddLog(ex.Message);
                        if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.error;
                    }
                ///DIARIO///
                ///

                if (!File.Exists(Path.Combine(pastaSaida, "enadiaria.log")))
                    try
                    {
                        if (wbCen == null)
                            wbCen = xlsApp.Workbooks.Open(pathCen, ReadOnly: true);

                        if (wb == null)
                            wb = xlsApp.Workbooks.Open(pathResult);

                        var cen1 = wbCen.Worksheets["Cen1"] as Excel.Worksheet;
                        var vals = cen1.Range["_cen1"].Value2;

                        var resPr = wb.Worksheets["PREVIVAZ"] as Excel.Worksheet;

                        resPr.Range["A2", "N321"].Value2 = vals;

                        var wsprevs = wbCen.Worksheets["Prevs"] as Excel.Worksheet;
                        var dats = wsprevs.Range["D3", "O3"].Value2;
                        resPr.Range["C1", "N1"].Value2 = dats;
                        wb.Save();

                        wb.Activate();

                        xlsApp.Run($"'CHUVAVAZAO_{code}.xlsm'!CriarCenarioDiario");
                        Excel.Workbook wbCenDiario = xlsApp.ActiveWorkbook;

                        var valoresDiarios = wbCenDiario.Worksheets["Cen1"].Range["B14", "N61"].Value as object[,];

                        try
                        {
                            decimal valu;
                            foreach (var valDia in valoresDiarios)
                            {
                                if (valDia != null)
                                {
                                    if (decimal.TryParse(valDia.ToString(), out valu))
                                        if (valu < 0)
                                            throw new Exception("Erro ao criando Ena diaria");
                                }
                                else
                                    continue;
                            }
                        }
                        catch { }

                        var enaText = "";
                        for (int i = 1; i <= valoresDiarios.GetLength(0); i++)
                        {
                            for (int j = 1; j <= valoresDiarios.GetLength(1); j++)
                            {
                                enaText += valoresDiarios[i, j]?.ToString() + "\t";
                            }
                            enaText += "\r\n";
                        }

                        File.WriteAllText(Path.Combine(pastaSaida, "enadiaria.log"), enaText);

                        if (wbCenDiario != null)
                            wbCenDiario.Close(SaveChanges: false);

                        if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.completed;

                    }
                    catch (Exception ex)
                    {
                        AddLog("Erro em processamento de enas diárias");

                        if (logF != null) logF.WriteLine("Erro em processamento de enas diárias");
                        AddLog(ex.Message);
                        if (statusF != null) statusF.PostProcessing = RunStatus.statuscode.error;
                    }
            }
            finally
            {
                if (wb != null)
                {
                    wb.Saved = true;
                    wb.Close(SaveChanges: false);
                }

                if (wbCen != null)
                {
                    wbCen.Saved = true;
                    wbCen.Close(SaveChanges: false);
                }

                if (xlsApp != null)
                {
                    xlsApp.DisplayAlerts = false;
                    xlsApp.Quit();
                    Helper.Release(xlsApp);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            chuvas.Clear();
            RefreshPrecipList();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            var logF = textLogger;
            logF.WriteLine("Iniciando AutoRoutine");
            var data = dtAtual.Value.Date;
            logF.WriteLine("Iniciando AutoDownload");
            Program.AutoDownload(data, logF);

            logF.WriteLine("Encerrando AutoRoutine");

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var logF = textLogger;
            logF.WriteLine("Iniciando AutoRoutine");
            var data = dtAtual.Value.Date;
            logF.WriteLine("Iniciando AutoRun");
            Program.AutoRun(data, logF);


            logF.WriteLine("Encerrando AutoRoutine");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var preliminar = false;
            //Tools.Tools.addHistory("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log\\report.txt", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "- tentativa de gerar relatório via botão(button6)");


            SaveFileDialog svd = new SaveFileDialog();
            if (File.Exists($"Relatorio_Compass_{dtAtual.Value.Date:dd_MM_yyyy}_(0 hrs)_Preliminar.pdf"))
            {
                svd.FileName = $"Relatorio_Compass_{dtAtual.Value.Date:dd_MM_yyyy}_(0 hrs).pdf";
                preliminar = false;
            }
            else
            {
                svd.FileName = $"Relatorio_Compass_{dtAtual.Value.Date:dd_MM_yyyy}_(0 hrs)_Preliminar.pdf";
                preliminar = true;
            }

            svd.OverwritePrompt = true;

            if (svd.ShowDialog() == DialogResult.OK)
            {
                Tools.Tools.addHistory("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\Log\\report.txt", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "- tentativa de gerar relatório via botão(button6)");
                var reportFile = svd.FileName;
                Report.Program.CriarRelatorio2(dtAtual.Value, reportFile, preliminar);

                if (MessageBox.Show("Deseja enviar o relatório por email?", "Relatório", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var resultEmail = ChuvaVazaoTools.Tools.Tools.SendMail(reportFile, "Relatório de acompanhamento disponível", "Relatório de Acompanhamento [AUTO]", "preco");
                    resultEmail.Wait();
                }
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            //if (listView_PrecPrev.SelectedItems.Count > 0)
            //{
            //    Ookii.Dialogs.VistaFolderBrowserDialog svdiag = new Ookii.Dialogs.VistaFolderBrowserDialog();
            //    //svdiag.FileName = "chuvavazao.log";
            //    //svdiag.OverwritePrompt = true;

            //    //if (!string.IsNullOrWhiteSpace(saveLogFile) || svdiag.ShowDialog() == DialogResult.OK)
            //    //{

            //    var caminho = "C:\\Users\\bruno.araujo.CPASS\\Desktop\\Nova pasta (2)";
            //    cptec.CreateCustomImages(DateTime.Today.AddDays(-1), caminho, "teste");






            //    //foreach (var prec in this.ChuvaConjunto)
            //    //{
            //    //    PrecipitacaoFactory.SalvarModeloBin(prec.Value,
            //    //        System.IO.Path.Combine(caminho,
            //    //        "pp" + Date.ToString("yyyyMMdd") + "_" + ((prec.Key - Date).TotalHours).ToString("0000")
            //    //        )
            //    //    );
            //    //}

            //    //var header = "Prec: " + (this.Tipo == WaitForm.TipoConjunto.Conjunto ? "ETA" + this.Eta.ToString() + " GEFS" + this.Gefs.ToString() : (
            //    //        this.Tipo == WaitForm.TipoConjunto.Eta40 ? "ETA" + this.Eta.ToString() : "GEFS" + this.Gefs.ToString()
            //    //     ));
            //    //+
            //    //" flags (vies-limite):" + this.RemoveViesETA.ToString() + " " + this.RemoveViesGEFS + " - " + this.RemoveLimiteETA.ToString() + " " + this.RemoveLimiteGEFS.ToString();


            //    //}
            //}
        }

        private void AutoDev_Click(object sender, EventArgs e)
        {
            var logF = textLogger;
            logF.WriteLine("Iniciando rodada digital");
            var data = dtAtual.Value.Date;
            logF.WriteLine("Iniciando...");
            Program.AutoExec(data, logF);


            logF.WriteLine("Encerrando rodada digital");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Convert_BinDat("ECMWF");
                MessageBox.Show("Realizado com Sucesso");
    }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao converter: " + ex.ToString());
            }
        }

        private void bt_execR_Click(object sender, EventArgs e)
        {
            ParteB_R();
        }

        public void Renomear_Eta40()
        {
            var dir = System.IO.Path.Combine(txtCaminho.Text);

            var dirMod = System.IO.Path.Combine(dir, "SMAP");

            string[] dir_Bacias = Directory.GetDirectories(dirMod);

            foreach (string bacias in dir_Bacias)
            {

                if (Directory.Exists(dirMod))
                {

                    string bacia = bacias.Split('\\').Last();
                    string[] arquivos_PMEDIA = Directory.GetFiles(Path.Combine(dirMod, bacia, "ARQ_ENTRADA"), "*_PMEDIA.txt");
                    string[] arquivos_ETA40 = Directory.GetFiles(Path.Combine(dirMod, bacia, "ARQ_ENTRADA"), "*_ETA40.txt");

                    foreach (string arq in arquivos_ETA40)
                    {
                        int t = arq.Split('\\').Last().Split('_').Length;
                        if (arq.Split('\\').Last().Split('_').Length < 3)
                        {

                            if (File.Exists(arq.Replace("ETA40", "PMEDIA")))
                            {
                                File.Delete(arq);
                            }
                           
                        }
                    }

                    foreach (string arq in arquivos_PMEDIA)
                    {
                        File.Move(arq, arq.Replace("PMEDIA", "ETA40"));

                    }


                }
            }




        }

        public void Mapas_R(string[] mapas)
        {
            var dir = System.IO.Path.Combine(txtCaminho.Text);

            var dirMod = System.IO.Path.Combine(dir, "SMAP");


            foreach (string arq_mapas in mapas)
            {

                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");

                var data_mapa = r.Match(arq_mapas);

                string mapa = data_mapa.ToString() + ".dat";

                if (!File.Exists(Path.Combine(dir, mapa)))
                {

                    File.Copy(arq_mapas, Path.Combine(dir, mapa));
                }
            }

            string[] dir_Bacias = Directory.GetDirectories(dirMod);

            foreach (string bacias in dir_Bacias)
            {

                if (Directory.Exists(dirMod))
                {

                    string bacia = bacias.Split('\\').Last();
                    var dir_Dest = Path.Combine(dirMod, bacia, "ARQ_ENTRADA");


                    foreach (string arq_mapas in mapas)
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"p(\d{2})(\d{2})(\d{2})a(\d{2})(\d{2})(\d{2})");

                        var data_mapa = r.Match(arq_mapas);

                        string mapa = "ETA40_LIMITES_" + data_mapa.ToString() + ".dat";

                        if (!File.Exists(Path.Combine(dir_Dest, mapa)))
                        {

                            File.Copy(arq_mapas, Path.Combine(dir_Dest, mapa));
                        }
                    }




                }
            }




        }

        public void SalvarPrecObserv_R()
        {
            foreach (var modelo in modelosChVz)
            {
                modelo.SalvarPrecObservada();
            }
            /*
            
            var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
            var chuvaMedia = remo.MediaBacias(chuvas.Where(x => x.Key <= dtAtual.Value.Date).ToDictionary(x => x.Key, x => x.Value));
            //chuvas = chuvaMedia;
            //RefreshPrecipList();
            
            var dadoslog = new StringBuilder();
            var header = "Precipitacao média";
            dadoslog.AppendLine(header);
            dadoslog.AppendLine("Bacia\t" + string.Join("\t", chuvaMedia.Keys.Select(x => x.ToString("yyyy-MM-dd"))));
            foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
            {
                dadoslog.Append(pCo.Key.Nome + "\t");
                dadoslog.AppendLine(string.Join("\t", pCo.First().precMedia.Select(x => x.ToString("0.00"))));
            }

            File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamediaObservada.log"), dadoslog.ToString());
            */


            AddLog("Arquivos de Preciptação Observada Salvos");
        }

        public void SalvarPrecPrev_R()
        {
            if (chuvas.Count == 0)
            {
                AddLog("Selecione as chuvas");
            }

            var img = false;
            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                SelecionarSaida();
                img = true;
            }

            if (String.IsNullOrWhiteSpace(this.ArquivosDeSaida) || !System.IO.Directory.Exists(this.ArquivosDeSaida))
            {
                return;
            }

            if (img)
            {
                this.Busy = true;
                /*
                foreach (var prec in chuvas.Where(x => x.Key > dtAtual.Value.Date))
                {
                    PrecipitacaoFactory.SalvarModeloBin(prec.Value,
                        System.IO.Path.Combine(this.ArquivosDeSaida,
                        "pp" + dtAtual.Value.ToString("yyyyMMdd") + "_" + ((prec.Key - dtAtual.Value).TotalHours).ToString("0000")
                        )
                    );
                }
                */
                cptec.CreateCustomImages(dtAtual.Value, this.ArquivosDeSaida, this.txtNomeChuvaPrev.Text);

                var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
                //var dic = new Dictionary<DateTime, Precipitacao>();
                // dic[pr.Data] = pr;
                ///
                var chuvaMEDIA = remo.ConjuntoLivre(chuvas, null);
                /// 
                //var chuvaMedia = remo.MediaBacias(chuvas);

                var dadoslog = new StringBuilder();

                var header = "Precipitacao média";

                dadoslog.AppendLine(header);
                foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ"))
                {
                    dadoslog.Append(pCo.Agrupamento.Nome + "\t" + pCo.Nome + "\t");
                    dadoslog.AppendLine(string.Join("\t", pCo.precMedia.Select(x => x.ToString("0.00"))));
                }

                File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamedia.log"), dadoslog.ToString());

                this.ArquivosDeSaida = "";

                this.Busy = false;
            }
            else
            {


                foreach (var modelo in modelosChVz)
                {
                    modelo.DataPrevisao = dtAtual.Value.Date;
                    modelo.SalvarPrecPrevista_R(chuvas);
                    modelo.SalvarParametros();
                }
                /*
                var remo = new PrecipitacaoConjunto(Config.ConfigConjunto);
                var chuvaMedia = remo.MediaBacias(chuvas.Where(x => x.Key > dtAtual.Value.Date).ToDictionary(x => x.Key, x => x.Value));
                //chuvas = chuvaMedia;
                //RefreshPrecipList();
                var dadoslog = new StringBuilder();
                var header = "Precipitacao média";
                dadoslog.AppendLine(header);
                dadoslog.AppendLine("Bacia\t" + string.Join("\t", chuvaMedia.Keys.Select(x => x.ToString("yyyy-MM-dd"))));
                foreach (var pCo in remo.RegioesConjunto.Where(x => x.Modelo == "CONJ").GroupBy(x => x.Agrupamento))
                {
                    dadoslog.Append(pCo.Key.Nome + "\t");
                    dadoslog.AppendLine(string.Join("\t", pCo.First().precMedia.Select(x => x.ToString("0.00"))));
                }

                File.WriteAllText(System.IO.Path.Combine(this.ArquivosDeSaida, "chuvamedia.log"), dadoslog.ToString());
                */
            }

            AddLog("- Precipitação Prevista Salva");
        }

        private void bt_MapasR_Click(object sender, EventArgs e)
        {
            PrecipitacaoPrevista_R();
        }
    }

    public class TextBoxLogger : TextWriter
    {
        TextBox textBox = null;
        private Form owner;

        public TextBoxLogger(TextBox output, Form owner)
        {
            textBox = output;
            this.owner = owner;
        }
        public override void Write(char value)
        {
            base.Write(value);
            var a = owner.BeginInvoke(new Action(() =>
            {
                textBox.AppendText(value.ToString());
            }));

           owner.EndInvoke(a);
        }
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}


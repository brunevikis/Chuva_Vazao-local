using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
//using Word = Microsoft.Office.Interop.Word;
using ChuvaVazaoTools.Tools;

namespace ChuvaVazaoTools.Classes
{
    class ExecutingProcess //: FrmMain
    {
        public List<Propagacao> ProcessResultsPart1(List<ModeloChuvaVazao> modelos)
        {
            var propagacoes = new List<Propagacao>();
            // = modelos;
            if (modelos == null)
            {
                new AddLog("Caminho não encontrado");
                return null;
            }

            try
            {
                #region Camargos
                var camargos = new Propagacao() { IdPosto = 1, NomePostoFluv = "CAMARGOS" };
                camargos.Modelo.Add(new ModeloSmap() { NomeVazao = "CAMARGOS", TempoViagem = 0 });
                propagacoes.Add(camargos);
                #endregion

                #region Itutinga
                var Itutinga = new Propagacao() { IdPosto = 2, NomePostoFluv = "Itutinga" };
                Itutinga.Modelo.Add(new ModeloSmap() { NomeVazao = "CAMARGOS", TempoViagem = 0, FatorDistribuicao = 0 });
                Itutinga.PostoMontantes.Add(new PostoMontante { Propaga = camargos, TempoViagem = 0 });
                propagacoes.Add(Itutinga);
                #endregion

                #region Funil
                var Funil = new Propagacao() { IdPosto = 211, NomePostoFluv = "Funil" };
                Funil.Modelo.Add(new ModeloSmap() { NomeVazao = "Funil mg", TempoViagem = 0 });
                Funil.PostoMontantes.Add(new PostoMontante { Propaga = Itutinga, TempoViagem = 13 });
                propagacoes.Add(Funil);
                #endregion

                #region Furnas
                var Furnas = new Propagacao() { IdPosto = 6, NomePostoFluv = "Furnas" };
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "Paraguacu", TempoViagem = 10 });
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "PBuenos", TempoViagem = 12 });
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "Furnas", TempoViagem = 0 });
                Furnas.PostoMontantes.Add(new PostoMontante { Propaga = Funil, TempoViagem = 36 });
                propagacoes.Add(Furnas);
                #endregion

                #region MMoraes
                var Mmoraes = new Propagacao() { IdPosto = 7, NomePostoFluv = "M Moraes" };
                Mmoraes.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.377f });
                Mmoraes.PostoMontantes.Add(new PostoMontante { Propaga = Furnas, TempoViagem = 23 });
                propagacoes.Add(Mmoraes);
                #endregion

                #region LCBarreto
                var LCBarreto = new Propagacao() { IdPosto = 8, NomePostoFluv = "LCBarreto" };
                LCBarreto.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.087f });
                LCBarreto.PostoMontantes.Add(new PostoMontante { Propaga = Mmoraes, TempoViagem = 7 });
                propagacoes.Add(LCBarreto);
                #endregion

                #region Jaguara
                var Jaguara = new Propagacao() { IdPosto = 9, NomePostoFluv = "Jaguara" };
                Jaguara.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.036f });
                Jaguara.PostoMontantes.Add(new PostoMontante { Propaga = LCBarreto, TempoViagem = 5 });
                propagacoes.Add(Jaguara);
                #endregion

                #region Igarapava
                var Igarapava = new Propagacao() { IdPosto = 10, NomePostoFluv = "Igarapava" };
                Igarapava.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.103f });
                Igarapava.PostoMontantes.Add(new PostoMontante { Propaga = Jaguara, TempoViagem = 10 });
                propagacoes.Add(Igarapava);
                #endregion

                #region Volta Grande
                var VoltaGrande = new Propagacao() { IdPosto = 11, NomePostoFluv = "Volta Grande" };
                VoltaGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.230f });
                VoltaGrande.PostoMontantes.Add(new PostoMontante { Propaga = Igarapava, TempoViagem = 12 });
                propagacoes.Add(VoltaGrande);
                #endregion

                #region Porto Colombia
                var PortoColombia = new Propagacao() { IdPosto = 12, NomePostoFluv = "Porto Colombia" };
                PortoColombia.Modelo.Add(new ModeloSmap() { NomeVazao = "PColombia", TempoViagem = 0, FatorDistribuicao = 0.167f });
                PortoColombia.Modelo.Add(new ModeloSmap() { NomeVazao = "capescuro", TempoViagem = 8, FatorDistribuicao = 1 });
                PortoColombia.PostoMontantes.Add(new PostoMontante { Propaga = VoltaGrande, TempoViagem = 11 });
                propagacoes.Add(PortoColombia);
                #endregion

                #region Caconde
                var Caconde = new Propagacao() { IdPosto = 14, NomePostoFluv = "Caconde" };
                Caconde.Modelo.Add(new ModeloSmap() { NomeVazao = "edacunha", TempoViagem = 0, FatorDistribuicao = 0.610f });
                propagacoes.Add(Caconde);
                #endregion

                #region Euc da Cunha
                var EucCunha = new Propagacao() { IdPosto = 15, NomePostoFluv = "Euc da Cunha" };
                EucCunha.Modelo.Add(new ModeloSmap() { NomeVazao = "edacunha", TempoViagem = 0, FatorDistribuicao = 0.390f });
                EucCunha.PostoMontantes.Add(new PostoMontante { Propaga = Caconde, TempoViagem = 12 });
                propagacoes.Add(EucCunha);
                #endregion

                #region Limoeiro
                var Limoeiro = new Propagacao() { IdPosto = 16, NomePostoFluv = "Limoeiro" };
                Limoeiro.Modelo.Add(new ModeloSmap() { NomeVazao = "marimbondo", TempoViagem = 0, FatorDistribuicao = 0.004f });
                Limoeiro.PostoMontantes.Add(new PostoMontante { Propaga = EucCunha, TempoViagem = 3 });
                propagacoes.Add(Limoeiro);
                #endregion

                #region Marimbondo
                var Marimbondo = new Propagacao() { IdPosto = 17, NomePostoFluv = "Marimbondo" };
                Marimbondo.Modelo.Add(new ModeloSmap() { NomeVazao = "Passagem", TempoViagem = 16, FatorDistribuicao = 1f });
                Marimbondo.Modelo.Add(new ModeloSmap() { NomeVazao = "Marimbondo", TempoViagem = 0, FatorDistribuicao = 0.996f });
                Marimbondo.PostoMontantes.Add(new PostoMontante { Propaga = Limoeiro, TempoViagem = 72 });
                Marimbondo.PostoMontantes.Add(new PostoMontante { Propaga = PortoColombia, TempoViagem = 20 });
                propagacoes.Add(Marimbondo);
                #endregion

                #region AguaVermelha
                var AguaVermelha = new Propagacao() { IdPosto = 18, NomePostoFluv = "AguaVermelha" };
                AguaVermelha.Modelo.Add(new ModeloSmap() { NomeVazao = "avermelha", TempoViagem = 0, FatorDistribuicao = 1 });
                AguaVermelha.PostoMontantes.Add(new PostoMontante { Propaga = Marimbondo, TempoViagem = 28 });
                propagacoes.Add(AguaVermelha);
                #endregion

                #region Guarapiranga
                var Guarapiranga = new Propagacao() { IdPosto = 117, NomePostoFluv = "Guarapiranga" };
                Guarapiranga.Modelo.Add(new ModeloSmap() { NomeVazao = "Esouza", TempoViagem = 0, FatorDistribuicao = 0.120f });
                propagacoes.Add(Guarapiranga);
                #endregion

                #region Billings Pedras
                var BillingsPedras = new Propagacao() { IdPosto = 119, NomePostoFluv = "Billings Pedras" };
                BillingsPedras.Modelo.Add(new ModeloSmap() { NomeVazao = "Esouza", TempoViagem = 0, FatorDistribuicao = 0.183f });
                propagacoes.Add(BillingsPedras);
                #endregion

                #region Billings
                var Billings = new Propagacao() { IdPosto = 118, NomePostoFluv = "Billings" };
                Billings.Modelo.Add(new ModeloSmap() { NomeVazao = "Esouza", TempoViagem = 0, FatorDistribuicao = 0.183f });
                propagacoes.Add(Billings);
                #endregion

                #region Alto Tiete
                var AltoTiete = new Propagacao() { IdPosto = 160, NomePostoFluv = "Alto Tiête" };
                AltoTiete.Modelo.Add(new ModeloSmap() { NomeVazao = "Esouza", TempoViagem = 0, FatorDistribuicao = 0.073f });
                propagacoes.Add(AltoTiete);
                #endregion

                #region E. souza
                var Esouza = new Propagacao() { IdPosto = 161, NomePostoFluv = "E. souza" };
                Esouza.Modelo.Add(new ModeloSmap() { NomeVazao = "Esouza", TempoViagem = 0, FatorDistribuicao = 0.624f });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = Guarapiranga, TempoViagem = 6 });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = Billings, TempoViagem = 0 });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = AltoTiete, TempoViagem = 15 });
                propagacoes.Add(Esouza);
                #endregion

                #region Barra Bonita
                var BBonita = new Propagacao() { IdPosto = 237, NomePostoFluv = "Barra Bonita" };
                BBonita.Modelo.Add(new ModeloSmap() { NomeVazao = "Bbonita", TempoViagem = 0, FatorDistribuicao = 1 });
                BBonita.PostoMontantes.Add(new PostoMontante { Propaga = Esouza, TempoViagem = 48 });
                propagacoes.Add(BBonita);
                #endregion

                #region Biriri
                var Biriri = new Propagacao() { IdPosto = 238, NomePostoFluv = "Biriri" };
                Biriri.Modelo.Add(new ModeloSmap() { NomeVazao = "Ibitinga", TempoViagem = 0, FatorDistribuicao = 0.344f });
                Biriri.PostoMontantes.Add(new PostoMontante { Propaga = BBonita, TempoViagem = 12 });
                propagacoes.Add(Biriri);
                #endregion

                #region Ibitinga
                var Ibitinga = new Propagacao() { IdPosto = 239, NomePostoFluv = "Ibitinga" };
                Ibitinga.Modelo.Add(new ModeloSmap() { NomeVazao = "Ibitinga", TempoViagem = 0, FatorDistribuicao = 0.656f });
                Ibitinga.PostoMontantes.Add(new PostoMontante { Propaga = Biriri, TempoViagem = 12 });
                propagacoes.Add(Ibitinga);
                #endregion

                #region Promissao
                var Promissao = new Propagacao() { IdPosto = 240, NomePostoFluv = "Promissao" };
                Promissao.Modelo.Add(new ModeloSmap() { NomeVazao = "NAvanhanda", TempoViagem = 0, FatorDistribuicao = 0.719f });
                Promissao.PostoMontantes.Add(new PostoMontante { Propaga = Ibitinga, TempoViagem = 29 });
                propagacoes.Add(Promissao);
                #endregion

                #region N. Avanhandava
                var NAvanhandava = new Propagacao() { IdPosto = 242, NomePostoFluv = "NAvanhandava" };
                NAvanhandava.Modelo.Add(new ModeloSmap() { NomeVazao = "NAvanhanda", TempoViagem = 0, FatorDistribuicao = 0.281f });
                NAvanhandava.PostoMontantes.Add(new PostoMontante { Propaga = Promissao, TempoViagem = 13 });
                propagacoes.Add(NAvanhandava);
                #endregion

                #region CorumbaIV
                var CorumbaIV = new Propagacao() { IdPosto = 205, NomePostoFluv = "Corumba IV" };
                CorumbaIV.Modelo.Add(new ModeloSmap() { NomeVazao = "CorumbaIV", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(CorumbaIV);
                #endregion

                #region CorumbaIII
                var CorumbaIII = new Propagacao() { IdPosto = 23, NomePostoFluv = "Corumba III" };
                CorumbaIII.Modelo.Add(new ModeloSmap() { NomeVazao = "Corumba1", TempoViagem = 0, FatorDistribuicao = 0.1f });
                CorumbaIII.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaIV, TempoViagem = 12 });
                propagacoes.Add(CorumbaIII);
                #endregion

                #region CorumbaI
                var CorumbaI = new Propagacao() { IdPosto = 209, NomePostoFluv = "Corumba III" };
                CorumbaI.Modelo.Add(new ModeloSmap() { NomeVazao = "Corumba1", TempoViagem = 0, FatorDistribuicao = 0.9f });
                CorumbaI.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaIII, TempoViagem = 24 });
                propagacoes.Add(CorumbaI);
                #endregion

                #region Batalha
                var Batalha = new Propagacao() { IdPosto = 22, NomePostoFluv = "Batalha" };
                Batalha.Modelo.Add(new ModeloSmap() { NomeVazao = "Sdofacao", TempoViagem = 0, FatorDistribuicao = 0.615f });
                propagacoes.Add(Batalha);
                #endregion

                #region SerraDoFacao
                var SerraDoFacao = new Propagacao() { IdPosto = 251, NomePostoFluv = "Serra Do Facao" };
                SerraDoFacao.Modelo.Add(new ModeloSmap() { NomeVazao = "SdoFacao", TempoViagem = 0, FatorDistribuicao = 0.385f });
                SerraDoFacao.PostoMontantes.Add(new PostoMontante { Propaga = Batalha, TempoViagem = 12 });
                propagacoes.Add(SerraDoFacao);
                #endregion

                #region Emborcacao
                var Emborcacao = new Propagacao() { IdPosto = 24, NomePostoFluv = "Emborcacao" };
                Emborcacao.Modelo.Add(new ModeloSmap() { NomeVazao = "Emborcacao", TempoViagem = 0, FatorDistribuicao = 1 });
                Emborcacao.PostoMontantes.Add(new PostoMontante { Propaga = SerraDoFacao, TempoViagem = 17 });
                propagacoes.Add(Emborcacao);
                #endregion

                #region NovaPonte
                var NovaPonte = new Propagacao() { IdPosto = 25, NomePostoFluv = "NovaPonte" };
                NovaPonte.Modelo.Add(new ModeloSmap() { NomeVazao = "NovaPonte", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(NovaPonte);
                #endregion

                #region Miranda
                var Miranda = new Propagacao() { IdPosto = 206, NomePostoFluv = "Miranda" };
                Miranda.Modelo.Add(new ModeloSmap() { NomeVazao = "Itumbiara", TempoViagem = 0, FatorDistribuicao = 0.040f });
                Miranda.PostoMontantes.Add(new PostoMontante { Propaga = NovaPonte, TempoViagem = 11 });
                propagacoes.Add(Miranda);
                #endregion

                #region Capim Branco 1
                var CapimBrancoI = new Propagacao() { IdPosto = 207, NomePostoFluv = "Capim Branco I" };
                CapimBrancoI.Modelo.Add(new ModeloSmap() { NomeVazao = "Itumbiara", TempoViagem = 0, FatorDistribuicao = 0.005f });
                CapimBrancoI.PostoMontantes.Add(new PostoMontante { Propaga = Miranda, TempoViagem = 5 });
                propagacoes.Add(CapimBrancoI);
                #endregion

                #region Capim Branco 2
                var CapimBrancoII = new Propagacao() { IdPosto = 28, NomePostoFluv = "Capim Branco II" };
                CapimBrancoII.Modelo.Add(new ModeloSmap() { NomeVazao = "Itumbiara", TempoViagem = 0, FatorDistribuicao = 0.012f });
                CapimBrancoII.PostoMontantes.Add(new PostoMontante { Propaga = CapimBrancoI, TempoViagem = 12 });
                propagacoes.Add(CapimBrancoII);
                #endregion

                #region Itumbiara
                var Itumbiara = new Propagacao() { IdPosto = 31, NomePostoFluv = "Itumbiara" };
                Itumbiara.Modelo.Add(new ModeloSmap() { NomeVazao = "Itumbiara", TempoViagem = 0, FatorDistribuicao = 0.943f });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = CapimBrancoII, TempoViagem = 17 });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = Emborcacao, TempoViagem = 17 });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaI, TempoViagem = 17 });
                propagacoes.Add(Itumbiara);
                #endregion

                #region Cachoeira Dourada
                var CachoeiraDourada = new Propagacao() { IdPosto = 32, NomePostoFluv = "Cachoeira Dourada" };
                CachoeiraDourada.Modelo.Add(new ModeloSmap() { NomeVazao = "SSimao2", TempoViagem = 0, FatorDistribuicao = 0.109f });
                CachoeiraDourada.PostoMontantes.Add(new PostoMontante { Propaga = Itumbiara, TempoViagem = 8 });
                propagacoes.Add(CachoeiraDourada);
                #endregion

                #region Sao Simao
                var SaoSimao = new Propagacao() { IdPosto = 33, NomePostoFluv = "Sao Simao" };
                SaoSimao.Modelo.Add(new ModeloSmap() { NomeVazao = "SSimao2", TempoViagem = 0, FatorDistribuicao = 0.891f });
                SaoSimao.Modelo.Add(new ModeloSmap() { NomeVazao = "RVerde", TempoViagem = 8, FatorDistribuicao = 1 });
                SaoSimao.PostoMontantes.Add(new PostoMontante { Propaga = CachoeiraDourada, TempoViagem = 15 });
                propagacoes.Add(SaoSimao);
                #endregion

                #region Jurumirim
                var Jurumirim = new Propagacao() { IdPosto = 47, NomePostoFluv = "Jurumirim" };
                Jurumirim.Modelo.Add(new ModeloSmap() { NomeVazao = "Jurumirim", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Jurumirim);
                #endregion

                #region Piraju
                var Piraju = new Propagacao() { IdPosto = 48, NomePostoFluv = "Piraju" };
                Piraju.Modelo.Add(new ModeloSmap() { NomeVazao = "chavantes", TempoViagem = 0, FatorDistribuicao = 0.046f });
                Piraju.PostoMontantes.Add(new PostoMontante { Propaga = Jurumirim, TempoViagem = 5.1f });
                propagacoes.Add(Piraju);
                #endregion

                #region Chavantes
                var Chavantes = new Propagacao() { IdPosto = 49, NomePostoFluv = "Chavantes" };
                Chavantes.Modelo.Add(new ModeloSmap() { NomeVazao = "chavantes", TempoViagem = 0, FatorDistribuicao = 0.954f });
                Chavantes.PostoMontantes.Add(new PostoMontante { Propaga = Piraju, TempoViagem = 10.52f });
                propagacoes.Add(Chavantes);
                #endregion

                #region Ourinhos
                var Ourinhos = new Propagacao() { IdPosto = 249, NomePostoFluv = "Ourinhos" };
                Ourinhos.Modelo.Add(new ModeloSmap() { NomeVazao = "Canoasi", TempoViagem = 0, FatorDistribuicao = 0.031f });
                Ourinhos.PostoMontantes.Add(new PostoMontante { Propaga = Chavantes, TempoViagem = 3 });
                propagacoes.Add(Ourinhos);
                #endregion

                #region Salto Grande
                var SaltoGrande = new Propagacao() { IdPosto = 50, NomePostoFluv = "Salto Grande" };
                SaltoGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "Canoasi", TempoViagem = 0, FatorDistribuicao = 0.778f });
                SaltoGrande.PostoMontantes.Add(new PostoMontante { Propaga = Ourinhos, TempoViagem = 2.8f });
                propagacoes.Add(SaltoGrande);
                #endregion

                #region Canoas II
                var CanoasII = new Propagacao() { IdPosto = 51, NomePostoFluv = "Canoas II" };
                CanoasII.Modelo.Add(new ModeloSmap() { NomeVazao = "Canoasi", TempoViagem = 0, FatorDistribuicao = 0.061f });
                CanoasII.PostoMontantes.Add(new PostoMontante { Propaga = SaltoGrande, TempoViagem = 2.8f });
                propagacoes.Add(CanoasII);
                #endregion

                #region Canoas I
                var CanoasI = new Propagacao() { IdPosto = 52, NomePostoFluv = "CanoasI" };
                CanoasI.Modelo.Add(new ModeloSmap() { NomeVazao = "Canoasi", TempoViagem = 0, FatorDistribuicao = 0.130f });
                CanoasI.PostoMontantes.Add(new PostoMontante { Propaga = CanoasII, TempoViagem = 2.8f });
                propagacoes.Add(CanoasI);
                #endregion

                #region Maua
                var Maua = new Propagacao() { IdPosto = 57, NomePostoFluv = "Maua" };
                Maua.Modelo.Add(new ModeloSmap() { NomeVazao = "Maua", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(Maua);
                #endregion

                #region Capivara
                var Capivara = new Propagacao() { IdPosto = 61, NomePostoFluv = "Capivara" };
                Capivara.Modelo.Add(new ModeloSmap() { NomeVazao = "Capivara", TempoViagem = 0, FatorDistribuicao = 1 });
                Capivara.PostoMontantes.Add(new PostoMontante { Propaga = CanoasI, TempoViagem = 17.2f });
                Capivara.PostoMontantes.Add(new PostoMontante { Propaga = Maua, TempoViagem = 31 });
                propagacoes.Add(Capivara);
                #endregion

                #region Taquarucu
                var Taquarucu = new Propagacao() { IdPosto = 62, NomePostoFluv = "Taquarucu" };
                Taquarucu.Modelo.Add(new ModeloSmap() { NomeVazao = "Rosana", TempoViagem = 0, FatorDistribuicao = 0.299f });
                Taquarucu.PostoMontantes.Add(new PostoMontante { Propaga = Capivara, TempoViagem = 9.3f });
                propagacoes.Add(Taquarucu);
                #endregion

                #region Rosana
                var Rosana = new Propagacao() { IdPosto = 63, NomePostoFluv = "Rosana" };
                Rosana.Modelo.Add(new ModeloSmap() { NomeVazao = "Rosana", TempoViagem = 0, FatorDistribuicao = 0.701f });
                Rosana.PostoMontantes.Add(new PostoMontante { Propaga = Taquarucu, TempoViagem = 13.9f });
                propagacoes.Add(Rosana);
                #endregion

                #region Santa Clara
                var SantaClara = new Propagacao() { IdPosto = 71, NomePostoFluv = "Santa Clara" };
                SantaClara.Modelo.Add(new ModeloSmap() { NomeVazao = "StaClara", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(SantaClara);
                #endregion

                #region Fundao
                var Fundao = new Propagacao() { IdPosto = 72, NomePostoFluv = "Fundao" };
                Fundao.Modelo.Add(new ModeloSmap() { NomeVazao = "JordSeg", TempoViagem = 0, FatorDistribuicao = 0.039f });
                Fundao.PostoMontantes.Add(new PostoMontante { Propaga = SantaClara, TempoViagem = 2 });
                propagacoes.Add(Fundao);
                #endregion

                #region Jordao
                var Jordao = new Propagacao() { IdPosto = 73, NomePostoFluv = "Jordão" };
                Jordao.Modelo.Add(new ModeloSmap() { NomeVazao = "JordSeg", TempoViagem = 0, FatorDistribuicao = 0.157f });
                Jordao.PostoMontantes.Add(new PostoMontante { Propaga = Fundao, TempoViagem = 1.8f });
                propagacoes.Add(Jordao);
                #endregion

                #region Foz de Areia
                var FozAreia = new Propagacao() { IdPosto = 74, NomePostoFluv = "Foz de Areia" };
                FozAreia.Modelo.Add(new ModeloSmap() { NomeVazao = "FOA", TempoViagem = 0, FatorDistribuicao = 1 });
                FozAreia.Modelo.Add(new ModeloSmap() { NomeVazao = "UVitoria", TempoViagem = 17.4f, FatorDistribuicao = 1 });
                propagacoes.Add(FozAreia);
                #endregion

                #region Segredo
                var Segredo = new Propagacao() { IdPosto = 76, NomePostoFluv = "Segredo" };
                Segredo.Modelo.Add(new ModeloSmap() { NomeVazao = "JordSeg", TempoViagem = 0, FatorDistribuicao = 0.804f });
                Segredo.PostoMontantes.Add(new PostoMontante { Propaga = FozAreia, TempoViagem = 12.7f });
                propagacoes.Add(Segredo);
                #endregion

                #region Salto Santiago
                var SaltoSantiago = new Propagacao() { IdPosto = 77, NomePostoFluv = "Salto Santiago" };
                SaltoSantiago.Modelo.Add(new ModeloSmap() { NomeVazao = "SCaxias", TempoViagem = 0, FatorDistribuicao = 0.258f });
                SaltoSantiago.PostoMontantes.Add(new PostoMontante { Propaga = Jordao, TempoViagem = 9.6f });
                SaltoSantiago.PostoMontantes.Add(new PostoMontante { Propaga = Segredo, TempoViagem = 11.7f });
                propagacoes.Add(SaltoSantiago);
                #endregion

                #region Salto Osorio
                var SaltoOsorio = new Propagacao() { IdPosto = 78, NomePostoFluv = "Salto Osorio" };
                SaltoOsorio.Modelo.Add(new ModeloSmap() { NomeVazao = "SCaxias", TempoViagem = 0, FatorDistribuicao = 0.102f });
                SaltoOsorio.PostoMontantes.Add(new PostoMontante { Propaga = SaltoSantiago, TempoViagem = 10 });
                propagacoes.Add(SaltoOsorio);
                #endregion

                #region B. Grande
                var BGrande = new Propagacao() { IdPosto = 215, NomePostoFluv = "B. Grande" };
                BGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "BG", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(BGrande);
                #endregion

                #region Garibaldi
                var Garibaldi = new Propagacao() { IdPosto = 89, NomePostoFluv = "Garibaldi" };
                Garibaldi.Modelo.Add(new ModeloSmap() { NomeVazao = "CN", TempoViagem = 0, FatorDistribuicao = 0.910f });
                propagacoes.Add(Garibaldi);
                #endregion

                #region C. Novos
                var CNovos = new Propagacao() { IdPosto = 216, NomePostoFluv = "C. Novos" };
                CNovos.Modelo.Add(new ModeloSmap() { NomeVazao = "CN", TempoViagem = 0, FatorDistribuicao = 0.090f });
                CNovos.PostoMontantes.Add(new PostoMontante { Propaga = Garibaldi, TempoViagem = 0 });
                propagacoes.Add(CNovos);
                #endregion

                #region Machadinho
                var Machadinho = new Propagacao() { IdPosto = 217, NomePostoFluv = "Machadinho" };
                Machadinho.Modelo.Add(new ModeloSmap() { NomeVazao = "Machadinho", TempoViagem = 0, FatorDistribuicao = 1 });
                Machadinho.PostoMontantes.Add(new PostoMontante { Propaga = BGrande, TempoViagem = 1 });
                Machadinho.PostoMontantes.Add(new PostoMontante { Propaga = CNovos, TempoViagem = 1 });
                propagacoes.Add(Machadinho);
                #endregion

                #region Ita
                var Ita = new Propagacao() { IdPosto = 92, NomePostoFluv = "Ita" };
                Ita.Modelo.Add(new ModeloSmap() { NomeVazao = "Ita", TempoViagem = 0, FatorDistribuicao = 1 });
                Ita.PostoMontantes.Add(new PostoMontante { Propaga = Machadinho, TempoViagem = 2 });
                propagacoes.Add(Ita);
                #endregion

                #region PassoFundo
                var PassoFundo = new Propagacao() { IdPosto = 93, NomePostoFluv = "Passo Fundo" };
                PassoFundo.Modelo.Add(new ModeloSmap() { NomeVazao = "Monjolinho", TempoViagem = 0, FatorDistribuicao = 0.586f });
                propagacoes.Add(PassoFundo);
                #endregion

                #region Monjolinho
                var Monjolinho = new Propagacao() { IdPosto = 220, NomePostoFluv = "Monjolinho" };
                Monjolinho.Modelo.Add(new ModeloSmap() { NomeVazao = "Monjolinho", TempoViagem = 0, FatorDistribuicao = 0.414f });
                Monjolinho.PostoMontantes.Add(new PostoMontante { Propaga = PassoFundo, TempoViagem = 0 });
                propagacoes.Add(Monjolinho);
                #endregion

                #region Foz do Chapecó
                var FozChapeco = new Propagacao() { IdPosto = 94, NomePostoFluv = "Foz do Chapecó" };
                FozChapeco.Modelo.Add(new ModeloSmap() { NomeVazao = "FozChapeco", TempoViagem = 0, FatorDistribuicao = 1 });
                FozChapeco.PostoMontantes.Add(new PostoMontante { Propaga = Ita, TempoViagem = 0 });
                FozChapeco.PostoMontantes.Add(new PostoMontante { Propaga = Monjolinho, TempoViagem = 0 });
                propagacoes.Add(FozChapeco);
                #endregion

                #region Q. Queixo
                var QQueixo = new Propagacao() { IdPosto = 286, NomePostoFluv = "Q. Queixo" };
                QQueixo.Modelo.Add(new ModeloSmap() { NomeVazao = "QQueixo", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(QQueixo);
                #endregion

                #region São José
                var SJose = new Propagacao() { IdPosto = 102, NomePostoFluv = "São José" };
                SJose.Modelo.Add(new ModeloSmap() { NomeVazao = "SJoao", TempoViagem = 0, FatorDistribuicao = 0.963f });
                propagacoes.Add(SJose);
                #endregion

                #region Passo São João
                var PassoSJoao = new Propagacao() { IdPosto = 103, NomePostoFluv = "Passo São João" };
                PassoSJoao.Modelo.Add(new ModeloSmap() { NomeVazao = "SJoao", TempoViagem = 0, FatorDistribuicao = 0.037f });
                PassoSJoao.PostoMontantes.Add(new PostoMontante { Propaga = SJose, TempoViagem = 0 });
                propagacoes.Add(PassoSJoao);
                #endregion

                #region Serra da Mesa
                var SerraMesa = new Propagacao() { IdPosto = 270, NomePostoFluv = "Serra da Mesa" };
                SerraMesa.Modelo.Add(new ModeloSmap() { NomeVazao = "SMesa", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(SerraMesa);
                #endregion

                #region Retiro Baixo
                var RetiroBaixo = new Propagacao() { IdPosto = 155, NomePostoFluv = "Retiro Baixo" };
                RetiroBaixo.Modelo.Add(new ModeloSmap() { NomeVazao = "RB-Smap", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(RetiroBaixo);
                #endregion

                #region Queimado
                var Queimado = new Propagacao() { IdPosto = 158, NomePostoFluv = "Queimado" };
                Queimado.Modelo.Add(new ModeloSmap() { NomeVazao = "QM", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Queimado);
                #endregion

                #region Tres Marias
                var TresMarias = new Propagacao() { IdPosto = 156, NomePostoFluv = "Tres Marias" };
                TresMarias.Modelo.Add(new ModeloSmap() { NomeVazao = "TM-Smap", TempoViagem = 0, FatorDistribuicao = 1 });
                TresMarias.PostoMontantes.Add(new PostoMontante { Propaga = RetiroBaixo, TempoViagem = 0 });
                propagacoes.Add(TresMarias);
                #endregion

                #region Três Irmãos
                var TresIrmaos = new Propagacao() { IdPosto = 243, NomePostoFluv = "Três Irmãos" };
                //TresIrmaos.PostoAcomph.Add(243);
                propagacoes.Add(TresIrmaos);
                #endregion

                #region Posto99
                var Posto99 = new Propagacao() { IdPosto = 99, NomePostoFluv = "Posto99" };
                //Posto99.PostoAcomph.Add(99);
                propagacoes.Add(Posto99);
                #endregion

                #region Posto241
                var Posto241 = new Propagacao() { IdPosto = 241, NomePostoFluv = "Posto241" };
                //Posto241.PostoAcomph.Add(241);
                propagacoes.Add(Posto241);
                #endregion

                #region Posto261
                var Posto261 = new Propagacao() { IdPosto = 261, NomePostoFluv = "Posto261" };
                //Posto261.PostoAcomph.Add(261);
                propagacoes.Add(Posto261);
                #endregion

                #region Ilha Solteira
                var IlhaSolteira = new Propagacao() { IdPosto = 34, NomePostoFluv = "Ilha Solteira" };
                //IlhaSolteira.PostoAcomph.Add(34);
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = AguaVermelha, TempoViagem = 18 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = SaoSimao, TempoViagem = 30 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = Posto99, TempoViagem = 35 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = Posto241, TempoViagem = 24 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = Posto261, TempoViagem = 24 });
                propagacoes.Add(IlhaSolteira);
                #endregion

                #region Jupia
                var Jupia = new Propagacao() { IdPosto = 245, NomePostoFluv = "Jupia" };
                //Jupia.PostoAcomph.Add(245);
                Jupia.PostoMontantes.Add(new PostoMontante { Propaga = TresIrmaos, TempoViagem = 7 });
                Jupia.PostoMontantes.Add(new PostoMontante { Propaga = IlhaSolteira, TempoViagem = 5 });
                propagacoes.Add(Jupia);
                #endregion

                #region Porto Primavera
                var PortoPrimavera = new Propagacao() { IdPosto = 246, NomePostoFluv = "Porto Primavera" };
                //PortoPrimavera.PostoAcomph.Add(245);
                PortoPrimavera.PostoMontantes.Add(new PostoMontante { Propaga = Jupia, TempoViagem = 48 });
                propagacoes.Add(PortoPrimavera);
                #endregion

                #region Itaipu 
                var Itaipu = new Propagacao() { IdPosto = 266, NomePostoFluv = "Itaipu" };
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "FLOR+ESTRA", TempoViagem = 33, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Ivinhema", TempoViagem = 45, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Ptaquara", TempoViagem = 36, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Itaipu", TempoViagem = 0, FatorDistribuicao = 1 });
                Itaipu.PostoMontantes.Add(new PostoMontante { Propaga = PortoPrimavera, TempoViagem = 56 });
                Itaipu.PostoMontantes.Add(new PostoMontante { Propaga = Rosana, TempoViagem = 56 });
                propagacoes.Add(Itaipu);
                #endregion

                new AddLog("Propagação foi preenchida com sucesso!");
            }
            catch (Exception e)
            {
                new AddLog("Erro ao adicionar as propagações dentro do método ExecutingProcess/ProcessResults");
                new AddLog("erro: " + e.Message);

                return null;
            }

            Action<Propagacao> CalculaVazaoPosto = null;
            Action<Propagacao> CalculaMedSemanal = null;
            List<CONSULTA_VAZAO> dadosAcompH = null;

            var ladroes = DateTime.Today.AddDays(-40).Date;

            dadosAcompH = new IPDOEntities1().CONSULTA_VAZAO.Where(x => x.data > ladroes).ToList();

            //Essa action foi criada para ajudar no processamento dos dados que compoem as rodadas, as operações matemáticas 
            //utilizam os dados do Acomph que são passados através da variável local chamada dadosAcomph, também são usados os
            //dados do Smap para calcular a propagação. OBS: As operações matemáticas que montam a variável propagacoes variam de usina para usina
            CalculaVazaoPosto = new Action<Propagacao>(propaga =>
            {
                try
                {
                    var vaz = dadosAcompH.Where(v => v.posto == propaga.IdPosto);

                    foreach (var vazdiariaSmapDia in modelos.Select(x => x.Vazoes.First().Vazoes).First().Keys)
                    {
                        if (!propaga.VazaoIncremental.ContainsKey(vazdiariaSmapDia)) propaga.VazaoIncremental[vazdiariaSmapDia] = 0;

                        if (vazdiariaSmapDia <= vaz.Select(x=> x.data).Last())
                        {
                            if (vaz.Select(k => k.data).Contains(vazdiariaSmapDia))
                                propaga.VazaoIncremental[vazdiariaSmapDia] = (Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qinc)) > 0 ? Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qinc) : Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qnat);

                        }
                        if (vazdiariaSmapDia > vaz.Select(x => x.data).Last() || (propaga.VazaoIncremental[vazdiariaSmapDia] <= 1))//(propaga.VazaoIncremental[vazdiariaSmapDia] == 0 && vazdiariaSmapDia > DateTime.Today.AddDays(-6)))
                        {
                            foreach (var ms in propaga.Modelo)
                            {
                                var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();


                                if (ms.FatorDistribuicao > 0 && ms.FatorDistribuicao < 1)
                                    propaga.VazaoIncremental[vazdiariaSmapDia] += modeloSmap.Vazoes[vazdiariaSmapDia] * ms.FatorDistribuicao;
                                else
                                {
                                    var dmenor = vazdiariaSmapDia.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                    var dmaior = dmenor.AddDays(1);

                                    if (!propaga.VazaoIncremental.ContainsKey(vazdiariaSmapDia)) propaga.VazaoIncremental[vazdiariaSmapDia] = 0;
                                    var horaatraso = (ms.TempoViagem % 24);
                                    if (modeloSmap.Vazoes.ContainsKey(dmaior) && modeloSmap.Vazoes.ContainsKey(dmenor))
                                    {
                                        propaga.VazaoIncremental[vazdiariaSmapDia] += modeloSmap.Vazoes[dmenor] * horaatraso / 24f;
                                        propaga.VazaoIncremental[vazdiariaSmapDia] += modeloSmap.Vazoes[dmaior] * (24f - horaatraso) / 24f;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var vazdiariaDia in propaga.VazaoIncremental.Keys.Union(propaga.PostoMontantes.SelectMany(x => x.Propaga.VazaoNatural.Keys)))
                    {
                        if (vazdiariaDia <= vaz.Select(x => x.data).Last())
                        {
                            if (!propaga.VazaoNatural.ContainsKey(vazdiariaDia)) propaga.VazaoNatural[vazdiariaDia] = 0;

                            if (vaz.Select(k => k.data).Contains(vazdiariaDia))
                            {
                                propaga.VazaoNatural[vazdiariaDia] = Convert.ToDouble(vaz.Where(a => a.data == vazdiariaDia).First().qnat);
                            }
                        }
                        else //if (vazdiariaDia >= DateTime.Today)
                        {
                            foreach (var pm in propaga.PostoMontantes)
                            {
                                var postoMontante = pm.Propaga;

                                if (!postoMontante.OK)
                                {
                                    CalculaVazaoPosto(postoMontante);
                                }

                                var dmenor = vazdiariaDia.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                var dmaior = dmenor.AddDays(1);

                                if (!propaga.VazaoNatural.ContainsKey(vazdiariaDia)) propaga.VazaoNatural[vazdiariaDia] = 0;
                                var horaatraso = (pm.TempoViagem % 24);
                                if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                {
                                    propaga.VazaoNatural[vazdiariaDia] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                    propaga.VazaoNatural[vazdiariaDia] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                }
                            }
                        }
                    }

                    foreach (var data in propaga.VazaoNatural.Keys.Union(propaga.VazaoIncremental.Keys).ToList())
                    {
                        if (!propaga.VazaoNatural.ContainsKey(data)) propaga.VazaoNatural[data] = 0;
                        if (!propaga.VazaoIncremental.ContainsKey(data)) propaga.VazaoIncremental[data] = 0;

                        if ((data >= DateTime.Today.AddDays(-6)) && propaga.VazaoIncremental[data] == 0)
                        {
                            if ((propaga.VazaoIncremental[data.AddDays(-3)] + propaga.VazaoIncremental[data.AddDays(-2)]) / 2 >= propaga.VazaoIncremental[data.AddDays(-1)])
                            {
                                propaga.VazaoIncremental[data] = ((((propaga.VazaoIncremental[data.AddDays(-3)] + propaga.VazaoIncremental[data.AddDays(-2)]) / 2) + propaga.VazaoIncremental[data.AddDays(-1)]) / 2) - (propaga.VazaoIncremental[data.AddDays(-3)] - propaga.VazaoIncremental[data.AddDays(-2)]);
                            }
                            else if ((propaga.VazaoIncremental[data.AddDays(-3)] + propaga.VazaoIncremental[data.AddDays(-2)]) / 2 <= propaga.VazaoIncremental[data.AddDays(-1)])
                            {
                                propaga.VazaoIncremental[data] = ((((propaga.VazaoIncremental[data.AddDays(-3)] + propaga.VazaoIncremental[data.AddDays(-2)]) / 2) + propaga.VazaoIncremental[data.AddDays(-1)]) / 2) + (propaga.VazaoIncremental[data.AddDays(-3)] - propaga.VazaoIncremental[data.AddDays(-2)]);
                            }
                        }

                        if (data <= vaz.Select(x => x.data).Last())
                        {
                            if (vaz.Select(k => k.data).Contains(data) && Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat) != propaga.VazaoNatural[data])
                            {
                                //var teste = Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat);
                                propaga.VazaoNatural[data] = Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat);
                            }
                            /*else
                                propaga.VazaoNatural[data] += 0;*/
                        }
                        else //if (data >= DateTime.Today)
                        {
                            if (propaga.IdPosto == 118)
                            {
                                propaga.VazaoNatural[data] = 0.8103f * propaga.VazaoIncremental[data] + 0.185f;
                            }
                            else
                            {
                                propaga.VazaoNatural[data] += propaga.VazaoIncremental[data] == propaga.VazaoNatural[data] ? 0 : propaga.VazaoIncremental[data];
                            }
                        }
                    }

                    propaga.OK = true;

                    new AddLog("O método ExecutingProcess/CalculaVazaoPosto foi concluido com sucesso!");
                }
                catch (Exception ex)
                {
                    new AddLog("Falha ao processar o método ExecutingProcess/CalculaVazaoPosto");
                    new AddLog("Falha: " + ex.Message);
                }
            });


            //O calculo da média pode ser feito de duas formas, a primeira é somando as vazões e depois dividindo pela quantidade
            //a segunda é feita atrávés do posto montante, fazendo a média incremental da usina atual e somando com a vazão média
            //do posto montante
            CalculaMedSemanal = new Action<Propagacao>(Pgacao =>
            {
                try
                {
                    DateTime dataNow = DateTime.Today.AddDays(-1);
                    DateTime semanaNow = dataNow.AddDays(-23);
                    var SOatualMaior = dataNow;

                    while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                        SOatualMaior = SOatualMaior.AddDays(+1);

                    while (semanaNow <= SOatualMaior.AddDays(+7))
                    {
                        if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                        {
                            //var media = 0f;
                            //if (Pgacao.PostoAcomph.Count == 0 || !(Pgacao.PostoAcomph.Count != 0 && (Pgacao.IdPosto != 166 || Pgacao.IdPosto != 266)))
                            // {
                            if (semanaNow <= SOatualMaior.AddDays(7))
                            {

                                var vazNaturais = Pgacao.VazaoNatural.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);
                                var vazIncrementais = Pgacao.VazaoIncremental.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);

                                if (vazNaturais.Count() == 7)
                                {
                                    Pgacao.medSemanalNatural.Add(semanaNow, vazNaturais.Average(x => x.Value));
                                    Pgacao.medSemanalIncremental.Add(semanaNow, vazIncrementais.Average(x => x.Value));
                                }
                            }
                        }
                        semanaNow = semanaNow.AddDays(+1);
                    }

                    new AddLog("O método ExecutingProcess/CalculaMedSemanal foi executado com sucesso!");
                }
                catch (Exception exc)
                {
                    new AddLog("Falha ao processar o método ExecutingProcess/CalculaMedSemanal");
                    new AddLog("Erro: " + exc.Message);
                }
            });

            try
            {
                foreach (var propaga in propagacoes)
                {
                    /*if(propaga.IdPosto == 18)
                    { }*/
                    CalculaVazaoPosto(propaga);
                    CalculaMedSemanal(propaga);
                }

                return propagacoes;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}





[DataContract]
public class Propagacao
{
    public bool OK { get; set; }
    public Propagacao()
    {
        VazaoIncremental = new Dictionary<DateTime, double>();
        VazaoNatural = new Dictionary<DateTime, double>();
        medSemanalNatural = new Dictionary<DateTime, double>();
        medSemanalIncremental = new Dictionary<DateTime, double>();
        PostoMontantes = new List<PostoMontante>();
        Modelo = new List<ModeloSmap>();
        //PostoAcomph = new List<int>();
    }

    [DataMember]
    public string NomePostoFluv { get; set; }

    [DataMember]
    public int IdPosto { get; set; }

    [DataMember]
    public Dictionary<DateTime, double> medSemanalNatural { get; set; }

    [DataMember]
    public Dictionary<DateTime, double> medSemanalIncremental { get; set; }

    [DataMember]
    public Dictionary<DateTime, double> VazaoIncremental { get; set; }

    [DataMember]
    public Dictionary<DateTime, double> VazaoNatural { get; set; }

    [DataMember]
    public List<PostoMontante> PostoMontantes { get; set; }

    [DataMember]
    public List<ModeloSmap> Modelo { get; set; }
}
public class PostoMontante
{
    public Propagacao Propaga { get; set; }
    public double TempoViagem { get; set; }
}

public class ModeloSmap
{
    public ModeloSmap()
    {
        FatorDistribuicao = 1;
    }
    public double FatorDistribuicao { get; set; }
    public string NomeVazao { get; set; }
    public double TempoViagem { get; set; }

}
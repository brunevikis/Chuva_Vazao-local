﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChuvaVazaoTools.Tools;
using ChuvaVazaoTools;
using System.Runtime.Serialization;
using System.Diagnostics;
//using Word = Microsoft.Office.Interop.Word;

namespace ChuvaVazaoTools.Classes
{
    class ExecutingProcess //: FrmMain
    {
        static List<int> comPrevivaz = new List<int>()
        {
            287,  296,  291 ,279, 145, 288, 229, 290, 190 /*,168,156,158*/ // 168,156,158 = calculo de sobradinho MVP etc
        };
        static List<int> regredidoDePrevivaz = new List<int>()
        {
            285,227,228,230 // 285 <-287 jirau <- stoantonio; 227,228,230 <- 229 sinop colider sao manoel <- teles pires; 155<-156 retiro baixo <- 3marias
        };

        public List<Propagacao> ProcessResultsPart1(List<ModeloChuvaVazao> modelos, string pastaSaida, DateTime dataForms, DateTime runrevDate, int revnum = 0, bool shadow = false)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            var propagacoes = new List<Propagacao>();
            // = modelos;
            if (modelos == null)
            {
                new AddLog("Caminho não encontrado");
                return null;
            }

            try
            {

                //Madeira


                #region GRANDE (grande e parnaiba)

                #region Camargos
                var camargos = new Propagacao() { IdPosto = 1, NomePostoFluv = "CAMARGOS" };
                camargos.Modelo.Add(new ModeloSmap() { NomeVazao = "Camargos", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(camargos);
                #endregion

                #region Itutinga
                var Itutinga = new Propagacao() { IdPosto = 2, NomePostoFluv = "Itutinga" };
                Itutinga.Modelo.Add(new ModeloSmap() { NomeVazao = "Camargos", TempoViagem = 0, FatorDistribuicao = 0 });
                Itutinga.PostoMontantes.Add(new PostoMontante { Propaga = camargos, TempoViagem = 0 });
                propagacoes.Add(Itutinga);
                #endregion

                #region Funil
                var Funil = new Propagacao() { IdPosto = 211, NomePostoFluv = "Funil" };
                Funil.Modelo.Add(new ModeloSmap() { NomeVazao = "FUNIL MG", TempoViagem = 0 });
                Funil.PostoMontantes.Add(new PostoMontante { Propaga = Itutinga, TempoViagem = 13 });
                propagacoes.Add(Funil);
                #endregion

                #region Furnas
                var Furnas = new Propagacao() { IdPosto = 6, NomePostoFluv = "Furnas" };
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "PARAGUACU", TempoViagem = 10 });
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "PBUENOS", TempoViagem = 12 });
                Furnas.Modelo.Add(new ModeloSmap() { NomeVazao = "FURNAS", TempoViagem = 0, FatorDistribuicao = 1 });
                Furnas.PostoMontantes.Add(new PostoMontante { Propaga = Funil, TempoViagem = 36 });
                propagacoes.Add(Furnas);
                #endregion

                #region MMoraes
                var Mmoraes = new Propagacao() { IdPosto = 7, NomePostoFluv = "M Moraes" };
                Mmoraes.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.377f });
                Mmoraes.PostoMontantes.Add(new PostoMontante { Propaga = Furnas, TempoViagem = 23 });
                propagacoes.Add(Mmoraes);
                #endregion

                #region LCBarreto
                var LCBarreto = new Propagacao() { IdPosto = 8, NomePostoFluv = "LCBarreto" };
                LCBarreto.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.087f });
                LCBarreto.PostoMontantes.Add(new PostoMontante { Propaga = Mmoraes, TempoViagem = 7 });
                propagacoes.Add(LCBarreto);
                #endregion

                #region Jaguara
                var Jaguara = new Propagacao() { IdPosto = 9, NomePostoFluv = "Jaguara" };
                Jaguara.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.036f });
                Jaguara.PostoMontantes.Add(new PostoMontante { Propaga = LCBarreto, TempoViagem = 5 });
                propagacoes.Add(Jaguara);
                #endregion

                #region Igarapava
                var Igarapava = new Propagacao() { IdPosto = 10, NomePostoFluv = "Igarapava" };
                Igarapava.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.103f });
                Igarapava.PostoMontantes.Add(new PostoMontante { Propaga = Jaguara, TempoViagem = 10 });
                propagacoes.Add(Igarapava);
                #endregion

                #region Volta Grande
                var VoltaGrande = new Propagacao() { IdPosto = 11, NomePostoFluv = "Volta Grande" };
                VoltaGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.230f });
                VoltaGrande.PostoMontantes.Add(new PostoMontante { Propaga = Igarapava, TempoViagem = 12 });
                propagacoes.Add(VoltaGrande);
                #endregion

                #region Porto Colombia
                var PortoColombia = new Propagacao() { IdPosto = 12, NomePostoFluv = "Porto Colombia" };
                PortoColombia.Modelo.Add(new ModeloSmap() { NomeVazao = "PCOLOMBIA", TempoViagem = 0, FatorDistribuicao = 0.167f });
                PortoColombia.Modelo.Add(new ModeloSmap() { NomeVazao = "CAPESCURO", TempoViagem = 8, FatorDistribuicao = 1 });
                PortoColombia.PostoMontantes.Add(new PostoMontante { Propaga = VoltaGrande, TempoViagem = 11 });
                propagacoes.Add(PortoColombia);
                #endregion

                #region Caconde
                var Caconde = new Propagacao() { IdPosto = 14, NomePostoFluv = "Caconde" };
                Caconde.Modelo.Add(new ModeloSmap() { NomeVazao = "EDACUNHA", TempoViagem = 0, FatorDistribuicao = 0.610f });
                propagacoes.Add(Caconde);
                #endregion

                #region Euc da Cunha
                var EucCunha = new Propagacao() { IdPosto = 15, NomePostoFluv = "Euc da Cunha" };
                EucCunha.Modelo.Add(new ModeloSmap() { NomeVazao = "EDACUNHA", TempoViagem = 0, FatorDistribuicao = 0.390f });
                EucCunha.PostoMontantes.Add(new PostoMontante { Propaga = Caconde, TempoViagem = 12 });
                propagacoes.Add(EucCunha);
                #endregion

                #region Limoeiro
                //A S OLIVEIRA
                var Limoeiro = new Propagacao() { IdPosto = 16, NomePostoFluv = "Limoeiro" };
                Limoeiro.Modelo.Add(new ModeloSmap() { NomeVazao = "MARIMBONDO", TempoViagem = 0, FatorDistribuicao = 0.004f });
                Limoeiro.PostoMontantes.Add(new PostoMontante { Propaga = EucCunha, TempoViagem = 3 });
                propagacoes.Add(Limoeiro);
                #endregion

                #region Marimbondo
                var Marimbondo = new Propagacao() { IdPosto = 17, NomePostoFluv = "Marimbondo" };
                Marimbondo.Modelo.Add(new ModeloSmap() { NomeVazao = "PASSAGEM", TempoViagem = 16, FatorDistribuicao = 1f });
                Marimbondo.Modelo.Add(new ModeloSmap() { NomeVazao = "MARIMBONDO", TempoViagem = 0, FatorDistribuicao = 0.996f });
                Marimbondo.PostoMontantes.Add(new PostoMontante { Propaga = Limoeiro, TempoViagem = 72 });
                Marimbondo.PostoMontantes.Add(new PostoMontante { Propaga = PortoColombia, TempoViagem = 20 });
                propagacoes.Add(Marimbondo);
                #endregion

                #region AguaVermelha
                var AguaVermelha = new Propagacao() { IdPosto = 18, NomePostoFluv = "AguaVermelha" };
                AguaVermelha.Modelo.Add(new ModeloSmap() { NomeVazao = "AVERMELHA", TempoViagem = 0, FatorDistribuicao = 1 });
                AguaVermelha.PostoMontantes.Add(new PostoMontante { Propaga = Marimbondo, TempoViagem = 28 });
                propagacoes.Add(AguaVermelha);
                #endregion
                #endregion

                #region Tiête (paranazao)

                #region Guarapiranga
                var Guarapiranga = new Propagacao() { IdPosto = 117, NomePostoFluv = "Guarapiranga" };
                Guarapiranga.Modelo.Add(new ModeloSmap() { NomeVazao = "ESouza", TempoViagem = 0, FatorDistribuicao = 0.120f });
                propagacoes.Add(Guarapiranga);
                #endregion

                #region Billings Pedras
                var BillingsPedras = new Propagacao() { IdPosto = 119, NomePostoFluv = "Billings Pedras" };
                BillingsPedras.Modelo.Add(new ModeloSmap() { NomeVazao = "ESouza", TempoViagem = 0, FatorDistribuicao = 0.183f });
                propagacoes.Add(BillingsPedras);
                #endregion

                #region Billings
                var Billings = new Propagacao() { IdPosto = 118, NomePostoFluv = "Billings" };//vai usar um fator de 0.146 na geração do dadvaz (foi fator calculado pq faz uma conta com usina que ainda não foi incluida nas propoçoes)
                Billings.Modelo.Add(new ModeloSmap() { NomeVazao = "ESouza", TempoViagem = 0, FatorDistribuicao = 0.183f });
                propagacoes.Add(Billings);
                #endregion

                #region Ponte Nova
                //Alto tiete
                var PonteNova = new Propagacao() { IdPosto = 160, NomePostoFluv = "Ponte Nova" };
                PonteNova.Modelo.Add(new ModeloSmap() { NomeVazao = "ESouza", TempoViagem = 0, FatorDistribuicao = 0.073f });
                propagacoes.Add(PonteNova);
                #endregion

                #region E. souza
                var Esouza = new Propagacao() { IdPosto = 161, NomePostoFluv = "E. souza" };
                Esouza.Modelo.Add(new ModeloSmap() { NomeVazao = "ESouza", TempoViagem = 0, FatorDistribuicao = 0.624f });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = Guarapiranga, TempoViagem = 6 });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = Billings, TempoViagem = 0 });
                Esouza.PostoMontantes.Add(new PostoMontante { Propaga = PonteNova, TempoViagem = 15 });
                propagacoes.Add(Esouza);
                #endregion

                #region Barra Bonita
                var BBonita = new Propagacao() { IdPosto = 237, NomePostoFluv = "Barra Bonita" };
                BBonita.Modelo.Add(new ModeloSmap() { NomeVazao = "BBonita", TempoViagem = 0, FatorDistribuicao = 1 });
                BBonita.PostoMontantes.Add(new PostoMontante { Propaga = Esouza, TempoViagem = 48 });
                propagacoes.Add(BBonita);
                #endregion

                #region Bariri
                var Bariri = new Propagacao() { IdPosto = 238, NomePostoFluv = "Bariri" };
                Bariri.Modelo.Add(new ModeloSmap() { NomeVazao = "Ibitinga", TempoViagem = 0, FatorDistribuicao = 0.342f });
                Bariri.PostoMontantes.Add(new PostoMontante { Propaga = BBonita, TempoViagem = 12 });
                propagacoes.Add(Bariri);
                #endregion

                #region Ibitinga
                var Ibitinga = new Propagacao() { IdPosto = 239, NomePostoFluv = "Ibitinga" };
                Ibitinga.Modelo.Add(new ModeloSmap() { NomeVazao = "Ibitinga", TempoViagem = 0, FatorDistribuicao = 0.658f });
                Ibitinga.PostoMontantes.Add(new PostoMontante { Propaga = Bariri, TempoViagem = 12 });
                propagacoes.Add(Ibitinga);
                #endregion

                #region Promissao
                var Promissao = new Propagacao() { IdPosto = 240, NomePostoFluv = "Promissao" };
                Promissao.Modelo.Add(new ModeloSmap() { NomeVazao = "NAvanhanda", TempoViagem = 0, FatorDistribuicao = 0.717f });
                Promissao.PostoMontantes.Add(new PostoMontante { Propaga = Ibitinga, TempoViagem = 29 });
                propagacoes.Add(Promissao);
                #endregion

                #region N. Avanhandava
                var NAvanhandava = new Propagacao() { IdPosto = 242, NomePostoFluv = "NAvanhandava" };
                NAvanhandava.Modelo.Add(new ModeloSmap() { NomeVazao = "NAvanhanda", TempoViagem = 0, FatorDistribuicao = 0.283f });
                NAvanhandava.PostoMontantes.Add(new PostoMontante { Propaga = Promissao, TempoViagem = 13 });
                propagacoes.Add(NAvanhandava);
                #endregion
                #endregion

                #region Paranaiba (grande parnaiba)

                #region CorumbaIV
                var CorumbaIV = new Propagacao() { IdPosto = 205, NomePostoFluv = "Corumba IV" };
                CorumbaIV.Modelo.Add(new ModeloSmap() { NomeVazao = "CORUMBAIV", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(CorumbaIV);
                #endregion

                #region CorumbaIII
                var CorumbaIII = new Propagacao() { IdPosto = 23, NomePostoFluv = "Corumba III" };
                CorumbaIII.Modelo.Add(new ModeloSmap() { NomeVazao = "CORUMBA1", TempoViagem = 0, FatorDistribuicao = 0.1f });
                CorumbaIII.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaIV, TempoViagem = 12 });
                propagacoes.Add(CorumbaIII);
                #endregion

                #region CorumbaI
                var CorumbaI = new Propagacao() { IdPosto = 209, NomePostoFluv = "Corumba I" };
                CorumbaI.Modelo.Add(new ModeloSmap() { NomeVazao = "CORUMBA1", TempoViagem = 0, FatorDistribuicao = 0.9f });
                CorumbaI.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaIII, TempoViagem = 24 });
                propagacoes.Add(CorumbaI);
                #endregion

                #region Batalha
                var Batalha = new Propagacao() { IdPosto = 22, NomePostoFluv = "Batalha" };
                Batalha.Modelo.Add(new ModeloSmap() { NomeVazao = "SDOFACAO", TempoViagem = 0, FatorDistribuicao = 0.615f });
                propagacoes.Add(Batalha);
                #endregion

                #region SerraDoFacao
                var SerraDoFacao = new Propagacao() { IdPosto = 251, NomePostoFluv = "Serra Do Facao" };
                SerraDoFacao.Modelo.Add(new ModeloSmap() { NomeVazao = "SDOFACAO", TempoViagem = 0, FatorDistribuicao = 0.385f });
                SerraDoFacao.PostoMontantes.Add(new PostoMontante { Propaga = Batalha, TempoViagem = 12 });
                propagacoes.Add(SerraDoFacao);
                #endregion

                #region Emborcacao
                var Emborcacao = new Propagacao() { IdPosto = 24, NomePostoFluv = "Emborcacao" };
                Emborcacao.Modelo.Add(new ModeloSmap() { NomeVazao = "EMBORCACAO", TempoViagem = 0, FatorDistribuicao = 1 });
                Emborcacao.PostoMontantes.Add(new PostoMontante { Propaga = SerraDoFacao, TempoViagem = 17 });
                propagacoes.Add(Emborcacao);
                #endregion

                #region NovaPonte
                var NovaPonte = new Propagacao() { IdPosto = 25, NomePostoFluv = "NovaPonte" };
                NovaPonte.Modelo.Add(new ModeloSmap() { NomeVazao = "NOVAPONTE", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(NovaPonte);
                #endregion

                #region Miranda
                var Miranda = new Propagacao() { IdPosto = 206, NomePostoFluv = "Miranda" };
                Miranda.Modelo.Add(new ModeloSmap() { NomeVazao = "ITUMBIARA", TempoViagem = 0, FatorDistribuicao = 0.040f });
                Miranda.PostoMontantes.Add(new PostoMontante { Propaga = NovaPonte, TempoViagem = 11 });
                propagacoes.Add(Miranda);
                #endregion

                #region Capim Branco 1
                var CapimBrancoI = new Propagacao() { IdPosto = 207, NomePostoFluv = "Capim Branco I" };
                CapimBrancoI.Modelo.Add(new ModeloSmap() { NomeVazao = "ITUMBIARA", TempoViagem = 0, FatorDistribuicao = 0.005f });
                CapimBrancoI.PostoMontantes.Add(new PostoMontante { Propaga = Miranda, TempoViagem = 5 });
                propagacoes.Add(CapimBrancoI);
                #endregion

                #region Capim Branco 2
                var CapimBrancoII = new Propagacao() { IdPosto = 28, NomePostoFluv = "Capim Branco II" };
                CapimBrancoII.Modelo.Add(new ModeloSmap() { NomeVazao = "ITUMBIARA", TempoViagem = 0, FatorDistribuicao = 0.012f });
                CapimBrancoII.PostoMontantes.Add(new PostoMontante { Propaga = CapimBrancoI, TempoViagem = 12 });
                propagacoes.Add(CapimBrancoII);
                #endregion

                #region Itumbiara
                var Itumbiara = new Propagacao() { IdPosto = 31, NomePostoFluv = "Itumbiara" };
                Itumbiara.Modelo.Add(new ModeloSmap() { NomeVazao = "ITUMBIARA", TempoViagem = 0, FatorDistribuicao = 0.943f });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = CapimBrancoII, TempoViagem = 17 });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = Emborcacao, TempoViagem = 17 });
                Itumbiara.PostoMontantes.Add(new PostoMontante { Propaga = CorumbaI, TempoViagem = 17 });
                propagacoes.Add(Itumbiara);
                #endregion

                #region Cachoeira Dourada
                var CachoeiraDourada = new Propagacao() { IdPosto = 32, NomePostoFluv = "Cachoeira Dourada" };
                CachoeiraDourada.Modelo.Add(new ModeloSmap() { NomeVazao = "SSIMAO2", TempoViagem = 0, FatorDistribuicao = 0.109f });
                CachoeiraDourada.PostoMontantes.Add(new PostoMontante { Propaga = Itumbiara, TempoViagem = 8 });//talvez zero
                propagacoes.Add(CachoeiraDourada);
                #endregion

                #region Sao Simao
                var SaoSimao = new Propagacao() { IdPosto = 33, NomePostoFluv = "Sao Simao" };
                SaoSimao.Modelo.Add(new ModeloSmap() { NomeVazao = "SSIMAO2", TempoViagem = 0, FatorDistribuicao = 0.891f });
                SaoSimao.Modelo.Add(new ModeloSmap() { NomeVazao = "RVerde", TempoViagem = 8, FatorDistribuicao = 1 });
                SaoSimao.PostoMontantes.Add(new PostoMontante { Propaga = CachoeiraDourada, TempoViagem = 15 });
                propagacoes.Add(SaoSimao);
                #endregion

                #region Espora 
                // incluido
                var Espora = new Propagacao() { IdPosto = 99, NomePostoFluv = "Espora" };
                Espora.Modelo.Add(new ModeloSmap() { NomeVazao = "Espora", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(Espora);
                #endregion

                #region Salto
                //incluido
                var Salto = new Propagacao() { IdPosto = 294, NomePostoFluv = "Salto" };
                Salto.Modelo.Add(new ModeloSmap() { NomeVazao = "SaltoVerdi", TempoViagem = 0, FatorDistribuicao = 0.923f });
                propagacoes.Add(Salto);
                #endregion

                #region S R Verdinho
                //incluido
                var SrVerdinho = new Propagacao() { IdPosto = 241, NomePostoFluv = "S R Verdinho" };
                SrVerdinho.Modelo.Add(new ModeloSmap() { NomeVazao = "SaltoVerdi", TempoViagem = 0, FatorDistribuicao = 0.077f });
                SrVerdinho.PostoMontantes.Add(new PostoMontante { Propaga = Salto, TempoViagem = 0 });
                propagacoes.Add(SrVerdinho);
                #endregion

                #region Caçu
                //incluido
                var Cacu = new Propagacao() { IdPosto = 247, NomePostoFluv = "Caçu" };
                Cacu.Modelo.Add(new ModeloSmap() { NomeVazao = "FozClaro", TempoViagem = 0, FatorDistribuicao = 0.894f });
                propagacoes.Add(Cacu);
                #endregion

                #region B Coqueiros
                //incluido
                var BCoqueiros = new Propagacao() { IdPosto = 248, NomePostoFluv = "Barra dos coqueiros" };
                BCoqueiros.Modelo.Add(new ModeloSmap() { NomeVazao = "FozClaro", TempoViagem = 0, FatorDistribuicao = 0.037f });
                BCoqueiros.PostoMontantes.Add(new PostoMontante { Propaga = Cacu, TempoViagem = 0 });
                propagacoes.Add(BCoqueiros);
                #endregion

                #region FozDo Rio Claro
                //incluido

                var FozRioClaro = new Propagacao() { IdPosto = 261, NomePostoFluv = "Foz do Rio Claro" };
                FozRioClaro.Modelo.Add(new ModeloSmap() { NomeVazao = "FozClaro", TempoViagem = 0, FatorDistribuicao = 0.069f });
                FozRioClaro.PostoMontantes.Add(new PostoMontante { Propaga = BCoqueiros, TempoViagem = 0 });
                propagacoes.Add(FozRioClaro);
                #endregion
                #endregion

                #region PARANAPANEMA(paranazao)

                #region Jurumirim
                var Jurumirim = new Propagacao() { IdPosto = 47, NomePostoFluv = "Jurumirim" };
                Jurumirim.Modelo.Add(new ModeloSmap() { NomeVazao = "Jurumirim", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Jurumirim);
                #endregion

                #region Piraju
                var Piraju = new Propagacao() { IdPosto = 48, NomePostoFluv = "Piraju" };
                Piraju.Modelo.Add(new ModeloSmap() { NomeVazao = "Chavantes", TempoViagem = 0, FatorDistribuicao = 0.046f });
                Piraju.PostoMontantes.Add(new PostoMontante { Propaga = Jurumirim, TempoViagem = 5.1f });
                propagacoes.Add(Piraju);
                #endregion

                #region Chavantes
                var Chavantes = new Propagacao() { IdPosto = 49, NomePostoFluv = "Chavantes" };
                Chavantes.Modelo.Add(new ModeloSmap() { NomeVazao = "Chavantes", TempoViagem = 0, FatorDistribuicao = 0.954f });
                Chavantes.PostoMontantes.Add(new PostoMontante { Propaga = Piraju, TempoViagem = 10.52f });
                propagacoes.Add(Chavantes);
                #endregion

                #region Ourinhos
                var Ourinhos = new Propagacao() { IdPosto = 249, NomePostoFluv = "Ourinhos" };
                Ourinhos.Modelo.Add(new ModeloSmap() { NomeVazao = "CanoasI", TempoViagem = 0, FatorDistribuicao = 0.031f });
                Ourinhos.PostoMontantes.Add(new PostoMontante { Propaga = Chavantes, TempoViagem = 3 });
                propagacoes.Add(Ourinhos);
                #endregion

                #region Salto Grande
                // também chamada de L.N.Garcez(esta com esse nome na planilha do chuva)
                var SaltoGrande = new Propagacao() { IdPosto = 50, NomePostoFluv = "Salto Grande" };
                SaltoGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "CanoasI", TempoViagem = 0, FatorDistribuicao = 0.778f });
                SaltoGrande.PostoMontantes.Add(new PostoMontante { Propaga = Ourinhos, TempoViagem = 3 });
                propagacoes.Add(SaltoGrande);
                #endregion

                #region Canoas II
                var CanoasII = new Propagacao() { IdPosto = 51, NomePostoFluv = "Canoas II" };
                CanoasII.Modelo.Add(new ModeloSmap() { NomeVazao = "CanoasI", TempoViagem = 0, FatorDistribuicao = 0.061f });
                CanoasII.PostoMontantes.Add(new PostoMontante { Propaga = SaltoGrande, TempoViagem = 2.8f });
                propagacoes.Add(CanoasII);
                #endregion

                #region Canoas I
                var CanoasI = new Propagacao() { IdPosto = 52, NomePostoFluv = "CanoasI" };
                CanoasI.Modelo.Add(new ModeloSmap() { NomeVazao = "CanoasI", TempoViagem = 0, FatorDistribuicao = 0.130f });
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

                #endregion

                #region IGUACU(sul)verficar talvez mudar tv saltocaxias

                #region Santa Clara
                var SantaClara = new Propagacao() { IdPosto = 71, NomePostoFluv = "Santa Clara" };
                SantaClara.Modelo.Add(new ModeloSmap() { NomeVazao = "STACLARA", TempoViagem = 0, FatorDistribuicao = 1f });
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
                // GB MUNHOZ
                var FozAreia = new Propagacao() { IdPosto = 74, NomePostoFluv = "Foz de Areia" };
                FozAreia.Modelo.Add(new ModeloSmap() { NomeVazao = "FOA", TempoViagem = 0, FatorDistribuicao = 1 });
                FozAreia.Modelo.Add(new ModeloSmap() { NomeVazao = "UVITORIA", TempoViagem = 17.4f, FatorDistribuicao = 1 });
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
                SaltoSantiago.Modelo.Add(new ModeloSmap() { NomeVazao = "BAIXOIG", TempoViagem = 0, FatorDistribuicao = 0.205f });
                SaltoSantiago.PostoMontantes.Add(new PostoMontante { Propaga = Jordao, TempoViagem = 9.6f });
                SaltoSantiago.PostoMontantes.Add(new PostoMontante { Propaga = Segredo, TempoViagem = 11.7f });
                propagacoes.Add(SaltoSantiago);
                #endregion

                #region Salto Osorio
                var SaltoOsorio = new Propagacao() { IdPosto = 78, NomePostoFluv = "Salto Osorio" };
                SaltoOsorio.Modelo.Add(new ModeloSmap() { NomeVazao = "BAIXOIG", TempoViagem = 0, FatorDistribuicao = 0.081f });
                SaltoOsorio.PostoMontantes.Add(new PostoMontante { Propaga = SaltoSantiago, TempoViagem = 10 });
                propagacoes.Add(SaltoOsorio);
                #endregion

                #region Salto Caxias 
                var SaltoCaxias = new Propagacao() { IdPosto = 222, NomePostoFluv = "Salto Caxias" };
                SaltoCaxias.Modelo.Add(new ModeloSmap() { NomeVazao = "BAIXOIG", TempoViagem = 0, FatorDistribuicao = 0.510f });
                SaltoCaxias.PostoMontantes.Add(new PostoMontante { Propaga = SaltoOsorio, TempoViagem = 9.4f });///talvez zero no tv
                propagacoes.Add(SaltoCaxias);
                #endregion

                #region baixo iguacu //verficar
                var baixoig = new Propagacao() { IdPosto = 81, NomePostoFluv = "baixo iguacu" };
                baixoig.Modelo.Add(new ModeloSmap() { NomeVazao = "BAIXOIG", TempoViagem = 0, FatorDistribuicao = 0.204f });
                baixoig.PostoMontantes.Add(new PostoMontante { Propaga = SaltoCaxias, TempoViagem = 5 });//verficar
                propagacoes.Add(baixoig);
                #endregion

                #endregion

                #region URUGUAI(sul)

                #region B. Grande
                var BGrande = new Propagacao() { IdPosto = 215, NomePostoFluv = "B. Grande" };
                BGrande.Modelo.Add(new ModeloSmap() { NomeVazao = "BG", TempoViagem = 0, FatorDistribuicao = 1f });
                propagacoes.Add(BGrande);
                #endregion

                #region Sao Roque
                var sRoque = new Propagacao() { IdPosto = 88, NomePostoFluv = "Sao Roque" };
                sRoque.Modelo.Add(new ModeloSmap() { NomeVazao = "CN", TempoViagem = 0, FatorDistribuicao = 0.745f });
                propagacoes.Add(sRoque);
                #endregion

                #region Garibaldi
                var Garibaldi = new Propagacao() { IdPosto = 89, NomePostoFluv = "Garibaldi" };
                Garibaldi.Modelo.Add(new ModeloSmap() { NomeVazao = "CN", TempoViagem = 0, FatorDistribuicao = 0.165f });//0.910f
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
                Machadinho.PostoMontantes.Add(new PostoMontante { Propaga = CNovos, TempoViagem = 1 });// ver esses tempos
                propagacoes.Add(Machadinho);
                #endregion

                #region Ita
                var Ita = new Propagacao() { IdPosto = 92, NomePostoFluv = "Ita" };
                Ita.Modelo.Add(new ModeloSmap() { NomeVazao = "Ita", TempoViagem = 0, FatorDistribuicao = 1 });
                Ita.PostoMontantes.Add(new PostoMontante { Propaga = Machadinho, TempoViagem = 2 });//ver esse tempo
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

                #endregion

                #region MADEIRA(norte)

                #region jirau
                //var Jirau = new Propagacao() { IdPosto = 285, NomePostoFluv = "Jirau" };
                //Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "JIRAU2", TempoViagem = 0, FatorDistribuicao = 1 });
                //Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "P_DA_BEIRA", TempoViagem = 56 });
                //Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "GUAJ-MIRIM", TempoViagem = 14 });
                //propagacoes.Add(Jirau);
                //string nomeVazao = shadow == true ? "JIRAU" : "JIRAU2";
                string nomeVazao = "JIRAU2";

                var Jirau = new Propagacao() { IdPosto = 285, NomePostoFluv = "Jirau" };
                Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = nomeVazao, TempoViagem = 0, FatorDistribuicao = 1 });
                Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "P_DA_BEIRA", TempoViagem = 56 });
                Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "GUAJ-MIRIM", TempoViagem = 14 });
                Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "AMARU_MAYU", TempoViagem = 135 });

                //if (shadow == true)
                //{
                //    Jirau.Modelo.Add(new ModeloSmap() { NomeVazao = "AMARU_MAYU", TempoViagem = 135 });
                //}

                propagacoes.Add(Jirau);
                #endregion

                #region STO ANTONIO
                var StoAnt = new Propagacao() { IdPosto = 287, NomePostoFluv = "Sto Antonio" };
                StoAnt.Modelo.Add(new ModeloSmap() { NomeVazao = "S.ANTONIO", TempoViagem = 0, FatorDistribuicao = 1 });
                StoAnt.PostoMontantes.Add(new PostoMontante { Propaga = Jirau, TempoViagem = 23 });
                propagacoes.Add(StoAnt);
                #endregion

                #region Dardanelos
                var Darda = new Propagacao() { IdPosto = 291, NomePostoFluv = "Dardanelos" };
                Darda.Modelo.Add(new ModeloSmap() { NomeVazao = "DARDANELOS", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Darda);
                #endregion

                #region Guaporé
                var Guapo = new Propagacao() { IdPosto = 296, NomePostoFluv = "Guapore" };
                Guapo.Modelo.Add(new ModeloSmap() { NomeVazao = "GUAPORE", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Guapo);
                #endregion

                #region RondonII
                var Rondon = new Propagacao() { IdPosto = 145, NomePostoFluv = "Rondon II" };
                Rondon.Modelo.Add(new ModeloSmap() { NomeVazao = "RONDONII", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Rondon);
                #endregion

                #region Samuel
                var Samuel = new Propagacao() { IdPosto = 279, NomePostoFluv = "Samuel" };
                Samuel.Modelo.Add(new ModeloSmap() { NomeVazao = "SAMUEL", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Samuel);
                #endregion

                #endregion

                #region Xingu(norte)

                #region Pimental
                var Piment = new Propagacao() { IdPosto = 288, NomePostoFluv = "Pimental" };
                Piment.Modelo.Add(new ModeloSmap() { NomeVazao = "PIMENTALT", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Piment);
                #endregion

                #region Sinop
                var Sinop = new Propagacao() { IdPosto = 227, NomePostoFluv = "Sinop" };
                Sinop.Modelo.Add(new ModeloSmap() { NomeVazao = "COLIDER", TempoViagem = 0, FatorDistribuicao = 0.915f });
                propagacoes.Add(Sinop);
                #endregion

                #region Colider
                var coli = new Propagacao() { IdPosto = 228, NomePostoFluv = "Colider" };
                coli.Modelo.Add(new ModeloSmap() { NomeVazao = "COLIDER", TempoViagem = 0, FatorDistribuicao = 0.085f });
                coli.PostoMontantes.Add(new PostoMontante { Propaga = Sinop, TempoViagem = 17 });
                propagacoes.Add(coli);
                #endregion

                #region Teles Pires
                var Telepi = new Propagacao() { IdPosto = 229, NomePostoFluv = "Teles Pires" };
                Telepi.Modelo.Add(new ModeloSmap() { NomeVazao = "SMANOEL", TempoViagem = 0, FatorDistribuicao = 0.991f });
                Telepi.PostoMontantes.Add(new PostoMontante { Propaga = coli, TempoViagem = 132 });
                propagacoes.Add(Telepi);
                #endregion

                #region Sao Manoel
                var Smano = new Propagacao() { IdPosto = 230, NomePostoFluv = "Sao Manoel" };
                Smano.Modelo.Add(new ModeloSmap() { NomeVazao = "SMANOEL", TempoViagem = 0, FatorDistribuicao = 0.009f });
                Smano.PostoMontantes.Add(new PostoMontante { Propaga = Telepi, TempoViagem = 7 });
                propagacoes.Add(Smano);
                #endregion

                #endregion

                #region Norte(norte)

                #region Balbina
                var Balb = new Propagacao() { IdPosto = 269, NomePostoFluv = "Balbina" };
                Balb.Modelo.Add(new ModeloSmap() { NomeVazao = "BALBINA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Balb);
                #endregion

                #region Curua-una
                var Curuauna = new Propagacao() { IdPosto = 277, NomePostoFluv = "Curuauna" };
                Curuauna.Modelo.Add(new ModeloSmap() { NomeVazao = "CURUAUNA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Curuauna);
                #endregion

                #region Santo Antonio do Jari
                var StaJari = new Propagacao() { IdPosto = 290, NomePostoFluv = "Antonio do Jari" };
                StaJari.Modelo.Add(new ModeloSmap() { NomeVazao = "STOANTJARI", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(StaJari);
                #endregion

                #region Cachoeira Caldeirao
                var CachoCa = new Propagacao() { IdPosto = 204, NomePostoFluv = "Cachoeira Caldeirao" };
                CachoCa.Modelo.Add(new ModeloSmap() { NomeVazao = "FGOMES", TempoViagem = 0, FatorDistribuicao = 0.989f });
                propagacoes.Add(CachoCa);
                #endregion

                #region Coaracy Nunes
                var Coaracy = new Propagacao() { IdPosto = 280, NomePostoFluv = "Coaracy Nunes" };
                Coaracy.Modelo.Add(new ModeloSmap() { NomeVazao = "FGOMES", TempoViagem = 0, FatorDistribuicao = 0.003f });
                Coaracy.PostoMontantes.Add(new PostoMontante { Propaga = CachoCa, TempoViagem = 2 });
                propagacoes.Add(Coaracy);
                #endregion

                #region Ferreira Gomes
                var Ferreira = new Propagacao() { IdPosto = 297, NomePostoFluv = "Ferreira Gomes" };
                Ferreira.Modelo.Add(new ModeloSmap() { NomeVazao = "FGOMES", TempoViagem = 0, FatorDistribuicao = 0.008f });
                Ferreira.PostoMontantes.Add(new PostoMontante { Propaga = Coaracy, TempoViagem = 2 });
                propagacoes.Add(Ferreira);
                #endregion

                #endregion

                #region Paraguai(outras se)

                #region Itiquira
                var Itiquira = new Propagacao() { IdPosto = 259, NomePostoFluv = "Itiquira" };
                Itiquira.Modelo.Add(new ModeloSmap() { NomeVazao = "ITIQUIRAI", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Itiquira);
                #endregion

                #region Jauru
                var Jauru = new Propagacao() { IdPosto = 295, NomePostoFluv = "Jauru" };
                Jauru.Modelo.Add(new ModeloSmap() { NomeVazao = "JAURU", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Jauru);
                #endregion

                #region MANSO
                var MANSO = new Propagacao() { IdPosto = 278, NomePostoFluv = "MANSO" };
                MANSO.Modelo.Add(new ModeloSmap() { NomeVazao = "MANSO", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(MANSO);
                #endregion

                #region PDEPEDRA
                var PDEPEDRA = new Propagacao() { IdPosto = 281, NomePostoFluv = "PDEPEDRA" };
                PDEPEDRA.Modelo.Add(new ModeloSmap() { NomeVazao = "PDEPEDRA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(PDEPEDRA);
                #endregion

                #endregion

                #region DOCE(outras se)

                #region CAndonga
                var CAndonga = new Propagacao() { IdPosto = 149, NomePostoFluv = "CAndonga" };
                CAndonga.Modelo.Add(new ModeloSmap() { NomeVazao = "CANDONGA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(CAndonga);
                #endregion

                #region Guiman amorim
                var guimam = new Propagacao() { IdPosto = 262, NomePostoFluv = "Guiman amorim" };
                guimam.Modelo.Add(new ModeloSmap() { NomeVazao = "SACARV", TempoViagem = 0, FatorDistribuicao = 0.978f });
                propagacoes.Add(guimam);
                #endregion

                #region SA CARVALHO
                var SACAR = new Propagacao() { IdPosto = 183, NomePostoFluv = "SA CARVALHO" };
                SACAR.Modelo.Add(new ModeloSmap() { NomeVazao = "SACARV", TempoViagem = 0, FatorDistribuicao = 0.022f });
                SACAR.PostoMontantes.Add(new PostoMontante { Propaga = guimam, TempoViagem = 4 });
                propagacoes.Add(SACAR);
                #endregion

                #region SALTO GRANDE CM
                var SALTOCM = new Propagacao() { IdPosto = 134, NomePostoFluv = "SALTO GRANDE CM" };
                SALTOCM.Modelo.Add(new ModeloSmap() { NomeVazao = "PTOESTRELA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(SALTOCM);
                #endregion

                #region PTO ESTRELA
                var PTOESTRE = new Propagacao() { IdPosto = 263, NomePostoFluv = "PTO ESTRELA" };
                PTOESTRE.Modelo.Add(new ModeloSmap() { NomeVazao = "PTOESTRELA", TempoViagem = 0, FatorDistribuicao = 0 });
                PTOESTRE.PostoMontantes.Add(new PostoMontante { Propaga = SALTOCM, TempoViagem = 2 });
                propagacoes.Add(PTOESTRE);
                #endregion

                #region BAGUARI
                var BAGUARI = new Propagacao() { IdPosto = 141, NomePostoFluv = "BAGUARI" };
                BAGUARI.Modelo.Add(new ModeloSmap() { NomeVazao = "MASCARENHA", TempoViagem = 0, FatorDistribuicao = 0.456f });
                BAGUARI.PostoMontantes.Add(new PostoMontante { Propaga = PTOESTRE, TempoViagem = 16.3f });
                BAGUARI.PostoMontantes.Add(new PostoMontante { Propaga = SACAR, TempoViagem = 15.8f });
                BAGUARI.PostoMontantes.Add(new PostoMontante { Propaga = CAndonga, TempoViagem = 19.6f });
                propagacoes.Add(BAGUARI);
                #endregion

                #region AIMORES
                var AIMORES = new Propagacao() { IdPosto = 148, NomePostoFluv = "AIMORES" };
                AIMORES.Modelo.Add(new ModeloSmap() { NomeVazao = "MASCARENHA", TempoViagem = 0, FatorDistribuicao = 0.195f });
                AIMORES.PostoMontantes.Add(new PostoMontante { Propaga = BAGUARI, TempoViagem = 12 });
                propagacoes.Add(AIMORES);
                #endregion

                #region MASCARENHAS
                var MASCARENHAS = new Propagacao() { IdPosto = 144, NomePostoFluv = "MASCARENHAS" };
                MASCARENHAS.Modelo.Add(new ModeloSmap() { NomeVazao = "MASCARENHA", TempoViagem = 0, FatorDistribuicao = 0.349f });
                MASCARENHAS.PostoMontantes.Add(new PostoMontante { Propaga = AIMORES, TempoViagem = 1 });
                propagacoes.Add(MASCARENHAS);
                #endregion

                #endregion

                #region OUTRAS(outras se)

                #region ROSAL
                var ROSAL = new Propagacao() { IdPosto = 196, NomePostoFluv = "ROSAL" };
                ROSAL.Modelo.Add(new ModeloSmap() { NomeVazao = "ROSAL", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(ROSAL);
                #endregion

                #region STA CLAR-MG
                var STACLAMG = new Propagacao() { IdPosto = 283, NomePostoFluv = "STA CLAR-MG" };
                STACLAMG.Modelo.Add(new ModeloSmap() { NomeVazao = "SCLARA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(STACLAMG);
                #endregion

                #endregion

                #region TOCANTINS(norte)

                #region Serra da Mesa
                var SerraMesa = new Propagacao() { IdPosto = 270, NomePostoFluv = "Serra da Mesa" };
                SerraMesa.Modelo.Add(new ModeloSmap() { NomeVazao = "SMesa", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(SerraMesa);
                #endregion

                #region Cana Brava
                var CanaBrava = new Propagacao() { IdPosto = 191, NomePostoFluv = "Cana Brava" };
                CanaBrava.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJEADO", TempoViagem = 0, FatorDistribuicao = 0.0558166862514689f });
                CanaBrava.PostoMontantes.Add(new PostoMontante { Propaga = SerraMesa, TempoViagem = 10 });
                propagacoes.Add(CanaBrava);
                #endregion

                #region Sao Salvador
                var SaoSalvador = new Propagacao() { IdPosto = 253, NomePostoFluv = "São Salvador" };
                SaoSalvador.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJEADO", TempoViagem = 0, FatorDistribuicao = 0.055f });
                SaoSalvador.PostoMontantes.Add(new PostoMontante { Propaga = CanaBrava, TempoViagem = 16 });
                propagacoes.Add(SaoSalvador);
                #endregion

                #region Peixe Angical
                var PeAngi = new Propagacao() { IdPosto = 257, NomePostoFluv = "Peixe Angical" };
                PeAngi.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJEADO", TempoViagem = 0, FatorDistribuicao = 0.434f });
                PeAngi.PostoMontantes.Add(new PostoMontante { Propaga = SaoSalvador, TempoViagem = 16 });
                propagacoes.Add(PeAngi);
                #endregion

                #region Lajeado
                var Lajeado = new Propagacao() { IdPosto = 273, NomePostoFluv = "Lajeado" };
                Lajeado.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJEADO", TempoViagem = 0, FatorDistribuicao = 0.455640423031727f });
                Lajeado.PostoMontantes.Add(new PostoMontante { Propaga = PeAngi, TempoViagem = 64 });
                propagacoes.Add(Lajeado);
                #endregion

                #region Estreito
                var Estreito = new Propagacao() { IdPosto = 271, NomePostoFluv = "Estreito" };
                Estreito.Modelo.Add(new ModeloSmap() { NomeVazao = "ESTREITO", TempoViagem = 0, FatorDistribuicao = 1 });
                Estreito.Modelo.Add(new ModeloSmap() { NomeVazao = "PORTO REAL", TempoViagem = 46 });
                Estreito.PostoMontantes.Add(new PostoMontante { Propaga = Lajeado, TempoViagem = 83 });
                propagacoes.Add(Estreito);
                #endregion

                #region Tucurui
                var Tucurui = new Propagacao() { IdPosto = 275, NomePostoFluv = "Tucurui" };
                Tucurui.Modelo.Add(new ModeloSmap() { NomeVazao = "TUCURUI", TempoViagem = 0, FatorDistribuicao = 1 });
                Tucurui.Modelo.Add(new ModeloSmap() { NomeVazao = "BANDEIRANT", TempoViagem = 144 });
                Tucurui.Modelo.Add(new ModeloSmap() { NomeVazao = "C.ARAGUAIA", TempoViagem = 72 });
                Tucurui.PostoMontantes.Add(new PostoMontante { Propaga = Estreito, TempoViagem = 0 });
                propagacoes.Add(Tucurui);
                #endregion

                #endregion

                #region PARNAIBA(NE)

                #region Irape
                var Irape = new Propagacao() { IdPosto = 255, NomePostoFluv = "Irape" };
                Irape.Modelo.Add(new ModeloSmap() { NomeVazao = "IRAPE", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Irape);
                #endregion

                #region Itapebi
                var Itapebi = new Propagacao() { IdPosto = 188, NomePostoFluv = "Itapebi" };
                Itapebi.Modelo.Add(new ModeloSmap() { NomeVazao = "ITAPEBI", TempoViagem = 0, FatorDistribuicao = 1 });
                Itapebi.PostoMontantes.Add(new PostoMontante { Propaga = Irape, TempoViagem = 0 });
                propagacoes.Add(Itapebi);
                #endregion

                #region boa  esperança
                var Besp = new Propagacao() { IdPosto = 190, NomePostoFluv = "Boa Esperanca" };
                Besp.Modelo.Add(new ModeloSmap() { NomeVazao = "UBESP", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Besp);
                #endregion

                #region Pedra do cavalo
                var Pcav = new Propagacao() { IdPosto = 254, NomePostoFluv = "Pedra do cavalo" };
                Pcav.Modelo.Add(new ModeloSmap() { NomeVazao = "PCAVALO", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Pcav);

                #endregion
                #endregion

                #region SAO FRANCISCO(NE)

                #region Retiro Baixo
                var RetiroBaixo = new Propagacao() { IdPosto = 155, NomePostoFluv = "Retiro Baixo" };
                RetiroBaixo.Modelo.Add(new ModeloSmap() { NomeVazao = "RB-SMAP", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(RetiroBaixo);
                #endregion

                #region Queimado
                var Queimado = new Propagacao() { IdPosto = 158, NomePostoFluv = "Queimado" };
                Queimado.Modelo.Add(new ModeloSmap() { NomeVazao = "QM", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Queimado);
                #endregion

                #region Tres Marias
                var TresMarias = new Propagacao() { IdPosto = 156, NomePostoFluv = "Tres Marias" };
                TresMarias.Modelo.Add(new ModeloSmap() { NomeVazao = "TM-SMAP", TempoViagem = 0, FatorDistribuicao = 1 });
                TresMarias.PostoMontantes.Add(new PostoMontante { Propaga = RetiroBaixo, TempoViagem = 0 });
                propagacoes.Add(TresMarias);
                #endregion
                // implementar logica para sao romao e sao francisco NAO É NECESSARIO UMA VEZ QUE OS DADOS NÃO SÃO USADOS EM CÁLCULO ALGUM

                #endregion

                #region PARANA(paranazao)

                #region Três Irmãos
                var TresIrmaos = new Propagacao() { IdPosto = 243, NomePostoFluv = "Três Irmãos" };
                TresIrmaos.Modelo.Add(new ModeloSmap() { NomeVazao = "IlhaEquiv", TempoViagem = 0, FatorDistribuicao = 0.060f });
                TresIrmaos.PostoMontantes.Add(new PostoMontante { Propaga = NAvanhandava, TempoViagem = 42 });

                //TresIrmaos.PostoAcomph.Add(243);
                propagacoes.Add(TresIrmaos);
                #endregion



                #region Ilha Solteira
                var IlhaSolteira = new Propagacao() { IdPosto = 34, NomePostoFluv = "Ilha Solteira" };
                IlhaSolteira.Modelo.Add(new ModeloSmap() { NomeVazao = "IlhaEquiv", TempoViagem = 0, FatorDistribuicao = 0.940f });

                //IlhaSolteira.PostoAcomph.Add(34);
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = AguaVermelha, TempoViagem = 18 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = SaoSimao, TempoViagem = 30 });
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = Espora, TempoViagem = 35 });//99
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = SrVerdinho, TempoViagem = 24 });//241
                IlhaSolteira.PostoMontantes.Add(new PostoMontante { Propaga = FozRioClaro, TempoViagem = 24 });//261
                propagacoes.Add(IlhaSolteira);
                #endregion

                #region Jupia
                var Jupia = new Propagacao() { IdPosto = 245, NomePostoFluv = "Jupia" };
                Jupia.Modelo.Add(new ModeloSmap() { NomeVazao = "Jupia", TempoViagem = 0, FatorDistribuicao = 1 });

                //Jupia.PostoAcomph.Add(245);
                Jupia.PostoMontantes.Add(new PostoMontante { Propaga = TresIrmaos, TempoViagem = 7 });
                Jupia.PostoMontantes.Add(new PostoMontante { Propaga = IlhaSolteira, TempoViagem = 5 });
                propagacoes.Add(Jupia);
                #endregion

                #region Sao Domingos
                var SaoDomingos = new Propagacao() { IdPosto = 154, NomePostoFluv = "Sao domingos" };
                SaoDomingos.Modelo.Add(new ModeloSmap() { NomeVazao = "SDO", TempoViagem = 0, FatorDistribuicao = 1 });

                propagacoes.Add(SaoDomingos);
                #endregion

                #region Porto Primavera
                var PortoPrimavera = new Propagacao() { IdPosto = 246, NomePostoFluv = "Porto Primavera" };
                PortoPrimavera.Modelo.Add(new ModeloSmap() { NomeVazao = "PPRI", TempoViagem = 0, FatorDistribuicao = 1 });
                PortoPrimavera.Modelo.Add(new ModeloSmap() { NomeVazao = "FZB", TempoViagem = 26 });
                //PortoPrimavera.PostoAcomph.Add(245);
                PortoPrimavera.PostoMontantes.Add(new PostoMontante { Propaga = Jupia, TempoViagem = 48 });
                PortoPrimavera.PostoMontantes.Add(new PostoMontante { Propaga = SaoDomingos, TempoViagem = 0 });//Esta correto sao domingos esta sendo adicionado aqui para respeitar a ordem  de execução das propagaçoes

                propagacoes.Add(PortoPrimavera);
                #endregion



                #region Itaipu 
                var Itaipu = new Propagacao() { IdPosto = 266, NomePostoFluv = "Itaipu" };
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "FLOR+ESTRA", TempoViagem = 33, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Ivinhema", TempoViagem = 45, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Balsa", TempoViagem = 32, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "PTaquara", TempoViagem = 36, FatorDistribuicao = 1 });
                Itaipu.Modelo.Add(new ModeloSmap() { NomeVazao = "Itaipu", TempoViagem = 0, FatorDistribuicao = 1 });
                Itaipu.PostoMontantes.Add(new PostoMontante { Propaga = PortoPrimavera, TempoViagem = 56 });
                Itaipu.PostoMontantes.Add(new PostoMontante { Propaga = Rosana, TempoViagem = 56 });
                propagacoes.Add(Itaipu);
                #endregion
                #endregion

                #region OSUL(sul)

                #region Enerstina
                var enerstina = new Propagacao() { IdPosto = 110, NomePostoFluv = "Enerstina" };
                enerstina.Modelo.Add(new ModeloSmap() { NomeVazao = "ERNESTINA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(enerstina);
                #endregion

                #region Passo Real
                var passoReal = new Propagacao() { IdPosto = 111, NomePostoFluv = "Passo Real" };
                passoReal.Modelo.Add(new ModeloSmap() { NomeVazao = "PASSOREAL", TempoViagem = 0, FatorDistribuicao = 1 });
                passoReal.PostoMontantes.Add(new PostoMontante { Propaga = enerstina, TempoViagem = 19.8f });//1
                propagacoes.Add(passoReal);
                #endregion

                #region Jacui
                var jacui = new Propagacao() { IdPosto = 112, NomePostoFluv = "Jacui" };
                jacui.Modelo.Add(new ModeloSmap() { NomeVazao = "DFRANC", TempoViagem = 0, FatorDistribuicao = 0.020f });
                jacui.PostoMontantes.Add(new PostoMontante { Propaga = passoReal, TempoViagem = 1.3f });//1
                propagacoes.Add(jacui);
                #endregion

                #region Itauba
                var itauba = new Propagacao() { IdPosto = 113, NomePostoFluv = "Itauba" };
                itauba.Modelo.Add(new ModeloSmap() { NomeVazao = "DFRANC", TempoViagem = 0, FatorDistribuicao = 0.464f });
                itauba.PostoMontantes.Add(new PostoMontante { Propaga = jacui, TempoViagem = 6.2f });//1
                propagacoes.Add(itauba);
                #endregion

                #region Dona Francisca
                var Dfran = new Propagacao() { IdPosto = 114, NomePostoFluv = "Dona Francisca" };
                Dfran.Modelo.Add(new ModeloSmap() { NomeVazao = "DFRANC", TempoViagem = 0, FatorDistribuicao = 0.516f });
                Dfran.PostoMontantes.Add(new PostoMontante { Propaga = itauba, TempoViagem = 1 });
                propagacoes.Add(Dfran);
                #endregion

                #region Castro Alves
                var CastroAl = new Propagacao() { IdPosto = 98, NomePostoFluv = "Castro Alves" };
                CastroAl.Modelo.Add(new ModeloSmap() { NomeVazao = "CALVES", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(CastroAl);
                #endregion

                #region Monte Claro
                var Montecl = new Propagacao() { IdPosto = 97, NomePostoFluv = "Monte Claro" };
                Montecl.Modelo.Add(new ModeloSmap() { NomeVazao = "14JULHO", TempoViagem = 0, FatorDistribuicao = 0.887f });
                Montecl.PostoMontantes.Add(new PostoMontante { Propaga = CastroAl, TempoViagem = 6 });
                propagacoes.Add(Montecl);
                #endregion

                #region 14 de Julho
                var XIVjulho = new Propagacao() { IdPosto = 284, NomePostoFluv = "14 de julho" };
                XIVjulho.Modelo.Add(new ModeloSmap() { NomeVazao = "14JULHO", TempoViagem = 0, FatorDistribuicao = 0.113f });
                XIVjulho.PostoMontantes.Add(new PostoMontante { Propaga = Montecl, TempoViagem = 4.5f });
                propagacoes.Add(XIVjulho);
                #endregion

                #region Capivari Cachoeira 
                //tambem chamado G.P.Souza
                var capiCacho = new Propagacao() { IdPosto = 115, NomePostoFluv = "Capivari Cachoeira" };
                capiCacho.Modelo.Add(new ModeloSmap() { NomeVazao = "GPSOUZA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(capiCacho);
                #endregion

                #region Salto Pilão
                var saltPilao = new Propagacao() { IdPosto = 101, NomePostoFluv = "Salto Pilão" };
                saltPilao.Modelo.Add(new ModeloSmap() { NomeVazao = "SALTOPILAO", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(saltPilao);
                #endregion

                //


                #region Salto RS
                var saltRS = new Propagacao() { IdPosto = 221, NomePostoFluv = "Salto RS" };
                saltRS.Modelo.Add(new ModeloSmap() { NomeVazao = "SALTORS", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(saltRS);
                #endregion

                #endregion

                #region Paraiba do sul(outras se)//verificar talvez ver tocos lajes

                #region Paraibuna
                var paraibuna = new Propagacao() { IdPosto = 121, NomePostoFluv = "paraibuna" };
                paraibuna.Modelo.Add(new ModeloSmap() { NomeVazao = "STABRANCA", TempoViagem = 0, FatorDistribuicao = 0.873f });
                propagacoes.Add(paraibuna);
                #endregion

                #region STA BRANCA
                var staBranca = new Propagacao() { IdPosto = 122, NomePostoFluv = "santa branca" };
                staBranca.Modelo.Add(new ModeloSmap() { NomeVazao = "STABRANCA", TempoViagem = 0, FatorDistribuicao = 0.127f });
                staBranca.PostoMontantes.Add(new PostoMontante { Propaga = paraibuna, TempoViagem = 6 });///verificar
                propagacoes.Add(staBranca);
                #endregion

                #region jaguari
                var jaguari = new Propagacao() { IdPosto = 120, NomePostoFluv = "jaguari" };
                jaguari.Modelo.Add(new ModeloSmap() { NomeVazao = "JAGUARI", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(jaguari);
                #endregion

                #region Funil paraiba
                var Fun = new Propagacao() { IdPosto = 123, NomePostoFluv = "Funil paraiba" };
                Fun.Modelo.Add(new ModeloSmap() { NomeVazao = "FUNIL", TempoViagem = 0, FatorDistribuicao = 1 });
                Fun.PostoMontantes.Add(new PostoMontante { Propaga = staBranca, TempoViagem = 84 });///verificar
                Fun.PostoMontantes.Add(new PostoMontante { Propaga = jaguari, TempoViagem = 72 });///verificar
                propagacoes.Add(Fun);
                #endregion

                #region Sta Cecilia
                var staceci = new Propagacao() { IdPosto = 125, NomePostoFluv = "sta cecilia" };
                staceci.Modelo.Add(new ModeloSmap() { NomeVazao = "STACECILIA", TempoViagem = 0, FatorDistribuicao = 1 });
                staceci.PostoMontantes.Add(new PostoMontante { Propaga = Fun, TempoViagem = 24 });///verificar
                propagacoes.Add(staceci);
                #endregion

                #region Picada
                var Picada = new Propagacao() { IdPosto = 197, NomePostoFluv = "Picada" };
                Picada.Modelo.Add(new ModeloSmap() { NomeVazao = "PICADA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Picada);
                #endregion

                #region Sobragi
                var Sobragi = new Propagacao() { IdPosto = 198, NomePostoFluv = "Sobragi" };
                Sobragi.Modelo.Add(new ModeloSmap() { NomeVazao = "SOBRAGI", TempoViagem = 0, FatorDistribuicao = 1 });
                Sobragi.PostoMontantes.Add(new PostoMontante { Propaga = Picada, TempoViagem = 5 });///verificar
                propagacoes.Add(Sobragi);
                #endregion

                #region ANTA
                var ANTA = new Propagacao() { IdPosto = 129, NomePostoFluv = "ANTA" };
                ANTA.Modelo.Add(new ModeloSmap() { NomeVazao = "ILHAP", TempoViagem = 0, FatorDistribuicao = 0.711f });
                ANTA.PostoMontantes.Add(new PostoMontante { Propaga = Sobragi, TempoViagem = 6 });///verificar
                ANTA.PostoMontantes.Add(new PostoMontante { Propaga = staceci, TempoViagem = 40 });///verificar
                propagacoes.Add(ANTA);
                #endregion

                #region ilha pombos
                var ilhaP = new Propagacao() { IdPosto = 130, NomePostoFluv = "ilha Pombos" };
                ilhaP.Modelo.Add(new ModeloSmap() { NomeVazao = "ILHAP", TempoViagem = 0, FatorDistribuicao = 0.289f });
                ilhaP.PostoMontantes.Add(new PostoMontante { Propaga = ANTA, TempoViagem = 10 });///verificar
                propagacoes.Add(ilhaP);
                #endregion

                #region tocos
                var tocos = new Propagacao() { IdPosto = 201, NomePostoFluv = "tocos" };
                tocos.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJESTOCOS", TempoViagem = 0, FatorDistribuicao = 0.68f });
                propagacoes.Add(tocos);
                #endregion

                #region lajes
                var lajes = new Propagacao() { IdPosto = 202, NomePostoFluv = "lajes" };
                lajes.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJESTOCOS", TempoViagem = 0, FatorDistribuicao = 0.32f });
                propagacoes.Add(lajes);
                #endregion

                #region santana
                var santana = new Propagacao() { IdPosto = 203, NomePostoFluv = "santana" };// vai usar fator de 0.317 para o dadvaz (calculado de fator de santana - fator de tocos)
                santana.Modelo.Add(new ModeloSmap() { NomeVazao = "LAJESTOCOS", TempoViagem = 0, FatorDistribuicao = 0.997f });
                propagacoes.Add(santana);
                #endregion

                #region Barra do Brauna

                var bBrau = new Propagacao() { IdPosto = 135, NomePostoFluv = "Barra Brauna" };
                bBrau.Modelo.Add(new ModeloSmap() { NomeVazao = "BBRAUNA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(bBrau);

                #endregion

                #region Suiça

                var suica = new Propagacao() { IdPosto = 213, NomePostoFluv = "Suica" };
                suica.Modelo.Add(new ModeloSmap() { NomeVazao = "Suica", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(suica);

                #endregion
                #endregion

                #region juruena salto apiacas


                #region Juruena
                var Juruena = new Propagacao() { IdPosto = 226, NomePostoFluv = "JURUENA" };
                Juruena.Modelo.Add(new ModeloSmap() { NomeVazao = "JURUENA", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(Juruena);
                #endregion

                #region SAPIACAS
                var SAPIACAS = new Propagacao() { IdPosto = 225, NomePostoFluv = "SAPIACAS" };
                SAPIACAS.Modelo.Add(new ModeloSmap() { NomeVazao = "SAPIACAS", TempoViagem = 0, FatorDistribuicao = 1 });
                propagacoes.Add(SAPIACAS);
                #endregion



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
            Action<Propagacao> RecalculaMediaNat = null;
            Action<Propagacao> RecalculaMediaInc = null;
            Action<Propagacao> AjustaVazaoNatPosto = null;
            Action<Propagacao> AjustaVazaoIncPosto = null;
            Action<Propagacao> CalculaMedSemanal = null;
            List<CONSULTA_VAZAO> dadosAcompH = null;

            var ladroes = DateTime.Today.AddDays(-100).Date;

            dadosAcompH = new IPDOEntities1().CONSULTA_VAZAO.Where(x => x.data > ladroes).ToList();

            //Essa action foi criada para ajudar no processamento dos dados que compoem as rodadas, as operações matemáticas 
            //utilizam os dados do Acomph que são passados através da variável local chamada dadosAcomph, também são usados os
            //dados do Smap para calcular a propagação. OBS: As operações matemáticas que montam a variável propagacoes variam de usina para usina
            CalculaVazaoPosto = new Action<Propagacao>(propaga =>
            {

                try
                {
                    var vaz = dadosAcompH.Where(v => v.posto == propaga.IdPosto);
                    var dataaa = modelos.Select(x => x.Vazoes.First().Vazoes).First().Keys;//todas as datas existentes no resultado do smap.
                                                                                           // foreach (var vazdiariaSmapDia in modelos.Select(x => x.Vazoes.First().Vazoes).First().Keys)
                                                                                           //var banCA = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "Bandeirant".ToUpper()).Select(x => x.Vazoes).First();

                    foreach (var vazdiariaSmapDia in modelos.Select(x => x.Vazoes.First().Vazoes).First().Keys)
                    {
                        if (!propaga.VazaoIncremental.ContainsKey(vazdiariaSmapDia)) propaga.VazaoIncremental[vazdiariaSmapDia] = 0;

                        if (vazdiariaSmapDia <= vaz.Select(x => x.data).Last())// verifica se as datas de vazões do acomph e smap batem
                        {
                            if (vaz.Select(k => k.data).Contains(vazdiariaSmapDia)) // verifica se o posto possui montantes(se nao possuir a VNI será igual à VNA)
                                propaga.VazaoIncremental[vazdiariaSmapDia] = (Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vaz.Where(a => a.data == vazdiariaSmapDia).First().qnat, Culture.NumberFormat);
                            if (propaga.IdPosto == 263)
                            {
                                //vazoes incrementais desse posto são sempre zeradas devido ao seu fator de distribuição ser zero
                                propaga.VazaoIncremental[vazdiariaSmapDia] = 0;
                            }
                        }
                        if (vazdiariaSmapDia > vaz.Select(x => x.data).Last() || (propaga.VazaoIncremental[vazdiariaSmapDia] <= 1 && propaga.IdPosto != 183))//(propaga.VazaoIncremental[vazdiariaSmapDia] == 0 && vazdiariaSmapDia > DateTime.Today.AddDays(-6)))
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

                            if (propaga.IdPosto == 263)
                            {
                                //vazoes incrementais desse posto são sempre zeradas devido ao seu fator de distribuição ser zero
                                propaga.VazaoIncremental[vazdiariaSmapDia] = 0;
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
                                propaga.VazaoNatural[vazdiariaDia] = Convert.ToDouble(vaz.Where(a => a.data == vazdiariaDia).First().qnat, Culture.NumberFormat);
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
                            if (vaz.Select(k => k.data).Contains(data) && Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat, Culture.NumberFormat) != propaga.VazaoNatural[data])
                            {
                                //var teste = Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat);
                                propaga.VazaoNatural[data] = Convert.ToDouble(vaz.Where(x => x.data == data).First().qnat, Culture.NumberFormat);
                            }
                            /*else
                                propaga.VazaoNatural[data] += 0;*/
                        }
                        else //if (data >= DateTime.Today)
                        {
                            if (propaga.IdPosto == 118)//billings
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

                    while (semanaNow <= Pgacao.VazaoNatural.Keys.Last())//SOatualMaior.AddDays(+14))//+7
                    {
                        if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                        {
                            //if (Pgacao.IdPosto == 266)
                            //{

                            //}
                            //var media = 0f;
                            //if (Pgacao.PostoAcomph.Count == 0 || !(Pgacao.PostoAcomph.Count != 0 && (Pgacao.IdPosto != 166 || Pgacao.IdPosto != 266)))
                            // {
                            if (semanaNow <= Pgacao.VazaoNatural.Keys.Last())//SOatualMaior.AddDays(14))//7
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

            RecalculaMediaNat = new Action<Propagacao>(Pgacao =>
            {
                try
                {


                    DateTime dataNow = DateTime.Today.AddDays(-1);
                    DateTime semanaNow = dataNow.AddDays(-23);
                    var SOatualMaior = dataNow;//

                    while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                        SOatualMaior = SOatualMaior.AddDays(+1);
                    if (pastaSaida.Contains("_ECENS45"))
                    {
                        while (semanaNow <= Pgacao.VazaoNatural.Keys.Last())//+7
                        {
                            if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (semanaNow <= SOatualMaior)
                                {
                                    Pgacao.calMedSemanal.Add(semanaNow, Pgacao.medSemanalNatural[semanaNow]);
                                }
                            }
                            semanaNow = semanaNow.AddDays(+1);
                        }
                    }
                    else
                    {
                        while (semanaNow <= SOatualMaior.AddDays(+14))//+7
                        {
                            if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (semanaNow <= SOatualMaior)
                                {
                                    Pgacao.calMedSemanal.Add(semanaNow, Pgacao.medSemanalNatural[semanaNow]);
                                }
                            }
                            semanaNow = semanaNow.AddDays(+1);
                        }
                    }


                    new AddLog("O método ExecutingProcess/RecalculaMediaNat foi executado com sucesso!");
                }
                catch (Exception exc)
                {
                    new AddLog("Falha ao processar o método ExecutingProcess/RecalculaMediaNat");
                    new AddLog("Erro: " + exc.Message);
                }
            });

            RecalculaMediaInc = new Action<Propagacao>(Pgacao =>
            {
                int[] postoExecao = new int[] {25, 166,251,247,261,294,241,99,47,57,154,270,116,117,118,119,160,161,318,155,156,158,74, 291, 296, 145, 279,
                                                       73,71,72,215,89,216,217,92,93,220,94,286,102,103 };//estas sao as exeçoes nos calculoas das VNI mencionadas anteriormente
                                                                                                          //o posto 166 ainda nao esta incluso nas propagaçoes 
                try
                {

                    double vazaoCalc = 0;

                    DateTime dataNow = DateTime.Today.AddDays(-1);
                    DateTime semanaNow = dataNow.AddDays(-23);
                    var SOatualMaior = dataNow;//

                    while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                        SOatualMaior = SOatualMaior.AddDays(+1);

                    while (semanaNow <= Pgacao.VazaoNatural.Keys.Last())//SOatualMaior.AddDays(+14))//+7
                    {
                        if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (semanaNow >= SOatualMaior.AddDays(7) || (semanaNow == SOatualMaior.AddDays(14))) //&& DateTime.Today.DayOfWeek == DayOfWeek.Friday))//tirar a parte do 14 e dayofweek se der erro
                            {//trocar para == se der errado
                                if (postoExecao.All(x => !x.Equals(Pgacao.IdPosto)))
                                {
                                    if (Pgacao.medSemanalIncremental.ContainsKey(semanaNow))
                                    {
                                        vazaoCalc = SomaInc(Pgacao, semanaNow);
                                        if (Pgacao.calMedSemanal.ContainsKey(semanaNow))
                                        {
                                            Pgacao.calMedSemanal[semanaNow] = vazaoCalc;

                                        }
                                        else
                                        {
                                            Pgacao.calMedSemanal.Add(semanaNow, vazaoCalc);
                                        }
                                    }

                                }

                                else
                                {
                                    if (Pgacao.medSemanalNatural.ContainsKey(semanaNow))
                                    {
                                        if (Pgacao.calMedSemanal.ContainsKey(semanaNow))
                                        {
                                            Pgacao.calMedSemanal[semanaNow] = Pgacao.medSemanalNatural[semanaNow];
                                        }
                                        else
                                        {
                                            Pgacao.calMedSemanal.Add(semanaNow, Pgacao.medSemanalNatural[semanaNow]);
                                        }
                                    }
                                }

                            }
                        }
                        semanaNow = semanaNow.AddDays(+1);
                    }

                    // new AddLog("O método ExecutingProcess/RecalculaMediaInc foi executado com sucesso!");
                }
                catch (Exception exc)
                {
                    new AddLog("Falha ao processar o método ExecutingProcess/RecalculaMediaInc");
                    new AddLog("Erro: " + exc.Message);
                }
            });

            AjustaVazaoIncPosto = new Action<Propagacao>(propaga =>
            {//essa foi foi feita para ajudar nos ajustes de vazao inc dos postos com calculos semelhantes 
                try
                {
                    var ultimaDiaAcomph = dadosAcompH.Select(x => x.data).Last();

                    foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                    {

                        if (dat > ultimaDiaAcomph)
                        {
                            if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                            if (!propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                            foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                            {
                                var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                var dmaior = dmenor.AddDays(1);

                                propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat] * ms.FatorDistribuicao;

                            }
                            if (propaga.IdPosto == 263)
                            {
                                //vazoes incrementais desse posto são sempre zeradas devido ao seu fator de distribuição ser zero
                                propaga.VazaoIncremental[dat] = 0;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            });

            AjustaVazaoNatPosto = new Action<Propagacao>(propaga =>
            {//essa foi foi feita para ajudar nos ajustes de vazao Nat dos postos com calculos semelhantes 
                try
                {
                    var ultimaDiaAcomph = dadosAcompH.Select(x => x.data).Last();

                    foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                    {
                        try
                        {
                            if (dat > ultimaDiaAcomph)
                            {
                                if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;

                                foreach (var pm in propaga.PostoMontantes)
                                {
                                    var postoMontante = pm.Propaga;

                                    var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                    var dmaior = dmenor.AddDays(1);

                                    if (pm.TempoViagem >= 72)
                                    {
                                        dmenor = dmenor.AddDays(1);
                                        dmaior = dmenor.AddDays(1);
                                    }

                                    var horaatraso = (pm.TempoViagem % 24);
                                    if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                    {
                                        propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                        propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                    }
                                }
                                propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            });

            try
            {
                foreach (var propaga in propagacoes)
                {
                    if (propaga.IdPosto == 183)
                    { }
                    CalculaVazaoPosto(propaga);
                    //CalculaMedSemanal(propaga);
                }
                var ultimaDiaAcomph = dadosAcompH.Select(x => x.data).Last();

                #region ajustar vazao nat e inc diarias
                //Nessa region sao feitos ajustes nos daods das usinas que possuem particularides que causaram discrepancia nos dados
                //apos passarem pela action CalculaVazaoPosto
                foreach (var propaga in propagacoes)
                {
                    if (propaga.IdPosto == 2)
                    {
                        propaga.VazaoIncremental = propagacoes.Where(x => x.IdPosto == 1).Select(x => x.VazaoIncremental).FirstOrDefault();
                        propaga.VazaoNatural = propagacoes.Where(x => x.IdPosto == 1).Select(x => x.VazaoNatural).FirstOrDefault();
                        propaga.medSemanalIncremental = propagacoes.Where(x => x.IdPosto == 1).Select(x => x.medSemanalIncremental).FirstOrDefault();
                        propaga.medSemanalNatural = propagacoes.Where(x => x.IdPosto == 1).Select(x => x.medSemanalNatural).FirstOrDefault(); ;
                    }

                    if (propaga.IdPosto == 119)
                    {
                        var fator161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.Modelo[0].FatorDistribuicao).FirstOrDefault();
                        var vazoes161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.VazaoIncremental).FirstOrDefault();

                        var fator119 = propaga.Modelo[0].FatorDistribuicao;
                        foreach (var vaz in vazoes161)
                        {
                            propaga.VazaoIncremental[vaz.Key] = (vaz.Value / fator161) * fator119;
                            propaga.VazaoNatural[vaz.Key] = propaga.VazaoIncremental[vaz.Key];

                        }
                        foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                        {
                            if (dat <= ultimaDiaAcomph)
                            {
                                propaga.VazaoIncremental[dat] = 0;
                                propaga.VazaoNatural[dat] = propaga.VazaoIncremental[dat];
                            }
                        }
                    }

                    if (propaga.IdPosto == 118)
                    {
                        var p119 = propagacoes.Where(x => x.IdPosto == 119).Select(x => x.VazaoIncremental).FirstOrDefault();
                        foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                        {
                            propaga.VazaoNatural[dat] = 0.8103f * p119[dat] + 0.185f;
                            propaga.VazaoIncremental[dat] = propaga.VazaoNatural[dat];
                        }

                    }

                    if (propaga.IdPosto == 160)
                    {
                        var fator161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.Modelo[0].FatorDistribuicao).FirstOrDefault();
                        var vazoes161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.VazaoIncremental).FirstOrDefault();

                        var fator160 = propaga.Modelo[0].FatorDistribuicao;
                        foreach (var vaz in vazoes161)
                        {
                            if (vaz.Key <= ultimaDiaAcomph)
                            {
                                propaga.VazaoIncremental[vaz.Key] = 0;
                                propaga.VazaoNatural[vaz.Key] = propaga.VazaoIncremental[vaz.Key];
                            }
                            else
                                propaga.VazaoIncremental[vaz.Key] = (vaz.Value / fator161) * fator160;
                            propaga.VazaoNatural[vaz.Key] = propaga.VazaoIncremental[vaz.Key];

                        }
                    }
                    if (propaga.IdPosto == 211)//funil
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;

                                            var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                            var dmaior = dmenor.AddDays(1);

                                            if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                            var horaatraso = (pm.TempoViagem % 24);
                                            if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                                propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }
                    if (propaga.IdPosto == 6)//furnas
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                        var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);

                                        var horaatraso = (ms.TempoViagem % 24);

                                        if (ms.NomeVazao.ToUpper() == "FURNAS")
                                        {
                                            propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dat];
                                        }
                                        else
                                        {
                                            if (modeloSmap.Vazoes.ContainsKey(dmaior) && modeloSmap.Vazoes.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dmaior] * (24f - horaatraso) / 24f;

                                            }
                                        }

                                    }
                                    foreach (var pm in propaga.PostoMontantes)
                                    {
                                        var postoMontante = pm.Propaga;

                                        var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);

                                        var horaatraso = (pm.TempoViagem % 24);
                                        if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                        {
                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }
                    if (propaga.IdPosto == 7)//MMoraes
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;

                                            var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                            var dmaior = dmenor.AddDays(1);

                                            if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                            var horaatraso = (pm.TempoViagem % 24);
                                            if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                            }
                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 8)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;

                                            var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                            var dmaior = dmenor.AddDays(1);

                                            if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                            var horaatraso = (pm.TempoViagem % 24);
                                            if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                            }
                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 9)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;

                                            var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                            var dmaior = dmenor.AddDays(1);

                                            if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                            var horaatraso = (pm.TempoViagem % 24);
                                            if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                            }
                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 10)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 11)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 12)//porto columbia
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                        var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);

                                        var horaatraso = (ms.TempoViagem % 24);

                                        if (ms.NomeVazao.ToUpper() == "PCOLOMBIA")
                                        {
                                            propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dat] * ms.FatorDistribuicao;
                                        }
                                        else
                                        {
                                            if (modeloSmap.Vazoes.ContainsKey(dmaior) && modeloSmap.Vazoes.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dmaior] * (24f - horaatraso) / 24f;

                                            }
                                        }

                                    }
                                    foreach (var pm in propaga.PostoMontantes)
                                    {
                                        var postoMontante = pm.Propaga;

                                        var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);

                                        var horaatraso = (pm.TempoViagem % 24);
                                        if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                        {
                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 15)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 16)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                            {
                                if (dat <= ultimaDiaAcomph)
                                {
                                    propaga.VazaoIncremental[dat] = 0;
                                }
                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                        var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);

                                        var horaatraso = (ms.TempoViagem % 24);

                                        if (ms.NomeVazao.ToUpper() == "MARIMBONDO")
                                        {
                                            propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat] * ms.FatorDistribuicao;
                                        }


                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }

                        AjustaVazaoNatPosto(propaga);

                    }

                    if (propaga.IdPosto == 17)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                            {
                                if (dat <= ultimaDiaAcomph)
                                {
                                    propaga.VazaoIncremental[dat] = 0;
                                }
                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                        var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);
                                        if (ms.NomeVazao.ToUpper() == "PASSAGEM")
                                        {
                                            var horaatraso = (ms.TempoViagem % 24);
                                            if (modeloSmap.Vazoes.ContainsKey(dmaior) && modeloSmap.Vazoes.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dmenor] * horaatraso / 24f;
                                                propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dmaior] * (24f - horaatraso) / 24f;
                                            }
                                        }

                                        if (ms.NomeVazao.ToUpper() == "MARIMBONDO")
                                        {
                                            propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat] * ms.FatorDistribuicao;
                                        }


                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }

                        // AjustaVazaoNatPosto(propaga);

                        try
                        {
                            //var ultimaDiaAcomph = dadosAcompH.Select(x => x.data).Last();

                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;

                                            var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                            var dmaior = dmenor.AddDays(1);
                                            if (pm.TempoViagem >= 72)
                                            {
                                                dmenor = dmenor.AddDays(1);
                                                dmaior = dmenor.AddDays(1);
                                            }


                                            var horaatraso = (pm.TempoViagem % 24);
                                            if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                            {
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                            }
                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }

                    }
                    if (propaga.IdPosto == 18)//AVERMELHA
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 161)
                    {
                        try
                        {

                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {
                                    if (dat > ultimaDiaAcomph)
                                    {
                                        if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;

                                        if (dat == ultimaDiaAcomph.AddDays(1))
                                        {
                                            propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                        }
                                        else
                                        {
                                            foreach (var pm in propaga.PostoMontantes)
                                            {
                                                var postoMontante = pm.Propaga;
                                                if (postoMontante.NomePostoFluv == "Guarapiranga")
                                                {
                                                    var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                                    var dmaior = dmenor.AddDays(1);

                                                    if (pm.TempoViagem >= 72)
                                                    {
                                                        dmenor = dmenor.AddDays(1);
                                                        dmaior = dmenor.AddDays(1);
                                                    }

                                                    var horaatraso = (pm.TempoViagem % 24);
                                                    if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                                    {
                                                        propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                                        propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                                    }
                                                }
                                                else if (postoMontante.NomePostoFluv == "Billings")
                                                {
                                                    propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dat];
                                                }
                                                else if (postoMontante.NomePostoFluv == "Ponte Nova")
                                                {
                                                    var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                                    var dmaior = dmenor.AddDays(1);

                                                    if (pm.TempoViagem >= 72)
                                                    {
                                                        dmenor = dmenor.AddDays(1);
                                                        dmaior = dmenor.AddDays(1);
                                                    }
                                                    var vazaoGuara = propagacoes.Where(x => x.IdPosto == 160).Select(x => x.VazaoNatural).FirstOrDefault();

                                                    var horaatraso = (pm.TempoViagem % 24);
                                                    if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                                    {
                                                        propaga.VazaoNatural[dat] += vazaoGuara[dmenor] * horaatraso / 24f;
                                                        propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                                    }
                                                }
                                            }
                                            propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];
                                        }


                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 237)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 238)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 239)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 240)
                    {
                        AjustaVazaoIncPosto(propaga);
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 242)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    if (!propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();
                                        var dmenor = dat.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                        var dmaior = dmenor.AddDays(1);
                                        if (ms.NomeVazao == "NAvanhanda")
                                        {
                                            propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat] * ms.FatorDistribuicao;

                                        }

                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 32)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {


                                    if (dat > ultimaDiaAcomph)
                                    {
                                        if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;


                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dat];

                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 33)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {
                                try
                                {


                                    if (dat > ultimaDiaAcomph)
                                    {
                                        if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                        foreach (var pm in propaga.PostoMontantes)
                                        {
                                            var postoMontante = pm.Propaga;


                                            propaga.VazaoNatural[dat] += postoMontante.VazaoNatural[dat];

                                        }
                                        propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];

                                    }
                                }
                                catch (Exception e)
                                {
                                    e.ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }
                    //if (propaga.IdPosto == 222)
                    //{
                    //    try
                    //    {
                    //        foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                    //        {

                    //            if (dat > ultimaDiaAcomph)
                    //            {
                    //                if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                    //                if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                    //                foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                    //                {
                    //                    var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();


                    //                    propaga.VazaoNatural[dat] += modeloSmap.Vazoes[dat];
                    //                }

                    //            }
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        e.ToString();
                    //    }
                    //}

                    if (propaga.IdPosto == 216)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    if (!propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();


                                        propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat];
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 220)
                    {
                        try
                        {
                            foreach (var dat in propaga.VazaoIncremental.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    if (!propaga.VazaoIncremental.ContainsKey(dat)) propaga.VazaoIncremental[dat] = 0;
                                    foreach (var ms in propaga.Modelo)//var ms in propaga.Modelo
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();


                                        propaga.VazaoIncremental[dat] += modeloSmap.Vazoes[dat];
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 94)
                    {
                        var machadinho = propagacoes.Where(x => x.IdPosto == 217).FirstOrDefault();
                        var ita = propagacoes.Where(x => x.IdPosto == 92).FirstOrDefault();
                        try
                        {
                            foreach (var dat in machadinho.VazaoNatural.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (machadinho.VazaoNatural.ContainsKey(dat)) machadinho.VazaoNatural[dat] = 0;
                                    if (!machadinho.VazaoNatural.ContainsKey(dat)) machadinho.VazaoNatural[dat] = 0;
                                    foreach (var mt in machadinho.PostoMontantes)//var ms in propaga.Modelo
                                    {
                                        machadinho.VazaoNatural[dat] += mt.Propaga.VazaoNatural[dat];
                                    }
                                    machadinho.VazaoNatural[dat] += machadinho.VazaoIncremental[dat];
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }

                        try
                        {
                            foreach (var dat in ita.VazaoNatural.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (ita.VazaoNatural.ContainsKey(dat)) ita.VazaoNatural[dat] = 0;
                                    if (!ita.VazaoNatural.ContainsKey(dat)) ita.VazaoNatural[dat] = 0;
                                    foreach (var mt in ita.PostoMontantes)//var ms in propaga.Modelo
                                    {
                                        ita.VazaoNatural[dat] += mt.Propaga.VazaoNatural[dat];
                                    }
                                    ita.VazaoNatural[dat] += ita.VazaoIncremental[dat];
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }

                        try
                        {
                            foreach (var dat in propaga.VazaoNatural.Keys.ToList())
                            {

                                if (dat > ultimaDiaAcomph)
                                {
                                    if (propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                    if (!propaga.VazaoNatural.ContainsKey(dat)) propaga.VazaoNatural[dat] = 0;
                                    foreach (var mt in propaga.PostoMontantes)//var ms in propaga.Modelo
                                    {
                                        propaga.VazaoNatural[dat] += mt.Propaga.VazaoNatural[dat];
                                    }
                                    propaga.VazaoNatural[dat] += propaga.VazaoIncremental[dat];
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }

                    if (propaga.IdPosto == 243)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 34)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 245)
                    {
                        AjustaVazaoNatPosto(propaga);
                    }

                    if (propaga.IdPosto == 246)
                    {
                        AjustaVazaoNatPosto(propaga);

                    }

                    if (propaga.IdPosto == 266)
                    {
                        AjustaVazaoNatPosto(propaga);

                    }
                }
                #endregion

                foreach (var propaga in propagacoes)
                {
                    /*if(propaga.IdPosto == 18)
                    { }*/
                    //CalculaVazaoPosto(propaga);
                    CalculaMedSemanal(propaga);
                }


                // recalcula as medias naturais e incrementais que serao usadas para o previvaz  obs:cada posto tem suas particularidades
                // em sua maioria, os dados da semana atual serão obtidos da media VNA, mas para a semana prevista serão usados os dados da 
                // media VNI dos postos somadas com as medias VNI dos seus montantes salvo algumas excecoes que serao descritas logo depois.
                foreach (var propaga in propagacoes)
                {
                    RecalculaMediaNat(propaga);

                    RecalculaMediaInc(propaga);
                }
                #region adiciona postos faltantes SMAP
                var pedras = new Propagacao() { IdPosto = 116, NomePostoFluv = "Pedras" };
                propagacoes.Add(pedras);

                var Hborden = new Propagacao() { IdPosto = 318, NomePostoFluv = "HBorden" };
                propagacoes.Add(Hborden);

                var iSolteiraInc = new Propagacao() { IdPosto = 139, NomePostoFluv = "iSolteiraInc" };
                propagacoes.Add(iSolteiraInc);

                var jupiaInc = new Propagacao() { IdPosto = 136, NomePostoFluv = "jupiaInc" };
                propagacoes.Add(jupiaInc);

                var pPrimaINc = new Propagacao() { IdPosto = 137, NomePostoFluv = "pPrimaINc" };
                propagacoes.Add(pPrimaINc);

                var tresIrmInc = new Propagacao() { IdPosto = 138, NomePostoFluv = "tresIrmInc" };
                propagacoes.Add(tresIrmInc);

                var itaipuInc = new Propagacao() { IdPosto = 166, NomePostoFluv = "itaipuInc" };
                propagacoes.Add(itaipuInc);

                var billingsCALc = new Propagacao() { IdPosto = 319, NomePostoFluv = "billingsCALc" };
                propagacoes.Add(billingsCALc);

                CalculaMediaFaltante(propagacoes, ultimaDiaAcomph);


                #endregion

                #region adiona postos previvaz
                //adiciona os postos previvaz que terao a media da semanal atual calculada usando dados do acomph e uma projeção para o fim da semana.
                var propagacoesAux = new List<Propagacao>();

                var todoPosto = dadosAcompH.Select(x => x.posto);
                todoPosto = todoPosto.Union(dadosAcompH.Select(x => x.posto));
                foreach (var item in todoPosto)
                {
                    if (propagacoes.All(x => !x.IdPosto.Equals(item))/* || item == 22 /*|| item == 222 || item == 248 */)//estes postos são Smap mas sao tratados como previvaz
                    {
                        if (item != 81 && item != 227 && item != 228)//estes postos nao possuem serão incluidos no método GetPrevs, pois seus dados serão obtidos atraves do prevs oficial
                        {
                            var propAux = new Propagacao() { IdPosto = item, NomePostoFluv = "PostoPreviz" };
                            var vaz = dadosAcompH.Where(v => v.posto == propAux.IdPosto);
                            foreach (var v in vaz)
                            {
                                propAux.VazaoNatural[v.data] = v.qnat;//atribui os dados do acomph 
                            }

                            propagacoesAux.Add(propAux);
                        }
                    }
                }

                CalcSemanaPrevivaz(propagacoesAux, propagacoes);
                foreach (var prop in propagacoesAux)
                {
                    //if (prop.IdPosto == 22 /*|| prop.IdPosto == 222*/ || prop.IdPosto == 248)
                    //{
                    //    var remove = propagacoes.Where(x => x.IdPosto == prop.IdPosto).FirstOrDefault();
                    //    propagacoes.Remove(remove);// substitui os dados desses postos pelos novos dados com a semana calculada como previvaz
                    //    propagacoes.Add(prop);
                    //}
                    //else
                    //{
                    //    propagacoes.Add(prop);
                    //}
                    propagacoes.Add(prop);

                }

                #endregion

                #region adiciona cpins
                bool ext = pastaSaida.Contains("_ECENS45");
                AdicionaMPV(propagacoes, dadosAcompH, ext);
                //AdicionaCPINS(propagacoes, dadosAcompH);

                #endregion
                PropagacaoMuskingun(propagacoes, dataForms, modelos, dadosAcompH, shadow);//propagação da bacia tocantins, madeira, jeq_Parnaiba
                //ExportaDadvaz(pastaSaida, propagacoes, runrevDate, modelos, revnum);

                GetPrevs(propagacoes, dataForms);

                #region adiciona postos caculados



                var estrela = propagacoes.Where(x => x.IdPosto == 260).FirstOrDefault();
                if (estrela == null)
                {
                    estrela = new Propagacao() { IdPosto = 260, NomePostoFluv = "estrela" };
                    propagacoes.Add(estrela);
                }




                var paranapanema = propagacoes.Where(x => x.IdPosto == 53).FirstOrDefault();
                if (paranapanema == null)
                {
                    paranapanema = new Propagacao() { IdPosto = 53, NomePostoFluv = "paranapanema" };
                    propagacoes.Add(paranapanema);
                }

                var p241Srverdinho = propagacoes.Where(x => x.IdPosto == 241).First();
                var p48Piraju = propagacoes.Where(x => x.IdPosto == 48).First();

                foreach (var dia in p48Piraju.VazaoNatural.Keys.ToList())
                {
                    paranapanema.VazaoNatural[dia] = p48Piraju.VazaoNatural[dia];
                }

                foreach (var dia in p48Piraju.VazaoIncremental.Keys.ToList())
                {
                    paranapanema.VazaoIncremental[dia] = p48Piraju.VazaoIncremental[dia];
                }

                foreach (var dia in p48Piraju.calMedSemanal.Keys.ToList())
                {
                    paranapanema.calMedSemanal[dia] = p48Piraju.calMedSemanal[dia];
                }
                foreach (var dia in p48Piraju.medSemanalIncremental.Keys.ToList())
                {
                    paranapanema.medSemanalIncremental[dia] = p48Piraju.medSemanalIncremental[dia];
                }
                foreach (var dia in p48Piraju.medSemanalNatural.Keys.ToList())
                {
                    paranapanema.medSemanalNatural[dia] = p48Piraju.medSemanalNatural[dia];
                }/////////////////

                foreach (var dia in p241Srverdinho.VazaoNatural.Keys.ToList())
                {
                    estrela.VazaoNatural[dia] = p241Srverdinho.VazaoNatural[dia] * 0.68;
                }

                foreach (var dia in p241Srverdinho.VazaoIncremental.Keys.ToList())
                {
                    estrela.VazaoIncremental[dia] = p241Srverdinho.VazaoIncremental[dia] * 0.68;
                }

                foreach (var dia in p241Srverdinho.calMedSemanal.Keys.ToList())
                {
                    estrela.calMedSemanal[dia] = p241Srverdinho.calMedSemanal[dia] * 0.68;
                }
                foreach (var dia in p241Srverdinho.medSemanalIncremental.Keys.ToList())
                {
                    estrela.medSemanalIncremental[dia] = p241Srverdinho.medSemanalIncremental[dia] * 0.68;
                }
                foreach (var dia in p241Srverdinho.medSemanalNatural.Keys.ToList())
                {
                    estrela.medSemanalNatural[dia] = p241Srverdinho.medSemanalNatural[dia] * 0.68;
                }



                #endregion

                propagacoes = propagacoes.OrderBy(x => x.IdPosto).ToList();

                if (pastaSaida.Contains("ECENS45"))
                {
                    DateTime dataEx = runrevDate.AddDays(7);
                    //while (DataR.DayOfWeek != DayOfWeek.Friday) DataR = DataR.AddDays(1);

                    // DataR = DataR.AddDays(14);

                    foreach (var prop in propagacoes.Where(x => comPrevivaz.Any(y => y == x.IdPosto) || regredidoDePrevivaz.Any(z => z == x.IdPosto)))
                    {
                        if (prop.IdPosto == 228)
                        {

                        }
                        if (prop.IdPosto == 287)
                        {

                        }
                        var datas = prop.calMedSemanal.Select(x => x.Key).Where(x => x.Date > dataEx).ToList();
                        datas.ForEach(x => prop.calMedSemanal.Remove(x));

                        var datasN = prop.VazaoNatural.Select(x => x.Key).Where(x => x.Date > dataEx).ToList();
                        datasN.ForEach(x => prop.VazaoNatural.Remove(x));

                        var datasI = prop.VazaoIncremental.Select(x => x.Key).Where(x => x.Date > dataEx).ToList();
                        datasI.ForEach(x => prop.VazaoIncremental.Remove(x));

                        var datasMI = prop.medSemanalIncremental.Select(x => x.Key).Where(x => x.Date > dataEx).ToList();
                        datasMI.ForEach(x => prop.medSemanalIncremental.Remove(x));

                        var datasMN = prop.medSemanalNatural.Select(x => x.Key).Where(x => x.Date > dataEx).ToList();
                        datasMN.ForEach(x => prop.medSemanalNatural.Remove(x));

                    }
                }




                foreach (var propaga in propagacoes)// escreve os id de postos e suas medias serão usadas no previvaz em Para_STR.txt para consulta do usuário
                {

                    try
                    {
                        //var caminho = pastaSaida + "\\Postos_Dados.txt";
                        var paraSTR = pastaSaida + "\\Para_STR.txt";
                        //ChuvaVazaoTools.Tools.Tools.addHistory(caminho, propaga.IdPosto.ToString());
                        //ChuvaVazaoTools.Tools.Tools.addHistory(caminho, "mediaNAT:");
                        //foreach (var item in propaga.medSemanalNatural.Keys.ToList())
                        //{
                        //    ChuvaVazaoTools.Tools.Tools.addHistory(caminho, item.ToString("dd/MM/yyyy") + " " + propaga.medSemanalNatural[item].ToString());
                        //}

                        //ChuvaVazaoTools.Tools.Tools.addHistory(caminho, "mediaINC:");
                        //foreach (var item in propaga.medSemanalNatural.Keys.ToList())
                        //{
                        //    ChuvaVazaoTools.Tools.Tools.addHistory(caminho, item.ToString("dd/MM/yyyy") + " " + propaga.medSemanalIncremental[item].ToString());
                        //}

                        //ChuvaVazaoTools.Tools.Tools.addHistory(caminho, "MEDIA REAL:");
                        //foreach (var item in propaga.calMedSemanal.Keys.ToList())
                        //{
                        //    ChuvaVazaoTools.Tools.Tools.addHistory(caminho, item.ToString("dd/MM/yyyy") + " " + propaga.calMedSemanal[item].ToString());
                        //}

                        ChuvaVazaoTools.Tools.Tools.addHistory(paraSTR, propaga.IdPosto.ToString());
                        ChuvaVazaoTools.Tools.Tools.addHistory(paraSTR, "PARA_STR:");
                        foreach (var item in propaga.calMedSemanal.Keys.ToList())
                        {
                            ChuvaVazaoTools.Tools.Tools.addHistory(paraSTR, item.ToString("dd/MM/yyyy") + " " + propaga.calMedSemanal[item].ToString());
                        }

                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }


                return propagacoes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<DadvazPrevs> GetDadvazPrevs()
        {
            string csv = @"H:\TI - Sistemas\UAT\ChuvaVazao\DADVAZ_para_CHUVA.csv";
            var csvLines = File.ReadAllLines(csv).Skip(1).ToList();
            List<DadvazPrevs> dadPrevs = new List<DadvazPrevs>();

            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

            foreach (var cl in csvLines)
            {
                var campos = cl.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                DadvazPrevs dadPre = new DadvazPrevs();
                dadPre.NomeUsina = campos[0];
                dadPre.Bacia = campos[1];
                dadPre.NumDadvaz = Convert.ToInt32(campos[2]);
                dadPre.NumPrevs = Convert.ToInt32(campos[3]);
                dadPre.Fator = Convert.ToDouble(campos[4], Culture.NumberFormat);
                dadPrevs.Add(dadPre);
            }

            return dadPrevs;
        }

        public void ExportaDadvaz(string pastaSaida, List<Propagacao> propagacoes, DateTime dataFim, List<ModeloChuvaVazao> modelos, int revnum)
        {
            List<Tuple<DateTime, double, double, double>> dadosMPV = GetMPV();
            List<DadvazPrevs> dadPrevs = GetDadvazPrevs();
            if (dadosMPV.Count() > 0 && dadPrevs.Count() > 0)
            {
                foreach (var dad in dadPrevs)
                {
                    for (DateTime dt = dataFim.AddDays(-6); dt <= dataFim; dt = dt.AddDays(1)) //foreach (var dad in dadPrevs)
                    {
                        if (!dad.Vazao.ContainsKey(dt)) dad.Vazao[dt] = 0;

                        int numeroProp = dad.NumPrevs;
                        double vazao = 0;
                        if (numeroProp == 22 || numeroProp == 248)
                        {

                        }
                        if (numeroProp == 271 || numeroProp == 275 || numeroProp == 285)//muskingun
                        {
                            vazao = propagacoes.Where(x => x.IdPosto == numeroProp).Select(x => x.VazaoIncremental[dt]).FirstOrDefault();
                            dad.Vazao[dt] = vazao;
                            continue;
                        }
                        else if (numeroProp == 168)//sobradinho
                        {
                            var dMPV = dadosMPV.Where(x => x.Item1.Date == dt.Date).FirstOrDefault();
                            if (dMPV != null)
                            {
                                vazao = dMPV.Item2 + dMPV.Item4;
                            }
                            dad.Vazao[dt] = vazao;
                            continue;
                        }
                        else
                        {
                            if (dad.Fator == 0)
                            {
                                dad.Vazao[dt] = 0;
                            }
                            else
                            {
                                var prop = propagacoes.Where(x => x.IdPosto == numeroProp).FirstOrDefault();
                                if (prop != null)
                                {
                                    if (prop.Modelo.Count() == 1)
                                    {
                                        var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == prop.Modelo[0].NomeVazao.ToUpper()).First();
                                        dad.Vazao[dt] = modeloSmap.Vazoes[dt] * dad.Fator;
                                        continue;
                                    }
                                    else
                                    {
                                        foreach (var ms in prop.Modelo)
                                        {
                                            var modeloSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == ms.NomeVazao.ToUpper()).First();


                                            if (ms.TempoViagem == 0)
                                                dad.Vazao[dt] += modeloSmap.Vazoes[dt] * ms.FatorDistribuicao;
                                            else
                                            {
                                                var dmenor = dt.AddDays(-(1 + Math.Floor(ms.TempoViagem / 24))).Date;
                                                //var dmenor = dt.AddHours(-ms.TempoViagem).Date;
                                                var dmaior = dmenor.AddDays(1);

                                                if (!dad.Vazao.ContainsKey(dt)) dad.Vazao[dt] = 0;
                                                var horaatraso = (ms.TempoViagem % 24);
                                                if (modeloSmap.Vazoes.ContainsKey(dmaior) && modeloSmap.Vazoes.ContainsKey(dmenor))
                                                {
                                                    dad.Vazao[dt] += modeloSmap.Vazoes[dmenor] * horaatraso / 24f;
                                                    dad.Vazao[dt] += modeloSmap.Vazoes[dmaior] * (24f - horaatraso) / 24f;
                                                }
                                            }

                                        }
                                    }

                                }
                                else
                                {
                                    dad.Vazao[dt] = 0;
                                }
                            }
                        }

                    }
                }
                string dadvazFile = @"H:\TI - Sistemas\UAT\ChuvaVazao\dadvaz.dat";


                for (DateTime dt = dataFim.AddDays(-6); dt <= dataFim; dt = dt.AddDays(1))
                {
                    string newdadvazName = $"dadvaz_RV{revnum}_{dt:ddMMyyyy}.dat";
                    Compass.CommomLibrary.Dadvaz.Dadvaz dadvaz = Compass.CommomLibrary.DocumentFactory.Create(dadvazFile) as Compass.CommomLibrary.Dadvaz.Dadvaz;

                    var vazoes = dadvaz.BlocoVazoes.ToList();
                    var comment = vazoes.First().Comment;
                    Dictionary<int, int> usiTipoVaz = new Dictionary<int, int>();
                    dadvaz.BlocoVazoes.ToList().ForEach(x =>
                    {
                        if (!usiTipoVaz.ContainsKey(x.Usina)) usiTipoVaz[x.Usina] = x.TipoVaz;

                    });

                    dadvaz.BlocoVazoes.Clear();

                    var dataLine = dadvaz.BlocoData.First();
                    dataLine.Dia = dt.Day;
                    dataLine.Mes = dt.Month;
                    dataLine.Ano = dt.Year;

                    var diaLine = dadvaz.BlocoDia.First();
                    int dia = 0;
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Saturday:
                            dia = 1;
                            break;
                        case DayOfWeek.Sunday:
                            dia = 2;
                            break;
                        case DayOfWeek.Monday:
                            dia = 3;
                            break;
                        case DayOfWeek.Tuesday:
                            dia = 4;
                            break;
                        case DayOfWeek.Wednesday:
                            dia = 5;
                            break;
                        case DayOfWeek.Thursday:
                            dia = 6;
                            break;
                        case DayOfWeek.Friday:
                            dia = 7;
                            break;
                        default:
                            dia = 1;
                            break;

                    }
                    diaLine.diainicial = dia;
                    foreach (var usi in usiTipoVaz.Keys)//foreach (var dad in dadPrevs)
                    {
                        var dad = dadPrevs.Where(x => x.NumDadvaz == usi).FirstOrDefault();
                        if (dad != null)
                        {
                            for (DateTime dtdad = dt; dtdad <= dataFim; dtdad = dtdad.AddDays(1))
                            {
                                var newVaz = new Compass.CommomLibrary.Dadvaz.VazoesLine();
                                newVaz.DiaInic = $"{dtdad.Day:00}";
                                newVaz.DiaFinal = $"F";
                                newVaz.Usina = dad.NumDadvaz;
                                newVaz.Nome = dad.NomeUsina;
                                newVaz.TipoVaz = usiTipoVaz[dad.NumDadvaz];
                                newVaz.Vazao = (float)dad.Vazao[dtdad];
                                dadvaz.BlocoVazoes.Add(newVaz);
                            }
                        }

                    }

                    var zerados = dadvaz.BlocoVazoes.Where(x => x.Vazao == 0).GroupBy(x => x.Usina).ToList();

                    foreach (var zs in zerados)
                    {
                        foreach (var z in zs.Where(x => x != zs.First()))
                        {
                            dadvaz.BlocoVazoes.Remove(z);
                        }
                    }


                    dadvaz.BlocoVazoes.First().Comment = comment;
                    dadvaz.SaveToFile(Path.Combine(pastaSaida, newdadvazName));

                }
            }

        }
        public void PropagacaoMuskingun(List<Propagacao> propagacoes, DateTime data, List<ModeloChuvaVazao> modelos, List<CONSULTA_VAZAO> dadosAcompH, bool shadow = false)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            var ultimoAcomph = dadosAcompH.Select(x => x.data).Last();

            var dataInicio = data.AddDays(-95);//inicio da propagação Muskingun para tocantins e Madeira 
            var dataInicioJeqParna = data.AddDays(-45); // inicio da propagação Muskingun Jeq_Parnaiba
            //todo: confirmar a questão das data inicio das propagaçoes(confirmar se pega as datas certas de acordo com rodadas d-1 e depois do acomph)
            try
            {

                #region paraiba do sul

                #region santana

                var santanaSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "LAJESTOCOS".ToUpper()).Select(x => x.Vazoes).First();
                var vazAcomphtocos = dadosAcompH.Where(x => x.posto == 201).ToList();
                var santanaLajes = propagacoes.Where(x => x.IdPosto == 203).FirstOrDefault();
                var tocos = propagacoes.Where(x => x.IdPosto == 201).FirstOrDefault();
                //santanaLajes.VazaoIncremental.Clear();
                //santanaLajes.VazaoNatural.Clear();
                //santanaLajes.calMedSemanal.Clear();
                //santanaLajes.medSemanalIncremental.Clear();
                //santanaLajes.medSemanalNatural.Clear();
                try
                {
                    foreach (var dia in santanaSmap.Keys.ToList())
                    {
                        if (dia <= ultimoAcomph)
                        {
                            var valor = Convert.ToDouble(vazAcomphtocos.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                            santanaLajes.VazaoNatural[dia] = santanaSmap[dia] * 0.997f;
                            //sCaxias.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                            santanaLajes.VazaoIncremental[dia] = santanaLajes.VazaoNatural[dia] - valor;


                        }
                        else
                        {
                            //sCaxias.VazaoNatural[dia] = SaltocaSmap[dia];
                            //sCaxias.VazaoIncremental[dia] = SaltocaSmap[dia] * 0.64f;
                            santanaLajes.VazaoNatural[dia] = santanaSmap[dia] * 0.997f;
                            santanaLajes.VazaoIncremental[dia] = santanaLajes.VazaoNatural[dia] - tocos.VazaoNatural[dia];
                        }

                    }
                }
                catch
                {

                }

                CalcMediaMuskingun(santanaLajes);


                #endregion

                #endregion
                #region iguacu

                #region saltacaxias

                //var SaltocaSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "BAIXOIG".ToUpper()).Select(x => x.Vazoes).First();
                //var vazAcomphCaxias = dadosAcompH.Where(x => x.posto == 222).ToList();
                //var sCaxias = propagacoes.Where(x => x.IdPosto == 222).FirstOrDefault();
                //sCaxias.VazaoIncremental.Clear();
                //sCaxias.VazaoNatural.Clear();
                //sCaxias.calMedSemanal.Clear();
                //sCaxias.medSemanalIncremental.Clear();
                //sCaxias.medSemanalNatural.Clear();

                //foreach (var dia in SaltocaSmap.Keys.ToList())
                //{
                //    if (dia <= ultimoAcomph)
                //    {
                //        var valor = Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                //        sCaxias.VazaoNatural[dia] = valor;
                //        sCaxias.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphCaxias.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);


                //    }
                //    else
                //    {
                //        //sCaxias.VazaoNatural[dia] = SaltocaSmap[dia];
                //        //sCaxias.VazaoIncremental[dia] = SaltocaSmap[dia] * 0.64f;
                //        sCaxias.VazaoNatural[dia] = SaltocaSmap[dia] * 0.51f;
                //        sCaxias.VazaoIncremental[dia] = SaltocaSmap[dia] * 0.51f;
                //    }

                //}
                //CalcMediaMuskingun(sCaxias);

                #endregion

                #endregion


                #region correcao postos uruguai

                #region sao roque

                var cnSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "CN".ToUpper()).Select(x => x.Vazoes).First();
                var vazAcomphGarib = dadosAcompH.Where(x => x.posto == 89).ToList();
                var sroque = propagacoes.Where(x => x.IdPosto == 88).FirstOrDefault();


                sroque.VazaoIncremental.Clear();
                sroque.VazaoNatural.Clear();
                sroque.calMedSemanal.Clear();
                sroque.medSemanalIncremental.Clear();
                sroque.medSemanalNatural.Clear();

                foreach (var dia in cnSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        var valor = Convert.ToDouble(vazAcomphGarib.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        sroque.VazaoNatural[dia] = valor * 0.82f;
                        sroque.VazaoIncremental[dia] = sroque.VazaoNatural[dia];

                    }
                    else
                    {
                        sroque.VazaoNatural[dia] = cnSmap[dia] * 0.745f;
                        sroque.VazaoIncremental[dia] = sroque.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(sroque);
                #endregion

                #region garibaldi

                var gari = propagacoes.Where(x => x.IdPosto == 89).FirstOrDefault();


                gari.VazaoIncremental.Clear();
                gari.VazaoNatural.Clear();
                gari.calMedSemanal.Clear();
                gari.medSemanalIncremental.Clear();
                gari.medSemanalNatural.Clear();

                foreach (var dia in cnSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        gari.VazaoNatural[dia] = Convert.ToDouble(vazAcomphGarib.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        gari.VazaoIncremental[dia] = gari.VazaoNatural[dia];

                    }
                    else
                    {
                        gari.VazaoNatural[dia] = cnSmap[dia] * 0.165f + sroque.VazaoNatural[dia];
                        gari.VazaoIncremental[dia] = gari.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(gari);

                #endregion

                #region camposnovos

                var campos = propagacoes.Where(x => x.IdPosto == 216).FirstOrDefault();
                var vazAcomphCampos = dadosAcompH.Where(x => x.posto == 216).ToList();


                campos.VazaoIncremental.Clear();
                campos.VazaoNatural.Clear();
                campos.calMedSemanal.Clear();
                campos.medSemanalIncremental.Clear();
                campos.medSemanalNatural.Clear();

                foreach (var dia in cnSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        campos.VazaoNatural[dia] = Convert.ToDouble(vazAcomphCampos.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        campos.VazaoIncremental[dia] = campos.VazaoNatural[dia];

                    }
                    else
                    {
                        campos.VazaoNatural[dia] = cnSmap[dia] * 0.09f + gari.VazaoNatural[dia];
                        campos.VazaoIncremental[dia] = campos.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(campos);

                #endregion

                #region machadinho

                var macha = propagacoes.Where(x => x.IdPosto == 217).FirstOrDefault();
                var vazAcomphMacha = dadosAcompH.Where(x => x.posto == 217).ToList();
                var machaSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "Machadinho".ToUpper()).Select(x => x.Vazoes).First();
                var bgSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "BG".ToUpper()).Select(x => x.Vazoes).First();


                macha.VazaoIncremental.Clear();
                macha.VazaoNatural.Clear();
                macha.calMedSemanal.Clear();
                macha.medSemanalIncremental.Clear();
                macha.medSemanalNatural.Clear();

                foreach (var dia in machaSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        macha.VazaoNatural[dia] = Convert.ToDouble(vazAcomphMacha.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        macha.VazaoIncremental[dia] = macha.VazaoNatural[dia];

                    }
                    else
                    {
                        macha.VazaoNatural[dia] = machaSmap[dia] + campos.VazaoNatural[dia] + bgSmap[dia];
                        macha.VazaoIncremental[dia] = macha.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(macha);

                #endregion

                #region ITA

                var ita = propagacoes.Where(x => x.IdPosto == 92).FirstOrDefault();
                var vazAcomphIta = dadosAcompH.Where(x => x.posto == 92).ToList();
                var itaSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "Ita".ToUpper()).Select(x => x.Vazoes).First();


                ita.VazaoIncremental.Clear();
                ita.VazaoNatural.Clear();
                ita.calMedSemanal.Clear();
                ita.medSemanalIncremental.Clear();
                ita.medSemanalNatural.Clear();

                foreach (var dia in itaSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        ita.VazaoNatural[dia] = Convert.ToDouble(vazAcomphIta.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        ita.VazaoIncremental[dia] = ita.VazaoNatural[dia];

                    }
                    else
                    {
                        ita.VazaoNatural[dia] = macha.VazaoNatural[dia] + itaSmap[dia];
                        ita.VazaoIncremental[dia] = ita.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(ita);

                #endregion

                #region Foz chapeco

                var chap = propagacoes.Where(x => x.IdPosto == 94).FirstOrDefault();
                var monjolinho = propagacoes.Where(x => x.IdPosto == 220).FirstOrDefault();
                var vazAcomphChap = dadosAcompH.Where(x => x.posto == 94).ToList();
                var chapSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "FozChapeco".ToUpper()).Select(x => x.Vazoes).First();


                chap.VazaoIncremental.Clear();
                chap.VazaoNatural.Clear();
                chap.calMedSemanal.Clear();
                chap.medSemanalIncremental.Clear();
                chap.medSemanalNatural.Clear();

                foreach (var dia in chapSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        chap.VazaoNatural[dia] = Convert.ToDouble(vazAcomphChap.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        chap.VazaoIncremental[dia] = chap.VazaoNatural[dia];

                    }
                    else
                    {
                        chap.VazaoNatural[dia] = ita.VazaoNatural[dia] + monjolinho.VazaoNatural[dia] + chapSmap[dia];
                        chap.VazaoIncremental[dia] = chap.VazaoNatural[dia];

                    }

                }
                CalcMediaMuskingun(chap);

                #endregion


                #endregion



                #region Jeq_Paraniba
                #region Irape
                var sIrape = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "IRAPE".ToUpper()).Select(x => x.Vazoes).First();
                var Irapepro = propagacoes.Where(x => x.IdPosto == 255).FirstOrDefault();

                Irapepro.VazaoIncremental.Clear();
                Irapepro.VazaoNatural.Clear();
                Irapepro.calMedSemanal.Clear();
                Irapepro.medSemanalIncremental.Clear();
                Irapepro.medSemanalNatural.Clear();

                foreach (var dia in sIrape.Keys.ToList())
                {
                    Irapepro.VazaoIncremental[dia] = sIrape[dia];
                    Irapepro.VazaoNatural[dia] = sIrape[dia];

                }
                CalcMediaMuskingun(Irapepro);
                #endregion

                #region irape-itapebi

                var irItSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "IRAPE".ToUpper()).Select(x => x.Vazoes).First();
                List<double> irItCoef = new List<double> { 0.166666666666667, 0.666666666666667, 0.166666666666667 };
                double vazaoP = 0;
                for (int i = 0; i < 5; i++)
                {
                    foreach (var dat in irItSmap.Keys.Where(x => x.Date >= dataInicioJeqParna).ToList())
                    {
                        if (dat == dataInicioJeqParna)
                        {
                            vazaoP = irItSmap[dat];
                            irItSmap[dat] = irItSmap[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = irItSmap[dat] * irItCoef[0] + vazaoP * irItCoef[1] + irItSmap[dat.AddDays(-1)] * irItCoef[2];
                            vazaoP = irItSmap[dat];
                            irItSmap[dat] = (float)vazao;
                        }
                    }
                }

                #endregion

                #region Itapebi

                var sItapebi = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "ITAPEBI".ToUpper()).Select(x => x.Vazoes).First();
                var Itapebipro = propagacoes.Where(x => x.IdPosto == 188).FirstOrDefault();

                Itapebipro.VazaoIncremental.Clear();
                Itapebipro.VazaoNatural.Clear();
                Itapebipro.calMedSemanal.Clear();
                Itapebipro.medSemanalIncremental.Clear();
                Itapebipro.medSemanalNatural.Clear();

                var vazAcomphItapebi = dadosAcompH.Where(x => x.posto == Itapebipro.IdPosto).ToList();

                foreach (var dia in sItapebi.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        Itapebipro.VazaoIncremental[dia] = sItapebi[dia];
                        Itapebipro.VazaoNatural[dia] = Convert.ToDouble(vazAcomphItapebi.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        Itapebipro.VazaoIncremental[dia] = sItapebi[dia];
                        Itapebipro.VazaoNatural[dia] = irItSmap[dia] + Itapebipro.VazaoIncremental[dia];
                    }

                }
                CalcMediaMuskingun(Itapebipro);

                #endregion

                #region BOA ESPERANÇA

                var BOASMAP = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "UBESP".ToUpper()).Select(x => x.Vazoes).First();
                var boapro = propagacoes.Where(x => x.IdPosto == 190).FirstOrDefault();

                boapro.VazaoIncremental.Clear();
                boapro.VazaoNatural.Clear();
                boapro.calMedSemanal.Clear();
                boapro.medSemanalIncremental.Clear();
                boapro.medSemanalNatural.Clear();

                var vazAcomphBOA = dadosAcompH.Where(x => x.posto == boapro.IdPosto).ToList();
                foreach (var dia in BOASMAP.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        boapro.VazaoIncremental[dia] = BOASMAP[dia];
                        boapro.VazaoNatural[dia] = Convert.ToDouble(vazAcomphBOA.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        boapro.VazaoIncremental[dia] = BOASMAP[dia];
                        boapro.VazaoNatural[dia] = BOASMAP[dia];
                    }

                }
                CalcMediaMuskingun(boapro);

                #endregion

                #endregion
                #region serra da mesa
                var sMesa = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "SMesa".ToUpper()).Select(x => x.Vazoes).First();
                var SerraMesa = propagacoes.Where(x => x.IdPosto == 270).FirstOrDefault();

                SerraMesa.VazaoIncremental.Clear();
                SerraMesa.VazaoNatural.Clear();
                SerraMesa.calMedSemanal.Clear();
                SerraMesa.medSemanalIncremental.Clear();
                SerraMesa.medSemanalNatural.Clear();

                var vazAcomphSerra = dadosAcompH.Where(x => x.posto == SerraMesa.IdPosto).ToList();
                foreach (var dia in sMesa.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        SerraMesa.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphSerra.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphSerra.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphSerra.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        SerraMesa.VazaoNatural[dia] = Convert.ToDouble(vazAcomphSerra.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        SerraMesa.VazaoIncremental[dia] = sMesa[dia];
                        SerraMesa.VazaoNatural[dia] = sMesa[dia];
                    }

                }
                CalcMediaMuskingun(SerraMesa);
                #endregion

                #region Cana brava
                var vazlajeado = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "LAJEADO".ToUpper()).Select(x => x.Vazoes).First();
                var canaBrava = propagacoes.Where(x => x.IdPosto == 191).FirstOrDefault();

                canaBrava.VazaoIncremental.Clear();
                canaBrava.VazaoNatural.Clear();
                canaBrava.calMedSemanal.Clear();
                canaBrava.medSemanalIncremental.Clear();
                canaBrava.medSemanalNatural.Clear();

                var vazAcomphCana = dadosAcompH.Where(x => x.posto == canaBrava.IdPosto).ToList();


                foreach (var dia in vazlajeado.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        canaBrava.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphCana.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphCana.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphCana.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        canaBrava.VazaoNatural[dia] = Convert.ToDouble(vazAcomphCana.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);

                    }
                    else
                    {
                        canaBrava.VazaoIncremental[dia] = vazlajeado[dia] * canaBrava.Modelo[0].FatorDistribuicao;
                    }
                }

                CalculaVazaoNat(canaBrava, dadosAcompH);
                foreach (var dat in canaBrava.VazaoNatural.Keys.Where(x => x.Date <= dataInicio).ToList())
                {
                    canaBrava.VazaoNatural[dat] = canaBrava.VazaoNatural[dataInicio.AddDays(1)];
                }
                CalcMediaMuskingun(canaBrava);

                #endregion

                #region São Salvador
                vazlajeado = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "LAJEADO".ToUpper()).Select(x => x.Vazoes).First();
                var sSalvador = propagacoes.Where(x => x.IdPosto == 253).FirstOrDefault();

                sSalvador.VazaoIncremental.Clear();
                sSalvador.VazaoNatural.Clear();
                sSalvador.calMedSemanal.Clear();
                sSalvador.medSemanalIncremental.Clear();
                sSalvador.medSemanalNatural.Clear();

                var vazAcomphSalvador = dadosAcompH.Where(x => x.posto == sSalvador.IdPosto).ToList();


                foreach (var dia in vazlajeado.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        sSalvador.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphSalvador.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphSalvador.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphSalvador.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        sSalvador.VazaoNatural[dia] = Convert.ToDouble(vazAcomphSalvador.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        sSalvador.VazaoIncremental[dia] = vazlajeado[dia] * sSalvador.Modelo[0].FatorDistribuicao;//confirmar fator de distribuição, trocar com lajeado?
                    }
                }                                                                                               //planilha propagaçoes tocantins e config smap com valores trocados

                CalculaVazaoNat(sSalvador, dadosAcompH);
                foreach (var dat in sSalvador.VazaoNatural.Keys.Where(x => x.Date <= dataInicio).ToList())
                {
                    sSalvador.VazaoNatural[dat] = sSalvador.VazaoNatural[dataInicio.AddDays(1)];
                }
                CalcMediaMuskingun(sSalvador);

                #endregion

                #region Peixe Angical
                vazlajeado = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "LAJEADO".ToUpper()).Select(x => x.Vazoes).First();
                var pAngi = propagacoes.Where(x => x.IdPosto == 257).FirstOrDefault();

                pAngi.VazaoIncremental.Clear();
                pAngi.VazaoNatural.Clear();
                pAngi.calMedSemanal.Clear();
                pAngi.medSemanalIncremental.Clear();
                pAngi.medSemanalNatural.Clear();

                var vazAcomphPangi = dadosAcompH.Where(x => x.posto == pAngi.IdPosto).ToList();

                foreach (var dia in vazlajeado.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        pAngi.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphPangi.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphPangi.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphPangi.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        pAngi.VazaoNatural[dia] = Convert.ToDouble(vazAcomphPangi.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        pAngi.VazaoIncremental[dia] = vazlajeado[dia] * pAngi.Modelo[0].FatorDistribuicao;
                    }
                }

                CalculaVazaoNat(pAngi, dadosAcompH);
                foreach (var dat in pAngi.VazaoNatural.Keys.Where(x => x.Date <= dataInicio).ToList())
                {
                    pAngi.VazaoNatural[dat] = pAngi.VazaoNatural[dataInicio.AddDays(1)];
                }
                CalcMediaMuskingun(pAngi);

                #endregion

                #region Lajeado
                vazlajeado = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "LAJEADO".ToUpper()).Select(x => x.Vazoes).First();
                var laj = propagacoes.Where(x => x.IdPosto == 273).FirstOrDefault();

                laj.VazaoIncremental.Clear();
                laj.VazaoNatural.Clear();
                laj.calMedSemanal.Clear();
                laj.medSemanalIncremental.Clear();
                laj.medSemanalNatural.Clear();

                var vazAcomphLaj = dadosAcompH.Where(x => x.posto == laj.IdPosto).ToList();

                foreach (var dia in vazlajeado.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        laj.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphLaj.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphLaj.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphLaj.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        laj.VazaoNatural[dia] = Convert.ToDouble(vazAcomphLaj.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        laj.VazaoIncremental[dia] = vazlajeado[dia] * laj.Modelo[0].FatorDistribuicao;
                    }
                }

                CalculaVazaoNat(laj, dadosAcompH);
                foreach (var dat in laj.VazaoNatural.Keys.Where(x => x.Date <= dataInicio.AddDays(3)).ToList())
                {
                    laj.VazaoNatural[dat] = laj.VazaoNatural[dataInicio.AddDays(4)];
                }
                CalcMediaMuskingun(laj);

                #endregion

                #region  BANDEIRANTES- CONCEIÇÃO DO ARAGUAIA

                var banCA = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "Bandeirant".ToUpper()).Select(x => x.Vazoes).First();
                List<double> banCACoef = new List<double> { 0.152775192170181, 0.152775192170181, 0.694449615659638 };
                double vazaoPassada = 0;
                for (int i = 0; i < 9; i++)
                {
                    foreach (var dat in banCA.Keys.Where(x => x.Date >= dataInicio).ToList())
                    {
                        if (dat == dataInicio)
                        {
                            vazaoPassada = banCA[dat];
                            banCA[dat] = banCA[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = banCA[dat] * banCACoef[0] + vazaoPassada * banCACoef[1] + banCA[dat.AddDays(-1)] * banCACoef[2];
                            vazaoPassada = banCA[dat];
                            banCA[dat] = (float)vazao;
                        }
                    }
                }
                #endregion
                #region CONCEIÇÃO DO ARAGUAIA - TUCURUÍ

                var CAtuc = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "C.ARAGUAIA".ToUpper()).Select(x => x.Vazoes).First();
                List<double> CAtucCoef = new List<double> { 0.0322580645161291, 0.612903225806452, 0.354838709677419 };
                vazaoPassada = 0;
                foreach (var date in CAtuc.Keys.ToList())//soma as vazoes do trecho bandeirant pois é seu montante
                {
                    CAtuc[date] = CAtuc[date] + banCA[date];
                }

                for (int i = 0; i < 3; i++)
                {
                    foreach (var dat in CAtuc.Keys.Where(x => x.Date >= dataInicio).ToList())
                    {
                        if (dat == dataInicio)
                        {
                            vazaoPassada = CAtuc[dat];
                            CAtuc[dat] = CAtuc[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = CAtuc[dat] * CAtucCoef[0] + vazaoPassada * CAtucCoef[1] + CAtuc[dat.AddDays(-1)] * CAtucCoef[2];
                            vazaoPassada = CAtuc[dat];
                            CAtuc[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region  LAJEADO-ESTREITO
                var lajeadoVaz = propagacoes.Where(x => x.IdPosto == 273).Select(x => x.VazaoNatural).FirstOrDefault();
                var lajEst = lajeadoVaz;

                List<double> lajEstCoef = new List<double> { 0.343207069827643, 0.366616629265668, 0.290176300906689 };

                for (int i = 0; i < 3; i++)
                {
                    foreach (var dat in lajEst.Keys.Where(x => x.Date >= dataInicio).ToList())
                    {
                        if (dat == dataInicio)
                        {
                            vazaoPassada = lajEst[dat];
                            lajEst[dat] = lajEst[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = lajEst[dat] * lajEstCoef[0] + vazaoPassada * lajEstCoef[1] + lajEst[dat.AddDays(-1)] * lajEstCoef[2];
                            vazaoPassada = lajEst[dat];
                            lajEst[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region  Porto Real-Estreito
                var PrEst = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "PORTO REAL".ToUpper()).Select(x => x.Vazoes).First();

                List<double> PrEstCoef = new List<double> { 0.235961557677904, 0.622494756568882, 0.141543685753214 };
                vazaoPassada = 0;

                for (int i = 0; i < 2; i++)
                {
                    foreach (var dat in PrEst.Keys.Where(x => x.Date >= dataInicio).ToList())
                    {
                        if (dat == dataInicio)
                        {
                            vazaoPassada = PrEst[dat];
                            PrEst[dat] = PrEst[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = PrEst[dat] * PrEstCoef[0] + vazaoPassada * PrEstCoef[1] + PrEst[dat.AddDays(-1)] * PrEstCoef[2];
                            vazaoPassada = PrEst[dat];
                            PrEst[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region Estreito
                var estreitoSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "ESTREITO".ToUpper()).Select(x => x.Vazoes).First();
                var estreito = propagacoes.Where(x => x.IdPosto == 271).FirstOrDefault();

                estreito.VazaoIncremental.Clear();
                estreito.VazaoNatural.Clear();
                estreito.calMedSemanal.Clear();
                estreito.medSemanalIncremental.Clear();
                estreito.medSemanalNatural.Clear();

                var vazAcomphEST = dadosAcompH.Where(x => x.posto == estreito.IdPosto).ToList();

                foreach (var dat in estreitoSmap.Keys.ToList())
                {
                    if (dat <= ultimoAcomph)
                    {
                        estreito.VazaoIncremental[dat] = (Convert.ToDouble(vazAcomphEST.Where(a => a.data == dat).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphEST.Where(a => a.data == dat).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphEST.Where(a => a.data == dat).First().qnat, Culture.NumberFormat); //estreitoSmap[dat];
                        estreito.VazaoNatural[dat] = Convert.ToDouble(vazAcomphEST.Where(a => a.data == dat).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        estreito.VazaoIncremental[dat] = estreitoSmap[dat] + PrEst[dat];
                        estreito.VazaoNatural[dat] = lajEst[dat] + estreito.VazaoIncremental[dat];//soma as vazoes do smap de estreito com as propagaçoes muskingun de Laj-est e Pr-est pois é seu montante
                    }

                }
                CalcMediaMuskingun(estreito);

                #endregion

                #region  Estreito-Tucurui
                var estreitoVaz = propagacoes.Where(x => x.IdPosto == 271).Select(x => x.VazaoNatural).FirstOrDefault();
                var estTuc = estreitoVaz;

                List<double> estTucCoef = new List<double> { 0.0502793296089386, 0.620111731843575, 0.329608938547486 };

                for (int i = 0; i < 3; i++)
                {
                    foreach (var dat in estTuc.Keys.Where(x => x.Date >= dataInicio).ToList())
                    {
                        if (dat == dataInicio)
                        {
                            vazaoPassada = estTuc[dat];
                            estTuc[dat] = estTuc[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = estTuc[dat] * estTucCoef[0] + vazaoPassada * estTucCoef[1] + estTuc[dat.AddDays(-1)] * estTucCoef[2];
                            vazaoPassada = estTuc[dat];
                            estTuc[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region Tucurui
                var tucuruiSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "TUCURUI".ToUpper()).Select(x => x.Vazoes).First();
                var tucurui = propagacoes.Where(x => x.IdPosto == 275).FirstOrDefault();

                tucurui.VazaoIncremental.Clear();
                tucurui.VazaoNatural.Clear();
                tucurui.calMedSemanal.Clear();
                tucurui.medSemanalIncremental.Clear();
                tucurui.medSemanalNatural.Clear();

                var vazAcomphTuc = dadosAcompH.Where(x => x.posto == tucurui.IdPosto).ToList();


                foreach (var dat in tucuruiSmap.Keys.ToList())
                {
                    if (dat <= ultimoAcomph)
                    {
                        tucurui.VazaoIncremental[dat] = (Convert.ToDouble(vazAcomphTuc.Where(a => a.data == dat).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphTuc.Where(a => a.data == dat).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphTuc.Where(a => a.data == dat).First().qnat, Culture.NumberFormat);
                        tucurui.VazaoNatural[dat] = Convert.ToDouble(vazAcomphTuc.Where(a => a.data == dat).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        tucurui.VazaoIncremental[dat] = tucuruiSmap[dat] + CAtuc[dat];
                        tucurui.VazaoNatural[dat] = estTuc[dat] + tucurui.VazaoIncremental[dat];//soma as vazoes do smap de tucurui com as propagaçoes muskingun de estTuc e CAtuc pois é seu montante
                    }

                }
                CalcMediaMuskingun(tucurui);

                #endregion

                //inclusão de postos ficticios para auxiliar no uso das vaões incrementais que serão usadas no previvaz dos postos do tocantins
                var saoSalCanaInc = new Propagacao() { IdPosto = 308, NomePostoFluv = "SSalvador e CanaBrava" };
                var cana191 = propagacoes.Where(x => x.IdPosto == 191).Select(x => x.medSemanalIncremental).FirstOrDefault();
                var sSalv253 = propagacoes.Where(x => x.IdPosto == 253).Select(x => x.medSemanalIncremental).FirstOrDefault();

                foreach (var dat in sSalv253.Keys.ToList())
                {
                    saoSalCanaInc.calMedSemanal[dat] = cana191[dat] + sSalv253[dat];
                }
                propagacoes.Add(saoSalCanaInc);

                var lajPeixeInc = new Propagacao() { IdPosto = 309, NomePostoFluv = "lajeado e peixeAngical" };
                var laj273 = propagacoes.Where(x => x.IdPosto == 273).Select(x => x.medSemanalIncremental).FirstOrDefault();
                var pAngi257 = propagacoes.Where(x => x.IdPosto == 257).Select(x => x.medSemanalIncremental).FirstOrDefault();

                foreach (var dat in laj273.Keys.ToList())
                {
                    lajPeixeInc.calMedSemanal[dat] = pAngi257[dat] + laj273[dat];
                }

                propagacoes.Add(lajPeixeInc);

                var EstreitoInc = new Propagacao() { IdPosto = 310, NomePostoFluv = "Estreito Incremental" };
                var estre271 = propagacoes.Where(x => x.IdPosto == 271).Select(x => x.medSemanalIncremental).FirstOrDefault();

                foreach (var dat in estre271.Keys.ToList())
                {
                    EstreitoInc.calMedSemanal[dat] = estre271[dat];
                }
                propagacoes.Add(EstreitoInc);

                var tucuruiInc = new Propagacao() { IdPosto = 311, NomePostoFluv = "Tucurui Incremental" };
                var tucur275 = propagacoes.Where(x => x.IdPosto == 275).Select(x => x.medSemanalIncremental).FirstOrDefault();

                foreach (var dat in tucur275.Keys.ToList())
                {
                    tucuruiInc.calMedSemanal[dat] = tucur275[dat];
                }

                propagacoes.Add(tucuruiInc);

                //Madeira================

                #region  principe da Beira - Guajará Mirim

                var PbGm = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "P_DA_BEIRA".ToUpper()).Select(x => x.Vazoes).First();
                //List<double> PbGmCoef = new List<double> { 0.375, 0.625, 0 };
                List<double> PbGmCoef = new List<double> { 0.23330956912145081605, 0.50318460079070015389, 0.26350583008784900230 };
                //if (shadow == true)
                //{
                //    PbGmCoef = new List<double> { 0.23330956912145081605, 0.50318460079070015389, 0.26350583008784900230 };
                //}
                vazaoPassada = 0;
                for (int i = 0; i < 2; i++)
                {
                    foreach (var dat in PbGm.Keys.ToList())
                    {
                        if (dat == PbGm.Keys.First())
                        {
                            vazaoPassada = PbGm[dat];
                            PbGm[dat] = PbGm[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = PbGm[dat] * PbGmCoef[0] + vazaoPassada * PbGmCoef[1] + PbGm[dat.AddDays(-1)] * PbGmCoef[2];
                            vazaoPassada = PbGm[dat];
                            PbGm[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region  Guajará Mirim - Jirau

                var GmJi = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "GUAJ-MIRIM".ToUpper()).Select(x => x.Vazoes).First();
                Dictionary<DateTime, float> auxiliar = new Dictionary<DateTime, float>();
                double tempVi = 14;

                foreach (var item in GmJi.Keys.ToList())
                {
                    auxiliar[item] = GmJi[item];
                }

                foreach (var dat in GmJi.Keys.ToList())
                {
                    if (dat == GmJi.Keys.First())
                    {
                        continue;
                    }

                    var dmenor = dat.AddDays(-(1 + Math.Floor(tempVi / 24))).Date;
                    var dmaior = dmenor.AddDays(1);

                    var horaatraso = (tempVi % 24);
                    auxiliar[dat] = 0;
                    auxiliar[dat] = ((PbGm[dmaior] + GmJi[dmaior]) * ((float)(24f - horaatraso)) + (PbGm[dmenor] + GmJi[dmenor]) * ((float)horaatraso)) / 24f;
                    //auxiliar[dat] += (PbGm[dmaior] + GmJi[dmaior]) * (float)(24f - horaatraso) / 24f;

                }
                var dataAux = GmJi.Keys.First();
                auxiliar[dataAux] = auxiliar[dataAux.AddDays(1)];
                GmJi = auxiliar;

                //List<double> GmJiCoef = new List<double> { 0.375, 0.625, 0 };
                List<double> GmJiCoef = new List<double> { 0.49899899799799601885, 0.50100300200600400569, -0.00000200000400001000 };
                //if (shadow == true)
                //{
                //    GmJiCoef = new List<double> { 0.49899899799799601885, 0.50100300200600400569, -0.00000200000400001000 };
                //}
                vazaoPassada = 0;
                for (int i = 0; i < 2; i++)
                {
                    foreach (var dat in GmJi.Keys.ToList())
                    {
                        if (dat == GmJi.Keys.First())
                        {
                            vazaoPassada = GmJi[dat];
                            GmJi[dat] = GmJi[dat];
                        }
                        else
                        {
                            double vazao = 0;
                            vazao = GmJi[dat] * GmJiCoef[0] + vazaoPassada * GmJiCoef[1] + GmJi[dat.AddDays(-1)] * GmJiCoef[2];
                            vazaoPassada = GmJi[dat];
                            GmJi[dat] = (float)vazao;
                        }
                    }
                }
                #endregion

                #region Amaru_mayu

                var AmaruSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "AMARU_MAYU".ToUpper()).Select(x => x.Vazoes).FirstOrDefault();
                //if (AmaruSmap != null && shadow == true)
                if (AmaruSmap != null)
                {

                    List<double> AmaruSmapCoef = new List<double> { 0.35236355811671099536, 0.56608358393819646626, 0.08155285794509256614 };

                    vazaoPassada = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        foreach (var dat in AmaruSmap.Keys.ToList())
                        {
                            if (dat == AmaruSmap.Keys.First())
                            {
                                vazaoPassada = AmaruSmap[dat];
                                AmaruSmap[dat] = AmaruSmap[dat];
                            }
                            else
                            {
                                double vazao = 0;
                                vazao = AmaruSmap[dat] * AmaruSmapCoef[0] + vazaoPassada * AmaruSmapCoef[1] + AmaruSmap[dat.AddDays(-1)] * AmaruSmapCoef[2];
                                vazaoPassada = AmaruSmap[dat];
                                AmaruSmap[dat] = (float)vazao;
                            }
                        }
                    }
                }

                #endregion

                #region JIRAU
                //var JirauSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "JIRAU2".ToUpper()).Select(x => x.Vazoes).First();
                //string nomeVazao = shadow == true ? "JIRAU" : "JIRAU2";
                string nomeVazao = "JIRAU2";

                var JirauSmap = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == nomeVazao.ToUpper()).Select(x => x.Vazoes).First();
                var jirau = propagacoes.Where(x => x.IdPosto == 285).FirstOrDefault();



                jirau.VazaoIncremental.Clear();
                jirau.VazaoNatural.Clear();
                jirau.calMedSemanal.Clear();
                jirau.medSemanalIncremental.Clear();
                jirau.medSemanalNatural.Clear();

                var vazAcomphJirau = dadosAcompH.Where(x => x.posto == jirau.IdPosto).ToList();
                foreach (var dia in JirauSmap.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        jirau.VazaoIncremental[dia] = (Convert.ToDouble(vazAcomphJirau.Where(a => a.data == dia).First().qinc, Culture.NumberFormat)) > 0 ? Convert.ToDouble(vazAcomphJirau.Where(a => a.data == dia).First().qinc, Culture.NumberFormat) : Convert.ToDouble(vazAcomphJirau.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                        jirau.VazaoNatural[dia] = Convert.ToDouble(vazAcomphJirau.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);
                    }
                    else
                    {
                        //jirau.VazaoIncremental[dia] = JirauSmap[dia] + GmJi[dia];
                        //jirau.VazaoNatural[dia] = JirauSmap[dia] + GmJi[dia];
                        //if (AmaruSmap != null && shadow == true)
                        if (AmaruSmap != null)
                        {
                            jirau.VazaoIncremental[dia] = JirauSmap[dia] + GmJi[dia] + AmaruSmap[dia];
                            jirau.VazaoNatural[dia] = JirauSmap[dia] + GmJi[dia] + AmaruSmap[dia];
                        }
                        else
                        {
                            jirau.VazaoIncremental[dia] = JirauSmap[dia] + GmJi[dia];
                            jirau.VazaoNatural[dia] = JirauSmap[dia] + GmJi[dia];
                        }

                    }

                }
                CalcMediaMuskingun(jirau);
                #endregion

                #region SANTO ANTONIO
                var vazStoAnt = modelos.SelectMany(x => x.Vazoes).Where(x => x.Nome.ToUpper() == "S.ANTONIO".ToUpper()).Select(x => x.Vazoes).First();
                var stoAnt = propagacoes.Where(x => x.IdPosto == 287).FirstOrDefault();

                stoAnt.VazaoIncremental.Clear();
                stoAnt.VazaoNatural.Clear();
                stoAnt.calMedSemanal.Clear();
                stoAnt.medSemanalIncremental.Clear();
                stoAnt.medSemanalNatural.Clear();

                var vazAcomphStoAnt = dadosAcompH.Where(x => x.posto == stoAnt.IdPosto).ToList();


                foreach (var dia in vazStoAnt.Keys.ToList())
                {
                    if (dia <= ultimoAcomph)
                    {
                        stoAnt.VazaoIncremental[dia] = vazStoAnt[dia];
                        stoAnt.VazaoNatural[dia] = Convert.ToDouble(vazAcomphStoAnt.Where(a => a.data == dia).First().qnat, Culture.NumberFormat);

                    }
                    else
                    {
                        stoAnt.VazaoIncremental[dia] = vazStoAnt[dia];
                    }
                }

                CalculaVazaoNat(stoAnt, dadosAcompH);

                CalcMediaMuskingun(stoAnt);

                #endregion
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public void CalcMediaMuskingun(Propagacao prop)
        {
            try
            {
                DateTime dataNow = DateTime.Today.AddDays(-1);
                DateTime semanaNow = dataNow.AddDays(-23);
                var SOatualMaior = dataNow;

                while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                    SOatualMaior = SOatualMaior.AddDays(+1);

                while (semanaNow <= prop.VazaoNatural.Keys.Last())//SOatualMaior.AddDays(+14))//+7
                {
                    if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                    {

                        if (semanaNow <= prop.VazaoNatural.Keys.Last())//SOatualMaior.AddDays(14))//7
                        {

                            var vazNaturais = prop.VazaoNatural.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);
                            var vazIncrementais = prop.VazaoIncremental.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);

                            if (vazNaturais.Count() == 7)
                            {
                                prop.medSemanalNatural.Add(semanaNow, vazNaturais.Average(x => x.Value));
                                prop.medSemanalIncremental.Add(semanaNow, vazIncrementais.Average(x => x.Value));
                                if (semanaNow <= SOatualMaior)
                                {
                                    prop.calMedSemanal.Add(semanaNow, vazNaturais.Average(x => x.Value));
                                }
                                else
                                {
                                    if (prop.PostoMontantes.Count() > 0 && prop.IdPosto != 287 && prop.IdPosto != 188 && prop.IdPosto != 216 && prop.IdPosto != 217 && prop.IdPosto != 92 && prop.IdPosto != 94)
                                    {
                                        var vazao = SomaInc(prop, semanaNow);
                                        prop.calMedSemanal.Add(semanaNow, vazao);
                                    }
                                    else
                                    {
                                        prop.calMedSemanal.Add(semanaNow, vazNaturais.Average(x => x.Value));
                                    }
                                }

                            }
                        }
                    }
                    semanaNow = semanaNow.AddDays(+1);
                }

                new AddLog("O método ExecutingProcess/CalcMediaMuskingun foi executado com sucesso!" + prop.NomePostoFluv.ToString());
            }
            catch (Exception exc)
            {
                new AddLog("Falha ao processar o método ExecutingProcess/CalcMediaMuskingun" + prop.NomePostoFluv.ToString());
                new AddLog("Erro: " + exc.Message);
            }

        }

        public void CalculaVazaoNat(Propagacao prop, List<CONSULTA_VAZAO> dadosAcompH)
        {
            try
            {
                var ultimaDiaAcomph = dadosAcompH.Select(x => x.data).Last();

                foreach (var dat in prop.VazaoIncremental.Keys.ToList())
                {
                    try
                    {
                        if (dat > ultimaDiaAcomph)
                        {
                            if (prop.VazaoNatural.ContainsKey(dat)) prop.VazaoNatural[dat] = 0;
                            if (!prop.VazaoNatural.ContainsKey(dat)) prop.VazaoNatural[dat] = 0;

                            foreach (var pm in prop.PostoMontantes)
                            {
                                var postoMontante = pm.Propaga;

                                var dmenor = dat.AddDays(-(1 + Math.Floor(pm.TempoViagem / 24))).Date;
                                var dmaior = dmenor.AddDays(1);

                                if (pm.TempoViagem >= 72)
                                {
                                    dmenor = dmenor.AddDays(1);
                                    dmaior = dmenor.AddDays(1);
                                }

                                var horaatraso = (pm.TempoViagem % 24);
                                if (postoMontante.VazaoNatural.ContainsKey(dmaior) && postoMontante.VazaoNatural.ContainsKey(dmenor))
                                {
                                    prop.VazaoNatural[dat] += postoMontante.VazaoNatural[dmenor] * horaatraso / 24f;
                                    prop.VazaoNatural[dat] += postoMontante.VazaoNatural[dmaior] * (24f - horaatraso) / 24f;
                                }
                            }
                            prop.VazaoNatural[dat] += prop.VazaoIncremental[dat];
                        }
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

        }
        public void GetPrevs(List<Propagacao> propagacoes, DateTime data)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            //esse metodo captura as vazões do arquivo prevsRVX oficial mais recente dos arquivos do GEVAZP para 
            //preencher os dados ds vazoes da RV0 do mês até a semana atual e tambem inclui os postos que ainda
            //não foram inclusos na lista de propagações
            try
            {
                var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(data);

                var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();
                var prevs = System.IO.Directory.GetFiles(pastaBase, "prevs.*", SearchOption.AllDirectories)[0];
                //var vazPrev = new StreamReader(prevs);
                List<double[]> vazPrevs = new List<double[]>();

                using (var reader = new System.IO.StreamReader(prevs))
                {
                    while (!reader.EndOfStream)
                    {
                        double[] vaz = new double[7];
                        var f = reader.ReadLine();
                        var dados = f.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
                        for (int i = 0; i < vaz.Length; i++)
                        {
                            vaz[i] = Convert.ToDouble(dados[i], Culture.NumberFormat);
                        }
                        vazPrevs.Add(vaz);// adiciona as vazoes do prevs na lista vazPrevs
                    }
                }

                DateTime inicioMes = new DateTime(currRev.revDate.Year, currRev.revDate.Month, 1);//data da Rv0 do mês para preencher com dados do prevs oficial
                var semanaZero = inicioMes;

                while (semanaZero.DayOfWeek != DayOfWeek.Saturday)
                {
                    semanaZero = semanaZero.AddDays(-1);
                }
                semanaZero = semanaZero.AddDays(6);//termino da semana rv0 do mês
                foreach (var item in vazPrevs)
                {
                    var dat = semanaZero;
                    var postoAlvo = propagacoes.Where(x => x.IdPosto == item[0]).FirstOrDefault();
                    if (postoAlvo == null)//o posto ainda não esta na lista de propagações 
                    {
                        var prop = new Propagacao() { IdPosto = Convert.ToInt32(item[0], Culture.NumberFormat), NomePostoFluv = "PostoPrevs" };
                        for (int i = 1; i < item.Count(); i++)
                        {
                            var vazoes = new Tuple<DateTime, double>(dat, item[i]);
                            if (dat <= currRev.revDate)
                            {
                                prop.calMedSemanal[vazoes.Item1] = vazoes.Item2;// atribui as vazoes do prevs até a semana atual na lista de vazoes que serao usadas no previvaz
                            }
                            dat = dat.AddDays(7);
                        }
                        propagacoes.Add(prop);
                    }
                    else if (postoAlvo != null)
                    {
                        for (int i = 1; i < item.Count(); i++)
                        {
                            var vazoes = new Tuple<DateTime, double>(dat, item[i]);
                            if (dat < currRev.revDate)
                            {
                                postoAlvo.calMedSemanal[vazoes.Item1] = vazoes.Item2;// atribui as vazoes do prevs das semanas anteriores à atual na lista de vazoes que serao usadas no previvaz
                            }
                            dat = dat.AddDays(7);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        public double SomaInc(Propagacao propaga, DateTime semanaNow)
        {
            try
            {
                double vazaoCalcinc = 0;
                if (propaga.PostoMontantes.Count() > 0)
                {
                    foreach (var prop in propaga.PostoMontantes)
                    {
                        var postoMontante = prop.Propaga;
                        vazaoCalcinc += SomaInc(postoMontante, semanaNow);
                    }
                }
                if (propaga.IdPosto != 2)//o posto 2 (ITUTINGA) é uma copia  do posto 1(camargos), esse if previne o erro no calculo.
                {
                    vazaoCalcinc += propaga.medSemanalIncremental[semanaNow];
                }


                return vazaoCalcinc;
            }
            catch (Exception e)
            {
                e.ToString();
                return 0;
            }

        }

        public void AdicionaCPINS(List<Propagacao> propagacoes, List<CONSULTA_VAZAO> dadosAcomph)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            try
            {

                var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(DateTime.Today);

                var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

                var PathModelo = Path.Combine(pastaBase, "Modelos_Chuva_Vazao", "CPINS", "Arq_Saida");
                DateTime dt_CPINS = DateTime.Today;
                var Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PLANILHA_USB.txt");

                while (!File.Exists(Arquivo)) // busca o txt com dados do cpins mais recente
                {
                    dt_CPINS = dt_CPINS.AddDays(-1);
                    Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PLANILHA_USB.txt");
                }

                var TxtCpins = File.ReadAllLines(Arquivo);

                var Num_linhas = TxtCpins.Length;
                List<Tuple<DateTime, double, double>> dados = new List<Tuple<DateTime, double, double>>();


                for (int i = 0; i <= Num_linhas - 1; i++)
                {
                    var Separa = TxtCpins[i].Split(';');
                    var d = double.Parse(Separa[0], Culture.NumberFormat);            //no txt a data esta codificada pelo excel 
                    var data = DateTime.FromOADate(d);         // esse passo converte para datetime  
                    var dado = new Tuple<DateTime, double, double>(data, Convert.ToDouble(Separa[1], Culture.NumberFormat), Convert.ToDouble(Separa[2], Culture.NumberFormat));
                    dados.Add(dado);

                }
                foreach (var prop in propagacoes)
                {
                    if (prop.IdPosto == 168 || prop.IdPosto == 169 || prop.IdPosto == 172 || prop.IdPosto == 173 || prop.IdPosto == 178)
                    {
                        var ultimoAcomph = dadosAcomph.Select(x => x.data).Last();
                        var dataFim = dados.Select(x => x.Item1).Last();

                        for (DateTime dat = ultimoAcomph.AddDays(1); dat <= dataFim; dat = dat.AddDays(1))
                        {
                            if (prop.IdPosto == 168)
                            {
                                prop.VazaoNatural[dat] = dados.Where(x => x.Item1 == dat).Select(x => x.Item2).FirstOrDefault();
                                prop.VazaoIncremental[dat] = prop.VazaoNatural[dat];
                            }
                            if (prop.IdPosto == 169 || prop.IdPosto == 172 || prop.IdPosto == 173 || prop.IdPosto == 178)
                            {
                                prop.VazaoNatural[dat] = dados.Where(x => x.Item1 == dat).Select(x => x.Item3).FirstOrDefault();
                            }

                        }
                        DateTime dataNow = DateTime.Today.AddDays(-1);

                        var SOatualMaior = dataNow;

                        while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                            SOatualMaior = SOatualMaior.AddDays(+1);//até sexta(fim da semana)atual
                        DateTime semanaNow = SOatualMaior.AddDays(1);
                        for (DateTime date = SOatualMaior.AddDays(1); date <= SOatualMaior.AddDays(14); date = date.AddDays(1))
                        {
                            if (date.DayOfWeek == DayOfWeek.Friday)
                            {

                                if (date <= SOatualMaior.AddDays(14))
                                {
                                    var vaz = prop.VazaoNatural.Where(x => x.Key >= date.AddDays(-6) && x.Key <= date);

                                    if (vaz.Count() == 7)
                                    {
                                        prop.calMedSemanal[date] = vaz.Average(x => x.Value);//media das  semanas seguintes à atual
                                    }
                                }
                            }
                        }
                        if (prop.IdPosto == 168 || prop.IdPosto == 169)
                        {
                            var vaz = prop.VazaoNatural.Where(x => x.Key >= SOatualMaior.AddDays(-6) && x.Key <= SOatualMaior);
                            if (vaz.Count() == 7)
                            {
                                prop.calMedSemanal[SOatualMaior] = vaz.Average(x => x.Value);//media da semana atual(para esses postos a media é calculada usando os dias de acomph e os dias de cpins para fechar a semana)
                            }                                                                //os dias de acomph ja haviam sido inseridos no metodo CalcSemanaPrevivaz
                        }
                    }

                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public List<Tuple<DateTime, double, double, double>> GetMPV()
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            List<Tuple<DateTime, double, double, double>> dados = new List<Tuple<DateTime, double, double, double>>();

            try
            {
                var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(DateTime.Today);

                var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

                var PathModelo = Path.Combine(pastaBase, "Modelos_Chuva_Vazao", "MPV", "Arq_Saida");
                DateTime dt_CPINS = DateTime.Today;
                var Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PlanilhaUSB_MPV.txt");

                while (!File.Exists(Arquivo)) // busca o txt com dados do cpins mais recente
                {
                    dt_CPINS = dt_CPINS.AddDays(-1);
                    Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PlanilhaUSB_MPV.txt");
                }

                var TxtCpins = File.ReadAllLines(Arquivo);

                var Num_linhas = TxtCpins.Length;


                for (int i = 0; i <= Num_linhas - 1; i++)
                {
                    var Separa = TxtCpins[i].Split(';');
                    var d = double.Parse(Separa[0], Culture.NumberFormat);            //no txt a data esta codificada pelo excel 
                    var data = DateTime.FromOADate(d);         // esse passo converte para datetime  
                    var dado = new Tuple<DateTime, double, double, double>(data, Convert.ToDouble(Separa[1], Culture.NumberFormat), Convert.ToDouble(Separa[2], Culture.NumberFormat), Convert.ToDouble(Separa[3], Culture.NumberFormat));
                    dados.Add(dado);
                }
            }
            catch (Exception e)
            {

                e.Message.ToString();
            }
            return dados;

        }

        public void AdicionaMPV(List<Propagacao> propagacoes, List<CONSULTA_VAZAO> dadosAcomph, bool ext = false)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            try
            {

                var currRev = ChuvaVazaoTools.Tools.Tools.GetCurrRev(DateTime.Today);

                var pastaBase = @"H:\Middle - Preço\Acompanhamento de vazões\" + currRev.revDate.ToString("MM_yyyy") + @"\Dados_de_Entrada_e_Saida_" + currRev.revDate.ToString("yyyyMM") + "_RV" + currRev.rev.ToString();

                var PathModelo = Path.Combine(pastaBase, "Modelos_Chuva_Vazao", "MPV", "Arq_Saida");
                DateTime dt_CPINS = DateTime.Today;
                var Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PlanilhaUSB_MPV.txt");

                while (!File.Exists(Arquivo)) // busca o txt com dados do cpins mais recente
                {
                    dt_CPINS = dt_CPINS.AddDays(-1);
                    Arquivo = Path.Combine(PathModelo, dt_CPINS.ToString("dd-MM-yyyy") + "_PlanilhaUSB_MPV.txt");
                }

                var TxtCpins = File.ReadAllLines(Arquivo);

                var Num_linhas = TxtCpins.Length;
                List<Tuple<DateTime, double, double>> dados = new List<Tuple<DateTime, double, double>>();


                for (int i = 0; i <= Num_linhas - 1; i++)
                {
                    var Separa = TxtCpins[i].Split(';');
                    var d = double.Parse(Separa[0], Culture.NumberFormat);            //no txt a data esta codificada pelo excel 
                    var data = DateTime.FromOADate(d);         // esse passo converte para datetime  
                    var dado = new Tuple<DateTime, double, double>(data, Convert.ToDouble(Separa[1], Culture.NumberFormat), Convert.ToDouble(Separa[2], Culture.NumberFormat));
                    dados.Add(dado);

                }
                foreach (var prop in propagacoes)
                {
                    if (prop.IdPosto == 168 || prop.IdPosto == 169 || prop.IdPosto == 172 || prop.IdPosto == 173 || prop.IdPosto == 178)
                    {
                        var ultimoAcomph = dadosAcomph.Select(x => x.data).Last();
                        var dataFim = dados.Select(x => x.Item1).Last();

                        for (DateTime dat = ultimoAcomph.AddDays(1); dat <= dataFim; dat = dat.AddDays(1))
                        {
                            if (prop.IdPosto == 168)
                            {
                                prop.VazaoNatural[dat] = dados.Where(x => x.Item1 == dat).Select(x => x.Item2).FirstOrDefault();
                                prop.VazaoIncremental[dat] = prop.VazaoNatural[dat];
                            }
                            if (prop.IdPosto == 169 || prop.IdPosto == 172 || prop.IdPosto == 173 || prop.IdPosto == 178)
                            {
                                prop.VazaoNatural[dat] = dados.Where(x => x.Item1 == dat).Select(x => x.Item3).FirstOrDefault();
                            }

                        }
                        DateTime dataNow = DateTime.Today.AddDays(-1);

                        var SOatualMaior = dataNow;

                        while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                            SOatualMaior = SOatualMaior.AddDays(+1);//até sexta(fim da semana)atual
                        DateTime semanaNow = SOatualMaior.AddDays(1);
                        if (ext == true)// para smap ext
                        {
                            for (DateTime date = SOatualMaior.AddDays(1); date <= dataFim; date = date.AddDays(1))
                            {
                                if (date.DayOfWeek == DayOfWeek.Friday)
                                {

                                    var vaz = prop.VazaoNatural.Where(x => x.Key >= date.AddDays(-6) && x.Key <= date);

                                    if (vaz.Count() == 7)
                                    {
                                        prop.calMedSemanal[date] = vaz.Average(x => x.Value);//media das  semanas seguintes à atual
                                    }
                                }
                            }
                        }
                        else//sem smap ext
                        {
                            for (DateTime date = SOatualMaior.AddDays(1); date <= SOatualMaior.AddDays(14); date = date.AddDays(1))
                            {
                                if (date.DayOfWeek == DayOfWeek.Friday)
                                {

                                    if (date <= SOatualMaior.AddDays(14))
                                    {
                                        var vaz = prop.VazaoNatural.Where(x => x.Key >= date.AddDays(-6) && x.Key <= date);

                                        if (vaz.Count() == 7)
                                        {
                                            prop.calMedSemanal[date] = vaz.Average(x => x.Value);//media das  semanas seguintes à atual
                                        }
                                    }
                                }
                            }
                        }

                        if (prop.IdPosto == 168 || prop.IdPosto == 169)
                        {
                            var vaz = prop.VazaoNatural.Where(x => x.Key >= SOatualMaior.AddDays(-6) && x.Key <= SOatualMaior);
                            if (vaz.Count() == 7)
                            {
                                prop.calMedSemanal[SOatualMaior] = vaz.Average(x => x.Value);//media da semana atual(para esses postos a media é calculada usando os dias de acomph e os dias de cpins para fechar a semana)
                            }                                                                //os dias de acomph ja haviam sido inseridos no metodo CalcSemanaPrevivaz
                        }
                    }

                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public void CalcSemanaPrevivaz(List<Propagacao> propagacoesAux, List<Propagacao> propagacoes)
        {
            //realiza a projeção para a semana atual dos postos previvaz utilizando dados do acomph,media ponderada com as variaçoes dos dados 
            try
            {

                DateTime dataNow = DateTime.Today.AddDays(-1);
                DateTime semanaNow = dataNow.AddDays(-23);
                var SOatualMaior = dataNow;

                while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                    SOatualMaior = SOatualMaior.AddDays(+1);//até sexta(fim da semana)atual

                var ultimoAcomph = propagacoesAux.First().VazaoNatural.Keys.Last();//data do ultimo dia de dados do acomph

                foreach (var prop in propagacoesAux)
                {
                    try
                    {
                        var semanAnt = semanaNow;

                        while (semanAnt <= SOatualMaior.AddDays(-7))
                        {
                            if (semanAnt.DayOfWeek == DayOfWeek.Friday)
                            {

                                if (semanAnt <= SOatualMaior.AddDays(-7))
                                {

                                    var vaz = prop.VazaoNatural.Where(x => x.Key >= semanAnt.AddDays(-6) && x.Key <= semanAnt);

                                    if (vaz.Count() == 7)
                                    {
                                        prop.calMedSemanal.Add(semanAnt, vaz.Average(x => x.Value));//media das semanas anteriores à atual
                                    }
                                }
                            }
                            semanAnt = semanAnt.AddDays(+1);
                        }

                        //semana atual 

                        if (prop.IdPosto == 97 || prop.IdPosto == 98 || prop.IdPosto == 110 || prop.IdPosto == 111 || prop.IdPosto == 112 || prop.IdPosto == 113 || prop.IdPosto == 114 /*|| prop.IdPosto == 222 || prop.IdPosto == 284*/)
                        {
                            CorrecaoSul(prop, propagacoes, SOatualMaior);
                        }
                        else
                        {
                            double[] difs = new double[4];
                            for (int i = 0; i < 4; i++)
                            {
                                double dif = prop.VazaoNatural[ultimoAcomph.AddDays(-i)] - prop.VazaoNatural[ultimoAcomph.AddDays(-(i + 1))];//variação dos ultimos 4 dias com dados
                                difs[i] = dif;
                            }
                            var mediaPon = (difs[0] * 3 + difs[1] * 2 + difs[2] * 1 + difs[3] * 1) / 7;// media ponderada com as variaçoes dos ultimos 4 dias de dados 

                            var fator = 0f;//usado para multiplicar com a media ponderada , obetendo-se um valor que tenderá á zero para a projeção dos dias faltantes da semana

                            for (DateTime date = ultimoAcomph; date <= SOatualMaior; date = date.AddDays(1))
                            {
                                if (!prop.VazaoNatural.ContainsKey(date))
                                {
                                    if ((/*prop.IdPosto == 222 ||*/ prop.IdPosto == 284) && (date.DayOfWeek == DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Friday))
                                    {
                                        double newFactor = FatorCorrecaoSulDiario(propagacoes, date);
                                        prop.VazaoNatural[date] = prop.VazaoNatural[date.AddDays(-1)] * newFactor;
                                    }
                                    else
                                    {
                                        prop.VazaoNatural[date] = prop.VazaoNatural[date.AddDays(-1)] + (mediaPon * (Math.Exp(fator)));
                                        fator += -0.4f;
                                    }

                                }
                            }
                            var vazNaturais = prop.VazaoNatural.Where(x => x.Key >= SOatualMaior.AddDays(-6) && x.Key <= SOatualMaior);

                            prop.calMedSemanal.Add(SOatualMaior, vazNaturais.Average(x => x.Value));//media das vazoes calculadas da semana atual                    
                        }
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }

                new AddLog("O método ExecutingProcess/SemanPrevivaz foi executado com sucesso!");
            }
            catch (Exception exc)
            {
                new AddLog("Falha ao processar o método ExecutingProcess/SemanPrevivaz");
                new AddLog("Erro: " + exc.Message);
            }

        }

        public double FatorCorrecaoSulDiario(List<Propagacao> propagacoes, DateTime date)
        {
            var p74 = propagacoes.Where(x => x.IdPosto == 74).Select(x => x.VazaoNatural).FirstOrDefault();
            var p73 = propagacoes.Where(x => x.IdPosto == 73).Select(x => x.VazaoNatural).FirstOrDefault();
            var p78 = propagacoes.Where(x => x.IdPosto == 78).Select(x => x.VazaoNatural).FirstOrDefault();
            var p77 = propagacoes.Where(x => x.IdPosto == 77).Select(x => x.VazaoNatural).FirstOrDefault();
            var p76 = propagacoes.Where(x => x.IdPosto == 76).Select(x => x.VazaoNatural).FirstOrDefault();
            var p71 = propagacoes.Where(x => x.IdPosto == 71).Select(x => x.VazaoNatural).FirstOrDefault();
            var p72 = propagacoes.Where(x => x.IdPosto == 72).Select(x => x.VazaoNatural).FirstOrDefault();

            var diaAnt = date.AddDays(-1);
            var mediaAtual = (p71[date] + p72[date] + p73[date] + p74[date] + p76[date] + p77[date] + p78[date]) / 7;
            var mediaAnt = (p71[diaAnt] + p72[diaAnt] + p73[diaAnt] + p74[diaAnt] + p76[diaAnt] + p77[diaAnt] + p78[diaAnt]) / 7;
            var fator = mediaAtual / mediaAnt;

            return fator;
        }

        public void CorrecaoSul(Propagacao prop, List<Propagacao> propagacoes, DateTime SOatualMaior)
        {
            //  o calculo da semana atual dos postos do sul é realizada de maneira diferente
            // divide-se a media do dia atual pela media do dia anterior
            var p74 = propagacoes.Where(x => x.IdPosto == 74).Select(x => x.VazaoNatural).FirstOrDefault();
            var p73 = propagacoes.Where(x => x.IdPosto == 73).Select(x => x.VazaoNatural).FirstOrDefault();
            var p78 = propagacoes.Where(x => x.IdPosto == 78).Select(x => x.VazaoNatural).FirstOrDefault();
            var p77 = propagacoes.Where(x => x.IdPosto == 77).Select(x => x.VazaoNatural).FirstOrDefault();
            var p76 = propagacoes.Where(x => x.IdPosto == 76).Select(x => x.VazaoNatural).FirstOrDefault();
            var p71 = propagacoes.Where(x => x.IdPosto == 71).Select(x => x.VazaoNatural).FirstOrDefault();
            var p72 = propagacoes.Where(x => x.IdPosto == 72).Select(x => x.VazaoNatural).FirstOrDefault();

            var ultimoAcomph = prop.VazaoNatural.Keys.Last();//data do ultimo dia de dados do acomph

            for (DateTime date = ultimoAcomph.AddDays(1); date <= SOatualMaior; date = date.AddDays(1))
            {
                // if (!prop.VazaoNatural.ContainsKey(date))//se der erro decomentar o if e tirar o adddays do ultimoacomph do for
                // {

                if ((prop.IdPosto == 110 || prop.IdPosto == 111 || prop.IdPosto == 112 || prop.IdPosto == 113 || prop.IdPosto == 114) && (date.DayOfWeek != DayOfWeek.Thursday && date.DayOfWeek != DayOfWeek.Friday))
                {
                    prop.VazaoNatural[date] = prop.VazaoNatural[ultimoAcomph];
                }
                else
                {
                    var diaAnt = date.AddDays(-1);
                    var mediaAtual = (p71[date] + p72[date] + p73[date] + p74[date] + p76[date] + p77[date] + p78[date]) / 7;
                    var mediaAnt = (p71[diaAnt] + p72[diaAnt] + p73[diaAnt] + p74[diaAnt] + p76[diaAnt] + p77[diaAnt] + p78[diaAnt]) / 7;
                    var fator = mediaAtual / mediaAnt;

                    prop.VazaoNatural[date] = prop.VazaoNatural[date.AddDays(-1)] * fator;
                }

                //}
            }
            var vazNaturais = prop.VazaoNatural.Where(x => x.Key >= SOatualMaior.AddDays(-6) && x.Key <= SOatualMaior);

            prop.calMedSemanal[SOatualMaior] = vazNaturais.Average(x => x.Value);//media das vazoes calculadas da semana atual    // sed er erro trocar pra versao de add          

        }
        public void CalculaMediaFaltante(List<Propagacao> propagacoes, DateTime ultimoAcomph)//calcula as medias semanais dos postos que foram incluidos antes da chamada desta função

        {
            try
            {
                DateTime dataNow = DateTime.Today.AddDays(-1);
                DateTime semanaNow = dataNow.AddDays(-23);
                var SOatualMaior = dataNow;

                while (SOatualMaior.DayOfWeek != DayOfWeek.Friday)
                    SOatualMaior = SOatualMaior.AddDays(+1);//até sexta(fim da semana)atual

                foreach (var prop in propagacoes)//propaga.VazaoIncremental = propagacoes.Where(x => x.IdPosto == 1).Select(x => x.VazaoIncremental).FirstOrDefault();
                {
                    if (prop.IdPosto == 116)
                    {
                        var pv119 = propagacoes.Where(x => x.IdPosto == 119).Select(x => x.VazaoNatural).FirstOrDefault();
                        var pv118 = propagacoes.Where(x => x.IdPosto == 118).Select(x => x.VazaoNatural).FirstOrDefault();
                        foreach (var dat in pv118.Keys.ToList())
                        {
                            prop.VazaoNatural[dat] = pv119[dat] - pv118[dat];
                        }

                        var p119 = propagacoes.Where(x => x.IdPosto == 119).Select(x => x.calMedSemanal).FirstOrDefault();
                        var p118 = propagacoes.Where(x => x.IdPosto == 118).Select(x => x.calMedSemanal).FirstOrDefault();
                        foreach (var dat in p118.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p119[dat] - p118[dat];
                        }
                    }
                    else if (prop.IdPosto == 318)// posto caculado de maneira diferente dos demais consulte:H:\TI - Sistemas\UAT\ChuvaVazao\MANUAL_USUARIO\Planilhas\CHUVAVAZAO_EXEMPLO.xlsm aba Propagações_TOT_TIETE
                    {
                        var p116 = propagacoes.Where(x => x.IdPosto == 116).Select(x => x.VazaoNatural).FirstOrDefault();
                        var p117 = propagacoes.Where(x => x.IdPosto == 117).Select(x => x.VazaoNatural).FirstOrDefault();
                        var p118 = propagacoes.Where(x => x.IdPosto == 118).Select(x => x.VazaoNatural).FirstOrDefault();
                        var p161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.VazaoNatural).FirstOrDefault();

                        foreach (var dat in p118.Keys.ToList())
                        {
                            if (dat <= ultimoAcomph)
                            {
                                prop.VazaoNatural[dat] = 0;
                                prop.VazaoIncremental[dat] = 0;
                            }
                            else
                            {
                                prop.VazaoNatural[dat] = p116[dat] + p117[dat] + p118[dat] + 0.1f * (p161[dat] - p117[dat] - p118[dat]);
                                prop.VazaoIncremental[dat] = prop.VazaoNatural[dat];

                            }
                        }

                        while (semanaNow <= p118.Keys.Last())//SOatualMaior.AddDays(+14))
                        {
                            if (semanaNow.DayOfWeek == DayOfWeek.Friday)
                            {

                                if (semanaNow <= p118.Keys.Last())//SOatualMaior.AddDays(14))
                                {

                                    var vazNaturais = prop.VazaoNatural.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);
                                    var vazIncrementais = prop.VazaoIncremental.Where(x => x.Key >= semanaNow.AddDays(-6) && x.Key <= semanaNow);

                                    if (vazNaturais.Count() == 7)
                                    {
                                        prop.medSemanalNatural.Add(semanaNow, vazNaturais.Average(x => x.Value));
                                        prop.medSemanalIncremental.Add(semanaNow, vazIncrementais.Average(x => x.Value));
                                        prop.calMedSemanal.Add(semanaNow, vazIncrementais.Average(x => x.Value));
                                    }
                                }
                            }
                            semanaNow = semanaNow.AddDays(+1);
                        }

                    }

                    else if (prop.IdPosto == 319)// posto caculado de maneira diferente dos demais consulte:H:\TI - Sistemas\UAT\ChuvaVazao\MANUAL_USUARIO\Planilhas\CHUVAVAZAO_EXEMPLO.xlsm aba Propagações_TOT_TIETE
                    {
                        var p117 = propagacoes.Where(x => x.IdPosto == 117).Select(x => x.calMedSemanal).FirstOrDefault();
                        var p118 = propagacoes.Where(x => x.IdPosto == 118).Select(x => x.calMedSemanal).FirstOrDefault();
                        var p161 = propagacoes.Where(x => x.IdPosto == 161).Select(x => x.calMedSemanal).FirstOrDefault();

                        foreach (var dat in p118.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p117[dat] + p118[dat] + 0.1f * (p161[dat] - p117[dat]);
                        }
                    }

                    else if (prop.IdPosto == 139)
                    {
                        var p34 = propagacoes.Where(x => x.IdPosto == 34).Select(x => x.medSemanalIncremental).FirstOrDefault();

                        foreach (var dat in p34.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p34[dat];
                        }
                    }

                    else if (prop.IdPosto == 136)
                    {
                        var p245 = propagacoes.Where(x => x.IdPosto == 245).Select(x => x.medSemanalIncremental).FirstOrDefault();

                        foreach (var dat in p245.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p245[dat];
                        }
                    }

                    else if (prop.IdPosto == 137)
                    {
                        var p246 = propagacoes.Where(x => x.IdPosto == 246).Select(x => x.medSemanalIncremental).FirstOrDefault();

                        foreach (var dat in p246.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p246[dat];
                        }
                    }

                    else if (prop.IdPosto == 138)
                    {
                        var p243 = propagacoes.Where(x => x.IdPosto == 243).Select(x => x.medSemanalIncremental).FirstOrDefault();

                        foreach (var dat in p243.Keys.ToList())
                        {
                            prop.calMedSemanal[dat] = p243[dat];
                        }
                    }
                    else if (prop.IdPosto == 166)
                    {
                        var itaipu = propagacoes.Where(x => x.IdPosto == 266).Select(x => x.medSemanalIncremental).FirstOrDefault();
                        foreach (var dat in itaipu.Keys.ToList())
                        {

                            prop.calMedSemanal[dat] = itaipu[dat];

                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }

        }
    }
}

public class PostoRegre
{
    public PostoRegre()
    {


    }
    public int IdPosto_Base { get; set; }
    public int Idposto_Regredido { get; set; }
}
public class Regressao
{
    public Regressao()
    {
        double Valor_Mensal;

    }
    public int IdPosto { get; set; }
    public List<double> Valor_mensal { get; set; }
}

public class Enas
{
    public Enas()
    {
        DadoEna = new Dictionary<DateTime, double>();
    }
    public int IdPosto { get; set; }
    public int subMercado { get; set; }
    public string bacia { get; set; }
    public Dictionary<DateTime, double> DadoEna { get; set; }
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
        calMedSemanal = new Dictionary<DateTime, double>();
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
    public Dictionary<DateTime, double> calMedSemanal { get; set; }

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

public class DadvazPrevs
{
    public DadvazPrevs()
    {
        Vazao = new Dictionary<DateTime, double>();
    }
    public string NomeUsina { get; set; }
    public string Bacia { get; set; }
    public int NumDadvaz { get; set; }
    public int NumPrevs { get; set; }
    public double Fator { get; set; }
    public Dictionary<DateTime, double> Vazao { get; set; }

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
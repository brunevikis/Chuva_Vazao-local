cat("\014") 
rm(list=ls())
library(parallel)
library(readxl)
source('./Codigos_R/Conjunto_V3.0.R')

# arquivo log
arq_log<- paste0("log.txt")
write("-----Calculando Previsao do conjunto com remocao de vies-----",file=arq_log,append=TRUE)

#=================================data (caso queira rodar outra data alterar aqui)================================================================================
dia_previsao<-as.Date(Sys.Date(), "%d/%m/%Y")
caminho_data = file.path("./Arq_Entrada/data.txt")
if(file.exists(caminho_data)){
  aux <- readLines(caminho_data)
  dia_previsao <- as.Date(aux,"%d/%m/%Y")
  file.remove(caminho_data)
}

texto = paste0("data da rodada:",dia_previsao,"\n")
cat(texto)
write(texto,file=arq_log,append=TRUE)

#=================================Parametros da rodada============================================================================================================
tempo_regressao<-120
dias_previstos<-14
agrupamento<-cbind(3,3,3,5)
modelos<-c('ETA40','GEFS')
alpha<-cbind(2,2,2,2)
beta<-cbind(1,1,1,1)
lambdas<-seq(0,0.5,by=0.02)

#=================================Leitura do arquivo de configuracao===============================================================================================
planilha <- read_xlsx("./Arq_Entrada/configuracao.xlsx",sheet = "Dados")
bacias<-planilha$'Codigo ANA'
texto = "Arquivo de configuracao lido com sucesso \n"
cat(texto)
write(texto,file=arq_log,append=TRUE)
#================================ cria o cluster ==================================================================================================================
numCores <- detectCores()
ensemble<-matrix(NA_real_,nrow=dias_previstos,ncol=length(bacias))
clust <- makeCluster(numCores-1, type = 'PSOCK') 
clusterExport(clust, varlist = c('roda_bacia', 'roda_lp','roda_vies','roda_fator','dia_previsao','tempo_regressao','dias_previstos','agrupamento','modelos','alpha','beta','bacias','lambdas'), envir = .GlobalEnv)

#=========================== roda o conjunto para as bacias ========================================================================================================
texto = "Gerando Conjunto \n"
cat(texto)
write(texto,file=arq_log,append=TRUE)

res<-parLapply(clust,1:length(bacias), function(x) roda_bacia(bacias[x],dia_previsao,tempo_regressao,dias_previstos,agrupamento,modelos,alpha,beta,lambdas))
stopCluster(clust)

texto = "Conjunto gerado \n"
cat(texto)
write(texto,file=arq_log,append=TRUE)

#======================================================= aplica limite semanal =====================================================================================
pesos_sem<-matrix(1,ncol =dias_previstos,nrow=length(bacias))
for (i in 1:length(bacias)){
  mes_prev<-as.numeric(format(dia_previsao,"%m"))
  if(mes_prev %in% c(12,1)){lim_sem<-planilha$`DEZ-JAN`[i]}
  if(mes_prev %in% c(2,3)){lim_sem<-planilha$`FEV-MAR`[i]}
  if(mes_prev %in% c(4,5)){lim_sem<-planilha$`ABR-MAI`[i]}
  if(mes_prev %in% c(6,7)){lim_sem<-planilha$`JUN-JUL`[i]}
  if(mes_prev %in% c(8,9)){lim_sem<-planilha$`AGO-SET`[i]}
  if(mes_prev %in% c(10,11)){lim_sem<-planilha$`OUT-NOV`[i]}
  for( j in 1: (dias_previstos/7)){
    soma<-sum(res[[i]][[1]][(1 + (j-1)*7):(j*7)])
    if( soma > lim_sem){
      fator<-lim_sem/soma
      for ( k in 1:7){
        res[[i]][[1]][(k+(j-1)*7)]<-res[[i]][[1]][(k+(j-1)*7)]*fator
        pesos_sem[i,(k+(j-1)*7)]<-fator
      }
    }
  }
}

#===========================aplica os limites diarios======================================================================================================================
pesos_d<-matrix(1,ncol =dias_previstos,nrow=length(bacias))
for( i in 1:length(bacias)){
  for ( j in 1:dias_previstos){
    pesos_d[i,j]<-min(1,planilha$Diario[i]/res[[i]][[1]][j])
    res[[i]][[1]][j]<-min(planilha$Diario[i],res[[i]][[1]][j])
  }
}



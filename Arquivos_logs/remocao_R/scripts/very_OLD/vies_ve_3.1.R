args = commandArgs(trailingOnly=TRUE)
load("rem.RData")
library(sgeostat)
library(readxl)
getwd <- function ()  "."

vies <- function(dia_remocao, dia_previsao, modelos, grade) {
  
  planilha <- read_xlsx("P:/Pedro/remoção_R/Contornos/configuracao.xlsx",sheet = "Plan1")
  planilha2 <- read_xlsx("./Arq_Entrada/configuracao.xlsx",sheet = "Dados")
  bacias<-cbind(planilha$'Macro-Bacia',planilha$Nome)
  diretorios<-NULL
  
  precs<-matrix(1,0,14)
  for ( i in 1:nrow(bacias)){
    precs2<-matrix(1,0,14)
    for ( n in 1:length(modelos)) {
      tg <- read.csv(file=paste0("P:/Pedro/remoção_R/Contornos/ETA40/", 
                                 planilha$`Macro-Bacia`[i], "/", planilha[[paste0("contorno_",grade[n])]], ".bln")[i],
                     head=FALSE,row.names=NULL)[-1,]
      llengthg <- length(tg$V1)
      lonblng <- tg$V1
      latblng <- tg$V2
      
      files.full.tot <- unlist(lapply(rep(dia_remocao,14)+1:14, function(x)
        list.files(path = paste0("./grid/", modelos[n]),pattern=
                     paste(modelos[n], "_p",format(dia_previsao, "%d%m%y"), "a", 
                           format(x, "%d%m%y"),sep=""),  full.names = TRUE)))
      if (!length(files.full.tot)==0) {
        start_char <- unlist(gregexpr(pattern = "a[0-9]{6}",files.full.tot[1]))+1
        files.full.tot<-files.full.tot[order(as.Date(substring(files.full.tot, first=start_char, last=start_char+5), format = "%d%m%y"))]
        
        dlistread<-lapply(files.full.tot,function(x) read.table(x, header=F,col.names = c("lon","lat","pr")))
        # subsetando pontos em ul retangulo ligeiramente maior que o contorno
        el<-which(dlistread[[1]]$lon >= min(lonblng) & dlistread[[1]]$lon <= max(lonblng) & dlistread[[1]]$lat >= min(latblng) & dlistread[[1]]$lat <= max(latblng), arr.ind =  TRUE)
        
        # applying to all points in dlistread
        dz<-lapply(dlistread,function(x) x[c(el),])
        
        # capturando as posicoes das linhas dentro do  bln
        elpolyg<-which(in.polygon(dz[[1]]$lon, dz[[1]]$lat, lonblng, latblng), arr.ind = TRUE)
        
        # capturando os pontos dentro do poligono e fazendo a media da 3a coluna (prec)
        dzpolyg<-lapply(dz,function(x) x[c(elpolyg),])
        meanpolyg<-unlist(lapply(dzpolyg, function(x) round(mean(x[,3]),2)))
      } else {meanpolyg<- -99}
      precs2 <- rbind(precs2, c(meanpolyg, rep(-99,14))[1:14])
    }
    precs2[3,which(precs2[3,]==-99, T)]=precs2[1,which(precs2[3,]==-99, T)]
    precs2[2,which(precs2[2,]==-99, T)]=precs2[1,which(precs2[2,]==-99, T)]
    precs<-rbind(precs, res[[i]][[2]][,1]*precs2[1,] 
                 + ifelse(is.na(res[[i]][[2]][,2]),0,res[[i]][[2]][,2])*precs2[2,] 
                 + ifelse(is.na(res[[i]][[2]][,3]),0,res[[i]][[2]][,3])*precs2[3,]
    )
    #precs2<-lapply(modelos,function(x) read.table(paste0(diretorios[i],"/",x,".csv"),header=T, dec=",",sep =";"))
    #precs<-rbind(precs, res[[i]][[2]][,1]*precs2[[1]][length(precs2[[1]][,1]),2:13]+
    #               ifelse(is.na(res[[i]][[2]][,2]),0,res[[i]][[2]][,2])*cbind(precs2[[2]][length(precs2[[2]][,1]),2:10],0,0,0))
  }
  n<-sum(precs[1,]>=0)
  precs2<-precs[,1:(n)]
  l<-length(precs2[1,])
  for (i in 1:length(bacias[,1])){
    mes_prev<-as.numeric(format(dia_previsao,"%m"))
    if(mes_prev %in% c(12,1)){lim_sem<-planilha2$`DEZ-JAN`[i]}
    if(mes_prev %in% c(2,3)){lim_sem<-planilha2$`FEV-MAR`[i]}
    if(mes_prev %in% c(4,5)){lim_sem<-planilha2$`ABR-MAI`[i]}
    if(mes_prev %in% c(6,7)){lim_sem<-planilha2$`JUN-JUL`[i]}
    if(mes_prev %in% c(8,9)){lim_sem<-planilha2$`AGO-SET`[i]}
    if(mes_prev %in% c(10,11)){lim_sem<-planilha2$`OUT-NOV`[i]}
    for( j in 1: (l/7)){
      soma<-sum(precs2[i,(1 + (j-1)*7):min(l,7*j)])
      if( soma > lim_sem){
        fator<-lim_sem/soma
        for ( k in 1:min(l+(1-j)*7,7)){
          precs2[i,(k+(j-1)*7)]<-precs2[i,(k+(j-1)*7)]*fator
          
        }
      }
    }
  }
  
  for( i in 1:length(bacias[,1])){precs2[i,][precs2[i,]>planilha2$Diario[i]]<-planilha2$Diario[i]}
  
  outdir<-paste0("./Arq_Saida/vies_shadow.", format(dia_remocao,"%d-%m"), "/")
  dir.create(outdir)
  for (n in 1:l) {
    arq<-paste0(outdir,"p",format(dia_previsao, format="%d%m%y"),"a",format((dia_remocao+n), format="%d%m%y"),".dat")
    file.create(arq)
    for( j in 1:nrow(bacias)){
      texto<-paste0(formatC(planilha$Longitude[j], 2, 6, "f", 0)," ",formatC(planilha$Latitude[j], 2, 6, "f", 0)," ", format(round(precs2[j,n],1),nsmall=2))
      write.table(texto,arq, dec=".",row.names = FALSE,col.names = FALSE,append = TRUE,quote = FALSE)
    }
    cat(arq)
    cat("\n")
  }
}
dia_previsao<-as.Date(Sys.Date(), "%d/%m/%Y")
modelos<- c('GEFS', 'ECMWF', 'ETA40')
grade<- c('ETA', 'ETA', 'ETA')
if (length(args) != 0) {
  for (i in 1:length(args)) {
    cat(args[i], "\n")
    vies(as.Date(args[i], "%d/%m/%y"), dia_previsao, modelos, grade)    
  }
}

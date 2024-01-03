args = commandArgs(trailingOnly=TRUE)
cat(args)
load("rem.RData")
library(sgeostat)
library(readxl)
getwd <- function ()  "."

vies <- function(dia_remocao, dia_previsao, modelos, grade) {
  
planilha <- read_xlsx("./Parametros/configuracao.xlsx",sheet = "Plan1")
bacias<-cbind(planilha$'Macro-Bacia',planilha$Nome)
diretorios<-NULL

precs<-matrix(1,0,12)
for ( i in 1:nrow(bacias)){
  precs2<-matrix(1,0,12)
  for ( n in 1:length(modelos)) {
    tg <- read.csv(file=paste0("./Parametros/Contornos/", ifelse(grade[n]=="GEFS","GEFS/contornos TIGGE_GFS_","ETA40/"), 
                                planilha$`Macro-Bacia`[i], "/", planilha[[paste0("contorno_",grade[n])]], ".bln")[i],
                   head=FALSE,row.names=NULL)[-1,]
    llengthg <- length(tg$V1)
    lonblng <- tg$V1
    latblng <- tg$V2
    files.full.tot <- unlist(lapply(rep(dia_remocao,12)+1:12, function(x)
                             list.files(path = paste0("./Arq_Entrada/Previsao/", modelos[n]),pattern=
                                          paste("*_p",format(dia_previsao, "%d%m%y"), "a", 
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
    meanpolyg<-unlist(lapply(dzpolyg, function(x) mean(x[,3])))
    } else {meanpolyg<- -99}
    precs2 <- rbind(precs2, c(meanpolyg, rep(-99,12))[1:12])
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

for( i in 1:length(diretorios)){precs[i,][precs[i,]>planilha$`P lim d`[i]]<-planilha$`P lim d`[i]}

for( i in 1:12){
}
outdir<-paste0("./Arq_Saida/vies.", format(dia_remocao,"%d-%m"), "/")
dir.create(outdir)
n=1
while (0 <= sum(precs[,n])) {
  arq<-paste0(outdir,"p",format(dia_previsao, format="%d%m%y"),"a",format((dia_remocao+n), format="%d%m%y"),".dat")
  file.create(arq)
  for( j in 1:nrow(bacias)){
    texto<-paste0(format(planilha$Longitude[j], nsmall=2)," ",format(planilha$Latitude[j], nsmall=2)," ", format(round(precs[j,n],1),nsmall=2))
    write.table(texto,arq, dec=".",row.names = FALSE,col.names = FALSE,append = TRUE,quote = FALSE)
  }
  cat(arq)
  cat("\n")
  n<-n+1
  if (n==13) break
}
}

#dia_remocao<- as.Date(args[1], "%d/%m/%y")
dia_previsao<-as.Date(Sys.Date(), "%d/%m/%Y")
#dia_remocao<-as.Date("13/08/20", "%d/%m/%y")
modelos<- c('GEFS50', 'ECMWF', 'ETA40')
grade<- c('ETA', 'ETA', 'ETA')

#if (length(dia_remocao)==0) {dia_remocao <- dia_previsao+7-as.numeric(format(dia_previsao+3, "%w"))}

#vies(dia_remocao, dia_previsao, modelos, grade)

#vies(dia_remocao+7-as.numeric(format(dia_remocao+3, "%w")), dia_previsao, modelos, grade)

if (length(args) != 0) {
  for (i in 1:length(args)) {
    vies(as.Date(args[i], "%d/%m/%y"), dia_previsao, modelos, grade)    
  }
}

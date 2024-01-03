args = commandArgs(trailingOnly=TRUE)
#args = c("26/11/20","03/12/20","10/12/20")
library(readxl)
library(tidyverse)
dia_previsao<-Sys.Date()
load("./t.RData")
load("./rem.RData")
config <- read_xlsx("./Arq_Entrada/Configuracao.xlsx", sheet = "Dados")
if (length(args) != 0) {
  for (g in 1:length(args)) {
    dia_remocao<-as.Date(args[g], "%d/%m/%y")
    w<-matrix(nrow=0,ncol = 6)
    for(u in 1:nrow(config)){
      w<-rbind(w,cbind(1:14,config[u,1:4],res[[u]][[2]],res[[u]][[3]]))
    }
    colnames(w)<-c("fday","Nome","Latitude","Longitude","Codigo ANA","c GEFS","c ETA40","p GEFS","p ETA40")
    w<-as_tibble(w)%>%mutate(`c ETA40`=coalesce(`c ETA40`,rep(0,nrow(w))),`p ETA40`=coalesce(`p ETA40`,rep(0,nrow(w))))
    p<-t%>%filter(model %in% c("ECMWF", "GEFS", "ETA40"),run==Sys.Date())%>% pivot_wider(names_from = model,values_from=prec)
    
    d1<-p%>%filter(date>as.Date(dia_remocao,"%Y%m%d"))%>%mutate(GEFS=coalesce(GEFS,ECMWF),ETA40=coalesce(ETA40,GEFS))%>%arrange(date)%>%group_by(`Codigo ANA`)%>% mutate(fday = row_number())
    p<-left_join(w,d1, by=c("Codigo ANA"="Codigo ANA","Latitude"="Latitude","Longitude"="Longitude","fday"="fday"))%>%
      mutate(prec=GEFS*`c GEFS`*`p GEFS`+ETA40*`c ETA40`*`p ETA40`)
    
    n<-length(setdiff(unique(p$date),NA))
    precs<-p[,c(2,10,15)]%>%pivot_wider(names_from = Nome,values_from = prec)%>%.[,-1]
    precs2<-precs[1:(n),]
    l<-nrow(precs2)
	
    for (i in 1:nrow(config)){
      mes_prev<-as.numeric(format(dia_remocao,"%m"))
      if(mes_prev %in% c(12,1)){lim_sem<-config$`DEZ-JAN`[i]}
      if(mes_prev %in% c(2,3)){lim_sem<-config$`FEV-MAR`[i]}
      if(mes_prev %in% c(4,5)){lim_sem<-config$`ABR-MAI`[i]}
      if(mes_prev %in% c(6,7)){lim_sem<-config$`JUN-JUL`[i]}
      if(mes_prev %in% c(8,9)){lim_sem<-config$`AGO-SET`[i]}
      if(mes_prev %in% c(10,11)){lim_sem<-config$`OUT-NOV`[i]}
      for( j in 1: (l/7)){
        soma<-sum(precs2[(1 + (j-1)*7):min(l,7*j),i])
        if( soma>lim_sem) {
          fator<-lim_sem/soma
          for ( k in 1:min(l+(1-j)*7,7)){
            precs2[(k+(j-1)*7),i]<-precs2[(k+(j-1)*7),i]*fator
            
          }
        }
      }
    }
    
    for( i in 1:nrow(config)){precs2[,i][precs2[,i]>config$Diario[i]]<-config$Diario[i]}
    outdir<-paste0("./Arq_Saida/vies_", format(dia_remocao,"%d-%m"), "/")
    dir.create(outdir)
    for (n in 1:l) {
      arq<-paste0(outdir,"p",format(dia_previsao, format="%d%m%y"),"a",format((dia_remocao+n), format="%d%m%y"),".dat")
      file.create(arq)
      for( j in 1:nrow(config)){
        texto<-paste0(formatC(config$Longitude[j], 2, 6, "f", 0)," ",formatC(config$Latitude[j], 2, 6, "f", 0)," ", format(as.numeric(round(precs2[n,j],1)),nsmall=2))
        write.table(texto,arq, dec=".",row.names = FALSE,col.names = FALSE,append = TRUE,quote = FALSE)
      }
    }
    
    m<-list.files(path=outdir, pattern = "p.*.dat")
    m2<-vector()
    q<-as.data.frame(config[c("Nome", "Longitude", "Latitude")])
    for (d in m) {
      i<-format(as.Date(substring(d, unlist(regexec("a",d))+1, unlist(regexec("d",d))-2),"%d%m%y"),"%Y-%m-%d")
      m2<-c(m2,i)
      cat(d,"- ",i,"\n")
      prc<-read.table(paste0(outdir, d), header = F, col.names = c("Longitude", "Latitude", i))
      q<-merge(q, prc, by = c("Longitude", "Latitude"), all = T, sort = F)
    }
    colnames(q)<-c("lon", "lat", "Nome", m2)
	t2<-cbind(formatC(q[,1], 2, 6, "f", 0), formatC(q[,2], 2, 6, "f", 0), format(as.numeric(rowMeans(q[,-c(1:3)])),nsmall=8))
    for (i in 1:5) {
      arq<-paste0(outdir, substring(d, 1, unlist(regexec("a",d))), format(as.Date(max(colnames(q[,-c(1:3)])))+i, "%d%m%y"), ".dat")
      write.table(t2,arq, dec=".",row.names = FALSE,col.names = FALSE,append = F,quote = FALSE)
    }
  }
}

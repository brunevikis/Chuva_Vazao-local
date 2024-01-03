args = commandArgs(trailingOnly=TRUE)
com <- args[1]



getwd <- function ()  "."
ls <- function(name, pos = -1L, envir = as.environment(pos), all.names = FALSE, pattern, sorted = TRUE) 
  setdiff(base::ls(name, pos, globalenv(), all.names, pattern, sorted), c("getwd","com"))

#if(grepl("Roda_Conjunto",com)) {
#  source("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\remocao_R\\scripts\\Roda_Conjunto_sem_ECMWF.R")
#  source("H:\\TI - Sistemas\\UAT\\ChuvaVazao\\remocao_R\\scripts\\Roda_Conjunto_com_ECMWF.R")
#  save(res, file = "rem2.RData")
#}

source(paste0(".\\Codigos_R\\", com))
com = commandArgs(trailingOnly=TRUE)[1]

if(grepl("Roda_Conjunto",com)) {
  save(res, file = "rem.RData")
  
  library(readxl)
  config <- read_xlsx("./Arq_Entrada/Configuracao.xlsx", sheet = "Dados")
  m<-list.files(path="./Arq_Saida/", pattern = "p.*.dat")
  m2<-vector()
  t<-as.data.frame(config[c("Nome", "Longitude", "Latitude")])
  for (d in m) {
    i<-format(as.Date(substring(d, unlist(regexec("a",d))+1, unlist(regexec("d",d))-2),"%d%m%y"),"%Y-%m-%d")
    m2<-c(m2,i)
    cat(d,"- ",i,"\n")
    prc<-read.table(paste0("./Arq_Saida/", d), header = F, col.names = c("Longitude", "Latitude", i))
    t<-merge(t, prc, by = c("Longitude", "Latitude"), all = T, sort = F)
  }
  colnames(t)<-c("lon", "lat", "Nome", m2)
  t2<-cbind(formatC(t[,1], 2, 6, "f", 0), formatC(t[,2], 2, 6, "f", 0), format(as.numeric(rowMeans(t[,-c(1:3)])),nsmall=8))
  for (i in 1:5) {
    arq<-paste0("./Arq_Saida/", substring(d, 1, unlist(regexec("a",d))), format(as.Date(max(colnames(t[,-c(1:3)])))+i, "%d%m%y"), ".dat")
    write.table(t2,arq, dec=".",row.names = FALSE,col.names = FALSE,append = F,quote = FALSE)
  }
}
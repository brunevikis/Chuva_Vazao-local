args = commandArgs(trailingOnly=TRUE)
library(readxl)
library(sgeostat)

modelo <- args[1]
grade <- args[ifelse(is.na(args[2]),1,2)]
bypass <- ifelse(is.na(args[3]),FALSE,TRUE)

file.xlsx <- "./Parametros/Configuracao.xlsx"
caminhocontornos <- paste0("./Parametros/Contornos/", ifelse(grade =="GEFS", "GEFS/contornos TIGGE_GFS_", "ETA40/"))

trim.trailing <- function (x) sub("\\s+$", "", x)

lookup.xlsx <- read_xlsx(file.xlsx, sheet = "Plan1")
input.bacias <- as.data.frame(matrix(NA_character_, nrow = length(lookup.xlsx$'Latitude'), ncol = 2), stringsAsFactors = F)
colnames(input.bacias) <- c("bacia", "subbacia")

for (i in 1:length(lookup.xlsx$Latitude)) {
  input.bacias[i, 1] <- lookup.xlsx$'Macro-Bacia'[i]
  input.bacias[i, 2] <- lookup.xlsx$'Nome'[i]
}

ifelse(!dir.exists(paste0("./Arq_Saida/", modelo)), dir.create(paste0("./Arq_Saida/", modelo)), FALSE)

maplist<-list.files(path = paste0("./Arq_Entrada/Previsao/", modelo), pattern = "*.dat")

for (m in maplist) {
  merge.dat <- read.table(paste0("./Arq_Entrada/Previsao/", modelo,"/", m), header = F, col.names = c("lon", "lat", "pr"))
  cat(m)
  n <- ifelse(bypass, m, ifelse(unlist(regexec("[0-9]{8}",m))>=0,paste0( "p", 
  format(as.Date(substring(m, unlist(regexec("[0-9]{8}_",m)), unlist(regexec("_",m))-1),"%Y%m%d"),"%d%m%y"), "a",
  format(as.Date(substring(m, unlist(regexec("[0-9]{8}_",m)), unlist(regexec("_",m))-1),"%Y%m%d")+
           which(maplist==m),"%d%m%y"), ".dat"),
  paste0( "p", substring(m, unlist(regexec("[0-9]{6}a",m)), unlist(regexec("a",m))),
         substring(m, unlist(regexec("[0-9]{6}\\.",m)), unlist(regexec("[0-9]\\.dat",m))), ".dat")))
		 cat(n)
  file.create(paste0("./Arq_Saida/", modelo, "/", n))
  for (count.bac in seq(from = 1, to = length(lookup.xlsx$'Latitude'), by = 1)) {
    check.contornos <-
      lookup.xlsx[trim.trailing(as.character(lookup.xlsx$'Macro-Bacia')) == input.bacias$bacia[count.bac] &
                    trim.trailing(as.character(lookup.xlsx$'Nome')) == input.bacias$subbacia[count.bac], ]
    contorno <-
      trim.trailing(as.character(check.contornos[[paste0("contorno_", sub("40", "", grade))]]))
    bac <- trim.trailing(as.character(check.contornos$'Macro-Bacia'))
    subbac <- trim.trailing(as.character(check.contornos$Nome))
    caminhocontornos.ETA40 <-paste0(caminhocontornos, bac, "/", contorno, ".bln")
    
    tg <- read.csv(file = paste0(caminhocontornos.ETA40), head = F, row.names = NULL)[-1, ]
    llengthg <- length(tg$V1)
    lonblng <- tg$V1
    latblng <- tg$V2
    
    el <-which( merge.dat$lon >= min(lonblng) & merge.dat$lon <= max(lonblng) &
          merge.dat$lat >= min(latblng) & merge.dat$lat <= max(latblng), arr.ind =  T)
    dz <- merge.dat[c(el), ]
    
    elpolyg <- which(in.polygon(dz$lon, dz$lat, lonblng, latblng), arr.ind = T)
    
    dzpolyg <- dz[c(elpolyg), ]
    
    meanpolyg <- mean(dzpolyg[, 3])
    psat <-paste(format(lookup.xlsx$Longitude[count.bac], nsmall=2), 
                 format(lookup.xlsx$Latitude[count.bac], nsmall=2),
                 format(round(meanpolyg, 2),nsmall=2))
    write(psat, file = paste0("./Arq_Saida/", modelo, "/", n), append = T)
  }
  cat(" -> ", n, "  OK\n")
}
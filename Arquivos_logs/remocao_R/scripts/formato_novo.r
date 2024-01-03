library(readxl)
library(sgeostat)
library(tidyverse)


trim.trailing <- function (x) sub("\\s+$", "", x)
  
cord<- function(q, prev = dat, xlsx = config, input.bacias = bacias) {
  caminhocontornos <-  "H:/TI - Sistemas/UAT/ChuvaVazao/remocao_R/Contornos/ETA40/"
  bacia <-xlsx[xlsx$`Codigo ANA` == input.bacias$`Codigo ANA`[q], ]
  contorno <- trim.trailing(as.character(bacia[["contorno_ETA"]]))
  bac <- trim.trailing(as.character(bacia$'Macro-Bacia'))
  contorno<-paste0(caminhocontornos, bac, "/", contorno, ".bln")
  tg <- read.csv(file = contorno, head = F, row.names = NULL)[-1, ]
  llengthg <- length(tg$V1)
  lonblng <- tg$V1
  latblng <- tg$V2
  el <-which( prev$lon >= min(lonblng) & prev$lon <= max(lonblng) &
                prev$lat >= min(latblng) & prev$lat <= max(latblng), arr.ind =  T)
  dz <- prev[c(el), ]
  elpolyg <- which(in.polygon(dz$lon, dz$lat, lonblng, latblng), arr.ind = T)
  dzpolyg <- dz[c(elpolyg), ]
  cbind(input.bacias[q,], dzpolyg[, c(1,2)])
}

models<-list.dirs("./grid")[-1]
#models<-"./grid/ECMWF"
config <- read_xlsx("H:/TI - Sistemas/UAT/ChuvaVazao/remocao_R/Contornos/Configuracao.xlsx", sheet = "Plan1")
bacias <- as_tibble(config) %>% select(`Codigo ANA`,Latitude,Longitude) 
t<- bacias %>% add_column(date=as.Date(Sys.Date(), "%d/%m/%Y"))
for (m in models) {
  dir.create(paste0("./Arq_Saida", gsub("\\./grid","",m)))
  dir.create(paste0("./Arq_Entrada", gsub("\\./grid","",m)))
  maplist<-list.files(m, pattern = "*.dat")
  prev<-unique(gsub("(.*_p)|(a[0-9].*)", "", maplist))
  k<-ifelse(identical(maplist[as.Date(gsub("(.*[0-9]a)|(\\.dat)", "", maplist),"%d%m%y")==Sys.Date()+1],character(0)),maplist[1],maplist[as.Date(gsub("(.*[0-9]a)|(\\.dat)", "", maplist),"%d%m%y")==Sys.Date()+1])
  dat <- read.table(paste0(m,"/",k), header = F, col.names = c("lon", "lat", "pr"))
  coord<-lapply(1:(length(config$'Latitude')), cord) %>% reduce(full_join,by = c("Codigo ANA", "Latitude", "Longitude", "lon", "lat"))
  for (p in prev) {
    x<-coord
    maplist<- list.files(m, pattern = paste0(".*p",p,".*\\.dat"))
    for (d in maplist) {
      a<-read.table(paste0(m,"/", d), header = F, col.names = c("lon", "lat", gsub("(.*[0-9]a)|(\\.dat)", "", d)))
      x<-left_join(x,a,by = c("lat", "lon"))
      txt<- left_join(coord,a,by = c("lat", "lon")) %>% group_by(`Codigo ANA`, Latitude, Longitude) %>%
        rename(X = paste0("X",gsub("(.*[0-9]a)|(\\.dat)", "", d))) %>% filter(X>=0) %>% summarise(prec = round(mean(X,na.rm = T), 1),.groups = 'drop') %>% 
        full_join(bacias,., by = c("Codigo ANA", "Latitude", "Longitude"))
      write.table(paste0(formatC(txt$Longitude, 2, 6, "f", 0)," ",formatC(txt$Latitude, 2, 6, "f", 0)," ", format(txt$prec,nsmall=2)),
                  paste0("./Arq_Saida", gsub("\\./grid","",m),"/",gsub("[A-z0-9]+_","",d)), dec=".",row.names = F,col.names = F,quote = F)
    }
    x<-x %>% pivot_longer(cols = 6:last_col(), names_to = "date", values_to = "prec") %>% 
      mutate(date = as.Date(date, "X%d%m%y")) %>%group_by(`Codigo ANA`, Latitude, Longitude, date) %>% filter(prec>=0) %>%
      summarise(prec = round(mean(prec,na.rm = T), 2),.groups = 'drop') %>% pivot_wider(names_from = date, values_from = prec)
    #gdata::write.fwf(as.data.frame(full_join(bacias,x))[,1:(min(17,length(x)))],paste0("./Arq_Entrada", strrep(gsub("\\./grid","",m), 2), "_m_", p,".dat"),quote = F, sep = " ", width = c(9,rep(7,min(17,length(x))-1)), colnames=F)
    ###
    gdata::write.fwf(as.data.frame(full_join(bacias,x, by = c("Codigo ANA", "Latitude", "Longitude"))),
                     paste0("./Arq_Entrada", gsub("\\./grid/","/_",m), "_m_", p,".dat"),quote = F, 
                     sep = " ", width = c(9,7,7,rep(6,length(x)-3)), colnames=F)
    fname<-paste0("./Arq_Entrada", strrep(gsub("\\./grid","",m), 2), "_m_", p,".dat")
    gdata::write.fwf(as.data.frame(full_join(bacias,x, by = c("Codigo ANA", "Latitude", "Longitude")))[,1:ifelse(length(x)>=17,17,min(12,length(x)))],
                     ifelse(file.exists(fname),gsub("\\.dat","_.dat",fname),fname),quote = F, 
                     sep = " ", width = c(9,7,7,rep(6,ifelse(length(x)>=17,14,min(9,length(x)-3)))), colnames=F)
    t <- x %>% pivot_longer(cols = 4:last_col(), names_to = "date", values_to = paste(gsub("\\./grid/","",m),p)) %>% 
      mutate(date = as.Date(date)) %>% full_join(t,.,by = c("Codigo ANA", "Latitude", "Longitude","date"))
  }
}
dir.create(paste0("./smap"))
b<-unique(config$d)
for (e in b) {dir.create(paste0("./smap/",e,"/ARQ_ENTRADA"),F,T)}
maplist<-list.files("./Arq_Entrada/Observado", pattern = "*.txt")
x<-read.table(paste0("./Arq_Entrada/Observado/", maplist[1]), header = F, col.names = c("Codigo ANA","Latitude","Longitude", gsub("(.*[0-9]a)|(\\.dat)", "", d)), stringsAsFactors = F)[,-4]
for (d in maplist) {
  print(paste0("./Arq_Entrada/Observado",d))
  a<-read.table(paste0("./Arq_Entrada/Observado/", d), header = F, col.names = c("Codigo ANA", "Latitude","Longitude", gsub("(psat_)|(\\.txt)", "", d)), stringsAsFactors = F)
  x<- full_join(x,a,by = c("Codigo.ANA","Longitude", "Latitude"))
}
t<- x %>% pivot_longer(cols = 4:last_col(), names_to = "date", values_to = paste0("psat ",format(Sys.Date(), "%d%m%y"))) %>% 
  mutate(date = as.Date(date, "X%d%m%Y")) %>% arrange(Codigo.ANA, date) %>%
    full_join(t,.,by = c("Codigo ANA" = "Codigo.ANA", "Latitude", "Longitude","date"))
t<- t %>% pivot_longer(cols = 5:last_col(), names_to = c("model","run"), names_pattern = "(.*) (.*)", values_to = "prec", values_drop_na=T) %>%
  mutate(run = as.Date(run,"%d%m%y"))
#for (r in 1:length(config$'Latitude')) {
#  y<-filter(t,`Codigo ANA` == config$`Codigo ANA`[r] & model %in% c("funceme", "psat")) %>% 
#    arrange(date)%>% pivot_wider(names_from = model,values_from = prec) %>% 
#    mutate(prec = coalesce(psat, funceme)) %>% slice_tail(n=config$obs[r])
#  txt<-str_pad(paste(substr(paste0("00",config$`Codigo ANA`[r]), nchar(paste0("00",config$`Codigo ANA`[r]))-7, nchar(paste0("00",config$`Codigo ANA`[r]))),format(y$date,"%d/%m/%Y"),"1000",round(y$prec,1)),33,"right")
#  write.table(txt,paste0("./smap/",config$d[r],"/ARQ_ENTRADA/",
#                         substr(paste0("00",config$`Codigo ANA`[r]), nchar(paste0("00",config$`Codigo ANA`[r]))-7, nchar(paste0("00",config$`Codigo ANA`[r]))),"_c.txt"), dec=".",row.names = F,col.names = F,append = F,quote = F)
#}
save(t, file = "t.RData")


 local({ # Adiciona os postos sombra no psat mais recente (d-1)
  if(!file.exists(
       paste0(
           "./Arq_Entrada/Observado/psat_",
           format(day, "%d%m%Y"), ".txt")
   )) {
     load(Sys.glob(paste0(
         "H:/Middle - Preço/16_Chuva_Vazao/",
         format((Sys.Date()-1) %m+% months(0:2, F), "%Y_%m"),
         "/RV?/", format(Sys.Date()-1, "%y-%m-%d"), "/Mapas Acomph*/t.RData"))[1])
     t |> filter(model=="funceme") |> 
       mutate(funceme=round(prec,2)) |>
       select(-prec) -> funceme
     day <- funceme$date |> unique() |> as.Date()
     read_table(paste0(
       "H:/Middle - Preço/Acompanhamento de Precipitação/Observado_Satelite/",
       format(day, "%Y/%m/psat_%d%m%Y"), ".txt"),
       c("Codigo ANA", "Latitude", "Longitude", "prec")) |>
       full_join(funceme,by=c("Codigo ANA", "Latitude", "Longitude")) |>
       mutate(prec=coalesce(prec, funceme)) |>
       select("Codigo ANA", "Latitude", "Longitude", "prec") |>
       as.data.frame() |>
       gdata::write.fwf(
         paste0(
           "./Arq_Entrada/Observado/psat_",
           format(day, "%d%m%Y"), ".txt"),
         quote = F, sep = " ", width = c(9,7,7,6), colnames=F)
   }
 })


w<-getwd()
cat(w)
rmarkdown::find_pandoc(F, "C:/Program Files/RStudio/bin/pandoc")
rmarkdown::render("P:/Pedro/ENP/ENP.Rmd", output_dir=getwd(), params = list(dir=w))
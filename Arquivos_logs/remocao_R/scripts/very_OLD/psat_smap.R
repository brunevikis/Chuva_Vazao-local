library(readxl)
library(sgeostat)
suppressMessages(library(tidyverse))
config <- read_xlsx("P:/Pedro/remoção_R/Contornos/Configuracao.xlsx", sheet = "Plan1")
t <- as_tibble(config) %>% select(`Codigo ANA`,Latitude,Longitude) %>% add_column(date=as.Date(Sys.Date()-1, "%d/%m/%Y"))
dir.create(paste0("./smap"),F,T)
b<-unique(config$d)
for (e in b) {dir.create(paste0("./smap/",e,"/ARQ_ENTRADA"),F,T)}
maplist<-list.files("./Arq_Entrada/Observado", pattern = "*.txt")
x<-read.table(paste0("./Arq_Entrada/Observado/", maplist[1]), header = F, col.names = c("Codigo ANA","Latitude","Longitude", gsub("(.*[0-9]a)|(\\.dat)", "", maplist[1])), stringsAsFactors = F)[,-4]
for (d in maplist) {
  a<-read.table(paste0("./Arq_Entrada/Observado/", d), header = F, col.names = c("Codigo ANA", "Latitude","Longitude", gsub("(psat_)|(\\.txt)", "", d)), stringsAsFactors = F)
  x<- full_join(x,a,by = c("Codigo.ANA","Longitude", "Latitude"))
}
t<- x %>% pivot_longer(cols = 4:last_col(), names_to = "date", values_to = "prec") %>% 
  mutate(date = as.Date(date, "X%d%m%Y")) %>% arrange(Codigo.ANA, date) %>%
  full_join(t,.,by = c("Codigo ANA" = "Codigo.ANA", "Latitude", "Longitude","date"))
for (r in 1:length(config$'Latitude')) {
  y<-filter(t,`Codigo ANA` == config$`Codigo ANA`[r]) %>% 
    arrange(date)%>% slice_tail(n=config$obs[r])
  c<-ifelse(nchar(y$`Codigo ANA`[1])<=7 ,str_pad(paste0("0",y$`Codigo ANA`),8,"left"),y$`Codigo ANA`)
  txt<-str_pad(paste(c
                     ,format(y$date,"%d/%m/%Y"),"1000",round(y$prec,2)),34,"right")
  write.table(txt,paste0("./smap/",config$d[r],"/ARQ_ENTRADA/",c,"_c.txt"), dec=".",row.names = F,col.names = F,append = F,quote = F)
}

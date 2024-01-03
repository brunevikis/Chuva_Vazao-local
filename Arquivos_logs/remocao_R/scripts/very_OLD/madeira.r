maplist<-list.files("./Arq_Saida/", pattern = "*[0-9].dat", recursive = T)
dir.create("./madeira")
a<-paste0("./madeira/",unique(gsub("(/.*)|(PMEDIA.*)", "",maplist)))
for (q in a) {dir.create(q)}
for (dia in maplist) {
  t<-read.table(paste0("Arq_Saida/",dia), header = F, col.names = c("lon", "lat", "pr"))
  i<-t[which(t[,1]==-69.12),3]*0.87+t[which(t[,1]==-64.65),3]*0.13
  g<-rbind(t,c(-64.66, -09.26, round(i, 2)))
  gdata::write.fwf(g,paste0("./madeira/", dia),quote = F, sep = " ", width = c(6,6,6), colnames=F)
}

maplist<-list.files("./Arq_Saida/", pattern = "*[0-9].dat", recursive = T)
dir.create("./madeira")
a<-paste0("./madeira/",unique(gsub("(/.*)|(PMEDIA.*)", "",maplist)))
for (q in a) {dir.create(q)}
for (dia in maplist) {
  if(!grepl("Pesos",dia)){
  t<-read.table(paste0("Arq_Saida/",dia), header = F, col.names = c("lon", "lat", "pr"))
  i<-t[which(t[,1]==-69.12),3]*0.87+t[which(t[,1]==-64.65),3]*0.13
  i2<-t[which(t[,2]==-6.74),3]*0.037+t[which(t[,1]==-51.77),3]*0.264+t[which(t[,1]==-51.91),3]*0.699
  z<-nchar(readLines(paste0("Arq_Saida/",dia)))[1]-17
  g<-rbind(t,c(-64.66, -09.26, round(i, z)))
  if(length(i2)>0) {
    g<-rbind(g,c(-51.77, -03.13, round(i2, z)))
  }
  gdata::write.fwf(g,paste0("./madeira/", dia),quote = F, sep = " ", width = c(6,6,z+4), colnames=F)
}
}
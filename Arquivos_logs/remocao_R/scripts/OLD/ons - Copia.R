args = commandArgs(trailingOnly=TRUE)
com <- args[1]



getwd <- function ()  "."
ls <- function(name, pos = -1L, envir = as.environment(pos), all.names = FALSE, pattern, sorted = TRUE) 
  setdiff(base::ls(name, pos, globalenv(), all.names, pattern, sorted), "getwd")


source(paste0(".\\Codigos_R\\", com))
com = commandArgs(trailingOnly=TRUE)[1]

if(grepl("Roda_Conjunto",com)) save(res, file = "rem.RData")
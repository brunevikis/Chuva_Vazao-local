#Rscript.exe "H:\TI - Sistemas\UAT\ChuvaVazao\remocao_R\scripts\smap_R.r"
args = commandArgs(trailingOnly=TRUE)#caminho até pasta contendo as bacias com as pastas ARQ_ENTRADA e ARQ_SAIDA ex: "C:\Files\16_Chuva_Vazao\2024_04\RV0\24-03-25\CV_ACOMPH_FUNC_d-1_ECENS45\SMAP\NE"

library(smapOnsR)
library(ggplot2)
#library(doParallel)

if (length(args) != 0) 
{
	diretorio<-args[1]
	setwd(paste0(diretorio))
	w<-getwd()
	#print(paste0("Diretorio de trabalho: ",w))
	sink("GERAL.log", append=TRUE, split=TRUE)

	print(paste0("Diretorio de trabalho: ",w))
	print("Iniciando smap R......")
#models<-list.dirs(w,recursive = FALSE)

#contagem <-length(models)

#parallelly::availableCores()
#cl <- makePSOCKcluster(contagem)
#registerDoParallel(cl)

tictoc::tic()
#print(paste0("Quantidade de bacias: ",contagem))
#for (m in models) 
#{
	tryCatch(
	{
		#print(m)
		print(w)
		if(dir.exists(paste0(w,"/logs")))
		{
			unlink(paste0(w,"/logs"), recursive=TRUE)
		}
		dir.create(paste0(w,"/logs"))

		if(file.exists(paste0(w,"/logs/SMAP.log")))
		{
			file.remove(paste0(w,"/logs/SMAP.log")) 
		}
		sink(paste0(w,"/logs/SMAP.log"), append=TRUE, split=TRUE)
		print("Iniciando processo smap")
		executa_caso_oficial(w)
		
		print("Processo smap realizado com sucesso")
	},
	warning=function(war){
		print(war)
	},
	error=function(e){
		print(e)
		print("Bacia nao sera executada")
	},
	finally={
		print("Finalizando programa")
		sink()
	}
	) 
	
	#Sys.sleep(2)
	
#}
tictoc::toc(log = TRUE)
sink()

}
#Rscript.exe "H:\TI - Sistemas\UAT\ChuvaVazao\remocao_R\scripts\smap_R_Paralelo.r"
args = commandArgs(trailingOnly=TRUE)#caminho até pasta contendo as bacias ex: "H:\Middle - Preço\Acompanhamento de vazões\03_2024\Dados_de_Entrada_e_Saida_202403_RV4\Modelos_Chuva_Vazao\SMAP"

library(smapOnsR)
library(ggplot2)
library(doParallel)

if (length(args) != 0) 
{
	diretorio<-args[1]
	setwd(paste0(diretorio))
	w<-getwd()
	print(paste0("Diretorio de trabalho: ",w))
	#setwd("C:/Development/SMAP_R-TESTE/TESTE_RODADA/Modelos_Chuva_Vazao/SMAP_Paralelo")
	sink("GERAL.log", append=TRUE, split=TRUE)
	#w<-getwd()
	print(paste0("Diretorio de trabalho: ",w))
	print("Iniciando smap R PARALELO......")
	models<-list.dirs(w,recursive = FALSE)

	contagem <-length(models)

	#parallelly::availableCores()
	cl <- makePSOCKcluster(contagem)
	registerDoParallel(cl)

	tictoc::tic()
	print(paste0("Quantidade de bacias: ",contagem))
	foreach (m=models, .packages='smapOnsR') %dopar%
	{
		tryCatch(
		{
		
			print(paste0(m))
			if(dir.exists(paste0(m,"/logs")))
			{
				unlink(paste0(m,"/logs"), recursive=TRUE)
			}
			dir.create(paste0(m,"/logs"))
			if(file.exists(paste0(m,"/logs/SMAP.log")))
			{
				file.remove(paste0(m,"/logs/SMAP.log")) 
			}
			sink(paste0(m,"/logs/SMAP.log"), append=TRUE, split=TRUE)
			print("Iniciando processo smap")
			executa_caso_oficial(m)
			
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
		
	}
	tictoc::toc(log = TRUE)
	sink()
}
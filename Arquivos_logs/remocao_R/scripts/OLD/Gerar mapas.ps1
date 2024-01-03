$hoje=Get-date
$h="H:\Middle - Preço\Acompanhamento de Precipitação"
$p="$h\Previsao_Numerica\"
$l='.\Trabalho\Uruguai\Passo Sao Joao\'
$acomph="H:\Middle - Preço\Acompanhamento de vazões\ACOMPH\1_historico\"
$cv1=($hoje).AddDays([int][dayofweek]::Thursday - ($hoje).AddDays(2).DayOfWeek+2)
$cv2=($hoje).AddDays([int][dayofweek]::Thursday - ($hoje).AddDays(2).DayOfWeek+9)
@("error.log", "log.log")+($(ls CV*).Name)|% {if($_ -in (ls).Name) {rm -r $_}}


$a=$hoje
while("ACOMPH$($a.tostring("_dd-MM-yyyy"))" -notin (ls "$acomph\$($a.ToString("yyyy\\MM_yyyy"))").basename){$a=$a.AddDays(-1)}
 -1..-$(New-TimeSpan $a $($hoje)).days|%{($hoje).adddays($_)|%{if("{0:'funceme_p'ddMMyyaddMMyy}" -f $_ -notin ((ls "$p\Modelo_R\funceme\$($_.ToString("yyyyMM\\dd"))\*.dat").BaseName)){"erro funceme">>error.log;exit}}}
$a=($hoje).tostring("yyyyMM\\dd")

#psat
0..-7|% {($hoje).AddDays(-1).AddMonths($_).tostring("yyyy\\MM") |% {cp "$h\Observado_Satelite\$_\*.txt" .\Arq_Entrada\Observado\}}
$b=0
while(-join("psat_", $($hoje).AddDays($b).tostring("ddMMyyyy")) -notin  (ls .\Arq_Entrada\Observado\).basename) {$b=$b-1}
if ( $b -ne -1) {[IO.File]::WriteAllLines("$PWD/Arq_Entrada/data.txt", ($hoje).AddDays($b+1).tostring("dd/MM/yyyy")); $datatxt=$true}
$PSAT=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\ons.R 'convert_psat_remvies_V2.R'}

#pasta trabalho
Expand-Archive "$p$a\historico_previsao_precipitacao.zip" .\Trabalho\ -Force

#ultimo dia de atualização da previsão
$c=(@("GEFS", "ETA40")|%{(New-TimeSpan $([datetime]::Parse((cat -tail 1 "$l$_.csv").Split(";")[0])) $(($hoje).adddays(-1))).Days}|measure -Maximum).Maximum

#15º dia do GEFS NOAA (ou os 15, se o do ONS não estiver disponivel)
@(0..-$c|%{($hoje).AddDays($_).tostring("yyyyMM\\dd")})|% {cp "$p\Modelo_R\GEFS00\$_\*.dat" .\Arq_Entrada\Previsao\GEFS\ -Exclude (ls "$p$_\GEFS_*.dat").name}
#14 dias do GEFS ONS dos das que não estão na pasta trabalho
@(0..-$c|%{($hoje).AddDays($_).tostring("yyyyMM\\dd")})|% {cp "$p$_\GEFS_*.dat" .\Arq_Entrada\Previsao\GEFS\}
if($datatxt){Wait-Job $PSAT ;rm  .\Arq_Entrada\data.txt}
$GEFSt=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\ons.R 'prev_gfs_remvies_V2.R'}
$GEFS=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\convert_grade.R GEFS}

#10 dias do ETA40 ONS dos das que não estão na pasta trabalho
@(0..-$c|%{($hoje).AddDays($_).tostring("yyyyMM\\dd")})|% {cp "$p$_\ETA40_*.dat" .\Arq_Entrada\Previsao\ETA40\}
$ETA=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\ons.R 'prev_eta_remvies_V2.R'}

"PSAT" >> log.log; Receive-Job -wait -AutoRemoveJob $PSAT >> log.log
"GEFS" >> log.log; Receive-Job -wait -AutoRemoveJob $GEFSt >> log.log
"ETA40" >> log.log; Receive-Job -wait -AutoRemoveJob $ETA >> log.log

if([datetime]::Parse((cat -tail 1 "$l\observado.csv").Split(";")[0]) -ne ($hoje).adddays($b).Date) {"erro psat">>error.log;exit}
#0..4|%{if([datetime]::Parse((cat -tail 5 "$l\GEFS.csv")[$_].Split(";")[0]) -ne $hoje.AddDays($_-4).date) {"erro GEFS">>error.log;exit}}
if([datetime]::Parse((cat -tail 1 "$l\GEFS.csv").Split(";")[0]) -ne ($hoje).Date) {"erro GEFS">>error.log;exit}
if([datetime]::Parse((cat -tail 1 "$l\ETA40.csv").Split(";")[0]) -ne ($hoje).Date) {"erro ETA40">>error.log;exit}

$c=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\ons.R 'Roda_Conjunto_V2.1.R'}

#ECWMF
ls "$p\Modelo_R\ECMWF00\$a\*.dat" |% {cp $_ .\Arq_Entrada\Previsao\ECMWF\}
$ECMWF=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\convert_grade.R ECMWF ETA40}

#GFS
ls "$p\Modelo_R\GFS00\$a\*.dat" |% {cp $_ .\Arq_Entrada\Previsao\GFS\}
$GFS=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\convert_grade.R GFS GEFS}

#numero de dias decoridos do ultimo Acomph, preenche com funceme
#merge quando disponivel
$a=($hoje).tostring("yyyy\\MM_yyyy")
$b=0
while(-join("ACOMPH", $($hoje).AddDays($b).tostring("_dd-MM-yyyy")) -notin (ls "H:\Middle - Preço\Acompanhamento de vazões\ACOMPH\1_historico\$a").basename){
    $a=($hoje).AddDays($b).tostring("yyyyMM\\dd")
    if("{0:'merge_p'ddMMyyaddMMyy}" -f ($hoje).AddDays($b) -notin ((ls "$p\Modelo_R\Merge\$(($hoje).AddDays($b).ToString("yyyyMM\\dd"))\*.dat").BaseName))
    {ls "$p\Modelo_R\funceme\$a\*.dat" |% {cp $_ .\Arq_Entrada\Previsao\Funceme\}}
    else {ls "$p\Modelo_R\merge\$a\*.dat" |% {cp $_ .\Arq_Entrada\Previsao\Funceme\}}
    $b=$b-1
    $a=($hoje).AddDays($b).tostring("yyyy\\MM_yyyy")
}
@(0..$b|% {"p$(($hoje).AddDays($_).ToString("ddMMyy"))"})|% {$d=$_;ls .\Arq_Entrada\Previsao\Funceme\|ren -NewName {$_ -replace $d ,$($hoje).ToString("ddMMyy")}}
$Funceme=start-job {Set-Location $using:PWD;Rscript.exe P:\Pedro\remoção_R\scripts\convert_grade.R Funceme ETA40}

while($c.HasMoreData -and $c.State -eq "running") {Receive-Job $c -keep;Receive-Job $c >> log.log}
"GEFS" >> log.log; Receive-Job -wait -AutoRemoveJob $GEFS >> log.log
"ECMWF" >> log.log; Receive-Job -wait -AutoRemoveJob $ECMWF >> log.log
"GFS" >> log.log; Receive-Job -wait -AutoRemoveJob $GFS >> log.log
"Funceme" >> log.log; Receive-Job -wait -AutoRemoveJob $Funceme >> log.log

# Remoção de vies a partir dos dias das variaveis cv1 e cv2
Rscript.exe P:\Pedro\remoção_R\scripts\vies_ve.R -join($cv1.ToString("dd/MM/yy"), $cv2.ToString("dd/MM/yy"))

# Organização das rodadas para rvx+1
$t=($hoje).ToString("ddMMyy")
[Void](mkdir @('CV_EURO', 'CV_VIES_VE', 'CV_GFS'))
cp .\Arq_Saida\GEFS\p$t*  .\CV_VIES_VE -Exclude (ls .\Arq_Saida\vies.*\*.dat).name
cp .\Arq_Saida\ECMWF\p$t*  .\CV_EURO -Exclude (ls .\Arq_Saida\vies.*\*.dat).name
cp .\Arq_Saida\GFS\p$t*  .\CV_GFS -Exclude (ls .\Arq_Saida\vies.*\*.dat).name
ls CV_* |% {cp ".\Arq_Saida\vies.$($cv1.ToString("dd-MM"))\*" $_ }

# Remoção de vies a partir do dia atual, completando com MCP se necessário
[Void](mkdir CV_FUNC)
cp .\Arq_Saida\*.dat  .\CV_FUNC
$c=1..9|% {$cv1.AddDays($_).ToString("ddMMyy") |% {-join("PMEDIA_p", ($hoje).ToString("ddMMyy"), "a", $_, ".dat") } |% {if($_ -notin (ls ".\Arq_Saida\PMEDIA*.dat").Name){$_}}}
$c|%{cp "$p\modelo_R\MCP\prec_mct1318_$($_.substring(10,2)).dat" ".\CV_FUNC\$_"}

# Organização das rodadas para rvx+2
[Void](mkdir @('CV2_EURO', 'CV2_GEFS', 'CV2_GFS'))
cp .\Arq_Saida\ECMWF\p$t*  .\CV2_EURO -Exclude (ls .\Arq_Saida\vies.$($cv2.ToString("dd-MM"))\*.dat).name
cp .\Arq_Saida\GEFS\p$t*  .\CV2_EURO -Exclude @($(ls .\Arq_Saida\vies.$($cv2.ToString("dd-MM"))\*.dat), $(ls .\CV2_EURO\*.dat)).name
cp .\Arq_Saida\GEFS\p$t*  .\CV2_GEFS -Exclude (ls .\Arq_Saida\vies.$($cv2.ToString("dd-MM"))\*.dat).name
cp .\Arq_Saida\GFS\p$t*  .\CV2_GFS -Exclude (ls .\Arq_Saida\vies.$($cv2.ToString("dd-MM"))\*.dat).name
ls CV2* |% {cp ".\Arq_Saida\vies.$($cv2.ToString("dd-MM"))\*" $_ }

#Preencher com mct até completar a semana rvx+2
$c=@(1..12|% {($cv2).AddDays($_).ToString("ddMMyy")} |% {-join("p", ($hoje).ToString("ddMMyy"), "a", $_, ".dat") } |% {if($_ -notin (ls .\Arq_Saida\vies.*\*.dat).Name){$_}})
ls CV2*|%{$f=$_; $c|%{cp "$p\modelo_R\MCP\prec_mct1318_$($_.substring(10,2)).dat" "$f\$_"}}

# Renomear os arquivos para a data do ultimo acomph disponivel, se necessario
if($b -ne 0) {
    ls CV* |% {cp .\Arq_Saida\Funceme\* $_}
    $y=($hoje).AddDays($b).tostring("ddMMyy")
    ls CV*\*.dat | ren -NewName { $_.name -Replace "p$t","p$y"}
}

if("p$("{0:ddMMyy}" -f $hoje)a$("{0:ddMMyy}" -f $hoje.adddays(1))" -notin (ls .\Arq_Saida\ECMWF\*).basename) {rm -r @('CV_EURO', 'CV2_EURO')}
if("p$("{0:ddMMyy}" -f $hoje)a$("{0:ddMMyy}" -f $hoje.adddays(1))" -notin (ls .\Arq_Saida\GFS\*).basename) {rm -r CV2_GFS}

#Fim
"Finalizado" >> log.log

$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
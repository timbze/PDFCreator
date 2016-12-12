rem Script copies multiple files into the directory given as first parameter 
if %1 == "" goto end
if %2 == "" goto end 
set targetDir=%1
md %targetDir%
:copy
if %2 == "" goto end
copy %2 %targetDir%
shift
goto copy
:end
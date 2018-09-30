@echo off
set pythonDownload=https://www.python.org/ftp/python/2.7.13/python-2.7.13.amd64.msi
set googleCloudDownload=https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe
set ffmpegDownload=https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-4.0.2-win64-static.zip
set ffmpegInstallPath=C:\ffmpeg
set processStatus=0


:CheckPython
:: Check for Python Installation
python --version 2>NUL
if errorlevel 1 goto errorNoPython
echo Python is installed.

goto InstallModules

:errorNoPython
echo.
echo Error^: Python not installed
if %processStatus% == 0 (
	goto InstallPython
) else (
	echo If you have just installed Python, please log off and on again. Otherwise please install Python and then launch the script again.
	goto END
)

:InstallPython
set PythonPath=%cd%\Python.msi
:: download Python
echo Downloading python installer...
if not exist Python.msi 2>NUL (
    ::bitsadmin.exe /transfer "Downloading Python" %pythonDownload% %PythonPath%
	::powershell.exe -nologo -noprofile -command "& { import-module bitstransfer; Start-BitsTransfer '%pythonDownload%' '%PythonPath%'}"
	powershell.exe -nologo -noprofile -command "& { $client = new-object System.Net.WebClient; $client.DownloadFile('%pythonDownload%', '%PythonPath%' )}"
	::powershell.exe -nologo -noprofile -command "& { iwr -outf '%PythonPath%' '%pythonDownload%' }"
	::powershell.exe -nologo -noprofile -command "& { Invoke-WebRequest -Uri "%pythonDownload%" -OutFile "%PythonPath%" }"
)
echo Please install Python. Make sure to check "Add python.exe to Path" option in the installer.
Python.msi
set processStatus=1
goto CheckPython

:InstallModules
echo Installing python modules...

set PythonModuleName=upgrade pip
python -m pip install --upgrade pip 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
set PythonModuleName=google-cloud-texttospeech
python -m pip install google-cloud-texttospeech 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
set PythonModuleName=openpyxl
python -m pip install openpyxl 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
set PythonModuleName=pydub
python -m pip install pydub 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
set PythonModuleName=tqdm
python -m pip install tqdm 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
set PythonModuleName=colorama
python -m pip install colorama 2>&1>NUL
if errorlevel 1 goto ModuleInstallError
:: set PythonModuleName=wxPython
:: python -m pip install wxPython 2>&1>NUL
:: if errorlevel 1 goto ModuleInstallError
echo All modules are installed successfully.
set processStatus=2
goto CheckGoogleCloud

:ModuleInstallError
echo The Module "%PythonModuleName%" Installation was NOT successfull
goto END

:CheckGoogleCloud

:: echo Checking Google Cloud version...
echo Installing Google Cloud. Please log in and create or sign into a project.
::gcloud --version 1>&2>NUL
::if errorlevel 1 goto ErrorNoGoogleCloud
goto InstallGoogleCloud



:InstallGoogleCloud
set GoogleCloudPath=%cd%\GoogleCloud.exe
:: download Google Cloud
echo Downloading google cloud installer...
if not exist GoogleCloud.exe 2>NUL (
    ::bitsadmin.exe /transfer "Downloading Google Cloud" %googleCloudDownload% %GoogleCloudPath%
	::powershell.exe -nologo -noprofile -command "& { import-module bitstransfer; Start-BitsTransfer '%googleCloudDownload%' '%GoogleCloudPath%'}"
	powershell.exe -nologo -noprofile -command "& { $client = new-object System.Net.WebClient; $client.DownloadFile('%googleCloudDownload%', '%GoogleCloudPath%' )}"
	::powershell.exe -nologo -noprofile -command "& { iwr -outf '%GoogleCloudPath%' '%googleCloudDownload%' }"
	::powershell.exe -nologo -noprofile -command "& { Invoke-WebRequest -Uri "%googleCloudDownload%" -OutFile "%GoogleCloudPath%" }"
)	
echo Please follow the install procedure of the google cloud.
GoogleCloud.exe
set processStatus=3

goto DownloadFFMPEG

:DownloadFFMPEG
set FFMPEGPath=%cd%\FFMPEG.zip
:: download Google Cloud
echo Downloading FFmpeg...
if not exist FFMPEG.zip 2>NUL (
    ::bitsadmin.exe /transfer "Downloading Google Cloud" %ffmpegDownload% %FFMPEGPath%
	::powershell.exe -nologo -noprofile -command "& { import-module bitstransfer; Start-BitsTransfer '%ffmpegDownload%' '%FFMPEGPath%'}"
	powershell.exe -nologo -noprofile -command "& { $client = new-object System.Net.WebClient; $client.DownloadFile('%ffmpegDownload%', '%FFMPEGPath%' )}"
	::powershell.exe -nologo -noprofile -command "& { iwr -outf '%FFMPEGPath%' '%ffmpegDownload%' }"
	::powershell.exe -nologo -noprofile -command "& { Invoke-WebRequest -Uri "%ffmpegDownload%" -OutFile "%FFMPEGPath%" }"
)
if not exist "%ffmpegInstallPath%" (
	mkdir %ffmpegInstallPath%
	powershell.exe -nologo -noprofile -command "& { $shell = New-Object -COM Shell.Application; $target = $shell.NameSpace('%ffmpegInstallPath%'); $zip = $shell.NameSpace('%FFMPEGPath%'); $target.CopyHere($zip.Items(), 16); }"
	setx path "%PATH%;%ffmpegInstallPath%\ffmpeg-4.0.2-win64-static\bin"
)
goto SetSystemVariable

:SetSystemVariable
echo Now open "https://console.cloud.google.com/apis/credentials" and create a credentials file for your project. 
echo Download it and put it somewhere save on your harddrive.
echo Then drag and drop your service account key .JSON file here and press ENTER.
set /p file="JSON File: "
setx GOOGLE_APPLICATION_CREDENTIALS "%file%"
echo Now restart your system for the environment variable to take effect.
echo.
echo Intallation is done. You should be able to run the script now.
goto END

:END
pause
goto:eof
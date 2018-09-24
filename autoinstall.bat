@echo off
set pythonDownload=https://www.python.org/ftp/python/2.7.13/python-2.7.13.amd64.msi
set googleCloudDownload=https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe
set ffmpegDownload=https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-4.0.2-win64-static.zip
set processStatus=0
goto DownloadFFMPEG
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
	echo Please install Python and then launch the script again.
	goto END
)

:InstallPython
set PythonPath=%cd%\Python.msi
:: download Python
if not exist Python.msi (
    bitsadmin.exe /transfer "Downloading Python" %pythonDownload% %PythonPath%
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
if not exist GoogleCloud.exe (
    bitsadmin.exe /transfer "Downloading Google Cloud" %googleCloudDownload% %GoogleCloudPath%
)
echo Please follow the install procedure of the google cloud.
GoogleCloud.exe
set processStatus=3
goto DownloadFFMPEG

:DownloadFFMPEG
set FFMPEGPath=%cd%\FFMPEG.zip
:: download Google Cloud
if not exist FFMPEG.zip (
    bitsadmin.exe /transfer "Downloading Google Cloud" %ffmpegDownload% %FFMPEGPath%
)

::TODO: unzip copy to program files and add to system variables
::Call :UnZipFile %FFMPEGPath% %cd%
if not exist "C:\ffmpeg" (
mkdir C:\ffmpeg
powershell.exe -nologo -noprofile -command "& { $shell = New-Object -COM Shell.Application; $target = $shell.NameSpace('C:\ffmpeg'); $zip = $shell.NameSpace('%FFMPEGPath%'); $target.CopyHere($zip.Items(), 16); }"
setx path "%PATH%;C:\ffmpeg\ffmpeg-4.0.2-win64-static\bin"
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

::source from https://stackoverflow.com/questions/21704041/creating-batch-script-to-unzip-a-file-without-additional-zip-tools
:UnZipFile <newzipfile> <ExtractTo> 
set vbs="%temp%\_.vbs"
if exist %vbs% del /f /q %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo objShell.NameSpace(%1).CopyHere(FilesInZip)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
cscript //nologo %vbs%
if exist %vbs% del /f /q %vbs%

:: py -m pip install --upgrade pip
:: py -m pip install google-cloud-texttospeech
:: py -m pip install openpyxl
:: py -m pip install pydub
:: py -m pip install tqdm
:: py -m pip install colorama
:: py -m pip install wxPython
:: set GOOGLE_APPLICATION_CREDENTIALS = D:\Projekte\Drohne\OpenTx\speechGenerator\wavenet-api-test-77b66fbba700.json
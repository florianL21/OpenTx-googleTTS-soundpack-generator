@echo off
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
	echo Please install Python and then launch the script again.
	goto END
)

:InstallPython
set PythonPath=%cd%\Python.msi
:: download Python
if not exist Python.msi (
    bitsadmin.exe /transfer "Downloading Python" https://www.python.org/ftp/python/2.7.13/python-2.7.13.amd64.msi %PythonPath%
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
echo Installing Google Cloud...
::gcloud --version 1>&2>NUL
::if errorlevel 1 goto ErrorNoGoogleCloud
goto ErrorNoGoogleCloud
echo Google Cloud is installed.

goto SetSystemVariable

:ErrorNoGoogleCloud
echo.
::echo Error^: Google Cloud not installed
if %processStatus% == 2 (
	goto InstallGoogleCloud
) else (
	echo Google cloud should be installed and initialized at this point.
	::goto END
	goto SetSystemVariable
)

:InstallGoogleCloud
set GoogleCloudPath=%cd%\GoogleCloud.exe
:: download Google Cloud
if not exist GoogleCloud.exe (
    bitsadmin.exe /transfer "Downloading Google Cloud" https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe %GoogleCloudPath%
)
echo Please follow the install procedure of the google cloud.
GoogleCloud.exe
set processStatus=3
goto CheckGoogleCloud


:SetSystemVariable
echo Now open "https://console.cloud.google.com/apis/credentials" and create a credentials file for your project. 
echo Download it and put it somewhere save on your harddrive.
echo Then drag and drop your service account key .JSON file here and press ENTER.
set /p file="JSON File: "
setx GOOGLE_APPLICATION_CREDENTIALS "%file%"
echo Now log off and on again for the environment variable to take effect.
echo.
echo Intallation is done. You should be able to run the script now.
goto END

:END
pause
goto:eof

:: py -m pip install --upgrade pip
:: py -m pip install google-cloud-texttospeech
:: py -m pip install openpyxl
:: py -m pip install pydub
:: py -m pip install tqdm
:: py -m pip install colorama
:: py -m pip install wxPython
:: set GOOGLE_APPLICATION_CREDENTIALS = D:\Projekte\Drohne\OpenTx\speechGenerator\wavenet-api-test-77b66fbba700.json
# OpenTx GoogleTTS soundpack generator

## What is it?

This is an app written in C# using the new .NET Core 3.0 preview which uses the Google Cloud TTS service to generate nice sounding voice packs for OpenTX.

Tipp: The best sounding voices are the ones with WAVENET in their names.

The application features an easy to use GUI:

![](https://github.com/florianL21/OpenTx-googleTTS-soundpack-generator/blob/master/wiki-images/Screenshot-main-GUI.png)


## Setup

Setup of this application is really easy. You can go to the [releases](https://github.com/florianL21/OpenTx-googleTTS-soundpack-generator/releases) and download the newest eecutable for Windows from there.

Some steps to get you up and running:

### 1. Prerequisits:
Some kind of .NET core installaiton is required so that you can start the application.
You can simply try to download and run the exe file on your PC and see if it opens. 
If not donwload and install the .NET Core runtime from [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).
If you plan to build the application from source you will have to get the SDK instead.

### 2. Create a Google Cloud project and download the acces key Json file

The application will ask you for a json file at its first startup.
A guide on how to setup the Google Cloud project and generate the key can be found [here](https://cloud.google.com/text-to-speech/docs/quickstart-client-libraries).

## Building from source

Building the app is as simple as clonig this repo and then opening a cmd window inside the SoundpackGenerator Folder. 
Make sure you have the dotnet core sdk installed and the simply execute `dotnet run` to start the application.

## References

In this project I make use of the following 3rd party software:

- [Google cloud TTS](https://cloud.google.com/text-to-speech/)
- [Json parser from Newtonsoft](https://www.newtonsoft.com/json)
- [Microsoft .NET Core](https://dotnet.microsoft.com/download)

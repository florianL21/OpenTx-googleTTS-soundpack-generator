# OpenTx GoogleTTS soundpack generator

## What is it?

This is an app written in C# using the new .NET Core 3.0 preview which uses the Google Cloud TTS service to generate nice sounding voice packs for OpenTX.

Tipp: The best sounding voices are the ones with WAVENET in their names.

The application features an easy to use GUI:

![](https://github.com/florianL21/OpenTx-googleTTS-soundpack-generator/blob/master/wiki-images/Screenshot-main-GUI.png)


## Setup

The setup is really easy. The hardest part about it is to setup a Google cloud project and access key for your google account. The application will ask you for the json file (which you can learn how to generate [here](https://cloud.google.com/text-to-speech/docs/quickstart-client-libraries)) at startup. 


## References

In this project I make use of the following 3rd party software:

- [Google cloud TTS](https://cloud.google.com/text-to-speech/)
- [Json parser from Newtonsoft](https://www.newtonsoft.com/json)
- [Microsoft .NET Core](https://dotnet.microsoft.com/download)

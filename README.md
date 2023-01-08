# Space.NET
<img src="Space.NET Banner.png">Ai-Generated Image by DALL-E-2; Prompt: "Space.NET"

<a name="info"></a>
## Information
**Space.NET** is a platform independent `HTTP Server` that is running with `.NET` and the programming language `C#`. This project was inspired by the well-known HTTP Server `Apache`. Instead of programming scripts in `PHP` and the file extension `*.php`, you write scripts in `C#` with the file extension `\*.cshtml`.
  
Feel free to use Space.NET on your own Projects.
⚠️ If you find and <a href="https://github.com/TheBarnyOfBarnim/Space.NET/issues">issues</a> or you want to <a href="https://github.com/TheBarnyOfBarnim/Space.NET/issues">contribute</a>, feel free to do so!  

___

## Table of Contents
- [Information](#info)
- [Setting up the Webserver](#setup)
- [Executing the Webserver](#execute) 
- [Creating a Website](#createWebsite)
  - [API](Docs/API.md#api)
  - [Debugging a Script](#debugScript)
  
___

<a name="setup"></a>
## Setting up the Webserver
⚠️IMPORTANT⚠️: First, in order to run Space.NET, you need to install the latest version of <a href="https://dotnet.microsoft.com/en-us/download">.NET</a>


1. Download the latest <a href="https://github.com/TheBarnyOfBarnim/CSharp-WebServer/releases">Release</a> (`Space-NET.zip`)  
Extract the archive into an empty folder
2. Download the latest <a href="https://github.com/TheBarnyOfBarnim/CSharp-WebServer/releases">Template</a> (`Template_Root_Folder.zip`)  
Extract the Template into another empty folder [`ServerRoot`]
4. Execute Space.NET with **elevated rights**⚠️  
  On `Windows` => Simply execute the Space-NET.exe  
  On `Linux` => execute the Space-NET.dll with an Terminal-Application via the following command: "`dotnet Space-NET.dll`"
5. The application will now prompt you to input some settings:  
  5.1. The Root folder of your WebServer [`ServerRoot`].  
The [`ServerRoot`] Folder is **not** the folder where the Application is in!
  5.2. A comma seperated list of prefixes. Prefixes are basically URL's that the WebServer has to register on the OS.  
        Set the Prefix(es) to your Domain Name or IP-Address
6. After inputting the settings, the WebServer will create the Folder [`ServerRoot`] with following Sub-Folders:
  - `Logs`: Log files of the Core and Incomming Web-Requests
  - `ErrorDocs`: <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">HTTP Error<a> Documents the server can encounter
  - `DocumentRoot`: The most top Folder a Internet-User can access
7. The WebServer will now start automaticly!

___

<a name="execute"></a>
## Executing the Webserver
Execute Space.NET with **elevated rights**⚠️  
  On `Windows` => Simply execute the Space-NET.exe  
  On `Linux` => execute the Space-NET.dll with an Terminal-Application via the following command: "`sudo dotnet Space-NET.dll`"

___

<a name="createWebsite"></a>
## Creating a Website
Creating a Website using the template is very easy.  
In order to create a dynamic Website you use `C#` Scripts in plain `HTML` files (like `PHP`).  
Look into the Documentation of the <a href="Docs/API.md#api">API</a>, to understand how to write Scripts ith the `.cshtml` file extension.

___

<a name="debugScript"></a>
## Debugging a Script
Debugging a Script is very easy:  
If the script could not compile or it get's a runtime Exception, just look at the Page in your browser!  
⚠️ An option to disable the display of the Exception for the User will be soon inplemented in the Config.json.

<a name="api"></a>
# Space.NET - API
___

## Table of Contents
- [The `.cshtml` file extension](#cshtmlEXT)
- [Methods](#methods)
  - [`print` method](#printMethod)
  - [`include` method](#includeMethod)
- [Objects](#Objects)
  - [Server](#Server)
  - [Request](#Request)
  - [Session](#Session)
  - [GET](#GET)
  - [POST](#POST)
  - [Response](#Response)
- [Static Classes](#StaticClasses)
___

<a name="cshtmlEXT"></a>
### The `.cshtml` file extension
⚠️The `.cshtml` file format is a custom format made by me. It's ⚠️**not**⚠️ the same as a <a href="https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-6.0">CSHTML - ASP.NET Razor Webpage</a>  
  
The `.cshtml` file format is like the `.php` file format. You can imagine the file as a normal plain-text `.html` file. Only Addition is a new custom tag, the `<csharp>` or `<cs>` Tag! You can use this Tag at ANY postion of the file  
(this is because the Script-Interpreter only reads those two Tags and not any other `HTML` Tag!)  
Adding a `\` before the opening or closing Tag makes it so, that the Interpreter ignores this tag.
  
Between the opening an closing Tag, you can freely write any `C#` `.NET` code.  
(In order to understand, why you can only use top-level statements read <a href="404">this article<a>.)  
Every time a user requests a file, it creates a Checksum of the real *(filesystem)* file and compares it with the Checksum of the already compiled *(memory)* file. If one Checksum is different from the other, only then it recompiles it. This has the effect of **WAAAAY** faster loading times! *(You can call me optimization-man)*
  
**An example:**
```cshtml
<html>
  <head>
  </head>
  <body>
    <button onclick='someFunction();'><cs>print("click me!");</cs></button>
  </body>
</html>
```
  
<a name="methods"></a>
### Methods
  
<a name="printMethod"></a>
- ### **`print` method**:  
  As you can see in the `<cs>` block, there is a new custom method (`print("click me!");`) that you can call right away without needing a class or an instance of an object!  
  
  Properties:
  - **Name:** `print`
  - **arguments:** `object obj`
  - **return value:** `void`

---
<a name="includeMethod"></a>
- ### **`include` method**:
  The `include` method is a special method where you can include text/code from another File or Script. It is the equivalent to the `include` method of `PHP`!  
  
  Properties:
  - **Name:** `include`
  - **arguments:** `string FileToInclude`
  - **return value:** `void`
  
⚠️⚠️⚠️  
This method is a **pseudo** method, wich means it will not be compiled by the real compiler. It is only parsed by the Script-Parser because it is impossible to compile code at runtime this fast every time!  
Because of this you can only use strings in quotes!  
There are only four options for this argument:
  - The absolute path of an File (e.g. `"C:\Folder\file.txt"`)
  - The relative path of an File relative to the [`DocumentRoot`] (e.g. `"./file.txt"` translates to e.g. `C:\ServerRoot\file.txt`)
  - The relative path of an File relative to the current File (e.g. `"__FILE__/file.txt"`)
  - The relative path of an File relative to the ErrorDocs Folder (e.g. `"__ErrorDocs__/500.cshtml"`)  
  
⚠️⚠️⚠️  

**An example:**
- Before interpreter:
    - MainFile.cshtml:
      ```cshtml
            <html>
              <head>
              </head>
              <body>
                <cs>
                  include("__FILE__/LoremIpsum.cshtml")
                </cs>
              </body>
            </html>
      ```

    - LoremIpsum.cshtml:
      ```cshtml
            <cs>
              print(DateTime.Now.ToString())
            </cs>
            <p>
              Lorem ipsum dolor sit amet, consetetur sadipscing elitr,<br>
              sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam
            </p>
      ```
      
  ---
  
- After interpreter:
    - MainFile.cshtml:
      ```diff
            <html>
              <head>
              </head>
              <body>
                <cs>
      +          </csharp>
      +            <cs>
      +              print(DateTime.Now.ToString())
      +            </cs>
      +            <p>
      +              Lorem ipsum dolor sit amet, consetetur sadipscing elitr,<br>
      +              sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam
      +            </p>
      +          <csharp>
                </cs>
              </body>
            </html>
      ```
      Conclusion/Result:  
      The `include("__FILE__/LoremIpsum.cshtml")` gets replaced with the following:  
        `</csharp>` + `the File's Content` + `<csharp>`

___

<a name="Objects"></a>
## Objects
You can access the following Objects directly via their Names:
  - <a href="#Server">`Server`</a>
  - <a href="#Request">`Request`</a>
  - <a href="#Session">`Session`</a>
  - <a href="#GET">`GET`</a>
  - <a href="#POST">`POST`</a>
  - <a href="#Response">`Response`</a>
  
<a name="Server"></a>
- ### `Server`
  The Server object contains contains the readonly Settings and Paths.
  
  **Properties:**
  - `Settings` `Settings` **:** The WebServer Settings
  - `string` `ServerRoot` **:** The Root Folder of the Website
  - `string` `LogFilesFolder` **:** The Logs Folder with (Core & Requests)
  - `string` `ErrorDocsFolder` **:** The Error Documents Folder
  - `string` `DocumentRoot` **:**  The most top Folder a Internet-User can access
  
   **Methods:**
  - `string` `ToString()` **:** Returns the preceding Properties (except `Settings`).
  
  ---
  
<a name="Request"></a>
- ### `Request`
  The Request object contains the properties from a User-Request.
  
  **Properties:**
  - `string` `Method` **:** Can be 'GET' or 'POST'
  - `string` `URL` **:** The URL that a User requested.
  - `string` `URLFolder` **:** The Path of the Folder of that `URL`
  - `string` `Path` **:** The Path of the Folder of that `URL` (relative to `Server.DocumentRoot`)
  - `string` `WebFolder` **:** Same as `Path` but '\' is replaced with '/'
  - `string` `RealPath` **:** Absolute Path of `Path` of the URL (Path of File in FileSystem)
  - `string` `RealPathFolder` **:** he Path of the Folder of `RealPath`
  - `string` `UserAgent` **:** The UserAgent the User is using  
  (e.g. `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36`)
  - `string` `TraceID` **:** The Unique ID of that Request
  
   **Methods:**
  - `string` `ToString()` **:** Returns the preceding Properties.
   
  ---
  
<a name="Session"></a>
- ### `Session`
  The Session object contains the properties from the current Session (not Request!)
  
  **Properties:**
  - `string` `SessionID` **:** ID of the Session
  - `dynamic` `Session[string Name]` **:** Gets or Sets a custom & dynamic Properties of the Session
  
   **Methods:**
  - `void` `Destroy()` **:** Makes the current Session invalid.
  - `void` `Destroy()` **:** Starts a new Session.
  - `void` `ToString()` **:** Returns the preceding and dynamic Properties.
  ---
  
<a name="GET"></a>
- ### `GET`
  The GET object contains the data that was sent by the user via the `GET` `Method`. Null if User sent data with `POST`
  
  **Properties:**
  - `dynamic` `GET[string Name]` **:** Gets the data or `FormFile` a User has sent.
  
   **Methods:**
  - `void` `ToString()` **:** Returns the preceding and dynamic Properties.
  ---
  
<a name="POST"></a>
- ### `POST`
  The POST object contains the data that was sent by the user via the `POST` `Method`. Null if User sent data with `GET`
  
  **Properties:**
  - `dynamic` `POST[string Name]` **:** Gets the data or `FormFile` a User has sent.
  
   **Methods:**
  - `void` `ToString()` **:** Returns the preceding and dynamic Properties.
  ---
  
<a name="Response"></a>
- ### `Response`
  The Response object contains the properties of the Response. You can only set every Property once!
  
  **Properties:**
  - `Encoding` `ContentEncoding` **:** Encoding of the Content for the User
  - `int` `StatusCode` **:** The HTTP Status Code that is being sent
  - `int` `StatusCode` **:** The HTTP Status Code that is being sent
  - `List<string>` `Headers` **:** The HTTP Headers that will be sent
  - `long` `ContentLength` **:** The Length of the Content (in Bytes) that is being sent (Use only if you want to send e.g. Files)
  
   **Methods:**
  - `void` `Write(string str)` **:** Writes a string to the Output-Stream (for greater efficiency use the <a href="#printMethod">`print(object obj)`</a> method)
  - `void` `Write(byte[] bytes)` **:** Writes Bytes to the Output-Stream
  - `void` `ToString()` **:** Returns the preceding and dynamic Properties.

___

<a name="StaticClasses"></a>
## Static Classes
You can access the following Classes directly via their Names:
  - <a href="#Hashing">`Hashing`</a>
  - <a href="#MIME">`MIME`</a>  
  
<a name="Hashing"></a>
- ### `Hashing`
  The Hashing Class is a Utility Class for Creating Hashes from different Sources
  
  **Properties:**
  - `SHA256` `SHA256` **:** SHA256 Class
  - `SHA1` `SHA1` **:** SHA256 Class
  - `MD5` `MD5` **:** SHA256 Class
  
   **Methods:**
  - `string` `GetHash(HashAlgorithm hashAlgorithm, string input)` **:** Creates a Hash with a specific `HashAlgorithm` from a string `Input`
  
  ---
  
<a name="MIME"></a>
- ### `MIME`
  The MIME Class is a Utility Class for getting MIME-Types from extensions
  
  **Properties:**
  - none
  
   **Methods:**
  - `string` `GetMimeType(string extension)` **:** Gets the MIME-Type for a specific File-Extension (e.g. '.html' => 'text/html')
  
  ---
  

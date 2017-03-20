CefSharp Nuget Package

Background:
  CefSharp is a .Net wrapping library for CEF (Chromium Embedded Framework) https://bitbucket.org/chromiumembedded/cef
  CEF is a C/C++ library that allows developers to embed the HTML content rendering strengths of Google's Chrome open source WebKit engine (Chromium).

Post Installation:
  - Read the release notes for your version https://github.com/cefsharp/CefSharp/releases (Any known issues will be listed here)
  - Set either `x86` or x64` as your target architecture. `AnyCpu` support was added in `51`, please see the release notes for details as additional steps are required to make it work.
  - After installing the `Nuget` package we recommend closing Visual Studio completely and then reopening (This ensures your references show up and you have full intellisense).
  - Check your output `\bin` directory to make sure the appropriate references have been copied.
  - Build fails even though packages are installed. Short term rebuild again and everything should be find. Long term we recommend reading http://www.xavierdecoster.com/migrate-away-from-msbuild-based-nuget-package-restore
  - If you are using `WPF`, please note there is no Designer support at this point in time, see the FAQ for details.
  
Deployment:
  - Make sure `Visual C++ 2013` is installed (`x86` or x64` depending on your build) or you package the runtime dlls with your application, see the FAQ for details.
  
What's New:
  See https://github.com/cefsharp/CefSharp/wiki/ChangeLog
  IMPORTANT NOTE - .NET Framework 4.5.2 is now required.  
  IMPORTANT NOTE - Chromium has removed support for Windows XP/2003 and Windows Vista/Server 2008 (non R2).
  
  The Microsoft .NET Framework 4.5.2 Developer Pack for Visual Studio 2012 and Visual Studio 2013 is available here: 
  https://www.microsoft.com/en-gb/download/details.aspx?id=42637

Basic Troubleshooting:
  - Minimum of .Net 4.5.2
  - Make sure `VC++ 2013 Redist` is installed (either `x86` or `x64` depending on your application)
  - Please ensure your binaries directory contains these required dependencies:
    * libcef.dll (CEF code)
    * icudtl.dat (Unicode Support data)
    * CefSharp.Core.dll, CefSharp.dll, 
      CefSharp.BrowserSubprocess.exe, CefSharp.BrowserSubProcess.Core.dll
        - These are required CefSharp binaries that are the common core logic binaries of CefSharp.
    * One of the following UI presentation approaches:
        * CefSharp.WinForms.dll
        * CefSharp.Wpf.dll
        * CefSharp.OffScreen.dll
  - Additional optional CEF files are described at: https://github.com/cefsharp/cef-binary/blob/master/README.txt#L82
    NOTE: CefSharp does not currently support CEF sandboxing.

For further help please read the following content:
  - CefSharp Tutorials https://github.com/cefsharp/CefSharp.Tutorial
  - CefSharp GitHub https://github.com/cefsharp/CefSharp
  - CefSharp's Wiki on github (https://github.com/cefsharp/CefSharp/wiki)
  - Minimal Example Projects showing the browser in action (https://github.com/cefsharp/CefSharp.MinimalExample)
  - FAQ: https://github.com/cefsharp/CefSharp/wiki/Frequently-asked-questions
  - Troubleshooting guide (https://github.com/cefsharp/CefSharp/wiki/Trouble-Shooting)
  - Google Groups (https://groups.google.com/forum/#!forum/cefsharp) - Historic reference only
  - CefSharp vs Cef (https://github.com/cefsharp/CefSharp/blob/master/CONTRIBUTING.md#cefsharp-vs-cef)
  - Join the active community and ask your question on Gitter Chat (https://gitter.im/cefsharp/CefSharp)
  - If you have a reproducible bug then please open an issue on `GitHub`

Please consider giving back, it's only with your help will this project to continue.

Regards,
CefSharp Team

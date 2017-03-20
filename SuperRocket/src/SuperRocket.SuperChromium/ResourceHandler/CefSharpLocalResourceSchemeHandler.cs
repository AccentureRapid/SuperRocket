// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace SuperRocket.SuperChromium.ResourceHandler
{
    internal class CefSharpLocalResourceSchemeHandler : IResourceHandler
    {
        private static readonly IDictionary<string, string> ResourceDictionary;

        private string mimeType;
        private MemoryStream stream;
        
        static CefSharpLocalResourceSchemeHandler()
        {
            
            ResourceDictionary = new Dictionary<string, string>();

            try
            {
                var rootPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(@"Resource\Modules\{0}", "Example");//The path for the home page of the module
                DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
                DirectoryInfo[] directories = directoryInfo.GetDirectories();

                FileInfo[] rootFiles = directoryInfo.GetFiles();

                foreach (var file in rootFiles)
                {
                    var filePath = file.FullName;
                    StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("utf-8"));
                    var result = reader.ReadToEnd().ToString();
                    var key = @"/Modules/Example/" + file.Name;
                    ResourceDictionary.Add(key, result);
                    reader.Close();
                }

                foreach (DirectoryInfo subfolder in directories)
                {
                    var currentPath = rootPath + @"\" + subfolder.Name;
                    FileInfo[] fileInfos = subfolder.GetFiles();
                    foreach (FileInfo file in fileInfos)
                    {
                        var filePath = file.FullName;
                        StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("utf-8"));
                        var result = reader.ReadToEnd().ToString();
                        var key = @"/Modules/Example/" + subfolder.Name + @"/" + file.Name;
                        if (!key.Contains(".jpg"))
                        {
                            ResourceDictionary.Add(key, result);
                        }
                        
                        reader.Close();
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
        {
            // The 'host' portion is entirely ignored by this scheme handler.
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;

            string resource;
            var fileExtension = Path.GetExtension(fileName);
            if (ResourceDictionary.TryGetValue(fileName, out resource) && !string.IsNullOrEmpty(resource))
            {
                Task.Run(() =>
                {
                    using (callback)
                    {
                        var bytes = Encoding.UTF8.GetBytes(resource);
                        stream = new MemoryStream(bytes);

                        //var fileExtension = Path.GetExtension(fileName);
                        mimeType = CefSharp.ResourceHandler.GetMimeType(fileExtension);

                        callback.Continue();
                    }
                });

                return true;
            }
            

            if (fileExtension == ".jpg")
            {
                Task.Run(() =>
                {
                    using (callback)
                    {
                        var path = AppDomain.CurrentDomain.BaseDirectory + string.Format(@"Resource\{0}", fileName);//The path for the home page of the module
                        FileStream fs = File.OpenRead(path);
                        int filelength = 0;
                        filelength = (int)fs.Length;
                        Byte[] bytes = new Byte[filelength];
                        fs.Read(bytes, 0, filelength);

                        stream = new MemoryStream(bytes);

                        mimeType = CefSharp.ResourceHandler.GetMimeType(fileExtension);
                        callback.Continue();
                    }
                });
                return true;
            }
            else
            {
                callback.Dispose();
            }

            return false;
        }
        

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = stream == null ? 0 : stream.Length;
            redirectUrl = null;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusText = "OK";
            response.MimeType = mimeType;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            //Dispose the callback as it's an unmanaged resource, we don't need it in this case
            callback.Dispose();

            if(stream == null)
            {
                bytesRead = 0;
                return false;
            }

            //Data out represents an underlying buffer (typically 32kb in size).
            var buffer = new byte[dataOut.Length];
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            
            dataOut.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }

        bool IResourceHandler.CanGetCookie(CefSharp.Cookie cookie)
        {
            return true;
        }

        bool IResourceHandler.CanSetCookie(CefSharp.Cookie cookie)
        {
            return true;
        }

        void IResourceHandler.Cancel()
        {
            
        }

        void IDisposable.Dispose()
        {
            
        }

       
    }
}

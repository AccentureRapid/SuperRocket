using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amba.AmazonS3.Services;
using Orchard.Commands;

namespace Amba.AmazonS3.Commands
{
    public class AmazonS3Commands : DefaultOrchardCommandHandler
    {
        private readonly IAmazonS3StorageProvider _amazonS3StorageProvider;

        public AmazonS3Commands(IAmazonS3StorageProvider amazonS3StorageProvider)
        {
            _amazonS3StorageProvider = amazonS3StorageProvider;
        }

        [CommandHelp("")]
        [CommandName("amazons3 test")]
        public void Test()
        {
            _amazonS3StorageProvider.Test();
            return;
            Console.WriteLine("TestFolder:");
            _amazonS3StorageProvider.ListObjects("test/").ForEach(x => Console.WriteLine(x.Key));

            Console.WriteLine("FileExists-false:");
            Console.WriteLine(
                _amazonS3StorageProvider.FileExists(@"/test_$folder$")
                );
            Console.WriteLine("FileExists-true:");
            Console.WriteLine(
                _amazonS3StorageProvider.FileExists("/upload-test/c6432ea20ca0430797c58a184bdad99f.JPG")
                ); 

            Console.WriteLine("GetPublicUrl:");
            Console.WriteLine(
                _amazonS3StorageProvider.GetPublicUrl("/upload-test/c6432ea20ca0430797c58a184bdad99f.JPG")
                );


            Console.WriteLine("ListFiles('/test'):");
            var files = _amazonS3StorageProvider.ListFiles("/test");
            foreach (var file in files)
            {
                Console.WriteLine(file.GetPath());
            }

            Console.WriteLine("ListFolders('/test'):");
            var folders = _amazonS3StorageProvider.ListFolders("/test");
            foreach (var file in folders)
            {
                Console.WriteLine(file.GetPath());
            }
            Console.WriteLine();
            Console.WriteLine("List('/'):");
            var xfiles = _amazonS3StorageProvider.ListFiles("/");
            foreach (var file in xfiles)
            {
                Console.WriteLine(file.GetPath());
            }

            Console.WriteLine("ListFolders('/'):");
            var xfolders = _amazonS3StorageProvider.ListFolders("/");
            foreach (var file in xfolders)
            {
                Console.WriteLine(file.GetPath());
            }
            /**/
        }



    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Laughlin.ErrorLog.Models;
using Laughlin.ErrorLog.ViewModels;

namespace Laughlin.ErrorLog.Controllers
{
    public class AdminController : Controller {
        
        private string _basePath = string.Empty;
        
        public AdminController() {
            
        }

        public ActionResult Index(string SelectedLogFileName)
        {
            _basePath = Server.MapPath(@"~/App_Data/Logs");
            var model = new IndexViewModel();
            
            var logFileName = string.IsNullOrEmpty(SelectedLogFileName)
                                 ? string.Format("orchard-error-{0}.{1}.{2}.log",
                                                 DateTime.Now.Year,
                                                 DateTime.Now.Month.ToString().PadLeft(2, '0'),
                                                 DateTime.Now.Day.ToString().PadLeft(2, '0'))
                                 : SelectedLogFileName;

            var fullLogFilePath = _basePath + "/" + logFileName;
            model.LogDate = logFileName;

            return View(GetModel(fullLogFilePath, model));
        }

        private IndexViewModel GetModel(string fileName, IndexViewModel model)
        {
            var errorLogsOnly = (from p in Directory.GetFiles(_basePath, "*.log")
                                 where p.IndexOf("orchard-error-") >= 0
                                 select p.Substring(p.LastIndexOf('\\') + 1))
                              .OrderByDescending(x => x)
                              .ToList();

            var dates = errorLogsOnly.Select(logFile => new SelectListItem
            {
                Text = logFile,
                Value = logFile
            }).ToList();

            model.Dates = dates;
            
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    model.LogText = reader.ReadToEnd();
                    var regex = new Regex(@"([0-9]{4}-[0-9]{2}-[0-9]{2}\s[0-9]{2}:[0-9]{2}:[0-9]{2})");

                    var matches = regex.Split(model.LogText).Where(s => s != String.Empty).ToArray();

                    for (var i = 0; i < matches.Count(); i++)
                    {
                        model.LogItems.Insert(0, new LogItem
                        {
                            Date = matches[i],
                            Text = matches[i + 1]
                        });

                        i++;
                    }
                }
            }

            return model;
        }
    }
}
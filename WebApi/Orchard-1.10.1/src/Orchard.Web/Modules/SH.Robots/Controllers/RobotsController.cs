﻿using System.Text;
using System.Web.Mvc;
using SH.Robots.Services;
using Orchard.Caching;

namespace SH.Robots.Controllers {
	public class RobotsController : Controller {
		private const string ContentType = "text/plain";

		private readonly IRobotsService _robotsService;
		private readonly ICacheManager _cacheManager;
		private readonly ISignals _signals;

		public RobotsController(IRobotsService robotsService, ICacheManager cacheManager, ISignals signals) {
			_robotsService = robotsService;
			_cacheManager = cacheManager;
			_signals = signals;
		}

		public ContentResult Index() {
			var content = _cacheManager.Get("RobotsFile.Settings",
							  ctx => {
								  ctx.Monitor(_signals.When("RobotsFile.SettingsChanged"));
								  var robotsFile = _robotsService.Get();
								  return robotsFile.FileContent;
							  });
			if (string.IsNullOrWhiteSpace(content)) {
				content = _robotsService.Get().FileContent;
			}
			return new ContentResult() {
				ContentType = ContentType,
				ContentEncoding = Encoding.UTF8,
				Content = content
			};
		}
	}
}
using System;
using System.Collections.Generic;
using Orchard;
using SH.Robots.Models;

namespace SH.Robots.Services {
	public interface IRobotsService : IDependency {
		RobotsFileRecord Get();
		Tuple<bool, IEnumerable<string>> Save(string text);
	}
}
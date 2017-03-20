﻿using System.Collections.Generic;

namespace Nwazet.Commerce.Models.Reporting {
    public class ReportDataPoint {
        public string Description { get; set; }
        public string Url { get; set; }
        public double Value { get; set; }
        public string ValueString { get; set; }
        public IDictionary<string, ReportDataPoint> Series { get; set; }
    }
}

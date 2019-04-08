﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TeknikServis.Entity.Models
{
    public class ResponseData
    {
        public string message { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
        public List<BosTeknisyenViewModel> Teknisyenler { get; set; }
        public DateTime responseTime { get; set; } = DateTime.Now;
        public string responseTimeU { get; set; } = $"{DateTime.Now:O}";
    
    }
}

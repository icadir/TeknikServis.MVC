using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeknikServis.Entity.Enums;

namespace TeknikServis.Entity.ViewModels.ApiViewModel.Operator
{
    public class OperatorIndexViewModel
    {
    
        public int ArızaId { get; set; }
        public List<string> Resim { get; set; }
        public string MusteriId { get; set; }
        public string Sistemdekiteknisyen { get; set; }
        public DateTime  ArizaCreatedDate { get; set; }
        public string Adres { get; set; }
        public string telno { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Models
{
    public class ExternalEmployeeData
    {
        public int ColaboradorID { get; set; }
        public string HRStatus { get; set; }
        public decimal AnnualSalary { get; set; } // Dato sensible que viene de fuera
        public string LastLogin { get; set; }
    }
}
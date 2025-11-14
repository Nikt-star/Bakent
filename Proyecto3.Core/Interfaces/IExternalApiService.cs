using Proyecto3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IExternalApiService
{
    Task<ExternalEmployeeData> GetEmployeeDetailsAsync(int colaboradorId);
}
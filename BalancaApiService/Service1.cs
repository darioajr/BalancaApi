using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace BalancaApiService
{
    public partial class Service1 : ServiceBase
    {
        private IDisposable _webapp = null;

        private string _baseAddress = System.Configuration.ConfigurationManager.AppSettings["baseAddress"].ToString();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Iniciar();
        }

        public void Iniciar()
        {
            _webapp = WebApp.Start<Startup>(url: _baseAddress);
        }

        protected override void OnStop()
        {
            _webapp?.Dispose();
            base.OnStop();
        }
    }
}

using MVCPartnersMatcher.Conroller;
using MVCPartnersMatcher.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MVCPartnersMatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Application app;
        private void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(e);
            MyController c = new MyController();
            MyModel m = new MyModel(c);
            MainWindow mw = new MainWindow();
            mw.C = c;
            c.SetModel(m);
            c.SetView(mw);
            if (Application.Current == null)
            {
                app = new App()
                {
                    ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
            }
            else
                app = Application.Current;
            mw.start();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using cineteca.ServiceReference; // includo servizi di referenza

namespace cineteca
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*Utente test = new Utente();
            test.email = "user@test.com";
            Application.Run(new Home(test));*/
            Application.Run(new LoginForm());
        }
    }
}

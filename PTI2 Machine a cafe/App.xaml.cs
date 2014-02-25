using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PTI2_Machine_à_café
{
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			this.RootVisual = new MainPage();
		}

		private void Application_Exit(object sender, EventArgs e)
		{
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// Si l'application s'exécute à l'extérieur du débogueur, signalez l'exception à l'aide
			// du mécanisme d'exception du navigateur. Dans IE, cela provoque l'affichage d'une icône 
			// d'alerte jaune dans la barre d'état. Firefox affiche une erreur de script.
			if (!System.Diagnostics.Debugger.IsAttached)
			{

				// REMARQUE : ceci permet de poursuivre l'exécution d'une application lorsqu'une exception s'est produite
				// mais n'a pas été traitée 
				// Pour les applications de production, ce traitement des erreurs doit être remplacé par un mécanisme qui 
				// signalera l'erreur au site web et arrêtera l'application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}
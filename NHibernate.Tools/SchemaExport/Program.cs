using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace NHibernate.Tools.SchemaExport
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 4)
			{
				Console.WriteLine("usage: SchemaExport <workingDirectory> <configFile> <outputCreateScript> <outputDropScript>");
				return -1;
			}

			string workingDirectory = args[0];
			string configFile = args[1];
			string outputCreateScript = args[2];
			string outputDropScript = args[3];

			try
			{
				var runnerAppDomain =
					AppDomain.CreateDomain(
						"SchemaExportRunner", AppDomain.CurrentDomain.Evidence,
						new AppDomainSetup
						{
							ApplicationBase = Path.GetPathRoot(workingDirectory),
							ConfigurationFile = configFile,
							PrivateBinPath = workingDirectory + ";" + Path.GetDirectoryName(typeof(Program).Assembly.Location) 
						});
				
				var schemaExportRunner = (SchemaExportRunner)runnerAppDomain.CreateInstanceAndUnwrap(SchemaExportRunner.AssemblyName, SchemaExportRunner.TypeName);

				return schemaExportRunner.Run(outputCreateScript, outputDropScript);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error generating create and drop script: \n" + ex);
				return Marshal.GetHRForException(ex);
			}
		}

	}
}

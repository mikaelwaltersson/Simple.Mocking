using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHibernate.Tools.SchemaExport
{
	public class SchemaExportRunner : MarshalByRefObject
	{
		public static readonly string AssemblyName = typeof(SchemaExportRunner).Assembly.GetName().Name;
		public static readonly string TypeName = typeof(SchemaExportRunner).FullName;

		public int Run(string outputCreateScript, string outputDropScript)
		{
			var configuration = new Cfg.Configuration();

			configuration.Configure();

			var schemaExport = new Tool.hbm2ddl.SchemaExport(configuration);

			CreateScriptFile(schemaExport, outputCreateScript, false);
			CreateScriptFile(schemaExport, outputDropScript, true);

			return 0;
		}


		static void CreateScriptFile(Tool.hbm2ddl.SchemaExport schemaExport, string scriptFile, bool dropOnly)
		{
			using (var stream = File.Open(scriptFile, FileMode.OpenOrCreate, FileAccess.Write))
			{
				stream.SetLength(0);

				using (var writer = new StreamWriter(stream, Encoding.UTF8))
				{
					schemaExport.Execute(false, false, dropOnly, null, writer);
				}
			}
		}

	}
}
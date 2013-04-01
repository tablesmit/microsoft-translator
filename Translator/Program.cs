using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Threading;
using System.Diagnostics;

namespace Yam.Microsoft.Translator
{
	class Program
	{
		static bool ArgsCheck(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Microsoft.Translator");
				Console.WriteLine("https://datamarket.azure.com/dataset/bing/microsofttranslator");
				Console.WriteLine("-------------------------------------------------------------------");
				Console.WriteLine("Param1: Language code from");
				Console.WriteLine("Param2: Language code to");
				Console.WriteLine("    http://msdn.microsoft.com/en-us/library/hh456380.aspx");
				Console.WriteLine("Param3: Corpus file path");
				return false;
			}
			if (Properties.Settings.Default.ClientID.Length == 0 || Properties.Settings.Default.ClientSecret.Length == 0)
			{
				Console.WriteLine("Please set ClientID and ClientSecret in config file.");
				Console.WriteLine("You can register the service from Microsoft Translator in the Windows Azure Marketplace");
				Console.WriteLine("    http://blogs.msdn.com/b/translation/p/gettingstarted1.aspx");
				return false;
			}
			return true;
		}

		static void Main(string[] args)
		{
			try
			{
				if (ArgsCheck(args) == false)
				{
					Console.ReadKey();
					return;
				}

				var languageFrom = args[0];
				var languageTo = args[1];
				var corpusFrom = args[2];

				var corpusTo = Path.ChangeExtension(corpusFrom, languageTo + Path.GetExtension(corpusFrom));
				using (var translator = new Translator(Properties.Settings.Default.ClientID, Properties.Settings.Default.ClientSecret, languageFrom, languageTo))
				using (var dest = File.CreateText(corpusTo))
				{
					var lines = File.ReadAllLines(corpusFrom);
					int count = 0;
					foreach (var s in translator.Translate(lines, 50))
					{
						count++;
						ConsoleFunctions.Code.RenderConsoleProgress((int)(count * 100.0f / lines.Count()), '=', ConsoleColor.Green, string.Format("{0}/{1}", count, lines.Count()));
						Trace.WriteLine(string.Format("{0}\t{1}", s.Item1, s.Item2));
						dest.WriteLine(s.Item2);
						dest.Flush();
					}
				}
				Console.WriteLine();
				Console.WriteLine("OK: " + corpusTo);
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Error:");
				Console.WriteLine(ex);
				Console.ReadKey();
			}
		}
	}	
}

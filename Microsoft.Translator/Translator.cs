using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Yam.Microsoft.Translator.TranslatorService;

namespace Yam.Microsoft.Translator
{
	/// <summary>
	/// http://msdn.microsoft.com/en-us/library/ff512438.aspx
	/// </summary>
	public class Translator : IDisposable
	{
		private readonly AdmAuthentication authentication;
		public LanguageServiceClient LanguageService { get; private set; }

		public string LanguageCodeFrom { get; private set; }
		public string LanguageCodeTo { get; private set; }

		public Translator(string clientId, string clientSecret, string languageCodeFrom, string languageCodeTo)
		{
			this.authentication = new AdmAuthentication(clientId, clientSecret);
			this.LanguageService = new LanguageServiceClient();
			this.LanguageCodeFrom = languageCodeFrom;
			this.LanguageCodeTo = languageCodeTo;
		}

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ff512437.aspx
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string Translate(string str)
		{
			return InvokeWithAuthentication(() => LanguageService.Translate(string.Empty, str, LanguageCodeFrom, LanguageCodeTo, "text/plain", "general"));
		}

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ff512438.aspx
		/// </summary>
		/// <param name="strs"></param>
		/// <returns></returns>
		public string[] TranslateArray(string[] strs)
		{
			var options = new TranslateOptions() { ContentType = "text/plain", Category = "general", };
			return InvokeWithAuthentication(() => (
				from r in LanguageService.TranslateArray(string.Empty, strs, LanguageCodeFrom, LanguageCodeTo, options)
				select r.TranslatedText).ToArray());
		}

		public IEnumerable<Tuple<string, string>> Translate(IEnumerable<string> strs, int batchSize = 20)
		{
			var gs =
				from i in strs.Select((s, i) => Tuple.Create(i, s))
				group i by i.Item1 / batchSize;
			return (
				from g in gs
				let f = (from i in g select i.Item2).ToArray()
				let t = this.TranslateArray(f)
				select f.Zip(t, (ff, tt) => Tuple.Create(ff, tt)))
				.SelectMany(s => s);
		}

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ff512432.aspx
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Tuple<string, string>> GetLanguagesForTranslate()
		{
			var codes = InvokeWithAuthentication(() => LanguageService.GetLanguagesForTranslate(string.Empty));
			var names = InvokeWithAuthentication(() => LanguageService.GetLanguageNames(string.Empty, "en", codes));
			return codes.Zip(names,(c, n) => Tuple.Create(c, n));
		}

		public T InvokeWithAuthentication<T>(Func<T> action)
		{
			using (var scope = new OperationContextScope(LanguageService.InnerChannel))
			{
				var property = new HttpRequestMessageProperty();
				property.Method = "POST";
				property.Headers.Add("Authorization", "Bearer " + authentication.GetAccessToken().access_token);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = property;
				return action();
			}
		}

		#region IDisposable 成员

		public void Dispose()
		{
			(LanguageService as IDisposable).Dispose();
		}

		#endregion
	}
}

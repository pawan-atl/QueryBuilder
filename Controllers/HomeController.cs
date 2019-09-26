using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LinqQueryBuilder.Helpers;
using LinqQueryBuilder.Sample;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LinqQueryBuilder.Controllers
{
	public class HomeController : Controller
	{
		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new CustomJsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding
			};
		}

		//Return the default definitions for the class, and the list of People
		public ActionResult Index()
		{
			var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

			var definitions = typeof(PersonRecord).GetDefaultColumnDefinitionsForType(false);
			var people = PersonBuilder.GetPeople();

			// Augment the definitions to show advanced scenarios not handled by GetDefaultColumnDefinitionsForType !!!

			// Example 1: Tweak FirstName to make it a select input, populated by the possible values from our dataset 
			var firstName = definitions.First(p => p.Field.ToLower() == "firstname");
			firstName.Values = people.Select(p => p.FirstName).Distinct().ToList();
			firstName.Input = "select";

			// Example 2: Tweak birthday to use the jQuery-UI datepicker plugin instead of just a text input (could be any plugin really)
			var birthday = definitions.First(p => p.Field.ToLower() == "birthday");
			birthday.Plugin = "datepicker";

			ViewBag.FilterDefinition = JsonConvert.SerializeObject(definitions, jsonSerializerSettings);
			ViewBag.Model = people;

			return View();
		}

		//Take the POST FilterRule, build query, and return results
		[HttpPost]
		public JsonResult Index(QueryBuilderFilterRule obj)
		{
			var people = PersonBuilder.GetPeople().BuildQuery(obj).ToList();
			return Json(people);
		}
	}

	public class CustomJsonResult : JsonResult
	{
		private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			HttpResponseBase response = context.HttpContext.Response;

			if (!String.IsNullOrEmpty(ContentType))
			{
				response.ContentType = ContentType;
			}
			else
			{
				response.ContentType = "application/json";
			}
			if (ContentEncoding != null)
			{
				response.ContentEncoding = ContentEncoding;
			}
			if (Data != null)
			{
				// Using Json.NET serializer
				var isoConvert = new IsoDateTimeConverter();
				isoConvert.DateTimeFormat = _dateFormat;
				response.Write(JsonConvert.SerializeObject(Data, isoConvert));
			}
		}
	}
}
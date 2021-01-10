using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

public class DotNetScraper
{
	public static void Main()
	{
		string url = "https://softuni.bg/trainings/3210/csharp-advanced-january-2021/";
		

		using (var client = new WebClient())
		{
			string html = client.DownloadString(url);

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var xpath = "//li[@class='lecture col-md-6 col-sm-12 visible-lg visible-md']";
			var xpathA = "//li[@class='lecture col-md-6 col-sm-12 visible-lg visible-md']/a";
			var xpathLectureDetailsContainer = "//li[@class='col-md-12 lecture-details-container']";

			var topicNodes = doc.DocumentNode.SelectNodes(xpath);
			var liNodes = doc.DocumentNode.SelectNodes(xpathA);
			var lectureDetailsNode = doc.DocumentNode.SelectNodes(xpathLectureDetailsContainer);
			
			var topics = new Dictionary<string, Dictionary<int, string>>();
			
			//Do all topics and their Id
			for (int i = 0; i < topicNodes.Count; i++)
			{
				var topic = ParseTopicUsingXpath(topicNodes[i]);
				
				//Get the lecture id
				var lectureAttribute = liNodes[i].Attributes.FirstOrDefault(x => x.Name == "data-id");
				var idOfTopic = int.Parse(lectureAttribute.Value.Trim());
				var topicData = new Dictionary<int, string>();
				
				//Get the lecture details id
				var detailsAttribute = lectureDetailsNode[i].ChildNodes.FindFirst("div").Id;
				var idOfDetails = int.Parse(detailsAttribute.Substring(7).Trim());
				
				//Get date and time by id
				if (idOfDetails == idOfTopic)
				{
					var details = doc.DocumentNode.SelectNodes($"//*[@id='lesson-{idOfDetails}']");
					// var detailsChild = details.ChildNodes.FindFirst("div");
					//TODO: CANNOT ACCESS THE CHILDREN OF DETAILS CONTAINER!!!
				}
				
				topicData.Add(idOfTopic, string.Empty);
				topics.Add(topic.Title.ToUpper(), topicData);
			}

			Console.WriteLine("Parsed topics are:\n");

			var str = DataToString(topics);
			Console.WriteLine(str);
			
			System.IO.File.WriteAllText(@"C:\Users\karlo\Desktop\Topics.txt", str);
			System.IO.File.WriteAllText(@"C:\Users\karlo\Desktop\htmlOfTopics.html", html);
		}		
	}
	
	
	public static Topic ParseTopicUsingXpath(HtmlNode htmlNode)
	{
		var title = htmlNode.InnerText.CleanParsedText();
		//TODO: Finish date and time data
		//var date = htmlNode.SelectSingleNode(".//span[@class='runtime']").InnerText.CleanParsedText();
		//var time = htmlNode.SelectSingleNode(".//span[@class='genre']").InnerText.CleanParsedText();
		
		// //for dvdReleaseDate IMDB uses two different formtats so need to check if first one exists first
		// var releaseDateNode =  htmlNode.SelectSingleNode(".//div[@class='list-description']/p/b");
		// string releaseDate;
		//
		// if (releaseDateNode != null)
		// {
		// 	releaseDate = releaseDateNode.InnerText.CleanParsedText();
		// }
		// else
		// {
		// 	releaseDate =  htmlNode.SelectSingleNode(".//div[@class='list-description']/p").InnerText.CleanParsedText();
		// }
		//
		// //releaseDate looks like this at this time 'VOD Release Date: Tuesday, December 29', so need to get only the Decemeber 29 part.
		// releaseDate = releaseDate.Substring(releaseDate.LastIndexOf(", ") + 2);
												  
		return new Topic
		{
			Title = title
			//Date = date,
			//Time = time
		};
	}

	public static string DataToString(Dictionary<string,Dictionary<int, string>> topics)
	{
		StringBuilder sb = new StringBuilder();

		foreach (var topic in topics)
		{
			sb.Append(topic.Key).Append(" ");

			foreach (var data in topic.Value)
			{
				sb.Append(data.Key).Append(" ");
				sb.Append(data.Value);
			}

			sb.AppendLine();
		}

		return sb.ToString();
	}


	public class Topic
	{
		public string Title {get ; set;}
		// public string Date {get; set;}
		// public string Time {get; set;}
	}	
}

public static class StringExtensions
{
	static Regex TextCleanRegex = new Regex("\\s{2,}");
	
	//This method removes all the junk which may be in html values
	public static string CleanParsedText(this string text)
	{
		if (String.IsNullOrWhiteSpace(text))
			return null;

		var cleanText = HttpUtility.HtmlDecode(text);
		cleanText = TextCleanRegex.Replace(cleanText, " ");
		cleanText = cleanText.Trim();
		return cleanText;
	}
}
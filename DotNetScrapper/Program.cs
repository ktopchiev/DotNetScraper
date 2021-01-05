using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

public class Program
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
			var topicNodes = doc.DocumentNode.SelectNodes(xpath);
			
			var topics = new List<string>();
			
			//Do all topics
			for (int i = 0; i < topicNodes.Count; i++)
			{
				var topic = ParseTopicUsingXpath(topicNodes[i]);
				//topic can also be parsed by traversing node tree like node.ChildNodes[0]....
				
				topics.Add(topic.Title.ToUpper());
			}
			
			Console.WriteLine("Parsed topics are:\n");
			Console.WriteLine(string.Join("\n", topics));
			
			System.IO.File.WriteAllText(@"C:\Users\karlo\Desktop\Topics.txt", string.Join("\n", topics));
		}		
	}
	
	
	public static Topic ParseTopicUsingXpath(HtmlNode htmlNode)
	{
		var title = htmlNode.InnerText.CleanParsedText();
		var neshtosi = htmlNode.Attributes;
		//TODO:
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
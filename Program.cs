using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HtmlAgilityPack;
using CsvHelper;

namespace WebScrap
{
  class Program
  {
    class Product
    {
      public string? Url { get; set; }
      public string? Image { get; set; }
      public string? Name { get; set; }
      public string? Price { get; set; }

      public Product(string? url, string? image, string? name, string? price)
      {
        this.Url = url;
        this.Image = image;
        this.Name = name;
        this.Price = price;
      }

      // public override string ToString()
      // {
      //   return $"[/n/tUrl: {this.Url}/n/tImage Url:{this.Image}/n/tName: {this.Name}/n/tPrice: {this.Price}";
      // }
    }

    static void Main(string[] args)
    {
      try
      {
        HtmlWeb TheWeb = new HtmlWeb();
        HtmlDocument Document = TheWeb.Load("https://www.scrapingcourse.com/ecommerce/");

        var products = new List<Product>();
        var productEle = Document.QuerySelectorAll("li.product");

        foreach (var p in productEle)
        {
          try
          {
            var url = HtmlEntity.DeEntitize(p.SelectSingleNode("a")?.Attributes["href"].Value);
            var img = HtmlEntity.DeEntitize(p.SelectSingleNode(".//img")?.Attributes["src"].Value);
            var name = HtmlEntity.DeEntitize(p.SelectSingleNode("//h2")?.InnerText);
            var price = HtmlEntity.DeEntitize(p.SelectSingleNode("//span")?.InnerText);

            var product = new Product(url, img, name, price);
            products.Add(product);
          }
          catch (System.Exception err)
          {
            Console.WriteLine($"Error on loop: {err}");
          }
        }

        string outputPath = "product.csv";
        using (var writer = new StreamWriter(outputPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
          csv.WriteRecords(products);
        }

        Console.WriteLine($"Products Found, Writen in {outputPath}");
      }
      catch (System.Exception err)
      {
        Console.WriteLine($"Error on main: {err}");
      }
    }
  }
}

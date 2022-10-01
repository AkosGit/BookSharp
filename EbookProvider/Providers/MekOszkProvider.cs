using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EbookProvider.Providers
{
    public class MekOszkProvider : Provider
    {

        public MekOszkProvider()
        {
            Languages = new List<Filters.Languages>() { Filters.Languages.Hungarian };
            Topics = new List<Filters.Topics>() { Filters.Topics.None };
            providerID = 2;
        }

        List<Book> ScrapeBooks(string searchString)
        {
            List<Book> books= new List<Book>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(searchString);
            int hits = int.Parse(document.DocumentNode.SelectSingleNode("//font[@color='#990033']").InnerHtml);
            if(hits > 0)
            {
                int lastpage = hits / 250 * 100;// 250 books in one page jumps 100 per page
                if (lastpage < 1) { lastpage = 1; }
                for (int i = 0; i <= lastpage; i = i + 100)
                {
                    if (i == 0)//first page is 1 not 0
                    {
                        document = web.Load(searchString + $"&offset={1}");
                    }
                    else
                    {
                        document = web.Load(searchString + $"&offset={i}");
                    }

                    HtmlNodeCollection _booklinks = document.DocumentNode.SelectNodes("//td[contains(@class, 'bor')]/a");
                    if (_booklinks == null)// due to error pages in the end resolting in error
                    {
                        i = lastpage;
                    }
                    else
                    {
                        HtmlNode[] bookLinks = _booklinks.ToArray();
                        foreach (HtmlNode hn in bookLinks)
                        {
                            string href = hn.GetAttributeValue("href", string.Empty);
                            HtmlDocument book = web.Load(href);
                            HtmlNode authorNode = book.DocumentNode.SelectSingleNode("//a[@class='szerzo']");
                            string author;
                            if (authorNode != null)
                            {
                                author = authorNode.InnerHtml;
                            }
                            else
                            {
                                author = "Unknown";
                            }
                            string title = book.DocumentNode.SelectSingleNode("//a[@class='focim']").InnerText;
                            //TODO: add more format options
                            books.Add(new Book() { Title = title, Author = author, bookID = href, CoverURL = href + "/borito.jpg", providerID = providerID });
                        }
                    }
                }
            }
            return books;
        }
        internal override List<Book> SearchWithAuthor(List<Filters.Languages> languages, List<Filters.Topics> topics, string author)
        {
            string searchString = "https://mek.oszk.hu/kereses.mhtml?dc_creator=" + author + "&dc_subject=&sort=rk_szerzo%2Crk_uniform&id=&Image3.x=0&Image3.y=0";
            return ScrapeBooks(searchString);
        }

        internal override List<Book> SearchWithAuthorAndTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title, string author)
        {
            string searchString = "https://mek.oszk.hu/kereses.mhtml?dc_creator=" + author + "&dc_title=" + title + "&dc_subject=&sort=rk_szerzo%2Crk_uniform&id=&Image3.x=0&Image3.y=0";
            return ScrapeBooks(searchString);
        }

        internal override List<Book> SearchWithTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title)
        {
            string searchString = "https://mek.oszk.hu/kereses.mhtml?dc_title="+title+"&dc_subject=&sort=rk_szerzo%2Crk_uniform&id=&Image3.x=0&Image3.y=0";
            return ScrapeBooks(searchString);
        }

        internal override string DownloadBook(Book book)
        {
            List<string> urls = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument site = web.Load(book.bookID);
            string bookid = book.bookID.Split("/").Last();
            var sources = site.DocumentNode.SelectNodes($"//a[contains(@href, \"{bookid}\")]");
            foreach (var url in sources)
            {
                urls.Add(url.GetAttributeValue("href", string.Empty));
            }
            return urls[0];

        }
    }
}

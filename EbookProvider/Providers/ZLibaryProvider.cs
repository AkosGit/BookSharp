using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EbookProvider.Providers
{
    public class ZLibaryProvider : Provider
    {
        RequestsSession session;
        string baseUrl;
        public ZLibaryProvider()
        {
            baseUrl = "https://b-ok.xyz/";
            Languages = new List<Filters.Languages>() { Filters.Languages.English, Filters.Languages.Hungarian };
            Topics = new List<Filters.Topics>() { Filters.Topics.None };
            session = new RequestsSession(baseUrl, 100);
            providerID = 4;
        }
        List<string> TranslateLang(List<Filters.Languages> languages)
        {
            List<string> langs = new List<string>();
            foreach (Filters.Languages lang in languages)
            {
                switch (lang)
                {
                    case Filters.Languages.Hungarian:
                        langs.Add("hungarian");
                        break;
                    case Filters.Languages.English:
                        langs.Add("english");
                        break;
                    default:
                        break;
                }
            }
            return langs;
        }
        List<Book> ScrapeBooks(List<string> languages, string searchstring)
        {
            List<Book> books = new List<Book>();
            foreach (string item in languages)
            {
                searchstring = searchstring + item + "&";
            }
            int page = 1;
            bool notFound=false;
            while (!notFound)
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(session.Get(searchstring + "&page=" + page.ToString()).GetAwaiter().GetResult());
                var _booklinks = document.DocumentNode.SelectNodes("//h3[@itemprop='name']/a");
                if(_booklinks == null) { notFound = true; }
                else
                {
                    HtmlNode[] bookLinks = _booklinks.ToArray();
                    foreach (HtmlNode hn in bookLinks)
                    {
                        string href = hn.GetAttributeValue("href", string.Empty);
                        HtmlDocument book = new HtmlDocument();
                        book.LoadHtml(session.Get(href).GetAwaiter().GetResult());
                        HtmlNode[] authors;
                        authors = book.DocumentNode.SelectNodes("//a[@itemprop='author']").ToArray();
                        string author = "";
                        foreach (HtmlNode node in authors)
                        {
                            author += $"{node.InnerHtml} ";
                        }
                        string title = book.DocumentNode.SelectSingleNode("//h1[@itemprop='name']").InnerHtml;
                        string coverURL = "";
                        if (book.DocumentNode.SelectSingleNode("//div[contains(@class, 'z-book-cover')]/img") != null)
                        {
                            coverURL = book.DocumentNode.SelectSingleNode("//div[contains(@class, 'z-book-cover')]/img").GetAttributeValue("src", string.Empty);
                        }
                        else
                        {
                            coverURL = "http://gjss.org/sites/all/themes/gjss2014/images/no-cover.png";
                        }
                        books.Add(new Book() { Title = title, Author = author, bookID = href, CoverURL = coverURL, providerID=providerID });
                    }
                    page++;
                }
            }
            return books;
        }
        internal override List<Book> SearchWithAuthor(List<Filters.Languages> languages, List<Filters.Topics> topics, string author)
        {
            return SearchWithTitle(languages, topics, author);
        }

        internal override List<Book> SearchWithAuthorAndTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title, string author)
        {
            return SearchWithTitle(languages, topics, title);
        }

        internal override List<Book> SearchWithTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title)
        {
            title = title.Replace(" ", "%20");
            return ScrapeBooks(TranslateLang(languages),$"s/{title}/?e=1&");
        }

        internal override string DownloadBook(Book book)
        {
            HtmlDocument site = new HtmlDocument();
            string resp = session.Get(book.bookID).GetAwaiter().GetResult();
            site.LoadHtml(resp);
            string durl = (site.DocumentNode.SelectSingleNode("//a[contains(@class, 'dlButton')]").GetAttributeValue("href", string.Empty)).Replace("//","/");
            //string dpath = $"C:\\Users\\Akos\\Downloads\\{durl.Split("/").Last()}";
            //session.DownloadFile(durl, dpath).GetAwaiter().GetResult();
            return durl;
        }
    }
}

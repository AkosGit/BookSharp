using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookProvider.Providers
{
    public class NcoreProvider : ProviderWithAuth
    {
        RequestsSession client = new RequestsSession("https://ncore.pro");
        public NcoreProvider(string username,string password): base(username,password)
        {
            Languages = new List<Filters.Languages>() { Filters.Languages.English, Filters.Languages.Hungarian };
            Topics = new List<Filters.Topics>() { Filters.Topics.None };
            providerID = 3;

        }
        List<string> TranslateLang(List<Filters.Languages> languages)
        {
            List<string> langs = new List<string>();
            foreach (Filters.Languages lang in languages)
            {
                switch (lang)
                {
                    case Filters.Languages.Hungarian:
                        langs.Add("ebook_hun");
                        break;
                    case Filters.Languages.English:
                        langs.Add("ebook");
                        break;
                    default:
                        break;
                }
            }
            return langs;
        }
        void login()
        {
            Dictionary<string, string> loginform = new Dictionary<string, string>();
            loginform.Add("nev", Username);
            loginform.Add("pass", Password);
            client.PostMultipartForm("/login.php", loginform).GetAwaiter().GetResult();
        }
        internal override string DownloadBook(Book book)
        {
            login();
            string resp = client.Get(book.bookID).GetAwaiter().GetResult();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(resp);
            string downurl = document.DocumentNode.SelectSingleNode("//div[@class='download']/a").GetAttributeValue("href", string.Empty);
            return downurl;
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
            login();
            List<Book> books = new List<Book>();
            List<string> langs = TranslateLang(languages);
            Dictionary<string, string> searchform = new Dictionary<string, string>();
            foreach (string s in langs)
            {
                searchform.Add("kivalasztott_tipus", s);
            }
            searchform.Add("nyit_konyv_resz", "true");
            searchform.Add("mire", title);
            searchform.Add("miben", "name");
            searchform.Add("tipus", "kivalasztottak_kozott");
            string resp=client.PostMultipartForm("/torrents.php", searchform).GetAwaiter().GetResult(); 
            HtmlDocument document = new HtmlDocument();
            document = new HtmlDocument();
            document.LoadHtml(resp);
            if (document.DocumentNode.SelectSingleNode("//div[@class='lista_mini_error']") == null)
            {
                Regex rg = new Regex(@"torrents\.php\?oldal=(\d*)");
                int lastpage = 1;
                var Reglastpage = rg.Matches(resp);
                if (Reglastpage.Count != 0)
                {
                    lastpage = Reglastpage.Select(x => int.Parse(x.Groups[1].Value)).Max();
                }
                for (int i = 1; i <= lastpage; i++)
                {
                    resp = client.PostMultipartForm($"/torrents.php?oldal={i}", searchform).GetAwaiter().GetResult();
                    document = new HtmlDocument();
                    document.LoadHtml(resp);
                    HtmlWeb web = new HtmlWeb();
                    HtmlNode[] bookLinks = document.DocumentNode.SelectNodes("//div[contains(@class, 'torrent_txt')]/a").ToArray();
                    foreach (HtmlNode hn in bookLinks)
                    {
                        string href = hn.GetAttributeValue("href", string.Empty);
                        document = new HtmlDocument();
                        document.LoadHtml(client.Get(href).GetAwaiter().GetResult());
                        string booktitle = document.DocumentNode.SelectSingleNode("//div[@class='torrent_reszletek_cim']").InnerHtml;
                        string coverURL = "http://gjss.org/sites/all/themes/gjss2014/images/no-cover.png";
                        if (document.DocumentNode.SelectSingleNode("//td[@class='inforbar_img']/img") != null)
                        {
                            coverURL = document.DocumentNode.SelectSingleNode("//td[@class='inforbar_img']/img").GetAttributeValue("src", string.Empty);
                        }
                        else
                        {
                            if (document.DocumentNode.SelectSingleNode("//a[@class='fancy_groups']") != null)
                            {
                                coverURL = document.DocumentNode.SelectSingleNode("//a[@class='fancy_groups']").GetAttributeValue("href", string.Empty);
                            }
                        }
                        books.Add(new Book() { Title = booktitle, Author = "Unknown", bookID = href, CoverURL = coverURL, providerID = providerID });
                    }
                }
            }

            return books;
        }
    }
}

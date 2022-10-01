
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace EbookProvider.Providers
{
    public class GutenbergProvider : Provider
    {
        RequestsSession session;
        List<string> TranslateLang(List<Filters.Languages> languages) {
            List<string> langs = new List<string>();
            foreach (Filters.Languages lang in languages)
            {
                switch (lang)
                {
                    case Filters.Languages.Hungarian:
                        langs.Add("hu");
                        break;
                    case Filters.Languages.English:
                        langs.Add("en");
                        break;
                    default:
                        break;
                }
            }
            return langs;
        }
        bool Match(string search,string title)//search algortimh needs tuning
        {
            var titleWords=title.Split(' ').Select(x => x.ToUpper());
            var searchWords = search.Split(' ').Select(x => x.ToUpper());
            foreach (string t in titleWords)
            {
                foreach (string s in searchWords)
                {
                    if (t.Contains(s))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        List<Book> ScrapeBooks(List<string> langs,string _author="",string _title="")
        {
            List<Book> books = new List<Book>();
            using (FileStream fs = File.Create("g.csv"))
            {  
                Byte[] title = new UTF8Encoding(true).GetBytes(session.Get("cache/epub/feeds/pg_catalog.csv").GetAwaiter().GetResult());
                fs.Write(title, 0, title.Length);
            }
            using (TextFieldParser csvParser = new TextFieldParser("g.csv"))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string id = fields[0];
                    string type = fields[1];
                    string issued = fields[2];
                    string title = fields[3];
                    string lang = fields[4];
                    string auth = fields[5];
                    string sub = fields[6];
                    if (Match(_title, title) && langs.Contains(lang))
                    {
                        books.Add(new Book(title, "http://gjss.org/sites/all/themes/gjss2014/images/no-cover.png", id, auth,providerID));
                    }
                }
            }
            return books;
        }
        public GutenbergProvider()
        {
            Languages = new List<Filters.Languages>() { Filters.Languages.English, Filters.Languages.Hungarian };
            Topics = new List<Filters.Topics>() { Filters.Topics.None };
            session = new RequestsSession("https://www.gutenberg.org/");
            providerID = 1;
        }
        internal override List<Book> SearchWithAuthor(List<Filters.Languages> languages, List<Filters.Topics> topics, string author)
        {
            return ScrapeBooks(TranslateLang(languages),_author:author);
        }

        internal override List<Book> SearchWithAuthorAndTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title, string author)
        {
            return ScrapeBooks(TranslateLang(languages), _author: author,_title: title);
        }

        internal override List<Book> SearchWithTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title)
        {
            return ScrapeBooks(TranslateLang(languages),_title: title);
        }

        internal override string DownloadBook(Book book)
        {
            List<string> urls = new List<string>();
            if (!File.Exists("1.zip"))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile("https://www.gutenberg.org/cache/epub/feeds/rdf-files.tar.zip", @"1.zip");
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(@"1.zip", "./");
                ExtractTar("rdf-files.tar", "./");
            }
            Regex rg = new Regex(@"(http.*\/ebook.*).>");
            string text = File.ReadAllText($"cache\\epub\\{book.bookID}\\pg{book.bookID}.rdf");
            //TODO: replace regex with xml parsing
            var links = rg.Matches(text);
            foreach (Match match in links)
            {
                urls.Add(match.Groups[1].Value);
            }
            links.Distinct();
            return urls[0];
        }
        static void ExtractTar(String tarFileName, String destFolder)
        {
            Stream inStream = File.OpenRead(tarFileName);
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();
        }
    }
}

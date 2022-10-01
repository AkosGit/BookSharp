using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EbookProvider.Providers;
using System.ComponentModel;
using EbookProvider.Exceptions;

namespace EbookProvider
{
    public class EbookManager
    {
        public List<Provider> providers { get; private set; }
        public EbookManager()
        {
            providers = new List<Provider>();

        }
        public void AddProvider(Provider pr)
        {
            providers.Add(pr);
        }
        public string DownloadBook(Book book)
        {
            if (providers.Count == 0) { throw new NoProviderException(); }
            return providers.Where(x => x.providerID==book.providerID).First().DownloadBook(book);
        }
        List<Book> Cleanup(List<Book> books)
        {
            for (int i = 0; i < books.Count; i++)
            {
                books[i].Title = books[i].Title.Replace("\n", "").Replace("  ", "").Replace("<br>", "");
            }
            return books;
        }
        public List<Book> DoSearch(List<Filters.Languages> languages, List<Filters.Topics> topics, string title = "NoN", string author = "NoN")
        {
            List<Book> books = new List<Book>();
            if(providers.Count == 0) { throw new NoProviderException(); }
            foreach (Provider pr in providers)
            {
                List<Filters.Languages> selectedlanguages =new List<Filters.Languages>();
                List<Filters.Topics> selectedtopics = new List<Filters.Topics>();
                selectedlanguages = languages.Intersect(pr.Languages).ToList();
                selectedtopics= topics.Intersect(pr.Topics).ToList();
                if(selectedlanguages.Count!=0 && selectedtopics.Count != 0)
                {
                    if(title=="NoN" && author != "NoN")
                    {
                        books.AddRange(pr.SearchWithAuthor(selectedlanguages, selectedtopics, author));

                    }
                    if(title!="NoN" && author == "NoN")
                    {
                        books.AddRange(pr.SearchWithTitle(selectedlanguages, selectedtopics, title));
                    }
                    if (title != "NoN" && author != "NoN")
                    {
                        books.AddRange(pr.SearchWithAuthorAndTitle(selectedlanguages, selectedtopics, title,author));
                    }
                }
            }
            return Cleanup(books);
        }
    }
}

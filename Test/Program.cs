
using EbookProvider;
using EbookProvider.Providers;
using System;
using System.Net;

namespace Test 
{
    internal class Program
    {
        //TODO: 
        //add website up checker
        //implement download feature
        //implement topics based search
        //make it async
        //handle timeouts
        //add test cases
        //implement Zlibary login
        static void Main(string[] args)
        {
            //DownloadTest();
            SearchTest();
        }
        static EbookManager ManagerFactory()
        {
            EbookManager m = new EbookManager();
            m.AddProvider(new GutenbergProvider());
            m.AddProvider(new MekOszkProvider());
            m.AddProvider(new NcoreProvider("USER", "PASS"));
            m.AddProvider(new ZLibaryProvider());
            return m;
        }
        static void SearchTest()
        {
            EbookManager m = ManagerFactory();  
            Console.Write("Title:");
            string title = Console.ReadLine();
            List<Book> books = m.DoSearch(new List<Filters.Languages> { Filters.Languages.Hungarian }, new List<Filters.Topics> { Filters.Topics.None }, title: title);
            if(books.Count == 0) {
                Console.WriteLine("No result found");  
            foreach (Book item in books)
            {
                Console.WriteLine(item.Author + ":  " + item.Title);
            }
            ; ;
        }
        static void DownloadTest()
        {
            EbookManager m = ManagerFactory();
            m.DownloadBook(new Book { bookID = " https://b-ok.xyz/book/550513/7d2b54", providerID=4});
           
        }

    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookProvider
{
    public class Book
    {
        public string Title { get; set; }
        public string CoverURL { get; set; } 
        public string bookID { get; set; }
        public string Author { get; set; }
        public int providerID { get; set; }
        public Book(string title, string coverURL, string bookID, string author,int providerID)
        {
            Title = title;
            CoverURL = coverURL;
            this.bookID = bookID;
            Author = author;
            this.providerID = providerID;
        }
        public Book()
        {

        }
    }
}

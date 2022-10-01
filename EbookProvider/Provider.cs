namespace EbookProvider
{
    public abstract class Provider
    {
        public List<Filters.Languages> Languages { get; set; }
        public List<Filters.Topics> Topics { get; set; }
        public int providerID { get; set; }

        public Provider()
        {
            Languages = new List<Filters.Languages>();
            Topics = new List<Filters.Topics>();
        }
        public Provider(List<Filters.Languages> languages, List<Filters.Topics> topics)
        {
            Languages = languages;
            Topics = topics;
        }
        internal abstract List<Book> SearchWithAuthor(List<Filters.Languages> languages, List<Filters.Topics> topics,string author);
        internal abstract List<Book> SearchWithTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title);
        internal abstract List<Book> SearchWithAuthorAndTitle(List<Filters.Languages> languages, List<Filters.Topics> topics, string title, string author);
        internal abstract string DownloadBook(Book book);
    }
    public abstract class ProviderWithAuth : Provider
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ProviderWithAuth(string username, string password)
        {
            Username = username;
            Password = password;    
        }
    }
}
# BookSharp: An E-book downloader library for C#
- Currently I'm working on supporting 4 sites and more in the future: Gutenberg project, Ncore, Zlibary, MEK.
- Books can be filtered by language and in the future by topics.
- Sites are represented as "Providers" and they can be loaded through the “EbookManager” class.
- Some providers require authentication those are derived from the "ProviderWithAuth" class.
## Example search
```csharp
EbookManager m = new EbookManager();
m.AddProvider(new GutenbergProvider());
m.AddProvider(new MekOszkProvider());
m.AddProvider(new NcoreProvider("USER", "PASS"));
m.AddProvider(new ZLibaryProvider());
List<Book> books = m.DoSearch(new List<Filters.Languages> { Filters.Languages.Hungarian }, 
  new List<Filters.Topics> { Filters.Topics.None }, title: "math");
string downloadURL=m.DownloadBook(books[0])
```
## Status
:warning: At this stage this is just a proof of concept not all functionality works! :warning:

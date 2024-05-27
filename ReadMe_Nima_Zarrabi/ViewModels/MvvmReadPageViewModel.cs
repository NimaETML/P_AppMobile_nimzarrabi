using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;
using HtmlAgilityPack;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace ReadMe_Nima_Zarrabi.ViewModels;

public partial class MvvmReadPageViewModel : ObservableObject
{

    [ObservableProperty]
    public EpubBook? book;

    [ObservableProperty]
    public string bookFullText = "slay";

    [ObservableProperty]
    public string bookPath = "books/norris-pit.epub";

    [ObservableProperty]
    public string btnText = "Read book";

    [RelayCommand]
    public void ReadBlob()
    {
        //Stream bookStream
        Book = EpubReader.ReadBook(bookPath);
        GetBlobText(Book);
    }

    public void GetBlobText(EpubBook book)
    {

        foreach (EpubLocalTextContentFile textContentFile in book.ReadingOrder)
        {
            PrintTextContentFile(textContentFile);
        }

    }

    public void PrintTextContentFile(EpubLocalTextContentFile textContentFile)
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(textContentFile.Content);
        StringBuilder sb = new();
        foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//text()"))
        {
            sb.AppendLine(node.InnerText.Trim());
        }
        BookFullText = sb.ToString();
    }
}
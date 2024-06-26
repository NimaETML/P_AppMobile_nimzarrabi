//Auteur : JMY
//Date   : 03.4.2024 
//Lieu   : ETML
//Descr. : squelette pour chargement de data à partir d’une api

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using System.IO.Compression;
using System.Xml;
using VersOne.Epub;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
//using static Android.Provider.MediaStore;

namespace ReadMe_Nima_Zarrabi;

public partial class ApiPage : ContentPage
{
	HttpClient client = new();
	bool useXml = false;
    public static string BaseAddress =
    DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:3000" : "http://localhost:3000";
    public static string ReadMeUrl = $"{BaseAddress}/api/epub/1";

    public string bookFullText;


    public ApiPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		try
		{
			//Call API
            var response = await client.GetAsync(endpoint.Text);
			if (response.IsSuccessStatusCode)
			{
				var content = response.Content;

				//Open epub ZIP
				ZipArchive archive = new ZipArchive(content.ReadAsStream());
				var coverEntry = archive.GetEntry("OEBPS/Images/cover.png");
                var page1Entry = archive.GetEntry("OEBPS/page1.xhtml");

                var coverStream = coverEntry.Open();


                EpubBook Book;

                //Attach cover to UI
                cover.Source = ImageSource.FromStream(() => coverStream);

				//Load CONTENT (meta data)
				var bookTitle = "not found";
                var bookLanguage = "not found";
				var contentString = new StreamReader(archive.GetEntry("OEBPS/content.opf").Open()).ReadToEnd();
                var fullBook = "not found";


                //Book = EpubReader.ReadBook(ReadMeUrl);
                //GetBlobText(Book);


                if (useXml)
				{
                    
                    #region XML version
                    //load meta-data from xml
                    var xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(contentString);

					// Retrieve the title element
					XmlNode titleNode = xmlDoc.SelectSingleNode("//dc:title", GetNamespaceManager(xmlDoc));

					bookTitle = titleNode != null ? titleNode.InnerText : "not found with xml";
					#endregion
                    
                }
				else
				{
                    #region plain text version

                    // Title
                    int BTstart = contentString.IndexOf("<dc:title>") + 10;
                    int BTend = contentString.IndexOf("</dc:title>");

                    bookTitle = (BTstart != -1 && BTend != -1) ? contentString.Substring(BTstart, BTend - BTstart) : "Title not found.";

                    // Language
                    int BLstart = contentString.IndexOf("<dc:language>") + 10;
                    int BLend = contentString.IndexOf("</dc:language>");

                    bookLanguage = (BLstart != -1 && BLend != -1) ? contentString.Substring(BLstart, BLend - BLstart) : "Language not found.";

                    int FBstart = contentString.IndexOf("page1.xhtml");
                    int FBend = contentString.IndexOf("page2.xhtml");

                    fullBook = (FBstart != -1 && FBend != -1) ? contentString.Substring(FBstart, FBend) : "chapter 1 not found.";

                    // FUll book content left to do
                    #endregion
                }

                // Y'a rien qui marche avec ces chapitres
                title.Text=bookTitle;
                Language.Text = bookLanguage;
                bookText.Text= fullBook;
                page1.Text = Convert.ToString(page1Entry);

            }
            else
			{
				throw new Exception($"Bad status : {response.StatusCode}, {response.Headers},{response.Content}");
			}
        }
		catch(Exception ex) {
			await DisplayAlert(ex.Message, ex.StackTrace,"ok");
		}
		

    }

    private static XmlNamespaceManager GetNamespaceManager(XmlDocument xmlDoc)
    {
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        return nsManager;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
		useXml = e.Value;
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
        bookFullText = sb.ToString();
        bookText.Text = bookFullText;
    }
}
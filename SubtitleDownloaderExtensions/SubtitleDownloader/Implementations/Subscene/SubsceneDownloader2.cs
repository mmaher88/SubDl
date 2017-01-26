using SubtitleDownloader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Web;
using System.Net;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.Subscene
{
    public class SubsceneDownloader2 : ISubtitleDownloader
    {

        private static string BaseUrl = "http://subscene.com";

        public int SearchTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string downloadLink = GetDownloadLink(subtitle.Id);

            string url = BaseUrl + downloadLink;

            string archiveFile = FileUtils.GetTempFileName();


            WebClient client = new WebClient();
            client.DownloadFile(BaseUrl + downloadLink, archiveFile);

            return FileUtils.ExtractFilesFromZipOrRarFile(archiveFile);

        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var encodedUri = HttpUtility.UrlEncode(query.Query);
            var queryUri = string.Format("http://subscene.com/subtitles/release?q={0}&r=true", encodedUri);

            var resultsPage = GetResultsPage(queryUri);

            return ParseResultsPage(resultsPage);

        }

        private HtmlDocument GetResultsPage(string queryUri)
        {
            HtmlDocument pageDoc = new HtmlDocument();
            string htmlCode = "";
            using (WebClient webClient = new WebClient())
            {
                htmlCode = webClient.DownloadString(queryUri);
            }

            pageDoc.LoadHtml(htmlCode);

            return pageDoc;
        }

        private List<Subtitle> ParseResultsPage(HtmlDocument resultsPage)
        {
            List<Subtitle> results = new List<Subtitle>();

            var subNodes = resultsPage.DocumentNode.SelectNodes("//td[@class='a1']");

            foreach (var subtitleNode in subNodes)
            {
                var linkNode = subtitleNode.SelectSingleNode(".//a");
                if (linkNode == null)
                {
                    continue;
                }

                var link = linkNode.GetAttributeValue("href", string.Empty);
                var linkSpanNodes = linkNode.SelectNodes(".//span");

                var languageNode = linkSpanNodes[0];
                if (languageNode == null)
                {
                    continue;
                }
                var language = languageNode.InnerText.Trim();

                var titleNode = linkSpanNodes.LastOrDefault();
                if (titleNode == null)
                {
                    continue;
                }

                var title = titleNode.InnerText.Trim();
                var commentNode = subtitleNode.ParentNode.SelectSingleNode(".//td[@class='a6']/div");
                string description = string.Empty;
                if (commentNode != null)
                {
                    description = commentNode.InnerText.Trim();
                }

                description = HttpUtility.HtmlDecode(description.Replace(Environment.NewLine, " "));
                title = HttpUtility.HtmlDecode(title);

                if (language == "English")
                {
                    results.Add(new Subtitle(link, title, title, "eng"));
                }
            }

            return results;
        }

        private string GetDownloadLink(string subUrl)
        {
            HtmlDocument pageDoc = new HtmlDocument();
            string htmlCode = "";
            using (WebClient webClient = new WebClient())
            {
                htmlCode = webClient.DownloadString(BaseUrl + subUrl);
            }

            pageDoc.LoadHtml(htmlCode);

            var downloadNode = pageDoc.DocumentNode.SelectSingleNode(".//div[@class='download']");

            var downloadLink = downloadNode.SelectSingleNode(".//a").GetAttributeValue("href", string.Empty);


            return downloadLink;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MusictasticReborn.BusinessLayer.Helpers;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.AlbumArtGetter
{
    public class AlbumArtGetter
    {
        public async Task FindAndSetAlbumArtAsync(IEnumerable<AlbumModel> albums)
        {
            var db = DatabaseHelper.OpenConnection();

            foreach (var album in albums.Where(album => String.IsNullOrWhiteSpace(album.ArtPath)))
            {
                await FindAndSetAlbumArtAsync(album);
                await db.UpdateAsync(album);
            }
        }

        private async Task FindAndSetAlbumArtAsync(AlbumModel forAlbum)
        {
            string query = BuildQueryUrl(forAlbum.Name);

            var foundAlbums = await ParseResultsIntoModelsAsync(query);

            var bestMatch = foundAlbums.OrderByDescending(
                    result => ResultMatchScoreCalculator.CalculateMatchScore(forAlbum, result)).First();
            
            string albumArt = await GetAlbumArtUrl(bestMatch);

            var downloadedImage = await DownloadAlbumArtAsync(albumArt);

            forAlbum.ArtPath = await StorageHelper.SaveImageAsync(downloadedImage);

            Debug.WriteLine(albumArt);
        }

        private async Task<Stream> DownloadAlbumArtAsync(string url)
        {
            if (!url.Contains("http:"))
            {
                url = "http:" + url;
            }

            HttpClient client = new HttpClient();

            return await client.GetStreamAsync(url);
        }

        private string BuildQueryUrl(string query)
        {
            query = query.Replace(' ', '+');

            return String.Format("http://musicbrainz.org/search?query={0}&type=release&method=indexed", query);
        }

        private async Task<List<MusicBrainzAlbumsResultRow>> ParseResultsIntoModelsAsync(string forQuery)
        {
            HtmlDocument document = await DownloadWebsiteIntoHtmlDocument(forQuery);

            HtmlNode resultsTable = GetTableFromDocument(document);

            var tableRows = GetTableRowsFromTable(resultsTable);

            var foundAlbums = new List<MusicBrainzAlbumsResultRow>(5);

            foundAlbums.AddRange(tableRows.Select(ParseTableRowIntoModel));

            foreach (var item in foundAlbums)
            {
                Debug.WriteLine(item.ToString());
            }

            return foundAlbums;
        }

        private async Task<HtmlDocument> DownloadWebsiteIntoHtmlDocument(string url)
        {
            HttpClient client = new HttpClient();

            var stringBody = await client.GetStringAsync(url);

            client.Dispose();

            return HtmlDocumentFactory.LoadFromString(stringBody);
        }

        private HtmlNode GetTableFromDocument(HtmlDocument document)
        {
            return document.DocumentNode.Descendants("table").
                FirstOrDefault(t => t.Attributes.Contains("class")
                               && t.Attributes["class"].Value.Contains("tbl"));
        }

        private IEnumerable<HtmlNode> GetTableRowsFromTable(HtmlNode table)
        {
            var tableBody = table.Descendants("tbody").First();

            return tableBody.Descendants("tr").Take(5).ToList();
        }

        private MusicBrainzAlbumsResultRow ParseTableRowIntoModel(HtmlNode tableRow)
        {
            var tdNodes = tableRow.Descendants("td").Take(7).ToList();

            int score = int.Parse(tdNodes[0].InnerText);
            string name = tdNodes[1].InnerText;

            string url = tdNodes[1].Descendants("a").First().Attributes["href"].Value;

            string artist = tdNodes[2].InnerText;

            int tracksNumber;

            int.TryParse(tdNodes[4].InnerText, out tracksNumber);

            return new MusicBrainzAlbumsResultRow
                {
                    Name = name,
                    Artist = artist,
                    Score = score,
                    NumberOfTracks = tracksNumber,
                    Url = url
                };
        }

        public async Task<String> GetAlbumArtUrl(MusicBrainzAlbumsResultRow fromModel)
        {
            HttpClient client = new HttpClient();

            var stringBody = await client.GetStringAsync(fromModel.Url);

            client.Dispose();

            HtmlDocument document = HtmlDocumentFactory.LoadFromString(stringBody);

            string imgUrl = string.Empty;

            var coverArtDiv =
                document.DocumentNode.Descendants("div")
                    .FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("cover-art"));

            if (coverArtDiv != null)
            {
                imgUrl = coverArtDiv.Descendants("a").First().Attributes["href"].Value;
            }

            return imgUrl;
        }

        private static class HtmlDocumentFactory
        {
            public static HtmlDocument LoadFromString(string content)
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);

                return document;
            }
        }

    }
    
}

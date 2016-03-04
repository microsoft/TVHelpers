using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace MediaAppSample.Core.Data.Channel9Data
{
    public sealed class Channel9DataSource : ClientApiBase, IDataSource
    {
        #region Queue

        public Task<IEnumerable<QueueModel>> GetQueue()
        {
            List<QueueModel> list = new List<QueueModel>();
            return Task.FromResult<IEnumerable<QueueModel>>(list);
        }

        public Task AddToQueue(ContentItemBase item)
        {
            return Task.FromResult<object>(null);
        }

        public Task RemoveFromQueue(ContentItemBase item)
        {
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Movies

        public async Task<MovieModel> GetMovieHero()
        {
            var list = await this.GetMovies();
            return list.FirstOrDefault();
        }

        public async Task<IEnumerable<MovieModel>> GetMovies()
        {
            string url = "https://channel9.msdn.com/Events/Build/2015/RSS";

            var client = new SyndicationClient();
            var feed = await client.RetrieveFeedAsync(new Uri(url, UriKind.Absolute));

            var list = new List<MovieModel>();
            foreach (var item in feed.Items.Take(25))
            {
                var m = new MovieModel();
                m.ContentID = item.Id;
                m.Title = item.Title.Text;
                m.ReleaseDate = item.PublishedDate.DateTime;
                m.ItemType = ItemTypes.Movie;
                m.Description = this.GetValue(item, "summary");
                m.URL = this.GetValue(item, "content", "url");
                m.ResumeImage = m.InlineImage = m.MediaImage = m.FeaturedImage = m.PosterImage = this.GetValue(item, "thumbnail", "url");
                m.Creators = this.GetAuthors(item.Authors);
                m.Creators.ForEach(f => f.Role = "Author");
                m.Cast = this.GetAuthors(item.Contributors);
                m.Cast.ForEach(f => f.Role = "Cast");

                list.Add(m);
            }
            return list;
        }

        private string GetValue(SyndicationItem item, string elementName, string attributeName = null)
        {
            var nodes = item.GetXmlDocument(SyndicationFormat.Atom10).GetElementsByTagName(elementName);
            if (nodes != null)
            {
                var node = nodes.LastOrDefault();
                if (node != null)
                {
                    if (string.IsNullOrEmpty(attributeName))
                        return node.InnerText;

                    var attrib = node.Attributes.LastOrDefault(f => f.NodeName.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase));
                    if (attrib != null)
                        return attrib.InnerText;
                }
            }
            return null;
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesFeatured()
        {
            return (await this.GetMovies()).Take(4);
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvFeatured()
        {
            return (await this.GetTvSeries()).Take(4);
        }

        public async Task<IEnumerable<TvEpisodeModel>> GetEpisodes()
        {
            return (await this.GetTvEpisodes());
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesNewReleases()
        {
            return (await this.GetMovies()).Take(7);
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesTrailers()
        {
            return (await this.GetMovies()).Take(6);
        }

        #endregion

        private ObservableCollection<CastAndCrewModel> GetAuthors(IEnumerable<SyndicationPerson> persons)
        {
            var models = new ObservableCollection<CastAndCrewModel>();
            foreach (var person in persons)
            {
                person.Email.Split(',').ForEach(f =>
                {
                    var model = new CastAndCrewModel();
                    model.Name = f.Trim();
                    model.Biography = f.Trim();
                    model.Image = "http://ia.media-imdb.com/images/M/MV5BMTk1MjM3NTU5M15BMl5BanBnXkFtZTcwMTMyMjAyMg@@._V1_UY317_CR14,0,214,317_AL_.jpg";
                    models.Add(model);
                });
            }
            return models;
        }

        public Task<ContentItemBase> GetContentItem(string contentID)
        {
            return Task.FromResult<ContentItemBase>(null);
        }

        public Task<IEnumerable<ContentItemBase>> GetRelated(string contentID)
        {
            var list = new List<ContentItemBase>();
            return Task.FromResult<IEnumerable<ContentItemBase>>(list);
        }

        public Task<IEnumerable<ContentItemBase>> GetTrailers(string contentID)
        {
            var list = new List<ContentItemBase>();
            return Task.FromResult<IEnumerable<ContentItemBase>>(list);
        }

        #region TV

        public async Task<IEnumerable<TvSeriesModel>> GetTvSeries()
        {
            string url = "https://channel9.msdn.com/Events/GDC/GDC-2015/RSS";

            var client = new SyndicationClient();
            var feed = await client.RetrieveFeedAsync(new Uri(url, UriKind.Absolute));

            var list = new List<TvSeriesModel>();
            foreach (var item in feed.Items.Take(25))
            {
                var m = new TvSeriesModel();
                m.Description = this.GetValue(item, "summary");
                m.Title = item.Title.Text;
                m.ReleaseDate = item.PublishedDate.DateTime.ToString();
                m.ItemType = ItemTypes.TvSeries;
                m.Description = this.GetValue(item, "summary");
                m.URL = this.GetValue(item, "content", "url");
                m.ResumeImage = m.InlineImage = m.MediaImage = m.FeaturedImage = m.TvEpisodeImage = this.GetValue(item, "thumbnail", "url");
                m.Creators = this.GetAuthors(item.Authors);
                m.Creators.ForEach(f => f.Role = "Author");
                m.Cast = this.GetAuthors(item.Contributors);
                m.Cast.ForEach(f => f.Role = "Cast");

                list.Add(m);
            }
            return list;
        }

        public async Task<TvSeriesModel> GetTvHero()
        {
            var list = await this.GetTvSeries();
            return list.FirstOrDefault();
        }

        public async Task<IEnumerable<TvEpisodeModel>> GetTvEpisodes()
        {
            return (await this.GetTvInline()).Take(3);
        }

        public async Task<IEnumerable<TvEpisodeModel>> GetTvInline()
        {
            string url = "https://channel9.msdn.com/Events/GDC/GDC-2015/RSS";

            var client = new SyndicationClient();
            var feed = await client.RetrieveFeedAsync(new Uri(url, UriKind.Absolute));

            var list = new List<TvEpisodeModel>();
            foreach (var item in feed.Items.Take(5))
            {
                var m = new TvEpisodeModel();
                m.Description = this.GetValue(item, "summary");
                m.Title = item.Title.Text;
                m.AirDate = item.PublishedDate.Date;
                m.ItemType = ItemTypes.TvSeries;
                m.Description = this.GetValue(item, "summary");
                m.URL = this.GetValue(item, "content", "url");
                m.ResumeImage = m.InlineImage = m.MediaImage = m.FeaturedImage = m.TvEpisodeImage = this.GetValue(item, "thumbnail", "url");
                m.Creators = this.GetAuthors(item.Authors);
                m.Creators.ForEach(f => f.Role = "Author");
                m.Cast = this.GetAuthors(item.Contributors);
                m.Cast.ForEach(f => f.Role = "Cast");

                list.Add(m);
            }
            return list;
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvNewReleases()
        {
            return (await this.GetTvSeries()).Take(7);
        }

        public IEnumerable<SeasonModel> GetSeasons(TvSeriesModel series)
        {
            var list = new List<SeasonModel>();
            return list;
        }

        #endregion
    }
}

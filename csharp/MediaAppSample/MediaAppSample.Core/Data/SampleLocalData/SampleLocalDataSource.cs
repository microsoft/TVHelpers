// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Data.SampleLocalData
{
    public sealed class SampleLocalDataSource : ClientApiBase, IDataSource
    {
        #region Sample Data Variables

        private const string SAMPLE_NAME = "Lorem Ipsum";
        private const string SAMPLE_DESCRIPTION = "Lorem Ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum fringilla felis diam. Meecenas mauris lorem, iaculis rhoncus volutpat vel, tritique ut sapien. Pellentesque eros neque, pharetra consectetur nisi nec, iaculis finibus lectus. Cras elit odio, ultricies eu fermentum ac, facilisis sed elit. Suspendisse molestie molestie mi vel lobortis. Praesent id bibendum nulla. Phasellus sed tortor purus. Duis ultrices consectetur diam ut commodo. Fusce et vulputate nulla. Maecenas porttitor quam id dui hendrerit, quis tempus mauris rhoncus. Aenean at nisi fermentum eros laoreet fringilla. Sed eu purus sed arcu pulvinar lobortis. Nulla blandit nec augue sit amet ultricies. Aliquam sit amet eleifend lorem. ";
        private const string SAMPLE_PATH_ROOT = "ms-appx:///MediaAppSample.Core/Data/SampleLocalData/";
        private const string SAMPLE_IMAGE_PATH_ROOT = SAMPLE_PATH_ROOT + "Images/";
        private const string SAMPLE_CAST_PATH_ROOT = SAMPLE_PATH_ROOT + "Cast/";
        private const string SAMPLE_MEDIA_PATH_ROOT = SAMPLE_PATH_ROOT + "Videos/";
        private const string SAMPLE_MEDIA_FILE = @"http://video.ch9.ms/ch9/df36/e76e6f63-0d2d-45b2-bd13-369a41d4df36/2-768_mid.mp4";
        private static readonly Random _random = new Random(1234);

        #endregion

        #region Constructors

        public SampleLocalDataSource()
            : base("http://api.contoso.com/v1/")
        {
        }

        #endregion

        #region Methods

        #region Content

        public IEnumerable<SeasonModel> GetItemsAsync(TvSeriesModel series, CancellationToken ct)
        {
            // curate seasons
            var list = new ModelList<SeasonModel>();
            GenerateItem<SeasonModel>(list, 43);
            GenerateItem<SeasonModel>(list, 44);
            GenerateItem<SeasonModel>(list, 45);
            return list;
        }

        public Task<IEnumerable<TrailerModel>> GetTrailersAsync(CancellationToken ct)
        {
            // Curate inline content
            var list = new ContentItemList();
            GenerateItem<TrailerModel>(list, 37);
            GenerateItem<TrailerModel>(list, 38);
            GenerateItem<TrailerModel>(list, 39);
            GenerateItem<TrailerModel>(list, 40);
            GenerateItem<TrailerModel>(list, 41);
            GenerateItem<TrailerModel>(list, 42);
            return Task.FromResult<IEnumerable<TrailerModel>>(list.OfType<TrailerModel>());
        }

        public async Task<IEnumerable<TrailerModel>> GetTrailersAsync(string contentID, CancellationToken ct)
        {
            // curate movie trailers
            var results = await this.GetContentItemsAsync(ItemTypes.Trailer, ct);
            return results.OfType<TrailerModel>();
        }

        public Task<IEnumerable<ContentItemBase>> GetContentItemsAsync(ItemTypes type, CancellationToken ct)
        {
            switch (type)
            {
                case ItemTypes.Movie:
                    {
                        var list = new ObservableCollection<MovieModel>();
                        for (var x = 1; x <= 18; x++)
                            GenerateItem<MovieModel>(list, x);
                        return Task.FromResult<IEnumerable<ContentItemBase>>(list);
                    }

                case ItemTypes.Trailer:
                    {
                        var list = new ObservableCollection<TrailerModel>();
                        for (var x = 8; x >= 1; x--)
                            GenerateItem<TrailerModel>(list, x);
                        return Task.FromResult<IEnumerable<ContentItemBase>>(list);
                    }

                case ItemTypes.TvSeries:
                    {
                        var list = new ObservableCollection<TvSeriesModel>();
                        for (var x = 19; x <= 36; x++)
                            GenerateItem<TvSeriesModel>(list, x);
                        return Task.FromResult<IEnumerable<ContentItemBase>>(list);
                    }

                case ItemTypes.TvEpisode:
                    {
                        var list = new ObservableCollection<TvEpisodeModel>();
                        for (var x = 41; x <= 49; x++)
                            GenerateItem<TvEpisodeModel>(list, x);
                        return Task.FromResult<IEnumerable<ContentItemBase>>(list);
                    }

                default:
                    throw new NotImplementedException("GetItemsAsync for " + type);
            }
        }

        public async Task<IEnumerable<ContentItemBase>> GetRelatedAsync(string id, CancellationToken ct)
        {
            // get some Content to populate Related

            if (id.Contains("series"))
            {
                var results = await this.GetContentItemsAsync(ItemTypes.TvSeries, ct);
                return results.OrderBy(o => o.Title);
            }
            else
            {
                var results = await this.GetContentItemsAsync(ItemTypes.Movie, ct);
                return results.OrderBy(o => o.Title);
            }
        }

        public async Task<ContentItemBase> GetItemAsync(string id, CancellationToken ct)
        {
            if (id.Contains("series"))
            {
                var results = await this.GetContentItemsAsync(ItemTypes.TvSeries, ct);
                return results.FirstOrDefault(s => s.ID == id);
            }
            else if(id.Contains("episode"))
            {
                var results = await this.GetContentItemsAsync(ItemTypes.TvEpisode, ct);
                return results.FirstOrDefault(s => s.ID == id);
            }
            else
            {
                var results = await this.GetContentItemsAsync(ItemTypes.Movie, ct);
                return results.FirstOrDefault(s => s.ID == id);
            }
        }

        public Task<ContentItemBase> GetResumeItem(string id, ItemTypes type, CancellationToken ct)
        {
            if (type == ItemTypes.TvSeries)
                return Task.FromResult<ContentItemBase>(GenerateItem<TvEpisodeModel>(42));
            else
                return Task.FromResult<ContentItemBase>(null);
        }

        #endregion

        #region Home

        public async Task<ContentItemBase> GetFeaturedItemAsync(CancellationToken ct)
        {
            // curated featured hero
            var results = await this.GetContentItemsAsync(ItemTypes.Movie, ct);
            var result = results.FirstOrDefault();
            result.Flag = "Featured";
            return result;
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesFeaturedAsync(CancellationToken ct)
        {
            // curated featured movies
            var list = new List<MovieModel>();
            var results = await this.GetContentItemsAsync(ItemTypes.Movie, ct);
            list.Add((MovieModel)results.First(o => o.ID == "movie02"));
            list.Add((MovieModel)results.First(o => o.ID == "movie18"));
            list.Add((MovieModel)results.First(o => o.ID == "movie12"));
            list.Add((MovieModel)results.First(o => o.ID == "movie13"));
            list.ForEach((r) => r.Flag = "Just Added");
            return list;
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesNewReleasesAsync(CancellationToken ct)
        {
            // curated new release movies
            return (await this.GetContentItemsAsync(ItemTypes.Movie, ct)).OfType<MovieModel>();
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvFeaturedAsync(CancellationToken ct)
        {
            // curate featured TV
            var list = new List<TvSeriesModel>();
            var results = await this.GetContentItemsAsync(ItemTypes.TvSeries, ct);
            list.Add((TvSeriesModel)results.First(o => o.ID == "series19"));
            list.Add((TvSeriesModel)results.First(o => o.ID == "series20"));
            list.Add((TvSeriesModel)results.First(o => o.ID == "series21"));
            list.Add((TvSeriesModel)results.First(o => o.ID == "series22"));
            list.ForEach((r) => r.Flag = "Most Popular");
            return list;

            //return new List<TvSeriesModel>
            //{
            //    tvSeriesModels.First(o => o.ContentID == "series03"),
            //    tvSeriesModels.First(o => o.ContentID == "series44"),
            //    tvSeriesModels.First(o => o.ContentID == "series31"),
            //    tvSeriesModels.First(o => o.ContentID == "series32")
            //};
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvNewReleasesAsync(CancellationToken ct)
        {
            // curate new TV releases
            return (await this.GetContentItemsAsync(ItemTypes.TvSeries, ct)).OfType<TvSeriesModel>();
        }

        #endregion

        #region Queue

        public async Task<IEnumerable<QueueModel>> GetQueueItemsAsync(string userID, CancellationToken ct)
        {
            var movies = (await this.GetContentItemsAsync(ItemTypes.Movie, ct)).ToArray();
            var tv = (await this.GetContentItemsAsync(ItemTypes.TvSeries, ct)).ToArray();
            return new List<QueueModel>
            {
                new QueueModel() {Item = movies[9]},
                new QueueModel() {Item = movies[8]},
                new QueueModel() {Item = tv[5]},
                new QueueModel() {Item = movies[10]},
                new QueueModel() {Item = tv[9]},
                new QueueModel() {Item = movies[12]},
                new QueueModel() {Item = tv[11]},
                new QueueModel() {Item = movies[14]},
                new QueueModel() {Item = tv[15]}
            };
        }

        public Task AddToQueueAsync(string userID, ContentItemBase item, CancellationToken ct)
        {
            return Task.FromResult<object>(null);
        }

        public Task RemoveFromQueueAsync(string userID, ContentItemBase item, CancellationToken ct)
        {
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Ratings/Reviews/Casts

        public Task<IEnumerable<RatingModel>> GetRatingsAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<RatingModel>>(GetRatings(contentID));
        }

        public Task<IEnumerable<ReviewModel>> GetReviewsAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<ReviewModel>>(GetReviews(contentID));
        }

        public Task<IEnumerable<PersonModel>> GetCastAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<PersonModel>>(GenerateCast(contentID));
        }

        public Task<IEnumerable<PersonModel>> GetCrewAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<PersonModel>>(GenerateCrew(contentID));
        }

        #endregion

        #region Account

        /// <summary>
        /// Performs authentication for the application.
        /// </summary>
        /// <param name="vm">Sign in view model instance that contains the user's login information.</param>
        /// <returns>Login reponse with authorization details.</returns>
        public async Task<UserResponse> AuthenticateAsync(AccountSignInViewModel vm, CancellationToken ct)
        {
            var response = new UserResponse() { AccessToken = "1234567890", RefreshToken = "abcdefghijklmnop", ID = vm.Username, Email = vm.Username, FirstName = "John", LastName = "Doe" };
            await Task.Delay(2000, ct);
            return response;

            //this.Client.DefaultRequestHeaders.Add("Authorization", "Basic YzExNGEzM2U4YjNhNDdmY2E3NzBhYmJiMGNlOWE0YjE6NDFjOTcxYTU3NzlhNGZhMGI4NGZmN2EzNTA4NTQ5M2U=");

            //var dic = new Dictionary<string, string>();
            //dic.Add("grant_type", "password");
            //dic.Add("username", vm.Username);
            //dic.Add("password", vm.Password);
            //dic.Add("scope", "streaming");
            //var contents = new HttpFormUrlEncodedContent(dic);

            //HttpStringContent content = new HttpStringContent(message.Stringify(), UnicodeEncoding.Utf8, "application/json");

            //return await this.PostAsync<UserResponse, HttpFormUrlEncodedContent>(URL_ACCOUNT_SIGNIN, contents, SerializerTypes.Json);
        }

        /// <summary>
        /// Performs account creation for the application.
        /// </summary>
        /// <param name="vm">Sign up view model instance containing all the user's registration information.</param>
        /// <returns>Login response and authorization information if the account creation process was successful.</returns>
        public async Task<UserResponse> RegisterAsync(AccountSignUpViewModel vm, CancellationToken ct)
        {
            var response = new UserResponse() { AccessToken = "0987654321", RefreshToken = "qrstuvwxwyz", ID = vm.Username, Email = vm.Username, FirstName = vm.FirstName, LastName = vm.LastName };
            await Task.Delay(2000, ct);
            return response;

            //var dic = new Dictionary<string, string>();
            //dic.Add("grant_type", "password");
            //dic.Add("username", vm.Username);
            //dic.Add("password", vm.Password1);
            //dic.Add("scope", "streaming");
            //var contents = new HttpFormUrlEncodedContent(dic);

            //HttpStringContent content = new HttpStringContent(message.Stringify(), UnicodeEncoding.Utf8, "application/json");

            //return await this.PostAsync<UserResponse, HttpFormUrlEncodedContent>(URL_ACCOUNT_SIGNUP, contents);
        }

        /// <summary>
        /// Requests forgotten account information when a user cannot rememeber their authentication details.
        /// </summary>
        /// <param name="vm">Account forgot view model instance contain partial account details.</param>
        /// <returns>Response information indicating whether the call was successful or not.</returns>
        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(AccountForgotViewModel vm, CancellationToken ct)
        {
            var response = new ForgotPasswordResponse() { IsValid = true, Message = "Your password has been sent to your e-mail!" };
            await Task.Delay(2000, ct);
            return response;
        }

        /// <summary>
        /// Authenticates a user account returned from the Web Account Manager service.
        /// </summary>
        /// <param name="wi">Web account info object instance representing an authenticated WAM user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response object from the server.</returns>
        public async Task<UserResponse> AuthenticateAsync(Services.WebAccountManager.WebAccountInfo wi, CancellationToken ct)
        {
            // This logic below should be server side. Token should be used to retrieve MSA and then check to see if MediaAppSample account exists else register new account.

            switch (wi.Type)
            {
                case Services.WebAccountManager.WebAccountTypes.Microsoft:
                    {
                        // Retrieve MSA profile data
                        MicrosoftAccountDetails msa = null;
                        using (var api = new MicrosoftApi())
                        {
                            msa = await api.GetUserProfile(wi.Token, ct);
                        }

                        if (msa == null)
                            throw new Exception("Could not retrieve Microsoft account profile data!");

                        var response = await this.IsMicrosoftAccountRegistered(msa.id, ct);
                        if (response != null)
                        {
                            // User account exists, return response
                            return response;
                        }
                        else
                        {
                            // No account exists, use MSA profile to register user
                            AccountSignUpViewModel vm = new AccountSignUpViewModel();

                            // Set all the MSA data to the ViewModel
                            vm.Populate(msa);

                            // Call the registration API to create a new account and return
                            return await this.RegisterAsync(vm, ct);
                        }
                    }

                default:
                    throw new NotImplementedException(wi.Type.ToString());
            }
        }

        /// <summary>
        /// Checks to see if a MSA account ID is an existing user of this app service or not.
        /// </summary>
        /// <param name="id">Unique MSA account ID.</param>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>Response object from the server.</returns>
        private Task<UserResponse> IsMicrosoftAccountRegistered(string id, CancellationToken ct)
        {
            // TODO server side logic to check if MSA user is existing or not as a user of this application. Returns "false" in this sample.
            return Task.FromResult<UserResponse>(null);
        }

        #endregion

        #region Search

        public async Task<IEnumerable<ContentItemBase>> SearchAsync(string searchText, CancellationToken ct)
        {
            var list = new ContentItemList();
            list.AddRange(await this.GetContentItemsAsync(ItemTypes.Movie, ct));
            list.AddRange(await this.GetContentItemsAsync(ItemTypes.TvSeries, ct));
            return list.Where(w => w.Title.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0 || w.Description.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        #endregion

        #region Recommended

        public async Task<IEnumerable<ContentItemBase>> GetRecommendedItemsAsync(CancellationToken ct)
        {
            // Reusing the queue results for recommended for this sample data only. Obviously not a real world thing to do. 
            var results = await this.GetQueueItemsAsync(null, ct);
            return results?.Select(s => s.Item).OrderByDescending(o => o.Title);
        }

        public async Task<IEnumerable<ContentItemBase>> GetFriendsWatchedItemsAsync(CancellationToken ct)
        {
            var results = await this.GetContentItemsAsync(ItemTypes.TvSeries, ct);
            return results?.OrderBy(o => o.Title);
        }

        #endregion

        #region Voice Integration

        public Task<IEnumerable<string>> GetTilesForVoiceIntegration(CancellationToken ct)
        {
            List<string> titles = new List<string>();
            for(int i=1;i<=61;i++)
            {
                titles.Add("Movie " + i);
                titles.Add("TV Series " + i);
            }
            return Task.FromResult<IEnumerable<string>>(titles.Distinct().OrderBy(o => o));
        }

        #endregion

        #region Generate Methods

        public static T GenerateItem<T>(int number) where T : class
        {
            var numberString = number.ToString();
            var imageNumber = number.ToString("00");

            if (typeof(T) == typeof(MovieModel))
            {
                var item = new MovieModel()
                {
                    ID = "movie" + imageNumber,
                    ContentRating = "G",
                    ItemType = ItemTypes.Movie,
                    Title = "Movie " + numberString,
                    UserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    ImageFeatured = SAMPLE_IMAGE_PATH_ROOT + string.Format("Featured_Image{0}.jpg", imageNumber),
                    ImageCustom = SAMPLE_IMAGE_PATH_ROOT + string.Format("Custom_Movie_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeSmall = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Small_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeLarge = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Large_Image{0}.jpg", imageNumber),
                    MediaUrl = new Uri(SAMPLE_MEDIA_FILE),
                    Description = SAMPLE_DESCRIPTION,
                    Length = _random.Next(50, 200),
                    ReleaseDate = GenerateRandomDate(),
                    Cast = new ModelList<PersonModel>(GenerateCast("movie" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GenerateCrew("movie" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("movie" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("movie" + imageNumber)),
                    //Flag = "Just Added",
                };
                //if (number == 1) item.Flag = "Featured";
                return item as T;
            }
            else if (typeof(T) == typeof(TrailerModel))
            {
                var item = new TrailerModel()
                {
                    ID = "movie" + imageNumber,
                    ContentRating = "G",
                    ItemType = ItemTypes.Movie,
                    Title = "Movie " + numberString,
                    UserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    ImageFeatured = SAMPLE_IMAGE_PATH_ROOT + string.Format("Featured_Image{0}.jpg", imageNumber),
                    ImageCustom = SAMPLE_IMAGE_PATH_ROOT + string.Format("Custom_Movie_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeSmall = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Small_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeLarge = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Large_Image{0}.jpg", imageNumber),
                    MediaUrl = new Uri(SAMPLE_MEDIA_FILE),
                    Description = SAMPLE_DESCRIPTION,
                    Length = _random.Next(1, 5),
                    ReleaseDate = new DateTime(2015, 1, 15),
                    Cast = new ModelList<PersonModel>(GenerateCast("movie" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GenerateCrew("movie" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("movie" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("movie" + imageNumber)),
                };
                return item as T;
            }
            else if (typeof(T) == typeof(TvSeriesModel))
            {
                var item = new TvSeriesModel()
                {
                    ID = "series" + imageNumber,
                    ItemType = ItemTypes.TvSeries,
                    Title = "TV Series " + number.ToString(),
                    UserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    ImageFeatured = SAMPLE_IMAGE_PATH_ROOT + string.Format("Featured_Image{0}.jpg", imageNumber),
                    ImageCustom = SAMPLE_IMAGE_PATH_ROOT + string.Format("Custom_TV_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeSmall = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Small_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeLarge = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Large_Image{0}.jpg", imageNumber),
                    MediaUrl = new Uri(SAMPLE_MEDIA_FILE),
                    Description = SAMPLE_DESCRIPTION,
                    ContentRating = "TV-13",
                    Length = _random.Next(22, 60),
                    Cast = new ModelList<PersonModel>(GenerateCast("series" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GenerateCrew("series" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("series" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("series" + imageNumber)),
                };
                item.Seasons = new ModelList<SeasonModel>();
                item.Seasons.Add(GenerateItem<SeasonModel>(1));
                item.Seasons.Add(GenerateItem<SeasonModel>(2));
                item.Seasons.Add(GenerateItem<SeasonModel>(3));
                return item as T;
            }
            else if (typeof(T) == typeof(TvEpisodeModel))
            {
                var item = new TvEpisodeModel()
                {
                    ID = "episode" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvEpisode,
                    Title = "Episode " + number.ToString(),
                    UserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = "Drama",
                    ImageFeatured = SAMPLE_IMAGE_PATH_ROOT + string.Format("Featured_Image{0}.jpg", imageNumber),
                    ImageCustom = SAMPLE_IMAGE_PATH_ROOT + string.Format("Custom_TV_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeSmall = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Small_Image{0}.jpg", imageNumber),
                    ImageThumbLandscapeLarge = SAMPLE_IMAGE_PATH_ROOT + string.Format("Thumb_Landscape_Large_Image{0}.jpg", imageNumber),
                    MediaUrl = new Uri(SAMPLE_MEDIA_FILE),
                    Description = SAMPLE_DESCRIPTION,
                    Length = _random.Next(22,60),
                    SeasonNumber = (number % 3) + 1,
                    EpisodeNumber = number,
                    ReleaseDate = GenerateRandomDate(),
                    Cast = new ModelList<PersonModel>(GenerateCast("series" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GenerateCrew("series" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("series" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("series" + imageNumber)),
                };
                return item as T;
            }
            else if (typeof(T) == typeof(SeasonModel))
            {
                var item = new SeasonModel()
                {
                    ID = "season" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvSeries,
                    SeasonNumber = number,
                    UserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    Description = SAMPLE_DESCRIPTION
                };

                switch (item.SeasonNumber)
                {
                    case 1:
                        GenerateItem<TvEpisodeModel>(item.Episodes, 41);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 42);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 43);
                        break;
                    case 2:
                        GenerateItem<TvEpisodeModel>(item.Episodes, 44);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 45);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 46);
                        break;
                    default:
                        GenerateItem<TvEpisodeModel>(item.Episodes, 47);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 48);
                        GenerateItem<TvEpisodeModel>(item.Episodes, 49);
                        break;
                }

                return item as T;
            }

            throw new NotImplementedException();
        }

        private static void GenerateItem<T>(IList list, int number) where T : class
        {
            var item = GenerateItem<T>(number);
            if(item != null)
                list?.Add(item);
        }

        private static IEnumerable<PersonModel> GenerateCrew(string contentID)
        {
            return new ObservableCollection<PersonModel>
            {
                new PersonModel()
                {
                    ID = "crew1",
                    Name = "Don Hall",
                    Role = "Director / Writer",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_01.jpg",
                },
                new PersonModel()
                {
                    ID = "crew2",
                    Name = "Chris Williams",
                    Role = "Director / Writer",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_02.jpg",
                },
            };
        }

        private static IEnumerable<PersonModel> GenerateCast(string contentID)
        {
            return new ObservableCollection<PersonModel>
            {
                new PersonModel()
                {
                    ID = "cast1",
                    Name = "Ryan Porter",
                    Role = "Hiro",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_06.jpg",
                },
                new PersonModel()
                {
                    ID = "cast2",
                    Name = "Scott Adsit",
                    Role = "Baymax",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_05.jpg",
                },
                new PersonModel()
                {
                    ID = "cast3",
                    Name = "Jenny Chung",
                    Role = "Go Go",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_04.jpg",
                },
                new PersonModel()
                {
                    ID = "cast4",
                    Name = "Daniel Henry",
                    Role = "Tadeshi",
                    Image = SAMPLE_CAST_PATH_ROOT + "CastImage_03.jpg",
                },
            };
        }

        private static IEnumerable<ReviewModel> GetReviews(string contentID)
        {
            return new ObservableCollection<ReviewModel>
            {
                new ReviewModel()
                {
                    ID = "review1",
                    FullName = SAMPLE_NAME,
                    Review = SAMPLE_DESCRIPTION
                },
                new ReviewModel()
                {
                    ID = "review2",
                    FullName = SAMPLE_NAME,
                    Review = SAMPLE_DESCRIPTION
                },
                new ReviewModel()
                {
                    ID = "review3",
                    FullName = SAMPLE_NAME,
                    Review = SAMPLE_DESCRIPTION
                },
            };
        }

        private static IEnumerable<RatingModel> GetRatings(string contentID)
        {
            return new ObservableCollection<RatingModel>
            {
                new RatingModel()
                {
                    ID = "rating1",
                    RatingSource = "Rating Source 1",
                    RatingDetails = "Rating Source 1 critics",
                    RatingScore = 89,
                    RatingScale = 100
                },
                new RatingModel()
                {
                    ID = "rating2",
                    RatingSource = "Rating Source 2",
                    RatingDetails = "Based on 35 critic reviews",
                    RatingScore = 74,
                    RatingScale = 100
                },
                new RatingModel()
                {
                    ID = "rating3",
                    RatingSource = "Rating Source 3",
                    RatingDetails = "From 15,242 user reviews",
                    RatingScore = 8.1,
                    RatingScale = 10
                }
            };
        }

        private static DateTime GenerateRandomDate()
        {
            int day = _random.Next(1, 28);
            int month = _random.Next(1, 12);
            int year = _random.Next(2005, DateTime.Now.Year);

            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return DateTime.Today;
            }
        }

        #endregion

        #endregion
    }
}
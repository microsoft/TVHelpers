//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
        private const string SAMPLE_MEDIA_FILE = "wildlife.mp4";
        private static readonly Random _random = new Random(1234);

        #endregion

        #region Constructors

        public SampleLocalDataSource() 
            : base("http://api.contoso.com/v1/")
        {
        }

        #endregion

        #region Methods

        #region Search

        public Task<IEnumerable<ContentItemBase>> SearchAsync(string searchText, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Movies

        public Task<IEnumerable<MovieModel>> GetMoviesAsync(CancellationToken ct)
        {
            // create movies list
            var list = new ObservableCollection<MovieModel>();
            for (var x = 1; x <= 18; x++)
            {
                CreateAndAddItemToList<MovieModel>(list, x);
            }
            return Task.FromResult<IEnumerable<MovieModel>>(list);
        }

        public async Task<MovieModel> GetFeaturedHeroAsync(CancellationToken ct)
        {
            // curated featured hero
            var results = await this.GetMoviesAsync(ct);
            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesFeaturedAsync(CancellationToken ct)
        {
            // curated featured movies
            var list = new List<MovieModel>();
            var results = await this.GetMoviesAsync(ct);
            list.Add(results.First(o => o.ContentID == "movie02"));
            list.Add(results.First(o => o.ContentID == "movie18"));
            list.Add(results.First(o => o.ContentID == "movie12"));
            list.Add(results.First(o => o.ContentID == "movie13"));
            return list;
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesNewReleasesAsync(CancellationToken ct)
        {
            // curated new release movies
            return await this.GetMoviesAsync(ct);
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesTrailersAsync(CancellationToken ct)
        {
            // curate movie trailers
            var results = await this.GetMoviesAsync(ct);
            return results.OrderByDescending(o => o.Title);
        }

        public async Task<IEnumerable<ContentItemBase>> GetTrailersAsync(string contentID, CancellationToken ct)
        {
            // curate movie trailers
            var results = await this.GetMoviesAsync(ct);
            return results.OrderByDescending(o => o.Title);
        }

        #endregion

        #region TV

        public Task<IEnumerable<TvSeriesModel>> GetTvSeriesAsync(CancellationToken ct)
        {
            // create TV series
            var list = new ObservableCollection<TvSeriesModel>();
            for (var x = 19; x <= 36; x++)
            {
                CreateAndAddItemToList<TvSeriesModel>(list, x);
            }
            return Task.FromResult<IEnumerable<TvSeriesModel>>(list);
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvNewReleasesAsync(CancellationToken ct)
        {
            // curate new TV releases
            return await this.GetTvSeriesAsync(ct);
        }

        public async Task<IEnumerable<TvSeriesModel>> GetTvFeaturedAsync(CancellationToken ct)
        {
            // curate featured TV
            var results = await this.GetTvSeriesAsync(ct);
            var tvSeriesModels = results as IList<TvSeriesModel> ?? results.ToList();

            return new List<TvSeriesModel>
            {
                tvSeriesModels.First(o => o.ContentID == "series19"),
                tvSeriesModels.First(o => o.ContentID == "series20"),
                tvSeriesModels.First(o => o.ContentID == "series21"),
                tvSeriesModels.First(o => o.ContentID == "series22")
            };

            //return new List<TvSeriesModel>
            //{
            //    tvSeriesModels.First(o => o.ContentID == "series03"),
            //    tvSeriesModels.First(o => o.ContentID == "series44"),
            //    tvSeriesModels.First(o => o.ContentID == "series31"),
            //    tvSeriesModels.First(o => o.ContentID == "series32")
            //};
        }

        public IEnumerable<SeasonModel> GetSeasonsAsync(TvSeriesModel series, CancellationToken ct)
        {
            // curate seasons
            var list = new ModelList<SeasonModel>();
            CreateAndAddItemToList<SeasonModel>(list, 43);
            CreateAndAddItemToList<SeasonModel>(list, 44);
            CreateAndAddItemToList<SeasonModel>(list, 45);
            return list;
        }

        public Task<IEnumerable<ContentItemBase>> GetSneakPeeksAsync(CancellationToken ct)
        {
            // Curate inline content
            var list = new ContentItemList();
            CreateAndAddItemToList<TvEpisodeModel>(list, 37);
            CreateAndAddItemToList<TvEpisodeModel>(list, 38);
            CreateAndAddItemToList<TvEpisodeModel>(list, 39);
            CreateAndAddItemToList<TvEpisodeModel>(list, 40);
            CreateAndAddItemToList<TvEpisodeModel>(list, 41);
            CreateAndAddItemToList<TvEpisodeModel>(list, 42);
            return Task.FromResult<IEnumerable<ContentItemBase>>(list.ToArray());
        }

        public Task<IEnumerable<TvEpisodeModel>> GetTvEpisodesAsync(CancellationToken ct)
        {
            // Curate inline content
            var list = new ModelList<TvEpisodeModel>();
            CreateAndAddItemToList<TvEpisodeModel>(list, 56);
            CreateAndAddItemToList<TvEpisodeModel>(list, 57);
            CreateAndAddItemToList<TvEpisodeModel>(list, 58);
            CreateAndAddItemToList<TvEpisodeModel>(list, 59);
            CreateAndAddItemToList<TvEpisodeModel>(list, 60);
            CreateAndAddItemToList<TvEpisodeModel>(list, 61);
            return Task.FromResult<IEnumerable<TvEpisodeModel>>(list);
        }

        #endregion

        #region Queue

        public async Task<IEnumerable<QueueModel>> GetQueueItemsAsync(CancellationToken ct)
        {
            var movies = (await this.GetMoviesAsync(ct)).ToArray();
            var tv = (await this.GetTvSeriesAsync(ct)).ToArray();
            var episodes = (await GetTvEpisodesAsync(ct)).ToArray();
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

        public Task AddToQueueAsync(ContentItemBase item, CancellationToken ct)
        {
            return Task.FromResult<object>(null);
        }

        public Task RemoveFromQueueAsync(ContentItemBase item, CancellationToken ct)
        {
            return Task.FromResult<object>(null);
        }

        #endregion

        #region ContentItemBase

        public async Task<IEnumerable<ContentItemBase>> GetRelatedAsync(string id, CancellationToken ct)
        {
            // get some Content to populate Related

            if (id.Contains("series"))
            {
                var results = await this.GetTvSeriesAsync(ct);
                return results.OrderBy(o => o.Title);
            }
            else
            {
                var results = await this.GetMoviesAsync(ct);
                return results.OrderBy(o => o.Title);
            }
        }

        public async Task<ContentItemBase> GetContentItemAsync(string contentId, CancellationToken ct)
        {
            var results = await this.GetMoviesAsync(ct);
            return results.FirstOrDefault(s => s.ContentID == contentId);
        }

        public static T CreateAndAddItemToList<T>(int number) where T : class
        {
            var numberString = number.ToString();
            var imageNumber = number.ToString("00");

            if (typeof(T) == typeof(MovieModel))
            {
                var item = new MovieModel()
                {
                    ContentID = "movie" + imageNumber,
                    ContentRating = "G",
                    ItemType = ItemTypes.Movie,
                    Title = "Movie " + numberString,
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("PosterArt_2x3_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    Length = "120",
                    ReleaseDate = new DateTime(2015, 1, 15),
                    Cast = new ModelList<PersonModel>(GetCast("movie" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GetCrew("movie" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("movie" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("movie" + imageNumber)),
                    //Flag = "Just Added",
                };
                //if (number == 1) item.Flag = "Featured";
                return item as T;
            }

            if (typeof(T) == typeof(TvSeriesModel))
            {
                var item = new TvSeriesModel()
                {
                    ContentID = "series" + imageNumber,
                    ItemType = ItemTypes.TvSeries,
                    Title = "TV Series " + number.ToString(),
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("TVArt_1x1_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    ContentRating = "TV-13",
                    ReleaseDate = "2013 - 2014",
                    Length = "44",
                    Cast = new ModelList<PersonModel>(GetCast("series" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GetCrew("series" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("series" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("series" + imageNumber)),
                    //Flag = "Most Popular",
                };
                //item.Seasons = new ObservableCollection<SeasonModel>(this.GetSeasons(item));
                return item as T;
            }

            if (typeof(T) == typeof(TvEpisodeModel))
            {
                var item = new TvEpisodeModel()
                {
                    ContentID = "episode" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvEpisode,
                    Title = "Episode " + number.ToString() + " Title",
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = "Drama",
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("TVArt_1x1_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    Length = "60",
                    SeasonNumber = 3,
                    EpisodeNumber = 1,
                    AirDate = new DateTime(2015, 1, 15),
                    //Cast = new ObservableCollection<PersonModel>(GetCast("episode" + imageNumber)),
                    //Creators = new ObservableCollection<PersonModel>(GetCrew("episode" + imageNumber)),
                    //CriticReviews = new ObservableCollection<ReviewModel>(GetReviews("episode" + imageNumber)),
                    //ContentRatings = new ObservableCollection<ReviewModel>(GetRatings("episode" + imageNumber)),
                };


                return item as T;
            }

            if (typeof(T) == typeof(SeasonModel))
            {
                var item = new SeasonModel()
                {
                    ContentID = "season" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvSeries,
                    SeasonNumber = number,
                    UserRating = 3.5,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    //FeaturedImage = IMAGE_PATH_PREFIX + "featured/hero3_nightfal.png",
                    //MediaImage = IMAGE_PATH_PREFIX + "16-9movie/hero2.png",
                    //SquareThumbnailImage = IMAGE_PATH_PREFIX + "1-1tv/odysseyteam.png",
                    //LandscapeThumbnailImage = IMAGE_PATH_PREFIX + "2-1landscape/taxi.png",
                    Description = SAMPLE_DESCRIPTION
                };

                switch (item.SeasonNumber)
                {
                    case 1:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 41);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 42);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 43);
                        break;
                    case 2:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 44);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 45);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 46);
                        break;
                    default:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 47);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 48);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 49);
                        break;
                }

                return item as T;
            }

            return default(T);
        }

        public static void CreateAndAddItemToList<T>(IList list, int number) where T : class
        {
            var numberString = number.ToString();
            var imageNumber = number.ToString("00");

            if (typeof(T) == typeof(MovieModel))
            {
                var item = new MovieModel()
                {
                    ContentID = "movie" + imageNumber,
                    ContentRating = "G",
                    ItemType = ItemTypes.Movie,
                    Title = "Movie " + numberString,
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("PosterArt_2x3_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    Length = "120",
                    ReleaseDate = new DateTime(2015, 1, 15),
                    Cast = new ModelList<PersonModel>(GetCast("movie" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GetCrew("movie" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("movie" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("movie" + imageNumber)),
                    //Flag = "Just Added",
                };
                //if (number == 1) item.Flag = "Featured";
                list.Add(item);
            }

            #region TV Series

            if (typeof(T) == typeof(TvSeriesModel))
            {
                var item = new TvSeriesModel()
                {
                    ContentID = "series" + imageNumber,
                    ItemType = ItemTypes.TvSeries,
                    Title = "TV Series " + (list.Count + 1).ToString(),
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("TVArt_1x1_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    ContentRating = "TV-13",
                    ReleaseDate = "2013 - 2014",
                    Length = "44",
                    Cast = new ModelList<PersonModel>(GetCast("series" + imageNumber)),
                    Creators = new ModelList<PersonModel>(GetCrew("series" + imageNumber)),
                    CriticReviews = new ModelList<ReviewModel>(GetReviews("series" + imageNumber)),
                    ContentRatings = new ModelList<RatingModel>(GetRatings("series" + imageNumber)),
                    //Flag = "Most Popular",
                };
                //item.Seasons = new ObservableCollection<SeasonModel>(this.GetSeasons(item));
                list.Add(item);
            }

            #endregion

            if (typeof(T) == typeof(TvEpisodeModel))
            {
                var item = new TvEpisodeModel()
                {
                    ContentID = "episode" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvEpisode,
                    Title = "Episode " + (list.Count + 1).ToString() + " Title",
                    UserRating = 0,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = "Drama",
                    FeaturedImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("FeaturedImage_2x1_Image{0}.jpg", imageNumber),
                    MediaImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("MediaTileTall_16x9_Image{0}.jpg", imageNumber),
                    LandscapeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("LandScape_2x1_Image{0}.jpg", imageNumber),
                    PosterImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("TVArt_1x1_Image{0}.jpg", imageNumber),
                    ResumeImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("ResumeTile_16x9_Image{0}.jpg", imageNumber),
                    InlineImage = SAMPLE_IMAGE_PATH_ROOT + string.Format("Inline_16x9_Image{0}.jpg", imageNumber),
                    URL = SAMPLE_MEDIA_PATH_ROOT + SAMPLE_MEDIA_FILE,
                    Description = SAMPLE_DESCRIPTION,
                    Length = "60",
                    SeasonNumber = 3,
                    EpisodeNumber = 1,
                    AirDate = new DateTime(2015, 1, 15),
                    //Cast = new ObservableCollection<PersonModel>(GetCast("episode" + imageNumber)),
                    //Creators = new ObservableCollection<PersonModel>(GetCrew("episode" + imageNumber)),
                    //CriticReviews = new ObservableCollection<ReviewModel>(GetReviews("episode" + imageNumber)),
                    //ContentRatings = new ObservableCollection<ReviewModel>(GetRatings("episode" + imageNumber)),
                };


                list.Add(item);
            }

            if (typeof(T) == typeof(SeasonModel))
            {
                var item = new SeasonModel()
                {
                    ContentID = "season" + imageNumber,
                    ContentRating = "TV-G",
                    ItemType = ItemTypes.TvSeries,
                    SeasonNumber = (list.Count + 1),
                    UserRating = 3.5,
                    AverageUserRating = 3.0 + _random.NextDouble() * 2.0,
                    Genre = ((Genres)_random.Next(6)).ToString(),
                    //FeaturedImage = IMAGE_PATH_PREFIX + "featured/hero3_nightfal.png",
                    //MediaImage = IMAGE_PATH_PREFIX + "16-9movie/hero2.png",
                    //SquareThumbnailImage = IMAGE_PATH_PREFIX + "1-1tv/odysseyteam.png",
                    //LandscapeThumbnailImage = IMAGE_PATH_PREFIX + "2-1landscape/taxi.png",
                    Description = SAMPLE_DESCRIPTION
                };

                switch (item.SeasonNumber)
                {
                    case 1:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 41);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 42);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 43);
                        break;
                    case 2:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 44);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 45);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 46);
                        break;
                    default:
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 47);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 48);
                        CreateAndAddItemToList<TvEpisodeModel>(item.Episodes, 49);
                        break;
                }

                list.Add(item);
            }
        }

        #endregion ContentItemBase
        
        #region Ratings/Reviews/Casts

        public Task<IEnumerable<RatingModel>> GetRatingsAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<RatingModel>>(GetRatings(contentID));
        }
        public static IEnumerable<RatingModel> GetRatings(string contentID)
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

        public Task<IEnumerable<ReviewModel>> GetReviewsAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<ReviewModel>>(GetReviews(contentID));
        }
        public static IEnumerable<ReviewModel> GetReviews(string contentID)
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

        public Task<IEnumerable<PersonModel>> GetCastAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<PersonModel>>(GetCast(contentID));
        }
        public static IEnumerable<PersonModel> GetCast(string contentID)
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

        public Task<IEnumerable<PersonModel>> GetCrewAsync(string contentID, CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<PersonModel>>(GetCrew(contentID));
        }
        private static IEnumerable<PersonModel> GetCrew(string contentID)
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

        #endregion
    }
}

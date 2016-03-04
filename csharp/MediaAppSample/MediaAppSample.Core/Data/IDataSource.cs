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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Data
{
    /// <summary>
    /// Interface defining all the functionality that any data source implementation must provide.
    /// </summary>
    public interface IDataSource
    {
        #region Generic

        Task<ContentItemBase> GetContentItem(string contentId);
        Task<IEnumerable<ContentItemBase>> GetRelated(string contentID);
        Task<IEnumerable<ContentItemBase>> GetTrailers(string contentID);

        #endregion

        #region Movies

        Task<IEnumerable<MovieModel>> GetMovies();
        Task<MovieModel> GetMovieHero();
        Task<IEnumerable<MovieModel>> GetMoviesFeatured();
        Task<IEnumerable<MovieModel>> GetMoviesNewReleases();
        Task<IEnumerable<MovieModel>> GetMoviesTrailers();

        #endregion

        #region TV

        Task<IEnumerable<TvSeriesModel>> GetTvSeries();
        Task<TvSeriesModel> GetTvHero();
        Task<IEnumerable<TvSeriesModel>> GetTvFeatured();
        Task<IEnumerable<TvSeriesModel>> GetTvNewReleases();
        Task<IEnumerable<TvEpisodeModel>> GetTvInline();
        Task<IEnumerable<TvEpisodeModel>> GetTvEpisodes();
        //Task<IEnumerable<SeasonModel>> GetSeasons(TvSeriesModel series);
        IEnumerable<SeasonModel> GetSeasons(TvSeriesModel series);

        #endregion

        #region Queue

        Task<IEnumerable<QueueModel>> GetQueue();
        Task AddToQueue(ContentItemBase item);
        Task RemoveFromQueue(ContentItemBase item);

        #endregion
    }
}

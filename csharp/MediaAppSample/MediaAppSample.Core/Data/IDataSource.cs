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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Data
{
    /// <summary>
    /// Interface defining all the functionality that any data source implementation must provide.
    /// </summary>
    public interface IDataSource
    {
        #region Account 

        Task<UserResponse> AuthenticateAsync(AccountSignInViewModel vm, CancellationToken ct);
        Task<UserResponse> AuthenticateAsync(Services.WebAccountManager.WebAccountInfo wi, CancellationToken ct);
        Task<UserResponse> RegisterAsync(AccountSignUpViewModel vm, CancellationToken ct);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(AccountForgotViewModel vm, CancellationToken ct);

        #endregion

        #region Content

        Task<ContentItemBase> GetContentItem(string contentId, CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetRelated(string contentID, CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetTrailers(string contentID, CancellationToken ct);
        
        Task<IEnumerable<MovieModel>> GetMovies(CancellationToken ct);
        Task<MovieModel> GetMovieHero(CancellationToken ct);
        Task<IEnumerable<MovieModel>> GetMoviesFeatured(CancellationToken ct);
        Task<IEnumerable<MovieModel>> GetMoviesNewReleases(CancellationToken ct);
        Task<IEnumerable<MovieModel>> GetMoviesTrailers(CancellationToken ct);
        
        Task<IEnumerable<TvSeriesModel>> GetTvSeries(CancellationToken ct);
        Task<TvSeriesModel> GetTvHero(CancellationToken ct);
        Task<IEnumerable<TvSeriesModel>> GetTvFeatured(CancellationToken ct);
        Task<IEnumerable<TvSeriesModel>> GetTvNewReleases(CancellationToken ct);
        Task<IEnumerable<TvEpisodeModel>> GetTvInline(CancellationToken ct);
        Task<IEnumerable<TvEpisodeModel>> GetTvEpisodes(CancellationToken ct);
        //Task<IEnumerable<SeasonModel>> GetSeasons(TvSeriesModel series);
        IEnumerable<SeasonModel> GetSeasons(TvSeriesModel series, CancellationToken ct);

        #endregion

        #region Queue

        Task<IEnumerable<QueueModel>> GetQueue(CancellationToken ct);
        Task AddToQueue(ContentItemBase item, CancellationToken ct);
        Task RemoveFromQueue(ContentItemBase item, CancellationToken ct);

        #endregion

        #region Search

        Task<IEnumerable<ContentItemBase>> SearchItems(string searchText, CancellationToken ct);

        #endregion
    }
}

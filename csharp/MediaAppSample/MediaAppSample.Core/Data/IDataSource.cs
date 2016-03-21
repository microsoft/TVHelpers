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

        Task<IEnumerable<ContentItemBase>> GetItemsAsync(ItemTypes type, CancellationToken ct);
        Task<ContentItemBase> GetItemAsync(string itemID, CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetRelatedAsync(string id, CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetTrailersAsync(string id, CancellationToken ct);

        IEnumerable<SeasonModel> GetItemsAsync(TvSeriesModel series, CancellationToken ct);

        #endregion

        #region Home 

        Task<ContentItemBase> GetFeaturedHeroAsync(CancellationToken ct);

        Task<IEnumerable<MovieModel>> GetMoviesFeaturedAsync(CancellationToken ct);
        Task<IEnumerable<MovieModel>> GetMoviesNewReleasesAsync(CancellationToken ct);
        
        Task<IEnumerable<TvSeriesModel>> GetTvFeaturedAsync(CancellationToken ct);
        Task<IEnumerable<TvSeriesModel>> GetTvNewReleasesAsync(CancellationToken ct);

        Task<IEnumerable<ContentItemBase>> GetSneakPeeksAsync(CancellationToken ct);

        #endregion

        #region People/Reviews

        Task<IEnumerable<ReviewModel>> GetReviewsAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<RatingModel>> GetRatingsAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<PersonModel>> GetCastAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<PersonModel>> GetCrewAsync(string itemID, CancellationToken ct);

        #endregion

        #region Queue

        Task<IEnumerable<QueueModel>> GetQueueItemsAsync(CancellationToken ct);
        Task AddToQueueAsync(ContentItemBase item, CancellationToken ct);
        Task RemoveFromQueueAsync(ContentItemBase item, CancellationToken ct);

        #endregion

        #region Search

        Task<IEnumerable<ContentItemBase>> SearchAsync(string searchText, CancellationToken ct);

        #endregion

        #region Recommended

        Task<IEnumerable<ContentItemBase>> GetRecommendedItemsAsync(CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetFriendsWatchedItemsAsync(CancellationToken ct);

        #endregion
    }
}

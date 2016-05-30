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

        Task<IEnumerable<ContentItemBase>> GetContentItemsAsync(ItemTypes type, CancellationToken ct);
        Task<ContentItemBase> GetItemAsync(string itemID, CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetRelatedAsync(string id, CancellationToken ct);
        IEnumerable<SeasonModel> GetItemsAsync(TvSeriesModel series, CancellationToken ct);
        Task<IEnumerable<TrailerModel>> GetTrailersAsync(CancellationToken ct);
        Task<IEnumerable<TrailerModel>> GetTrailersAsync(string id, CancellationToken ct);
        Task<ContentItemBase> GetResumeItem(string id, ItemTypes type, CancellationToken ct);

        #endregion

        #region Home 

        Task<ContentItemBase> GetFeaturedItemAsync(CancellationToken ct);

        Task<IEnumerable<MovieModel>> GetMoviesFeaturedAsync(CancellationToken ct);
        Task<IEnumerable<MovieModel>> GetMoviesNewReleasesAsync(CancellationToken ct);
        
        Task<IEnumerable<TvSeriesModel>> GetTvFeaturedAsync(CancellationToken ct);
        Task<IEnumerable<TvSeriesModel>> GetTvNewReleasesAsync(CancellationToken ct);
        
        #endregion

        #region People/Reviews

        Task<IEnumerable<ReviewModel>> GetReviewsAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<RatingModel>> GetRatingsAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<PersonModel>> GetCastAsync(string itemID, CancellationToken ct);

        Task<IEnumerable<PersonModel>> GetCrewAsync(string itemID, CancellationToken ct);

        #endregion

        #region Queue

        Task<IEnumerable<QueueModel>> GetQueueItemsAsync(string userID, CancellationToken ct);
        Task AddToQueueAsync(string userID, ContentItemBase item, CancellationToken ct);
        Task RemoveFromQueueAsync(string userID, ContentItemBase item, CancellationToken ct);

        #endregion

        #region Search

        Task<IEnumerable<ContentItemBase>> SearchAsync(string searchText, CancellationToken ct);

        #endregion

        #region Recommended

        Task<IEnumerable<ContentItemBase>> GetRecommendedItemsAsync(CancellationToken ct);
        Task<IEnumerable<ContentItemBase>> GetFriendsWatchedItemsAsync(CancellationToken ct);

        #endregion

        #region Voice Integration

        Task<IEnumerable<string>> GetTilesForVoiceIntegration(CancellationToken ct);

        #endregion
    }
}

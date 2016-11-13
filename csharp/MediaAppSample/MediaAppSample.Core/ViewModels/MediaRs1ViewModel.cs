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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.Threading;
using MediaAppSample.Core.Data;
using System;
using System.Linq;

namespace MediaAppSample.Core.ViewModels
{
    public partial class MediaRs1ViewModel : ViewModelBase
    {
        #region Properties

        private ContentItemBase _Item;
        public ContentItemBase Item
        {
            get { return _Item; }
            private set { this.SetProperty(ref _Item, value); }
        }

        #endregion

        #region Constructors

        public MediaRs1ViewModel()
        {
            this.Title = "Media";

            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;
        }

        #endregion Constructors

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                if(this.ViewParameter is string && !this.ViewParameter.ToString().Equals(this.Item?.ID, StringComparison.CurrentCultureIgnoreCase))
                    await this.RefreshAsync();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);
                this.Item = await DataSource.Current.GetItemAsync(this.ViewParameter.ToString(), ct);
                if (this.Item == null)
                    this.Item = (await DataSource.Current.SearchAsync(this.ViewParameter.ToString(), ct)).FirstOrDefault();
                this.Title = this.Item?.Title;
                this.ClearStatus();
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while trying to load data for ID '{0}'", this.ViewParameter);
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
            }

            await base.OnRefreshAsync(ct);
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        #endregion Methods
    }

    public partial class MediaRs1ViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public MediaRs1ViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class MediaRs1ViewModel : MediaAppSample.Core.ViewModels.MediaRs1ViewModel
    {
        public MediaRs1ViewModel()
        {
        }
    }
}
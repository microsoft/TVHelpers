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
using System;

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Command for pinning tiles.
    /// </summary>
    public sealed class PinTileCommand : GenericCommand<IModel>
    {
        #region Properties

        /// <summary>
        /// Custom action to perform after Execute runs successfully.
        /// </summary>
        public Action OnSuccessAction { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command instance for pinning IModel objects to the user's start screen.
        /// </summary>
        public PinTileCommand()
            : base("PinTileCommand", null, Platform.Current.Notifications.HasTile)
        {
        }

        #endregion Constructors

        #region Methods

        public async override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (parameter is IModel)
            {
                // Create tile
                if (await Platform.Current.Notifications.CreateOrUpdateTileAsync(parameter as IModel))
                {
                    // Tile created, execute post-create actions
                    this.RaiseCanExecuteChanged();
                    if (this.OnSuccessAction != null)
                        this.OnSuccessAction();
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            return !base.CanExecute(parameter);
        }

        #endregion
    }

    /// <summary>
    /// Command for unpinning tiles.
    /// </summary>
    public sealed class UnpinTileCommand : GenericCommand<IModel>
    {
        #region Properties

        /// <summary>
        /// Custom action to perform after Execute runs successfully.
        /// </summary>
        public Action OnSuccessAction { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new command instance for removing IModel objects from a user's start screen.
        /// </summary>
        public UnpinTileCommand()
            : base("UnpinTileCommand", null, Platform.Current.Notifications.HasTile)
        {
        }

        #endregion Constructors

        #region Methods

        public async override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (parameter is IModel)
            {
                // Delete tile
                if (await Platform.Current.Notifications.DeleteTileAsync(parameter as IModel))
                {
                    // Tile was deleted, execute post delete actions
                    this.RaiseCanExecuteChanged();
                    if (this.OnSuccessAction != null)
                        this.OnSuccessAction();
                }
            }
        }

        #endregion
    }
}
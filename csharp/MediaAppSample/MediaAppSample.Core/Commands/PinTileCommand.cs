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
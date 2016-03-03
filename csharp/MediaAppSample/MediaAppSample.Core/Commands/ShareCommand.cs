using MediaAppSample.Core.Models;

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ShareCommand : GenericCommand<IModel>
    {
        #region Constructors

        /// <summary>
        /// Creates a new command instance for sharing IModel objects to other apps.
        /// </summary>
        public ShareCommand()
            : base("ShareCommand", Platform.Current.Navigation.Share)
        {
        }

        #endregion
    }
}
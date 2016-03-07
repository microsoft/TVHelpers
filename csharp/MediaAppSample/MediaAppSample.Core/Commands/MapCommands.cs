using MediaAppSample.Core.Models;

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Command for launching an external maps app passing an ILocationModel command parameter instance passed to this command.
    /// </summary>
    public sealed class MapExternalCommand : GenericCommand<ILocationModel>
    {
        #region Constructors

        public MapExternalCommand(MapExternalOptions option = MapExternalOptions.Normal)
            : base("MapExternalCommand-" + option, (loc) => Platform.Current.Navigation.MapExternal(loc, loc?.LocationDisplayName, option))
        {
        }

        #endregion
    }
}
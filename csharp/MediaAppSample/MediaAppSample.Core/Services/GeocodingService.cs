using MediaAppSample.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the geocoding service adapter implement of the platform currently executing.
        /// </summary>
        public GeocodingService Geocode
        {
            get { return this.GetAdapter<GeocodingService>(); }
            protected set { this.Register<GeocodingService>(value); }
        }
    }

    /// <summary>
    /// Base class providing access to the geocoding service on the executing platform.
    /// </summary>
    public sealed class GeocodingService : ServiceBase
    {
        #region Constructors

        internal GeocodingService()
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Retrieves a formatted address for a given location.
        /// </summary>
        /// <param name="loc">Location to geocode.</param>
        /// <returns>String name representing the specified location.</returns>
        public async Task<string> GetAddressAsync(ILocationModel loc, CancellationToken? ct = null)
        {
            // Reverse geocode the specified geographic location.
            var result = await MapLocationFinder.FindLocationsAtAsync(loc?.AsGeoPoint()).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
                return loc.LocationDisplayName = result?.Locations[0].Address?.FormattedAddress;
            else
                return loc.LocationDisplayName = null;
        }

        /// <summary>
        /// Retrieves the city and state for a given location.
        /// </summary>
        /// <param name="loc">Location to geocode.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>String name representing the specified location.</returns>
        public async Task<string> GetCityStateAsync(ILocationModel loc, CancellationToken? ct = null)
        {
            // Reverse geocode the specified geographic location.
            var result = await MapLocationFinder.FindLocationsAtAsync(loc?.AsGeoPoint()).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
                return loc.LocationDisplayName = this.ConcatAddressParts(2, result.Locations[0].Address?.Town, result.Locations[0].Address?.Region, result.Locations[0].Address?.Country);
            else
                return loc.LocationDisplayName = null;
        }

        /// <summary>
        /// Retrieves the city, state, and country for a given location.
        /// </summary>
        /// <param name="loc">Location to geocode.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>String name representing the specified location.</returns>
        public async Task<string> GetCityStateCountryAsync(ILocationModel loc, CancellationToken? ct = null)
        {
            // Reverse geocode the specified geographic location.
            var result = await MapLocationFinder.FindLocationsAtAsync(loc?.AsGeoPoint()).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
                return loc.LocationDisplayName = this.ConcatAddressParts(3, result.Locations[0].Address?.Town, result.Locations[0].Address?.Region, result.Locations[0].Address?.Country, result.Locations[0].Address?.Continent);
            else
                return loc.LocationDisplayName = null;
        }

        /// <summary>
        /// Retrieves the country name for a given location.
        /// </summary>
        /// <param name="loc">Location to geocode.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>String name representing the specified location.</returns>
        public async Task<string> GetCountryAsync(ILocationModel loc, CancellationToken? ct = null)
        {
            // Reverse geocode the specified geographic location.
            var result = await MapLocationFinder.FindLocationsAtAsync(loc?.AsGeoPoint()).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
                return loc.LocationDisplayName = this.ConcatAddressParts(1, result.Locations[0].Address?.Country, result.Locations[0].Address?.Continent);
            else
                return loc.LocationDisplayName = null;
        }

        #endregion

        #region Private 

        /// <summary>
        /// Concatonates strings for user display with comma separation.
        /// </summary>
        /// <param name="parts">Strings to concat.</param>
        /// <returns>Concatonated string for user display.</returns>
        private string ConcatAddressParts(params string[] parts)
        {
            if (parts == null)
                return null;
            else
                return this.ConcatAddressParts(parts.Length, parts);
        }

        /// <summary>
        /// Concatonates strings for user display with comma separation.
        /// </summary>
        /// <param name="maxParts">Max number of string parts to use from the array.</param>
        /// <param name="parts">Strings to concat.</param>
        /// <returns>Concatonated string for user display.</returns>
        private string ConcatAddressParts(int maxParts, params string[] parts)
        {
            if (parts == null)
                return null;
            else
            {
                var list = parts.Where(w => !string.IsNullOrWhiteSpace(w));
                if (maxParts > 0)
                    list = list.Take(maxParts);
                return string.Join(", ", list);
            }
        }

        #endregion

        #endregion
    }
}

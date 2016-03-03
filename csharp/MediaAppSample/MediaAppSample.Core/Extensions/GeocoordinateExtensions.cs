using MediaAppSample.Core.Models;
using Windows.Devices.Geolocation;

public static class GeocoordinateExtensions
{
    /// <summary>
    /// Converts this Geocoordinate object into a LocationModel instance.
    /// </summary>
    /// <param name="coord">Geocoordinate to convert to LocationModel.</param>
    /// <returns>LocationModel instance representing this Geocoordinate.</returns>
    public static LocationModel AsLocationModel(this Geocoordinate coord)
    {
        if (coord != null)
        {
            return new LocationModel()
            {
                Latitude = coord.Point.Position.Latitude,
                Longitude = coord.Point.Position.Longitude
            };
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a ILocationModel into a BasicGeoposition object.
    /// </summary>
    /// <param name="loc">ILocationModel to convert to BasicGeoposition.</param>
    /// <returns>BasicGeoposition instance representing this ILocationModel.</returns>
    public static BasicGeoposition AsBasicGeoposition(this ILocationModel loc)
    {
        return new BasicGeoposition()
        {
            Latitude = loc.Latitude,
            Longitude = loc.Longitude
        };
    }

    /// <summary>
    /// Converts a ILocationModel into a GeoPoint object.
    /// </summary>
    /// <param name="loc">ILocationModel to convert to Geopoint.</param>
    /// <returns>Geopoint instance representing this ILocationModel.</returns>
    public static Geopoint AsGeoPoint(this ILocationModel loc)
    {
        return new Geopoint(loc.AsBasicGeoposition());
    }
}

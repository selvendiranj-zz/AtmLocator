export interface AtmLocation
{
    Address: AtmAddress;
    Distance: number;
    Type: string;
}

interface AtmAddress
{
    Street: string;
    HouseNumber: string;
    PostalCode: string;
    City: string;
    GeoLocation: GeoLocation;
}

interface GeoLocation
{
    Lat: number;
    Lng: number;
}

export interface AtmSimplified
{
    Street: string;
    HouseNumber: string;
    PostalCode: string;
    City: string;
}

export interface AtmLocation
{
    address: AtmAddress;
    distance: number;
    type: string;
}

interface AtmAddress
{
    street: string;
    houseNumber: string;
    postalCode: string;
    city: string;
    geoLocation: GeoLocation;
}

interface GeoLocation
{
    lat: number;
    lng: number;
}

export interface AtmSimplified
{
    street: string;
    houseNumber: string;
    postalCode: string;
    city: string;
}

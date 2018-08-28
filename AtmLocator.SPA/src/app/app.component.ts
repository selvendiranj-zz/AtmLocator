import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AtmLocation } from './AtmLocation';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit
{
    title = 'atm-locator';
    @ViewChild('mapcanvasfocused', { read: ElementRef })
    mapcanvasfocused: ElementRef<any>;
    atmLocatorUrl = 'https://atmlocatorfunctionapp.azurewebsites.net/api/GetAtmsAllRaw';
    atmLocsConst: AtmLocation[];
    atmLocations: AtmLocation[];
    atmLocation: AtmLocation;
    myPosition: any;

    inpCity = '';
    searchedCity = 'ATM Locations for all cities';

    constructor(private http: HttpClient)
    {
        this.http = http;
    }

    ngAfterViewInit(): void
    {
        this.http.get(this.atmLocatorUrl)
            .subscribe((data: AtmLocation[]) =>
            {
                this.atmLocations = this.atmLocsConst = data;
            });

        // The location of Toronto
        const toronto = {
            lat: 43.6532,
            lng: -79.3832
        };
        // The map, centered at Toronto
        const map = new google.maps.Map(document.getElementById('mapcanvas'), {
            zoom: 8,
            center: toronto
        });
        // The marker, positioned at Toronto
        const marker = new google.maps.Marker({
            position: toronto,
            map: map
        });
    }

    public atmLocationSelected(atmLocation: AtmLocation)
    {
        this.atmLocation = atmLocation;
        // The selected location
        const selPos = {
            lat: atmLocation.address.geoLocation.lat,
            lng: atmLocation.address.geoLocation.lng
        };
        // The map, centered at selected location
        const map = new google.maps.Map(document.getElementById('mapcanvas'), {
            zoom: 5,
            center: selPos
        });
        // The marker, positioned at selected location
        const marker = new google.maps.Marker({
            position: selPos,
            map: map,
            title: 'Click to zoom'
        });

        marker.addListener('click', function ()
        {
            map.setZoom(7);
            map.setCenter(marker.getPosition());
        });
    }

    public getDirections()
    {
        const that = this;
        this.mapcanvasfocused.nativeElement.scrollTop += 20;

        // Try HTML5 geolocation.
        if (navigator.geolocation)
        {
            navigator.geolocation.getCurrentPosition(function (position)
            {
                // my current location
                let myPos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };

                // TODO: remove - hardcoded to nearest position
                const locCount = that.atmLocations.length;
                myPos = {
                    lat: that.atmLocations[locCount - 1].address.geoLocation.lat,
                    lng: that.atmLocations[locCount - 1].address.geoLocation.lng
                };
                // remove - hardcoded to nearest position

                that.myPosition = myPos;

                const selPos = {
                    lat: that.atmLocation.address.geoLocation.lat,
                    lng: that.atmLocation.address.geoLocation.lng
                };

                // get source and current location and open map application
                const saddr = `${selPos.lat},${selPos.lng}`;
                const daddr = `${that.myPosition.lat},${that.myPosition.lng}`;
                that.showDirections(saddr, daddr);

            }, this.getLocationError);
        } else
        {
            // Browser doesn't support Geolocation
            this.getLocationError();
        }
    }

    public getLocationError()
    {
        console.log('Error: The Geolocation service failed.' +
            'Error: Your browser doesn\'t support geolocation.');
    }

    public searchCity()
    {
        if (this.inpCity === '')
        {
            this.atmLocations = this.atmLocsConst;
        }
        else
        {
            this.searchedCity = 'ATM Locations for city ' + this.inpCity;
            const locations = this.atmLocsConst.filter(
                loc => loc.address.city === this.inpCity);
            this.atmLocations = locations;

            if (locations.length === 0)
            {
                this.searchedCity = 'No ATM found in the searched city';
                return;
            }
            let map = null;
            let latLng = {
                lat: locations[0].address.geoLocation.lat,
                lng: locations[0].address.geoLocation.lng
            };

            map = new google.maps.Map(document.getElementById('mapcanvas'), {
                zoom: 10,
                center: latLng,
                mapTypeId: 'terrain'
            });

            for (let i = 0; i < locations.length; i++)
            {
                latLng = {
                    lat: locations[i].address.geoLocation.lat,
                    lng: locations[i].address.geoLocation.lng
                };
                // latLng = new google.maps.LatLng(latLng);
                const markers = new google.maps.Marker({
                    position: latLng,
                    map: map
                });
            }
        }
    }

    public showDirections(saddr: string, daddr: string)
    {
        const directionsService = new google.maps.DirectionsService;
        const directionsDisplay = new google.maps.DirectionsRenderer;
        const myLatlng = { lat: 52.3702, lng: 4.8952 };
        const map = new google.maps.Map(document.getElementById('mapcanvas'), {
            zoom: 7,
            center: myLatlng
        });

        const marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            title: 'Click to zoom'
        });

        directionsDisplay.setMap(map);
        this.calculateAndDisplayRoute(directionsService, directionsDisplay);

        map.addListener('click', function ()
        {
            /* if we're on iOS, open in Apple Maps */
            if ((navigator.platform.indexOf('iPhone') !== -1) ||
                (navigator.platform.indexOf('iPod') !== -1) ||
                (navigator.platform.indexOf('iPad') !== -1))
            {
                window.open(`maps://maps.google.com/maps?saddr=${saddr}&daddr=${daddr}`);
            }

            else /* else use Google */
            {
                window.open(`https://maps.google.com/maps?saddr=${saddr}&daddr=${daddr}`);
            }
        });

    }

    public calculateAndDisplayRoute(directionsService, directionsDisplay)
    {
        const selPos = {
            lat: this.atmLocation.address.geoLocation.lat,
            lng: this.atmLocation.address.geoLocation.lng
        };

        const myPos = {
            lat: this.myPosition.lat,
            lng: this.myPosition.lng
        };

        directionsService.route({
            origin: myPos,
            destination: selPos,
            travelMode: 'DRIVING'
        }, function (response, status)
            {
                if (status === 'OK')
                {
                    directionsDisplay.setDirections(response);
                } else
                {
                    window.alert('Directions request failed due to ' + status);
                }
            });
    }
}

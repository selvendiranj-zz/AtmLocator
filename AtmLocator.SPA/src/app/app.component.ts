import { Component, OnInit, AfterViewInit, Input } from '@angular/core';
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
    atmLocatorUrl = 'https://atmlocatorfass.azurewebsites.net/api/GetAtmsAllRaw';
    atmLocsConst: any;
    atmLocations: any;
    atmLocation: any;
    myLocation: any;

    inpCity = '';
    searchedCity = 'ATM Locations for all cities';

    constructor(private http: HttpClient)
    {
        this.http = http;
    }

    ngAfterViewInit(): void
    {
        this.http.get(this.atmLocatorUrl)
            .subscribe((data: any) =>
            {
                this.atmLocations = this.atmLocsConst = data;
            });

        // The location of Uluru
        const uluru = {
            lat: 43.6532,
            lng: -79.3832
        };
        // The map, centered at Uluru
        const map = new google.maps.Map(document.getElementById('mapcanvas'), {
            zoom: 8,
            center: uluru
        });
        // The marker, positioned at Uluru
        const marker = new google.maps.Marker({
            position: uluru,
            map: map
        });
    }

    public atmLocationSelected(atmLocation: any)
    {
        this.atmLocation = atmLocation;
        // The location of Uluru
        const uluru = {
            lat: atmLocation.address.geoLocation.lat,
            lng: atmLocation.address.geoLocation.lng
        };
        // The map, centered at Uluru
        const map = new google.maps.Map(document.getElementById('mapcanvas'), {
            zoom: 5,
            center: uluru
        });
        // The marker, positioned at Uluru
        const marker = new google.maps.Marker({
            position: uluru,
            map: map
        });
    }

    public getDirections()
    {
        let map: any;
        let infoWindow: any;
        const that = this;

        map = new google.maps.Map(document.getElementById('mapcanvas'), {
            center: {
                lat: that.atmLocations[that.atmLocations.length - 1].address.geoLocation.lat,
                lng: that.atmLocations[that.atmLocations.length - 1].address.geoLocation.lng
            },
            zoom: 6
        });
        infoWindow = new google.maps.InfoWindow;

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

                // temp hardcoded to  nearest position
                myPos = {
                    lat: that.atmLocations[2].address.geoLocation.lat,
                    lng: that.atmLocations[2].address.geoLocation.lng
                };
                that.myLocation = myPos;
                const selPos = {
                    lat: that.atmLocation.address.geoLocation.lat,
                    lng: that.atmLocation.address.geoLocation.lng
                };
                const url = `https://google.com/maps/dir/${selPos.lat},${selPos.lng}/${that.myLocation.lat},${that.myLocation.lng}`;
                window.open(url);

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
}

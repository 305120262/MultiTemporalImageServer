require([
    "esri/Map",
    "esri/views/MapView",
    "esri/layers/BaseTileLayer",
    "dojo/dom",
    "dojo/on"
], function (Map, MapView, BaseTileLayer,dom,on) {


    var MyCustomTileLayer = BaseTileLayer.createSubclass({
        // properties of the custom tile layer
        properties: {
            urlTemplate: null,
            tm: 0
        },

        // override getTileUrl()
        // generate the tile url for a given level, row and column
        getTileUrl: function (level, row, col) {
            return this.urlTemplate.replace("{tm}", this.tm).replace("{z}", level).replace("{x}", col).replace("{y}", row);
        }
    });

    var temporalLayer = new MyCustomTileLayer({
        urlTemplate: "http://localhost:3000/api/{tm}/{z}/{y}/{x}",
        tm:1144
    });
    var map = new Map({
        layers: [temporalLayer],
        basemap: "streets"
    });
    var view = new MapView({
        container: "viewDiv",  // Reference to the DOM node that will contain the view
        map: map,             // References the map object created in step 3
        zoom: 4
    });

    var tm = dom.byId("tm");
    on(tm, "change", function (e) {
        temporalLayer.tm = e.target.value;
        temporalLayer.refresh();
    }.bind(this));
});

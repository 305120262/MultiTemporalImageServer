var MyCustomTileLayer = BaseTileLayer.createSubclass({
    // properties of the custom tile layer
    properties: {
        urlTemplate: "http://localhost:3000/api/{ts}/{z}/{y}/{x}",
        ts:1143
    },

    // override getTileUrl()
    // generate the tile url for a given level, row and column
    getTileUrl: function (level, row, col) {
        return this.urlTemplate.replace("{z}", level).replace("{x}", col).replace("{y}", row);
    }
});
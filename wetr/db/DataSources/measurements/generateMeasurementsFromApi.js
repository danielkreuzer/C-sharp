//API Documentation:
//http://at-wetter.tk/index.php?men=api
//All available stations:
//http://at-wetter.tk/api/v1/stations

const request = require('sync-request');
const csv = require('node-csv').createParser(';');
const fs = require('fs');

/////////////////////////////////////////

// change these values:

const fileName = './measurement.sql';

const numberOfStations = 130; // how many stations should receive data
const daysFromDate = 75; // data for how many days will be inserted for each stations

// 130 stations and 2 days into the past: 50k entries
// 130 stations and 30 days into the past: 500k entries
// 130 stations and 75 days into the past: 1.2 million entries


// ids for the stations on the api-server
const ids = [11157, 11244, 11101, 11190, 11, 11155, 11240, 11121, 11331, 11012, 11130, 11204,
    11010, 11171, 11126, 11022, 11150, 11343, 11389, 11265, 11035, 11036
];

// measurement types on the api-server
const weatherTypes = ['t', 'ldred', 'regen', 'rf', 'wg', 'wsr'];
//const weatherTypesDb = ['temperature', 'air_pressure', 'rainfall', 'humidity', 'wind_speed', 'wind_direction'];

// type_ids for local database
const typeIds = [1, 2, 3, 4, 5, 6];

/////////////////////////////////////////


// min and max are included
function randomBetween(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

function removeFirstAndLastChar(str) {
    return str.substring(1).slice(0, -1);
}

// returns array of arrays of strings
function customCsvParse(data) {
    let result = data.split('\n');
    result = result.map(e => e.split(';'));
    result.pop(); // last element is an empty array - pop removes it
    return result;
}

fs.writeFileSync(fileName, '');

let date = new Date();
date.setFullYear(date.getFullYear() - 2);

// try/catch to catch stream reader errors
try {
    for (let stationId = 1; stationId <= numberOfStations; stationId++) {
        const id = ids[randomBetween(0, ids.length - 1)];

        let urlDate = date.toISOString().split('T')[0];

        weatherTypes.forEach((type, index) => {
            let url = `http://at-wetter.tk/api/v1/station/${id}/${type}/${urlDate}/${daysFromDate}`;
            let res = request('GET', url);
            let body = res.getBody('utf8');

            // alternative to customCsvParse: csv.parse(body, (err,data) => {}) - doesn't work for me though
            let data = customCsvParse(body);

            values = [];
            data.forEach(e => {
                if (!e) return;

                let value = e[5];
                let timestamp = e[7];

                if (value && timestamp) {
                    values.push({
                        value: removeFirstAndLastChar(value),
                        timestamp: removeFirstAndLastChar(timestamp)
                    })
                }
            });

            values.forEach(e => {
                sqlString = `INSERT INTO measurement (value, timestamp, station_id, type_id, unit_id) VALUES  (${e.value}, '${e.timestamp}', ${stationId}, ${typeIds[index]}, ${typeIds[index]});\n`;

                fs.appendFileSync(fileName, sqlString);
            });
        });

        date.setDate(date.getDate() + 3); // increment date by 3 days to cover ~1 year of data  
    }
} catch (e) {}
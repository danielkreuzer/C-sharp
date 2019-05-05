const fs = require('fs');

/////////////////////////////////////////

// change these values:
const measurements = 10000;
const stations = 130;


const filename = './measurement.sql';

// if true, the script will show how many measurements are connected to each station
const log = false;

/////////////////////////////////////////


function roundToDecimals(number, decimals) {
    let multiplicator = Math.pow(10, decimals);
    return Math.floor(number * multiplicator) / multiplicator;
}

// min and max are included
function randomBetween(min, max) {
    return roundToDecimals(Math.random() * (max - min + 1) + min, 2);
}

function getWindDirection() {
    function getWindDirectionString() {
        switch (Math.floor(randomBetween(1, 4))) {
            case 1:
                return 'N';
            case 2:
                return 'E';
            case 3:
                return 'S';
            case 4:
                return 'W';
        }
    }

    let dir = getWindDirectionString() + getWindDirectionString();

    // remove double directions i.e. NN -> N
    if (dir[0] === dir[1]) dir = dir[0];

    if (dir.length === 2) {
        // N and S needs to be at beginning of string i.e. EN -> NE
        if (['N', 'S'].includes(dir[1])) {
            dir = dir[1] + dir[0];
        }

        // N and S || E and W can't both be in string i.e. SN -> S
        if (['N', 'S'].every(e => dir.includes(e)) || ['E', 'W'].every(e => dir.includes(e))) {
            dir = dir[0];
        }
    }

    return dir;
}


let timestamp = new Date();
timestamp.setFullYear(timestamp.getFullYear() - 2);

const secondsBetweenMeasurements = Math.ceil((365 * 24 * 60 * 60) / measurements);

fs.writeFileSync(filename, '');

let logger = [];
function logStationId(id) {
    if (logger[id]) logger[id]++;
    else logger[id] = 1;
}

for (let i = 0; i < measurements; i++) {
    let temperature = randomBetween(-20, 40); //celsius
    let air_pressure = randomBetween(600, 1013); // hPa
    let rainfall = randomBetween(0, 50); //l/m²
    let humidity = randomBetween(0, 100); // percent
    let wind_speed = randomBetween(0, 100); // km/h

    let wind_direction = getWindDirection();

    timestamp.setSeconds(timestamp.getSeconds() + secondsBetweenMeasurements);

    // mysql needs 'YYYY-MM-DD HH:MM:SS' format
    let timestamp_string = timestamp.toISOString().replace('T', ' ').slice(0, -5)

    let station_id = Math.floor(randomBetween(1, stations));
    if (log) logStationId(station_id);

    let sqlString = `INSERT INTO measurement (temperature, temperature_unit, air_pressure, air_pressure_unit, rainfall, rainfall_unit, humidity, humidity_unit, wind_speed, wind_speed_unit, wind_direction, timestamp, station_id) VALUES 
    (${temperature}, ${1}, ${air_pressure}, ${2}, ${rainfall}, ${3}, ${humidity}, ${4}, ${wind_speed}, ${5}, '${wind_direction}', '${timestamp_string}', ${station_id});\n`

    fs.appendFileSync(filename, sqlString);
}

if (log) console.table(logger); 
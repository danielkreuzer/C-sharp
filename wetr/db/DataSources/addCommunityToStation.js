let fs = require('fs');
let mysql = require('mysql');

let sqlString = "";

let con = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "",
    database: "WetrDB"
});

con.connect(function(err){
    if(err) throw err;
    console.log("Connected to WetrDB");
    // Get stations
    con.query("SELECT station.name, station.id FROM station", function(err, result, fields) {
        if(err) throw err;
        let test = 0;
        let stationType = 0;
        result.forEach(element => {
            // Find community to station
            con.query("SELECT community.id FROM community WHERE name LIKE \"" + element.name +  "\"", function(err, result, fields) {
                if(err) throw err;
                if(result.length > 0) {
                    test++;
                    stationType = Math.floor((Math.random() * 8) + 1);
                    //console.log(test + " " + result[0].id);
                    sqlString += "UPDATE station SET station.community_id = "+result[0].id+", station.type_id = "+stationType+" WHERE station.id = " + element.id +";\n";
                    if(test === 130) {
                        writeFile();
                    }
                }
            });
        });
    });

});

function writeFile() {
    fs.writeFile('./stationUpdate.sql', sqlString, () => {});
    console.log('./stationUpdate.sql created!');
}


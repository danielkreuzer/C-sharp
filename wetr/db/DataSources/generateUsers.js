let fs = require('fs');

let users = [
    'Tobias.Hofer', 'Max.Müller', 'Anna.Kennerick', 'Lukas.Scherb', 'Johann.Hofer',
    'Daniel.Mayr', 'Andreas.Gruber', 'Kevin.Bauer', 'Gottfried.Leibelt', 'Richard.Brauer',
    'Johannes.Blaugruber', 'Maria.Gruber', 'Thomas.Maurer', 'Katharina.Rother', 'Markus.Bonner',
    'Alice.Grüner', 'Julius.Grauer', 'Julia.Wolfer', 'Martin.Binder', 'Sebastian.Ahmer'
]

let sqlString = '';


users.forEach((elem, index) => {
    let fn = elem.split('.')[0];
    let ln = elem.split('.')[1];

    let date = new Date(); // today's date

    let age = 0;
    while (age < 18 || age > 80) {
        // make sure age is realistic
        age = Math.floor(Math.random() * 100);
    }

    let random = Math.floor(Math.random() * 10) + index;

    // randomize date
    date.setFullYear(date.getFullYear() - age);
    date.setMonth(date.getMonth() + random);
    date.setDate(date.getDate() + random);

    let community_id = 0;
    while (community_id < 1 || community_id > 2120) {
        community_id = Math.floor(Math.random() * 10000);
    }

    sqlString += `INSERT INTO WetrDB.user (username, password, email, first_name, last_name, date_of_birth, community_id) VALUES ("${elem.toLowerCase()}", "${fn.toLowerCase() + random}", "${elem.toLowerCase() + '@gmail.com'}", "${fn}", "${ln}", "${date.toLocaleDateString('de-AT')}", ${community_id});\n`
});

fs.writeFile('./user.sql', sqlString, () => {});
const sqlite3 = require("sqlite3").verbose();

exports.GetObject = async function() {
    let db = new sqlite3.Database('./data/data.db');
    db.Aget = function(query, parm) {
        return new Promise(cb => {
            db.get(query, parm, function(err, result) {
                cb([err, result]);
            });
        });
    }

    db.Arun = function(query, parm) {
        return new Promise(cb => {
            db.run(query, parm, function(err) {
                cb(err || true);
            });
        });
    }

    db.Aall = function(query, parm) {
        return new Promise(cb => {
            db.all(query, parm, function(err, result) {
                cb([err, result]);
            });
        });
    }

    return db;
}
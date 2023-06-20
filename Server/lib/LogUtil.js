const fs = require("fs");

exports.LogWrite = function(content) {
    const Time = new Date();
    const FileName = Time.getFullYear()+"-"+String(Time.getMonth()+1).padStart(2,"0")+"-"+String(Time.getDate()).padStart(2,"0");
    const TimeFormat = Time.getHours()+":"+String(Time.getMinutes()).padStart(2,"0")+":"+String(Time.getSeconds()).padStart(2,"0");
    
    fs.appendFileSync("./logs/"+FileName+".txt", TimeFormat+" => "+content+"\n");
}
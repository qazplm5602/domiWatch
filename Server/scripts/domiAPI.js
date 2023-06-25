const express = require("express");
const app = express();

app.get("/", function(req, res) {
    res.send(String(Object.keys(Players).length));
});

app.listen(Config.APIport, () => console.log("[domiAPI] 준비 완료! / "+Config.APIport));
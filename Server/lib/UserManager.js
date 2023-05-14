global.Players = {};
let GenId = 0;

class Player {
    nickname = "unknown";
    socket = undefined;

    constructor(_name, _ws) {
        this.nickname = _name;
        this.socket = _ws;
    }
}

// 플레이어 추가
exports.AddPlayer = function(id, name, ws) {
    global.Players[String(id)] = new Player(name, ws);
}

// 플레이어 삭제 (반환: 없지롱)
exports.RemovePlayer = function(id) {
    delete global.Players[String(id)];
}
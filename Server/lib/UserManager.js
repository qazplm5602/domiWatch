global.Players = {};

class Player {
    name = "unknown";
    socket = undefined;

    constructor(_name, _ws) {
        this.name = _name;
        this.socket = _ws;
    }
}

// 플레이어 추가
exports.AddPlayer = function(id, name, ws) {
    global.Players[String(id)] = new Player(name, ws);
    // 이벤트
    const cb = TriggerEvent["system.PlayerJoin"];
    if (typeof(cb) === "function")
        cb(String(id));
}

// 플레이어 삭제 (반환: 없지롱)
exports.RemovePlayer = function(id) {
    const cb = TriggerEvent["system.PlayerLeave"];
    if (typeof(cb) === "function")
        cb(String(id));

    delete global.Players[String(id)];
}
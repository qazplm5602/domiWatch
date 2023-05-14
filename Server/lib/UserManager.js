global.Players = {};
let GenId = 0;

class Player {
    id = undefined;
    nickname = "unknown";
    socket = undefined;

    constructor(_id, _name, _ws) {
        this.id = _id;
        this.nickname = _name;
        this.socket = _ws;
    }
}

// 플레이어 추가 (반환: id)
exports.AddPlayer = function(name, ws) {
    const ID = GenId ++;
    global.Players[ID] = new Player(ID, name, ws);

    return ID;
}

// 플레이어 삭제 (반환: 없지롱)
exports.RemovePlayer = function(id) {
    delete global.Players[id];
}
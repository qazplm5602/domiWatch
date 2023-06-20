const Log = require("../../lib/LogUtil.js");

TriggerEvent["Room.SendMessage"] = function(id, message) {
    const Player = Players[id];
    const RoomPlayer = RoomPlayers[id];

    // 무결성 검사
    if (Player === undefined || RoomPlayer === undefined || typeof(message) !== "string" || message.length <= 0) return;
    
    Log.LogWrite(`채팅 / ${Player.name}(${id}) ${message}`);
    for (const N_PlayerID in Players) {
        const N_Player = Players[N_PlayerID];
        N_Player.socket.send("Room.MessageAdd", `[${Player.name}] ${message}`);
    }
}
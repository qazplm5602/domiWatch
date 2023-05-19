global.RoomPlayers = {};

class RoomInterface {
    coords = [0,0,0];
    rotate = [90, -13.35];
    ready = false;
}

require("./RoomSync.js");

// global.RoomPlayers["qazplm5602"] = {
//     coords: [0,0,0],
//     rotate: [90, -13.35],
// }

// 플레이어가 들어옴
TriggerEvent["Room.Join"] = function(id) {
    // 테스트로 비활성화
    // if (global.RoomPlayers[id] !== undefined) {
    //     console.error("이미 방에 접속중입니다. : "+id);
    //     return;
    // }

    // 플레이어 생성
    global.RoomPlayers[id] = new RoomInterface();

    // ㅇㅋ 들어와
    const Player = Players[id];

    Player.socket.send("Room.ClientInit", null);

    // 모두에게 알려야지
    for (const PlayerID in global.RoomPlayers) {
        if (id !== PlayerID) { // 자기자신은 소환하면 안되지
            const Player = Players[PlayerID];
            console.log(PlayerID + " - Room Player Add");
            Player.socket.send("Room.PlayerAdd", id);
        }
    }
}

setInterval(() => {
    console.log(global.RoomPlayers);
}, 1000);
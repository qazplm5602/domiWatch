global.RoomPlayers = {};

class RoomInterface {
    coords = [0,0,0];
    rotate = [90, -13.35];
    Weapon = 0;
    Dead = false;
    Score = {
        kill: 0,
        death: 0
    }
}

require("./RoomSync.js");
require("./RoomWeapon.js");
require("./RoomHealth.js");

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
            const N_Player = Players[PlayerID];
            console.log(PlayerID + " - Room Player Add");
            N_Player.socket.send("Room.PlayerAdd", {
                id:id,
                name: Player.name
            });
        }
    }

    console.log("[RoomMain] "+Player.name+"님이 게임에서 입장하였습니다.");
}

// 방 나가는건 플레이어가 강종했을수도 있음
TriggerEvent["Room.Left"] = function(id, old_name) {
    // 머야 왜업성
    if (global.RoomPlayers[id] === undefined) return;

    const Cache_RoomPlayer = global.RoomPlayers[id];
    delete global.RoomPlayers[id]; // 방에서 나갓

    let Player_Name = Players[id] === undefined ? old_name : Players[id].name;
    console.log("[RoomMain] "+Player_Name+"님이 게임에서 퇴장하였습니다.");

    // 방안에 있는 사람한테 다 알려주쟈
    for (const PlayerID in global.RoomPlayers) {
        const Player = Players[PlayerID];
        Player.socket.send("Room.PlayerRemove", id);
    }
}

// setInterval(() => {
//     console.log(global.RoomPlayers);
// }, 1000);
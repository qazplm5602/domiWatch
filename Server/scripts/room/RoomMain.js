let RoomPlayers = {};

class RoomInterface {
    coords = [0,0,0];
    rotate = [90, -13.35];
}

// RoomPlayers["qazplm5602"] = {
//     coords: [0,0,0],
//     rotate: [90, -13.35],
// }

TriggerEvent["Room.Join"] = function(id) {
    if (RoomPlayers[id] !== undefined) {
        console.error("이미 방에 접속중입니다. : "+id);
        return;
    }

    // 플레이어 생성
    RoomPlayers[id] = new RoomInterface();

    // ㅇㅋ 들어와
    const Player = Players[id];

    Player.socket.send("Room.ClientInit", null);
}
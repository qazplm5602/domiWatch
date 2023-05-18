let RoomPlayers = {};

class RoomInterface {
    coords = [0,0,0];
    rotate = [90, -13.35];
}

// RoomPlayers["qazplm5602"] = {
//     coords: [0,0,0],
//     rotate: [90, -13.35],
// }

// 플레이어가 들어옴
TriggerEvent["Room.Join"] = function(id) {
    // 테스트로 비활성화
    // if (RoomPlayers[id] !== undefined) {
    //     console.error("이미 방에 접속중입니다. : "+id);
    //     return;
    // }

    // 플레이어 생성
    RoomPlayers[id] = new RoomInterface();

    // ㅇㅋ 들어와
    const Player = Players[id];

    Player.socket.send("Room.ClientInit", null);

    // 모두에게 알려야지
}

// 처음 들어온 클라이언트가 플레이어들을 동기화 하기 위해 모든 플레이어 데이터를 불러오는 것
TriggerEvent["Room.GetAllPlayer"] = function(id) {
    if (RoomPlayers[id] === undefined) return; // 방에 접속하지 않음
    
    const Player = Players[id];

    let SendPlayers = [];

    for (const PlayerID in RoomPlayers) {
        const Player = RoomPlayers[PlayerID];
        // if (id !== PlayerID) { (테스트로 일단 비활)
            SendPlayers.push({
                id: PlayerID,
                coords: Player.coords,
                rotate: Player.rotate
            });
        // }
    }

    Player.socket.send("Room.ResultAllPlayer", SendPlayers);
}


// 클라이언트가 좌표, 아니면 방향을 바꿔달라고 요청했다!!
TriggerEvent["Room.RequestPlayerUpdate"] = function(id, data) {
    if (RoomPlayers[id] === undefined) return; // 방에 접속하지 않음

    // 이상한 데이터???
    if (data === undefined || data.Coords === undefined || data.MouseX === undefined || data.MouseY === undefined) return;

    const coords = data.Coords;
    RoomPlayers[id].coords = coords;
    RoomPlayers[id].rotate = [data.MouseY, data.MouseX];

    console.log("업데이트!!!");

    // 이 코드 에서 이제 클라이언트들에게 업데이트가 되었다고 알려주는
    // 코드를 넣을 예정임 ㅅㄷ
    // Object.keys(RoomPlayers).forEach(PlayerID => {
    //     const Player = Players[PlayerID];
    //     Player.socket.send("Room.PlayerUpdate", {
    //         id: id,
            
    //     });
    // });
}

setInterval(() => {
    console.log(RoomPlayers);
}, 1000);
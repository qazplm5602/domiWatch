// 처음 들어온 클라이언트가 플레이어들을 동기화 하기 위해 모든 플레이어 데이터를 불러오는 것
TriggerEvent["Room.GetAllPlayer"] = function(id) {
    if (RoomPlayers[id] === undefined) return; // 방에 접속하지 않음
    
    const Player = Players[id];

    let SendPlayers = [];

    for (const PlayerID in RoomPlayers) {
        if (id !== PlayerID) { // (테스트로 일단 비활)
            const Player = RoomPlayers[PlayerID];
            SendPlayers.push({
                id: PlayerID,
                coords: Player.coords,
                rotate: Player.rotate,
                weapon: Player.Weapon,
                dead: Player.Dead
            });
        }
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

    Object.keys(RoomPlayers).forEach(PlayerID => {
        if (id === PlayerID) return; // 자기자신은 보내지 않음 (테스트로 비활)

        const Player = Players[PlayerID];
        Player.socket.send("Room.PlayerUpdate", {
            id: id,
            coords: coords,
            rotate: [data.MouseY, data.MouseX]
        });
    });
}   
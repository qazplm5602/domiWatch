const RandomCoords = require("../../lib/RoomSpawn.js");
// 처음 들어온 클라이언트가 플레이어들을 동기화 하기 위해 모든 플레이어 데이터를 불러오는 것
TriggerEvent["Room.GetAllPlayer"] = function(id) {
    if (RoomPlayers[id] === undefined) return; // 방에 접속하지 않음
    
    const Player = Players[id];

    let SendPlayers = [];

    for (const PlayerID in RoomPlayers) {
        const originPlayer = Players[PlayerID];
        if (id !== PlayerID) { // (테스트로 일단 비활)
            const Player = RoomPlayers[PlayerID];
            SendPlayers.push({
                id: PlayerID,
                name: originPlayer.name,
                coords: Player.coords,
                rotate: Player.rotate,
                weapon: Player.Weapon,
                dead: Player.Dead,
                score_kill: Player.Score.kill,
                score_death: Player.Score.death,
            });
        }
        
        // 접속 안내
        originPlayer.socket.send("Room.MessageAdd", `<color=#E5D85C>[${Player.name}] 님이 접속하였습니다.</color>`);
    }

    if (Object.keys(RoomPlayers).length <= 1) // 솔로 ㅠㅠ
        Player.socket.send("Room.MessageAdd", `<color=#F15F5F>[domiServer] 이 게임은 멀티게임인데 혼자계시네요. 다른 친구도 초대해보세요!</color>`);
    Player.socket.send("Room.ScoreAddMY", { id: id, name: Player.name }); // 자기자신 스코어보드 추가
    Player.socket.send("Room.ResultAllPlayer", SendPlayers);

    // 좌표 랜덤으로 소환
    Player.socket.send("Room.PlayerSetCoords", RandomCoords());
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
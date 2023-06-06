TriggerEvent["Room.PlayerDamage"] = function(id, data) {
    const Player = Players[id];
    const RoomPlayer = RoomPlayers[id];

    // 무결성 검사
    if (Player === undefined || RoomPlayer === undefined || RoomPlayer.Dead || data == null || data.AttackID === undefined || data.isDie === undefined) return;
    
    const Attacker_Player = Players[data.AttackID];
    const Attacker_RoomPlayer = RoomPlayers[data.AttackID];
    if (Attacker_Player === undefined || Attacker_RoomPlayer === undefined) return; // 플레이어 검사

    if (data.isDie)
        RoomPlayer.Dead = true;

    console.log(`[데미지] ${Attacker_Player.name} -> ${Player.name} [${(data.isDie ? "죽음" : "안죽엉")}]`);

    if (!data.isDie) return; // 죽지 않았다면 밑 코드는 실행하지 않음

    RoomPlayer.Score.death ++; // 자신 데스 추가
    Attacker_RoomPlayer.Score.kill ++; // 상대방 킬 추가
 
    for (const PlayerID in RoomPlayers) {
        const N_Player = Players[PlayerID];
        N_Player.socket.send("Room.BroadcastPlayerDie", {
            DiePlayer: id,
            DiePlayerName: Player.name,
            Attacker: Attacker_Player.name,
            YouKill: data.AttackID === PlayerID,
            YouDead: id === PlayerID
        });

        // 스코어 수정
        N_Player.socket.send("Room.ScoreEdit", { mode: "death", id: id, value: RoomPlayer.Score.death });
        N_Player.socket.send("Room.ScoreEdit", { mode: "kill", id: data.AttackID, value: Attacker_RoomPlayer.Score.kill });
    }
}

const RandomCoords = require("../../lib/RoomSpawn.js");
TriggerEvent["Room.RequestRespawn"] = function(id) {
    const Player = Players[id];
    const RoomPlayer = RoomPlayers[id];
    if (Player === undefined || RoomPlayer === undefined || !RoomPlayer.Dead) return;
    RoomPlayer.Dead = false;

    // 좌표 랜덤으로 소환
    Player.socket.send("Room.PlayerSetCoords", RandomCoords());

    for (const PlayerID in RoomPlayers) {
        const N_Player = Players[PlayerID];
        N_Player.socket.send("Room.BroadcastPlayerRespawn", {
            id: id,
            my: id === PlayerID
        });
    }
}
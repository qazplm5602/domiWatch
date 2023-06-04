const SpawnCoords = [ // 소환 좌표
    [2.77999997,2.09199882,-84.0699997],
    [17.1113777,2.09199977,-83.160881],
    [26.2810383,2.09199882,-90.8771286],
    [-2.19501925,2.09199882,-90.0604858],
    [-1.02498806,5.43613529,-71.2431488],
    [20.8652611,5.43613529,-71.5572128],
    [55.5075378,2.09238648,-66.4166336],
    [62.8738251,2.09238505,-79.6959],
    [79.4568863,2.09238601,-91.6278229],
    [87.2438583,2.09238672,-90.3016891],
    [92.4015808,2.09238553,-81.9625549],
    [61.7224464,2.09199882,-20.3479633],
    [69.3747406,2.09199929,1.34688818],
    [90.9203491,0.55999887,15.6830435],
    [95.8714905,0.559999585,19.5979652],
    [48.2741661,0.559998989,18.7319717],
    [47.2050858,2.09200096,12.4479303],
    [27.6708412,2.09199977,5.22396231],
    [7.25229073,0.091999054,2.3114419],
    [-24.5152397,0.0919997692,8.79492378],
    [-37.1063919,1.62933612,-6.18289185],
    [-36.8972778,1.62933683,-37.9945374],
    [-30.8068123,1.62938619,-33.8315392],
    [-23.4340248,1.62938619,-39.3135872],
    [-16.12463,1.62938619,-39.670063],
    [-20.2065163,2.09238648,-49.4101715],
    [-19.6590214,2.09238529,-77.4044876],
]

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

    // 좌표 랜덤으로 소환
    const SelectXYZ = SpawnCoords[Math.floor( ( Math.random() * (SpawnCoords.length - 0) + 0 ) )];
    Player.socket.send("Room.PlayerSetCoords", {
        x:SelectXYZ[0],
        y:SelectXYZ[1],
        z:SelectXYZ[2]
    });
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
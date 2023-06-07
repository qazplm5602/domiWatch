TriggerEvent["system.PlayerJoin"] = function(PlayerId) {
    const Player = Players[PlayerId];
    console.log(`${Player.name}(${PlayerId})님이 로그인하였습니다.`);

    Player.socket.send("Lobby.Init", {id: PlayerId, name: Player.name});
}

TriggerEvent["system.PlayerLeave"] = function(PlayerId, cache_playerinfo) {
    console.log(`${cache_playerinfo.name}(${PlayerId})님이 로그아웃 하셨습니다.`);

    // 게임에 이미 참여중이라면 나감처리
    if (RoomPlayers[PlayerId] !== undefined)
        TriggerEvent["Room.Left"](PlayerId, cache_playerinfo.name);
}
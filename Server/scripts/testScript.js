TriggerEvent["system.PlayerJoin"] = function(PlayerId) {
    const Player = Players[PlayerId];
    console.log(`${Player.name}(${PlayerId})님이 로그인하였습니다.`);

    Player.socket.send("Lobby.Init", {id: PlayerId, name: Player.name});
}

TriggerEvent["system.PlayerLeave"] = function(PlayerId) {
    const Player = Players[PlayerId];
    console.log(`${Player.name}(${PlayerId})님이 로그아웃 하셨습니다.`);
}
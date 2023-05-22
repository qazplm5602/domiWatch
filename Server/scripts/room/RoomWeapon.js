TriggerEvent["Room.WeaponChange"] = function(id, weaponID) {
    const Player = RoomPlayers[id];

    // 방에 들어와있지 않거나 무기아이디가 이상한경우 / 이미 같은 무기 일경우
    if (Player === undefined || typeof(weaponID) !== "number" || weaponID === Player.Weapon) return;
    Player.Weapon = weaponID;

    for (const nplayer_ID in RoomPlayers) {
        if (nplayer_ID !== id) {
            const nplayer = Players[nplayer_ID];
            nplayer.socket.send("Room.PlayerWeaponChange", {
                id: id,
                WeaponID: weaponID
            });
        }
    }
}

// 총을 쐇다
TriggerEvent["Room.BulletCreate"] = function(id, data) {
    const Player = RoomPlayers[id];

    // 방에 들어와있지 않거나 무기아이디가 이상한경우 / 무기가 서버와 다른 경우
    if (Player === undefined || typeof(data.WeaponID) !== "number" || data.WeaponID !== Player.Weapon) return;

    data.AttackID = id;

    for (const nplayer_ID in RoomPlayers) {
        if (nplayer_ID !== id) {
            const nplayer = Players[nplayer_ID];
            nplayer.socket.send("Room.PlayerFireWeapon", data);
        }
    }
}
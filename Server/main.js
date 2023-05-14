const net = require("net");
const Config = global.Config = require("./config.json");
global.TriggerEvent = {};

// 스크륍트
const UserManager = require("./lib/UserManager.js");

const server = net.createServer(function(socket) {
    ////////// socket 초기화 //////////
    // utf-8 로 설정
    socket.setEncoding('utf8');

    // Server -> Client 전송 메서드
    socket.send = function(type, data) {
        let Packet = {
            type: type,
            data: data   
        }
        const jsonEncode = JSON.stringify(Packet);

        // 검사
        if (jsonEncode.indexOf("|domi\\SLICE\\packet|") >= 0) {
            throw new Error("데이터에 SlicePacket 내용이 있습니다.\n"+jsonEncode);
        }

        socket.write(jsonEncode+"|domi\\SLICE\\packet|");
    }

    const MyID = UserManager.AddPlayer(null, socket);
    console.log(`[main] ${socket.remoteAddress} ${MyID} 연결.`);

    let PacketPlus = "";
    socket.on("data", function(data) {
        // 누락된 데이터가 있으면 붙임
        if (PacketPlus.length > 0) {
            data = PacketPlus+data;
            PacketPlus = ""; // 초기화
        }
        
        const SlicePacket = data.split("|domi\\SLICE\\packet|");

        for (let index = 0; index < SlicePacket.length; index++) {
            const message = SlicePacket[index];
            
            if (message.length > 0)
                if (index === (SlicePacket.length - 1)) { // 패킷 손실 감지
                    PacketPlus = message;
                } else {
                    try {
                        const MessageDecode = JSON.parse(message);
                        if (MessageDecode.type !== undefined && MessageDecode.data !== undefined) {
                            const Callback = global.TriggerEvent[MessageDecode.type];
                            if (typeof(Callback) === "function") {
                                Callback(MyID, MessageDecode.data); // 콜백 실행
                            } else
                                console.error(`[main] ${MyID} 알수없는 Trigger : ${MessageDecode.type}`);
                        } else {
                            console.error(`[main] ${MyID} 잘못된 정보를 전송함.`);
                        }
                    } catch {
                        console.error(`[main] ${MyID} JSON 데이터 형식이 아님.\n${message}`);
                    }
                }
        }
    });
    socket.once("close", function() {
        UserManager.RemovePlayer(MyID);
        console.log(`[main] ${socket.remoteAddress} ${MyID} 나감.`);
    });
    socket.on("error", function(err) {
        // console.error(err);
    });
});

server.listen(Config.port, () => console.log("[main] 서버 출항 준비 완료! Port: "+Config.port));
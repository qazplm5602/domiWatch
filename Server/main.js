const net = require("net");
const Config = global.Config = require("./config.json");
global.TriggerEvent = {};

// 스크륍트
const UserManager = require("./lib/UserManager.js");
const LoginSys = require("./scripts/LoginSystem.js");
const LogSys = require("./lib/LogUtil.js");
require("./scripts/room/RoomMain.js");
require("./scripts/JoinLeft.js");

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
    // 추방 메서드
    socket.kick = function(why) {
        // 사유 알려주기
        socket.send("disconnect.why", why);
        // 연결 끊음
        socket.destroy();
    }

    // 이제!! 로그인을 성공했으니 데이터 받아준다.
    const SocketEvent_Init = (MyID) => {
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
                                    try {
                                        Callback(MyID, MessageDecode.data); // 콜백 실행
                                    } catch (error) {
                                        console.error("CallBack 함수 구현오류");
                                        console.error(error);
                                    }
                                } else
                                    console.error(`[main] ${MyID} 알수없는 Trigger : ${MessageDecode.type}`);
                            } else {
                                console.error(`[main] ${MyID} 잘못된 정보를 전송함.`);
                            }
                        } catch (err) {
                            console.error(`[main] ${MyID} 데이터 파싱 오류. ${err}\n${message}`);
                        }
                    }
            }
        });
        socket.once("close", function() {
            UserManager.RemovePlayer(MyID);
        });
        socket.on("error", function(err) {
            // console.error(err);
        });
    }

    // 로그인...
    socket.once("data", async function(data) {
        data = data.replace("|domi\\SLICE\\packet|","");
        let message;
        try {
            message = JSON.parse(data);
        } catch {}

        if (message === undefined || message.type !== "domiServer.Login" || message.data === undefined || typeof(message.data.name) !== "string" || typeof(message.data.HWID) !== "string" || message.data.HWID.length <= 0) {
            socket.kick("잘못된 로그인 데이터 입니다.");
            return;
        }
        
        const UserName = message.data.name;
        const HWID = message.data.HWID; // 하드웨어 수집
        
        if (UserName.length < 2) {
            socket.kick("이름은 2글자 이상이여야 합니다.");
            return;
        }

        // 누군가 이미 접속되어있음
        // if (Players[String(ID)] !== undefined) {
        //     socket.kick("다른 클라이언트에서 접속중입니다.");
        //     return;
        // }

        const ID = LoginSys.RandomString(20);
        LogSys.LogWrite(`유저 접속! / ${UserName}(${ID}) / HWID : ${HWID}`);

        // 문을 열어주쟈
        SocketEvent_Init(ID);
        // 로그인 성공!
        UserManager.AddPlayer(ID, UserName, socket);
    });
});

server.listen(Config.port, () => console.log("[main] 서버 출항 준비 완료! Port: "+Config.port));
LogSys.LogWrite("서버 켜짐!");

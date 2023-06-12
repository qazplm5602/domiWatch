const readline = require("readline");
const Encrypt = require("./lib/Encrypt.js");

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

console.log("비밀번호를 입력하세요.")
rl.on("line", async function(result) {
    const domi = await Encrypt.createHashedPassword(result);
    console.log(domi.salt+":"+domi.hashedPassword);
    rl.close();
});

rl.on('close', () => {
    // 입력이 끝난 후 실행할 코드
    process.exit();
})
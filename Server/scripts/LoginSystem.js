const SQL = require("../lib/SqlModule.js");
const Encrypt = require("../lib/Encrypt.js");

exports.Login = async function(id, Password) {
    const db = await SQL.GetObject();

    let [err, account] = await db.Aget("SELECT * FROM Account WHERE id = ?", id);
    db.close(); // connection 닫기

    if (err)
        return {err:"데이터 불러오는데 실패하였습니다."};

    if (account === undefined) // 아이디 없음
        return {err:"아아디 / 비밀번호가 올바른지 확인하세요."};

    let [Salt, HashPass] = account.password.split(":");

    let Pass = await Encrypt.verifyPassword(Password, Salt , HashPass);
    if (!Pass)
        return {err:"아아디 / 비밀번호가 올바른지 확인하세요."};

    return {id:account.id,name:account.name};
}

exports.RandomString = function(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
}
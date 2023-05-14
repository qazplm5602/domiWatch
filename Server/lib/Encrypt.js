const util = require("util");
const crypto = require("crypto");

const randomBytesPromise = util.promisify(crypto.randomBytes);
const pbkdf2Promise = util.promisify(crypto.pbkdf2);

const createSalt = async () => {
    const buf = await randomBytesPromise(64);
    return buf.toString("base64");
  };
  
exports.createHashedPassword = async (password) => {
    const salt = await createSalt();
    const key = await pbkdf2Promise(password, salt, 100000, 64, "sha512");
    const hashedPassword = key.toString("base64");
    return { hashedPassword, salt };
};
  
exports.verifyPassword = async (password, salt, hashedPassword) => {
    const key = await pbkdf2Promise(password, salt, 100000, 64, "sha512");
    const inputHashedPassword = key.toString("base64");
    return inputHashedPassword === hashedPassword;
};
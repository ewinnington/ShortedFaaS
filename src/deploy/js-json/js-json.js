const fs = require("fs");
const input = JSON.parse(fs.readFileSync(0, "utf-8")); 

const output = {c : input.a * input.b };
console.log(JSON.stringify(output));
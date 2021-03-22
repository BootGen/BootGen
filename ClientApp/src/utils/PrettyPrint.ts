function getLine(idx: number, str: string): number{
  if(navigator.userAgent.indexOf("Firefox") != -1){
    return idx;
  }
  let charCount = 0;
  const strArray: string[] = str.split("\n");
  for(let i = 0; i < strArray.length; i++){
    charCount += strArray[i].length+1;
    if(charCount >= idx){
      return i;
    }
  }
  return -1;
}
export function jsonError(text: string) {
  const json = text.split("\n").filter(line => !line.trim().startsWith("//")).join("\n");
  try {
    JSON.parse(json);
    return false;
  } catch (err) {
    const idx = err.message.match(/\d+/g)[0]-1;
    const lines = text.split("\n");
    let errorLine = getLine(idx, json);
    for(let i = 0; i < errorLine; i++){
      if(lines[i].includes("//")){
        errorLine++;
      }
    }
    return {line: errorLine, message: err.message};
  }
}
export function formatJson(json: string): string{
  json = json.split("\r").join("");
  json = json.replace(/( )*(}|]|{|\[|,)/g, "$2");
  json = json.replace(/( {2})*/g, "");
  json = json.replace(/(": *)/g, "\": ");
  json = json.split("'").join("\"");
  json = json.replace(/[\n\t]/g, "");
  json = json.split("#>comment<").join("#>comment<\n");
  json = json.split("{").join("{\n");
  json = json.split("}").join("}\n");
  json = json.split("[").join("[\n");
  json = json.split("]").join("]\n");
  json = json.split(",").join(",\n");
  json = json.split("\n ").join("\n");
  json = json.split("\n,").join(",");
  json = json.split("\"}").join("\"\n}");
  json = json.split("\"]").join("\"\n]");
  return json;
}
function indentLines(lines: string[]): string[]{
  let tabCount = 0;
  for(let i = 0; i < lines.length; i++){
    if(lines[i].includes("{") || lines[i].includes("[")){
      lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
      tabCount++;
    }else if(lines[i].includes("}") || lines[i].includes("]")){
      --tabCount;
      lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
    }else{
      lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
    }
  }
  return lines;
}
function replaceToComment(comments: string[], lines: string[]): string[]{
  comments.forEach(comment => {
    for(let i = 0; i < lines.length; i++){
      if(lines[i].includes("#>comment<")){
        lines[i] = lines[i].replace(/#>comment</g, comment.slice(0, -1));
        break;
      }
    }
  });
  return lines;
}
export function prettyPrint(json: string){
  const error = jsonError(json);
  if(error !== false){
    return error;
  }else{
    json = json.split("\r").join("");
    const comments = json.match(/(\/\/.*)(\n)/g);
    json = json.replace(/(\/\/.*)(\n)/g, "#>comment<");
    json = formatJson(json);
    let lines = indentLines(json.split("\n"));
    if(comments){
      lines = replaceToComment(comments, lines);
    }
    return lines.join("\n");
  }
}
interface SnackBar{
  dismissible: boolean;
  timeout: number;
  type: string;
  text: string;
  visible: boolean;
}

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
  json = json.replace(/(: *)/g, ": ");
  json = json.split("'").join("\"");
  json = json.split("\t").join("");
  json = json.split("\n").join("");
  json = json.split("#>comment<").join("#>comment<\n");
  json = json.split("{").join("{\n");
  json = json.split("}").join("}\n");
  json = json.split("[").join("[\n");
  json = json.split("]").join("]\n");
  json = json.split(",").join(",\n");
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
export function unsetHighlight(cmId: number, from: string){
  const elementById = document.getElementById("cm" + cmId);
  if(!elementById){
    return;
  }
  const e = elementById.getElementsByClassName(from);
  for(let i = 0; i < e.length; i++){
    e[i].setAttribute("style", "background-color: unset;");
  }
}
export function highlightLine(cmId: number, line: number, errorMessage: string, color: string, snackbar: SnackBar, minLine: number, maxLine: number){
  snackbar.dismissible = true,
  snackbar.timeout = -1;
  snackbar.type = "orange darken-2";
  snackbar.text = errorMessage;
  snackbar.visible = true;
  unsetHighlight(0, "CodeMirror-line");
  const elementById = document.getElementById("cm" + cmId);
  if(!elementById || (minLine > line && minLine < 5000000) || (maxLine < line && maxLine > -1)){
    return;
  }
  if(minLine < line && line < maxLine){
    elementById.getElementsByClassName("CodeMirror-line")[line-minLine+2].setAttribute("style", `background-color:${color};`);
  }else{
    elementById.getElementsByClassName("CodeMirror-line")[line].setAttribute("style", `background-color:${color};`);
  }
}
export function prettyPrint(json: string, snackbar: SnackBar, minLine: number, maxLine: number){
  console.log("PrettyPrint");
  const error = jsonError(json);
  if(error !== false){
    highlightLine(0, error.line, error.message, "red", snackbar, minLine, maxLine);
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
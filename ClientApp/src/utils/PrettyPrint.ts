function getLine(idx: number, str: string): number{
  if(navigator.userAgent.indexOf('Firefox') != -1){
    return idx;
  }
  let charCount = 0;
  const strArray: string[] = str.split('\n');
  for(let i = 0; i < strArray.length; i++){
    charCount += strArray[i].length+1;
    if(charCount >= idx){
      return i;
    }
  }
  return -1;
}

export interface JsonValidationResult {
  error: boolean;
  line: number;
  message: string;
}

export function validateJson(text: string): JsonValidationResult {
  const json = text.split('\n').filter(line => !line.trim().startsWith('//')).join('\n');
  try {
    JSON.parse(json);
    return {error: false, line: -1, message: ''};
  } catch (err) {
    const idx = err.message.match(/\d+/g)[0]-1;
    const lines = text.split('\n');
    let errorLine = getLine(idx, json);
    for(let i = 0; i < errorLine; i++){
      if(lines[i].includes('//')){
        errorLine++;
      }
    }
    return {error: true, line: errorLine, message: err.message};
  }
}
export function formatJson(json: string): string{
  json = json.replaceAll(/\s/g,'');
  json = json.replaceAll('//','//\n');
  json = json.replaceAll('{','{\n');
  json = json.replaceAll('}','}\n');
  json = json.replaceAll('[','[\n');
  json = json.replaceAll(']',']\n');
  json = json.replaceAll(',',',\n');
  json = json.replaceAll(':',': ');
  json = json.replaceAll('"}','"\n}');
  return json;
}
function indentLines(lines: string[]): string[]{
  let tabCount = 0;
  for(let i = 0; i < lines.length; i++){
    if(lines[i].includes('{') || lines[i].includes('[')){
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
      tabCount++;
    }else if(lines[i].includes('}') || lines[i].includes(']')){
      --tabCount;
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
    }else{
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
    }
  }
  return lines;
}
function replaceToComment(comments: string[], lines: string[]): string[]{
  let startIdx = 0;
  comments.forEach(comment => {
    for(let i = startIdx; i < lines.length; i++){
      if(lines[i].includes('//')){
        startIdx = i+1;
        lines[i] = lines[i].replace('//', comment.replace('\n', ''));
        break;
      }
    }
  });
  return lines;
}
function replaceToString(strings: string[], lines: string[]): string[]{
  strings.forEach(comment => {
    for(let i = 0; i < lines.length; i++){
      if(lines[i].includes('""')){
        lines[i] = lines[i].replace('""', comment);
        break;
      }
    }
  });
  return lines;
}
export function prettyPrint(json: string): string {
  json = json.replace('\r','');
  json = json.replaceAll('\'','"');
  const strings = json.match(/"[^"]*"/g);
  json = json.replace(/"[^"]*"/g, '""');
  const comments = json.match(/(\/\/.*)(\n)/g);
  json = json.replace(/(\/\/.*)(\n)/g, '//');
  json = formatJson(json);
  let lines = indentLines(json.split('\n'));
  if (comments) {
    lines = replaceToComment(comments, lines);
  }
  if (strings) {
    lines = replaceToString(strings, lines);
  }
  return lines.join('\n');
}

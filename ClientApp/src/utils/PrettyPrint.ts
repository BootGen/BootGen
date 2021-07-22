
import axios from 'axios';
import { toCamelCase } from './Helper';

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
  let json = text.replace(/\*(\*(?!\/)|[^*])*\*/g, '');
  json = json.split('\n').filter(line => !line.trim().startsWith('//')).join('\n');
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
    axios.post('errors/log', {
      kind: 'JSON',
      message: 'validation error',
      info: text
    });
    return {error: true, line: errorLine, message: err.message};
  }
}
export function formatJson(json: string): string{
  json = json.replaceAll(/\s/g,'');
  json = json.replaceAll('//','\n//\n');
  json = json.replaceAll('/**/','\n/**/\n');
  json = json.replaceAll('{','{\n');
  json = json.replaceAll('}','\n}');
  json = json.replaceAll('[','[\n');
  json = json.replaceAll(']','\n]');
  json = json.replaceAll(',',',\n');
  json = json.replaceAll(':',': ');
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
        lines[i] = lines[i].replace('//', comment);
        break;
      }
    }
  });
  return lines;
}
function replaceToCommentBlock(comments: string[], lines: string[]): string[]{
  let startIdx = 0;
  comments.forEach(comment => {
    for(let i = startIdx; i < lines.length; i++){
      if(lines[i].includes('/**/')){
        startIdx = i+1;
        lines[i] = lines[i].replace('/**/', `/${comment}/`);
        break;
      }
    }
  });
  return lines;
}
function replaceToKey(strings: string[], lines: string[]): string[]{
  strings.forEach(key => {
    const name = key.replace(/"([^"]*)":/g, '$1')
    for(let i = 0; i < lines.length; i++){
      if(lines[i].includes('key')){
        lines[i] = lines[i].replace('key', `"${toCamelCase(name)}": `);
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
  json = json.replaceAll('\r','');
  json = json.replaceAll('\'','"');
  const keys = json.match(/"([^"]*)":/g);
  json = json.replace(/"([^"]*)":/g, 'key');
  const strings = json.match(/"[^"]*"/g);
  json = json.replace(/"[^"]*"/g, '""');
  const comments = json.match(/\/\/.+?(?=\n)/g);
  json = json.replace(/\/\/.+(?=\n)/g, '//');
  const commentBlocks = json.match(/\*(\*(?!\/)|[^*])*\*/g);
  json = json.replace(/\*(\*(?!\/)|[^*])*\*/g, '**');
  json = formatJson(json);
  let lines = indentLines(json.split('\n'));
  if(commentBlocks) {
    lines = replaceToCommentBlock(commentBlocks, lines);
  }
  if (comments) {
    lines = replaceToComment(comments, lines);
  }
  if (keys) {
    lines = replaceToKey(keys, lines);
  }  
  if (strings) {
    lines = replaceToString(strings, lines);
  }
  lines.forEach((line, idx) =>{
    if((/^(\t)+$/g).test(line) || line === ''){
      lines.splice(idx, 1);
    } 
  })
  return lines.join('\n');
}

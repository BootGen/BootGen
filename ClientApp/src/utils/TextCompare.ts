
function compare(lines1: string[], lines2: string[]): number[] {
  const result: number[] = [];
  let position = 0;
  lines2.forEach((line, idx2) => {
    for(let idx1 = position; idx1 < lines1.length; ++idx1) {
      if (lines1[idx1] == line) {
        position = idx1 + 1;
        result[idx2] = idx1;
        break;
      }
    }
  });
  return result;
}

export function getChanges(file1: string, file2: string): Array<number> {
  const lines1 = file1.split('\n');
  const lines2 = file2.split('\n');
  const mapping = compare(lines1, lines2);
  const result = Array<number>();
  lines2.forEach((line, idx) => {
    if (mapping[idx] === undefined){
      result.push(idx);
    }
  });
  return result;
}

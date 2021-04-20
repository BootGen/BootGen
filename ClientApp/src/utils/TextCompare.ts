export class Compare{
  lines1: string[];
  lines2: string[];
  indices = Array<number>();
  matched = Array<boolean>();

  constructor(lines1: string[], lines2: string[]){
    this.lines1 = lines1;
    this.lines2 = lines2;
  }

  compareWithOffset(offset: number){
    this.lines2.forEach(line2 => {
      for(let idx1 = 0; idx1 < this.lines1.length; ++idx1) {
        for(let idx3 = offset; idx3 >= 0; idx3--){
          if(!this.matched[idx1 + idx3]){
            if((idx1 + idx3) >= 0 && (idx1 + idx3) <= this.lines1.length){
              if(this.lines1[idx1 + idx3] === line2) {
                this.indices[idx1 + idx3] = idx1 + idx3;
                this.matched[idx1 + idx3] = true;
              }
            }
          }
          if(!this.matched[idx1 - idx3]){
            if((idx1 - idx3) >= 0 && (idx1 - idx3) <= this.lines1.length){
              if(this.lines1[idx1 - idx3] === line2) {
                this.indices[idx1 - idx3] = idx1 - idx3;
                this.matched[idx1 - idx3] = true;
              }
            }
          }
        }
      }
    });
  }
  getMaxMissingBlock(missingLines: number[]): number{
    let block = 0;
    let missingMaxBlock = 0;
    for(let i = 0; i < missingLines.length-1; i++){
      if(missingLines[i] + 1 === missingLines[i+1]){
        if(block === 0){
          block++;
        }
        block++;
      }else{
        if(missingMaxBlock < block){
          missingMaxBlock = block;
          block = 0;
        }
      }
    }
    return missingMaxBlock;
  }
  public getChanges(): Array<number> {
    this.compareWithOffset(0);
    const missingLines: number[] = [];
    for(let i = 0; i < this.matched.length; i++){
      if(!this.matched[i]){
        missingLines.push(i);
      }
    }
    const offset = this.getMaxMissingBlock(missingLines);
    if(offset > 1){
      this.compareWithOffset(offset-1);
    }
    const changes: number[] = [];
    for(let i = 0; i < this.matched.length; i++){
      if(!this.matched[i]){
        changes.push(i);
      }
    }
    return changes;
  }
}

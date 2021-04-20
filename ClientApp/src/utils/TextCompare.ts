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
    this.lines2.forEach((line2, idx2) => {
      const idx1 = idx2 + offset;
      if (!this.matched[idx1]) {
        if (this.lines1[idx1] === line2) {
          this.matched[idx1] = true;
          this.indices[idx2] = idx1;
        }
      }
    });
  }
  public getChanges(): Array<number> {
    this.compareWithOffset(0);
    let offset = 0;
    let maxOffset = 0;
    for(let i = 0; i < this.matched.length; i++){
      if(this.matched[i]){
        ++offset;
        if (maxOffset < offset)
          maxOffset = offset;
      } else {
        offset = 0;
      }
    }
    for (let i = 1; i < maxOffset; ++i) {
      this.compareWithOffset(i);
      this.compareWithOffset(-i);
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

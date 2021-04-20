import { expect } from 'chai'
import { Compare } from '@/utils/TextCompare';
import * as fs from 'fs';

function read(fileName: string): string {
  return fs.readFileSync(`./tests/unit/${fileName}`, 'utf8');
}


describe('TextCompare', function() {
  describe('getChanges', function() {
    it('should compare two C# files', function() {
      const compare = new Compare(read('Tag.cs(1)').split('\n'), read('Tag.cs(2)').split('\n'));
      const changes = compare.getChanges();
      expect(changes.length).to.equal(1);
      expect(changes[0]).to.equal(8);
    });
  });
});

describe('YmlTextCompare', function() {
  describe('getChanges', function() {
    it('should compare two yml files', function() {
      const compare = new Compare(read('restapi.yml(1)').split('\n'), read('restapi.yml(2)').split('\n'));
      const changes = compare.getChanges();
      expect(changes.length).to.equal(4);
      expect(changes[0]).to.equal(583);
      expect(changes[1]).to.equal(584);
      expect(changes[2]).to.equal(601);
      expect(changes[3]).to.equal(603);
    });
  });
});

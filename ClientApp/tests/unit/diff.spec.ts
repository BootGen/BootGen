import { expect } from 'chai'
import { getChanges } from '@/utils/TextCompare';
import * as fs from 'fs';

function read(fileName: string): string {
  return fs.readFileSync(`./tests/unit/${fileName}`, 'utf8');
}


describe('TextCompare', function() {
  describe('getChanges', function() {
    it('should compare two C# files', function() {
      const changes = getChanges(read('Tag.cs(1)'), read('Tag.cs(2)'));
      expect(changes.length).to.equal(1);
      expect(changes[0]).to.equal(8);
    });
  });
});

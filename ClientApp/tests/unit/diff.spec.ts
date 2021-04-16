import { expect } from 'chai'
import { getChanges } from '@/utils/TextCompare';

describe('TextCompare', function() {
  describe('getChanges', function() {
    it('smoke test', function() {
      const changes = getChanges('a', 'b');
      expect(changes.length).to.equal(1);
      expect(changes[0]).to.equal(0);
    });
  });
});

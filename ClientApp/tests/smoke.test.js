import { Selector } from 'testcafe';

fixture(`Editor page`)
  .page('http://localhost:8080');

test('Smoke test', async testController => {
  await testController.maximizeWindow();
  const itemSelector = await new Selector('.v-list > div:nth-child(1)');

  await testController.expect(itemSelector.innerText).eql('Editor');
});
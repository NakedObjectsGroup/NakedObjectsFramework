import { NakedobjectsspaPage } from './app.po';

describe('nakedobjectsspa App', function() {
  let page: NakedobjectsspaPage;

  beforeEach(() => {
    page = new NakedobjectsspaPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

// https://docs.cypress.io/api/introduction/api.html

describe('My First Test', () => {
  it('Visits the app root url', () => {
    cy.visit('localhost:8080/login')
    
    cy.get('input[name="email"]').type('example@email.com')
    cy.get('input[name="password"]').type('password123')

    cy.request('login', {email: 'example@email.com', password: 'password123',}).should((response) => {
      expect(response.status).to.eq(200)
      expect(response).to.have.property('headers')
      expect(response).to.have.property('duration')
    })
    
    cy.get('button').contains('Sign in').click()
    cy.url().should('eq', 'http://localhost:8080/profile')
  })
})

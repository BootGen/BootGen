# BootGen Editor

Online rapid prototyping tool for ASP.Net and Vue.js projects. Turn JSON into well coded application at https://bootgen.com!

## Supported Languages
C#, TypeScript, Swagger (OAS3)

## How Does it Work?

From a set of sample data given JSON format BootGen will determine a data model. Using this data model and a set of Scriban templates it will generate a server and a client application.

An example set of Scriban templates can be found [here.](https://github.com/BootGen/BootGen/tree/master/BootGenTest/templates)

### An Example JSON Input

```js
{
  "users": [
    {
      "userName": "Test User",
      "email": "example@email.com",
      "tasks": [
        //timestamps
        {
          "title": "Task Title",
          "description": "Task description",
          "isOpen": true,
          "dueDate": "2021-12-30T12:00:05",
          "estimatedHours": 1.5,
          "priority": 1,
          "tags": [
            //manyToMany
            {
              "name": "important",
              "color": "red"
            }
          ]
        }
      ]
    }
  ]
}
```
#### Conventions
   * Property and class names should be camelCase. Casing will be set in the generated code according to the type of file generated. In C# PascalCase will be used, in TypeScript property names will remain camelCase.
   * Array names should be plural nouns, everything else should be in singular form. In the generated code the fitting plural or singular form of names will be used.

### Hinting

You can give hints in the form of comments for the generator. Hints can be placed at the beginning of arrays. Possible hints:
  * `timestamps`: Adds a `Created` and an `Updated` timestamp property to the class.
  * `manyToMany`: Declears that the given relation is a Many-To-Many relation, as opposed to the default One-To-Many relation.
  * `class:[name]`: Substitute `[name]` with the intended name of the class. Example:

```js
{
  "users": [
    {
      "userName": "Test User",
      "email": "example@email.com",
      "friends": [
        //class:user
        //manyToMany
        {
          "userName": "Test User 2",
          "email": "example2@email.com"
        }
      ]
    }
  ]
}
```


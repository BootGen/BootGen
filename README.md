<p align="center">
  <img src="ClientApp/src/assets/img/bootgen_full.png" width="50%">
</p>
<h1 align="center">Starter Project Generator <br> Develop ASP.NET 5 + Vue 3 applications much faster!</h1> 

BootGen generates a well coded, human readable, ASP.NET 5 project with Vue 3 based on the sample JSON data you provide. From this sample data it infers the types you need, and creates entity classes, data services, controllers and Vuex state management that fits your use case. 
  
Try it at https://bootgen.com!

## How Does it Work?
<p align="center">
<a href="https://youtu.be/bdgWl8Ia7u8" target="_blank"><img width="800px" height="450px" src="Images/diff_demo.gif"></a>
</p>
Create sample JSON data, and generate your application. Edit the JSON, and regenerate until you are satisfied with the result. Changes will be highlighted in the generated files.

When you are finnished click download, and start coding!

The downloaded project will compile and run out of the box. We generate code for you, not just the compiler. We intend to create code that is easy to read, understand and edit.

## A Basic Example

```js
{
  "users": [
    {
      "userName": "Jon",
      "email": "jon@arbuckle.com",
      "pets": [
        {
          "name": "Garfield",
          "species": "cat"
        },
        {
          "name": "Odie",
          "species": "dog"
        }
      ]
    }
  ]
}
```

For this example, an appllication with two entity classes `User` and `Pet` will be created. The database will be seeded with entities for Jon, Garfield and Odie. [Try it online!](https://bootgen.com/editor)

#### JSON conventions
   * Property and class names should be camelCase.
   * Array names should be plural nouns, everything else should be in singular form.

## Advanced Features

Although comments are non-standard feature in JSON, many JSON processing libraries support it. We use them as annotations. Annotations can be placed at the beginning of arrays. Possible annotations:
  * `timestamps`: Adds a `Created` and an `Updated` timestamp property to the class.
  * `manyToMany`: Declears that the given relation is a Many-To-Many relation, as opposed to the default One-To-Many relation.

### Advanced Example: Task Management System

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
## Known Limitations

There are many limitations of this project, simply because for many things there is no single accepted way to code it. We intend to generate only those parts of your project that are fairly standard. We aim for the wide useability of the generated code, not completeness. Please treat the generated code as a basis of your work, and not something that you can hand over to a client out of the box.

There are however limitations that we do plan to overcome in the future. These are:
 * It is currently not possible to create self referencing Many-To-Many connections.

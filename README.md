# BootGen Editor [![Build Status](https://github.com/BootGen/Editor/workflows/Test/badge.svg?branch=develop)](https://github.com/BootGen/BootGen/actions)

Online rapid prototyping tool for ASP.Net and Vue.js projects. Turn JSON into well coded application!

## Example

The following example describes a task management application:

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
            //many-to-many
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

For this input BootGen can generate a working application using ASP.Net 5 and Vue.js 2. Try it at https://bootgen.com!
